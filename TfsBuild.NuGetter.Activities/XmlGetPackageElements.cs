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
    [ToolboxBitmap(typeof(XmlGetPackageElements), "Resources.version.ico")]
    [BuildActivity(HostEnvironmentOption.All)]
    [BuildExtension(HostEnvironmentOption.All)]
    public sealed class XmlGetPackageElements : CodeActivity
    {
        #region Workflow Arguments

        /// <summary>
        /// Path of the xml file to search
        /// </summary>
        [RequiredArgument]
        public InArgument<string> PackageInfoFilePath { get; set; }

        [RequiredArgument]
        public InArgument<int> PackageIndex { get; set; }

        [RequiredArgument]
        public InArgument<string> AdditionalOptionsFromBldDef { get; set; }

        [RequiredArgument]
        public InArgument<string> BasePathFromBldDef { get; set; }

        [RequiredArgument]
        public InArgument<string> OutputDirectoryFromBldDef { get; set; }

        [RequiredArgument]
        public InArgument<bool> SwitchInvokePushFromBldDef { get; set; }

        [RequiredArgument]
        public InArgument<string> PushDestinationFromBldDef { get; set; }

        [RequiredArgument]
        public InArgument<bool> SwitchInvokePowerShellFromBldDef { get; set; }

        [RequiredArgument]
        public InArgument<string> PowerShellScriptPathFromBldDef { get; set; }
        
        [RequiredArgument]
        public InArgument<string> VersionFromBldDef { get; set; }

        #region Out Arguments
        public OutArgument<string> Name { get; set; }
        public OutArgument<string> NuSpecFilePath { get; set; }
        public OutArgument<string> BasePath { get; set; }
        public OutArgument<string> AdditionalOptions { get; set; }
        public OutArgument<string> Version { get; set; }

        public OutArgument<string> OutputDirectory { get; set; }
        public OutArgument<bool> SwitchInvokePush { get; set; }
        public OutArgument<string> PushDestination { get; set; }
        public OutArgument<bool> SwitchInvokePowerShell { get; set; }
        
        public OutArgument<string> PowerShellScriptPath { get; set; }
        
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

            var outputDirectoryFromBldDef = context.GetValue(OutputDirectoryFromBldDef);
            var switchInvokePushFromBldDef = context.GetValue(SwitchInvokePushFromBldDef);
            var pushDestinationFromBldDef = context.GetValue(PushDestinationFromBldDef);
            var switchInvokePowerShellFromBldDef = context.GetValue(SwitchInvokePowerShellFromBldDef);
            var versionFromBldDef = context.GetValue(VersionFromBldDef);

            var basePathFromBldDef = context.GetValue(BasePathFromBldDef);
            var additionalOptionsFromBldDef = context.GetValue(AdditionalOptionsFromBldDef);
            var powerShellScriptPathFromBldDef = context.GetValue(PowerShellScriptPathFromBldDef);


            var packageData = Execute(packageInfoFilePath, packageIndex, basePathFromBldDef, additionalOptionsFromBldDef,
                outputDirectoryFromBldDef, switchInvokePushFromBldDef,
                pushDestinationFromBldDef, switchInvokePowerShellFromBldDef, powerShellScriptPathFromBldDef, versionFromBldDef);

            Name.Set(context, packageData.Name);
            AdditionalOptions.Set(context, packageData.AdditionalOptions);
            BasePath.Set(context, packageData.BasePath);
            NuSpecFilePath.Set(context, packageData.NuSpecFilePath);
            PowerShellScriptPath.Set(context, packageData.PowerShellScriptPath);
            Version.Set(context, packageData.Version);

            OutputDirectory.Set(context, packageData.OutputDirectory);
            SwitchInvokePush.Set(context, packageData.SwitchInvokePush);
            PushDestination.Set(context, packageData.PushDestination);
            SwitchInvokePowerShell.Set(context, packageData.SwitchInvokePowerShell);
        }

        public NuGetterPackageInfo Execute(string packageInfoFilePath, int packageIndex, string basePathFromBldDef, string additionalOptionsFromBldDef, 
            string outputDirectoryFromBldDef, bool switchInvokePushFromBldDef, string pushDestinationFromBldDef, 
            bool switchInvokePowerShellFromBldDef, string powerShellScriptPathFromBldDef, string versionFromBldDef)
        {
            if (packageIndex < 0) throw new ArgumentException("Project Index cannot be less than 0.");

            if (!File.Exists(packageInfoFilePath)) throw new ArgumentException("XML Project file: " + packageInfoFilePath + " does not exist.");

            var xdoc = XDocument.Load(packageInfoFilePath);

            var nuGetterPackageInfoList = (from xml in xdoc.Elements("NuGetterPackages").Elements("NuGetterPackage")
                                           let name = xml.Attribute("name")
                                           let addnlOptionsElement = xml.Element("AdditionalOptions")
                                           let basePathElement = xml.Element("BasePath")
                                           let nuspecFilePath = xml.Element("NuSpecFilePath")
                                           let powerShellScriptPath = xml.Element("PowerShellScriptPath")
                                           let version = xml.Element("Version")

                                           let outputDirectory = xml.Element("OutputDirectory")
                                           let switchInvokePush = xml.Element("InvokePush")
                                           let pushDestination = xml.Element("PushDestination")
                                           let switchInvokePowerShell = xml.Element("InvokePowerShell")


                                           select new NuGetterPackageInfo
                                           {
                                               Name = name == null ? string.Empty : name.Value,
                                               AdditionalOptions = addnlOptionsElement == null ? additionalOptionsFromBldDef : addnlOptionsElement.Value,
                                               BasePath = basePathElement == null ? basePathFromBldDef : basePathElement.Value,
                                               NuSpecFilePath = nuspecFilePath == null ? string.Empty : nuspecFilePath.Value,
                                               PowerShellScriptPath = powerShellScriptPath == null ? powerShellScriptPathFromBldDef : powerShellScriptPath.Value,
                                               Version = version == null ? versionFromBldDef : version.Value,

                                               OutputDirectory = outputDirectory == null ? outputDirectoryFromBldDef : outputDirectory.Value,
                                               SwitchInvokePush = switchInvokePush == null ? switchInvokePushFromBldDef : bool.Parse(switchInvokePush.Value),
                                               PushDestination = pushDestination == null ? pushDestinationFromBldDef : pushDestination.Value,
                                               SwitchInvokePowerShell = switchInvokePowerShell == null ? switchInvokePowerShellFromBldDef : bool.Parse(switchInvokePowerShell.Value),
                                           
                                           }).ToList();

            for (var i = 0; i < nuGetterPackageInfoList.Count; i++)
            {
                if (nuGetterPackageInfoList[i].Name == string.Empty)
                {
                    nuGetterPackageInfoList[i].Name = "Package " + i;
                }
            }

            if (nuGetterPackageInfoList.Count <= packageIndex) throw new ArgumentException("Out of Range. Requested package index from XML was: " + packageIndex +
                " and there are " + nuGetterPackageInfoList.Count + " packages in the list.");

            var packageInfoData = nuGetterPackageInfoList[packageIndex];

            if (string.IsNullOrWhiteSpace(packageInfoData.NuSpecFilePath)) throw new DataException("XML Package Data for '" + nuGetterPackageInfoList[packageIndex].Name + "' did not contain the path for the NuSpec file.");

            return packageInfoData;
        }

    }


    public class NuGetterPackageInfo
    {
        public string Name { get; set; }
        public string AdditionalOptions { get; set; }
        public string BasePath { get; set; }
        public string NuSpecFilePath { get; set; }
        public string PowerShellScriptPath { get; set; }
        public string Version { get; set; }
        public string OutputDirectory { get; set; }
        public bool SwitchInvokePush { get; set; }
        public string PushDestination { get; set; }
        public bool SwitchInvokePowerShell { get; set; }  
    }

}