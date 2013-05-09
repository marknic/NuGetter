using System;
using System.Drawing;
using System.Activities;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.TeamFoundation.Build.Client;

// ==============================================================================================
// http://NuGetter.codeplex.com/
//
// Author: Mark S. Nichols
//
// Copyright (c) 2013 Mark Nichols
//
// This source is subject to the Microsoft Permissive License. 
// ==============================================================================================

namespace TfsBuild.NuGetter.Activities
{
    [ToolboxBitmap(typeof(GetVersionPattern), "Resources.nugetter.ico")]
    [BuildActivity(HostEnvironmentOption.All)]
    [BuildExtension(HostEnvironmentOption.All)]
    public sealed class GetVersionPattern : CodeActivity
    {
        public static readonly string QueryPackageId = "/VersionSeed/NuGetPackage[@id='{0}']/VersionPattern";
        public static readonly string QuerySolutionName = "/VersionSeed/Solution[@name='{0}']/AssemblyVersionPattern";

        #region Workflow Arguments
        
        /// <summary>
        /// The path to the seed file or the version number
        /// </summary>
        [RequiredArgument]
        public InArgument<string> VersionPatternOrSeedFilePath { get; set; }

        [RequiredArgument]
        public InArgument<string> PackageId { get; set; }

        [RequiredArgument]
        public InArgument<string> SourcesDirectory { get; set; }

        /// <summary>
        /// Pattern for the version number for the package
        /// </summary>
        public OutArgument<string> VersionPattern { get; set; }

        #endregion

        protected override void Execute(CodeActivityContext context)
        {
            #region Workflow Arguments

            // This will be the api key or a path to the file with the key in it (or blank)
            var versionPatternOrSeedFilePath = VersionPatternOrSeedFilePath.Get(context);

            // The local build directory
            var packageId = PackageId.Get(context);

            var sourcesDirectory = SourcesDirectory.Get(context);

            #endregion

            string versionPattern;

            try
            {
                versionPattern = ExtractVersion(versionPatternOrSeedFilePath, packageId, QueryPackageId, sourcesDirectory);
            }
            catch (ArgumentException)
            {
                versionPattern = ExtractVersion(versionPatternOrSeedFilePath, packageId, QuerySolutionName, sourcesDirectory);
            }

            // Write to the log
            context.WriteBuildMessage(string.Format("Version Pattern: {0}", versionPattern), BuildMessageImportance.Normal);
            
            // Return the value back to the workflow
            context.SetValue(VersionPattern, versionPattern);
        }

        public string ExtractVersion(string versionPatternOrSeedFilePath, string packageId, string queryString, string sourcesDirectory)
        {
            var versionPattern = string.Empty;

            #region Validate Parameters

            if (string.IsNullOrWhiteSpace(versionPatternOrSeedFilePath))
            {
                throw new ArgumentNullException(versionPatternOrSeedFilePath);
            }

            if (string.IsNullOrWhiteSpace(packageId))
            {
                throw new ArgumentNullException(packageId);
            }

            if (string.IsNullOrWhiteSpace(sourcesDirectory))
            {
                throw new ArgumentNullException(sourcesDirectory);
            }

            if (string.IsNullOrWhiteSpace(queryString))
            {
                throw new ArgumentNullException(queryString);
            }

            #endregion

            //var regex = new Regex(@"^(\d+\.)?((\d+|[a-zA-Z])\.)?((\d+|[a-zA-Z])\.)?(\*|\d+|[a-zA-Z])$");

            var regex =
                new Regex(
                    @"^(\d+)?(\.((\d{1,5})|([a-zA-Z]{1,4})))?(\.((\d{1,5})|([a-zA-Z]{1,9})))?(\.((\d)+|(\d+[a-zA-Z0-9+-]*)|([a-zA-Z]{1,9})|([a-zA-Z]{1,9})([-|+][a-zA-Z0-9+-]*)|(\*)))?$");

            var match = regex.Match(versionPatternOrSeedFilePath);

            // Was the value passed in actually an version pattern)
            if (match.Success)
            {
                // Write to the log

                versionPattern = versionPatternOrSeedFilePath;
            }
            else // 
            {
                // full path or do i have to create one?
                var seedFilePath = Path.IsPathRooted(versionPatternOrSeedFilePath)
                                        ? versionPatternOrSeedFilePath
                                        : Path.Combine(sourcesDirectory, versionPatternOrSeedFilePath);

                if (File.Exists(seedFilePath))
                {
                    var xPathQuery = string.Format(queryString, packageId);

                    var xmlGetElement = new XmlGetElement();

                    versionPattern = xmlGetElement.GetXmlElementValue(seedFilePath, xPathQuery, "", "");
                }
            }

            return versionPattern;
        }
    }
}
