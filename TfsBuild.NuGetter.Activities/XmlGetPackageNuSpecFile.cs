using System;
using System.Activities;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Microsoft.TeamFoundation.Build.Client;

// ==============================================================================================
// http://tfsversioning.codeplex.com/
//
// Author: Mark S. Nichols
//
// Copyright (c) 2013 Mark Nichols
//
// This source is subject to the Microsoft Permissive License. 
// ==============================================================================================

namespace TfsBuild.NuGetter.Activities
{
    /// <summary>
    /// Used to search the version "seed file" and return the value
    /// </summary>
    [ToolboxBitmap(typeof(XmlGetPackageNuSpecFile), "Resources.version.ico")]
    [BuildActivity(HostEnvironmentOption.All)]
    [BuildExtension(HostEnvironmentOption.All)]
    public sealed class XmlGetPackageNuSpecFile : CodeActivity
    {
        #region Workflow Arguments

        /// <summary>
        /// Path of the xml file to search
        /// </summary>
        [RequiredArgument]
        public InArgument<string> PackageInfoFilePath { get; set; }

        [RequiredArgument]
        public InArgument<int> PackageIndex { get; set; }


        #region Out Arguments
        
        public OutArgument<string> NuSpecFilePath { get; set; }

        #endregion

        #endregion

        /// <summary>
        /// Searches an XML file with an XPath expression
        /// </summary>
        /// <param name="context"></param>
        protected override void Execute(CodeActivityContext context)
        {
            // get the value of the FilePath
            var packageInfoFilePath = context.GetValue(PackageInfoFilePath);
            var packageIndex = context.GetValue(PackageIndex);



            var nuSpecFile = Execute(packageInfoFilePath, packageIndex);

            NuSpecFilePath.Set(context, nuSpecFile);
        }

        public string Execute(string packageInfoFilePath, int packageIndex)
        {
            if (packageIndex < 0) throw new ArgumentException("Project Index cannot be less than 0.");

            if (!File.Exists(packageInfoFilePath)) throw new ArgumentException("XML Project file: " + packageInfoFilePath + " does not exist.");

            var xdoc = XDocument.Load(packageInfoFilePath);

            var nuGetterNuSpecList = (from xml in xdoc.Elements("NuGetterPackages").Elements("NuGetterPackage")
                                           let nuspecFilePath = xml.Element("NuSpecFilePath")
                                           
                                           select nuspecFilePath).ToList();

            if (nuGetterNuSpecList.Count <= packageIndex) throw new ArgumentException("Out of Range. Requested package index from XML was: " + packageIndex +
                " and there are " + nuGetterNuSpecList.Count + " packages in the list.");

            var nuSpecFile = nuGetterNuSpecList[packageIndex].Value;

            if (string.IsNullOrWhiteSpace(nuSpecFile)) throw new DataException(string.Format("XML Package Data did not contain the path for the NuSpec file - index number: {0}.", packageIndex));

            return nuSpecFile;
        }
    }
}