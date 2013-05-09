using System;
using System.Activities;
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
    [ToolboxBitmap(typeof(XmlGetPackageElementsCount), "Resources.version.ico")]
    [BuildActivity(HostEnvironmentOption.All)]
    [BuildExtension(HostEnvironmentOption.All)]
    public sealed class XmlGetPackageElementsCount : CodeActivity
    {
        #region Workflow Arguments

        /// <summary>
        /// Path of the xml file to search
        /// </summary>
        [RequiredArgument]
        public InArgument<string> PackageInfoFilePath { get; set; }

        #region Out Arguments
        
        public OutArgument<int> PackageInfoElementsCount { get; set; }
        
        #endregion

        #endregion

        /// <summary>
        /// Counts the number of package entries in the xml file and returns the count.  
        /// The value is then used for looping through all the entries within the workflow.
        /// </summary>
        /// <param name="context"></param>
        protected override void Execute(CodeActivityContext context)
        {
            // get the value of the FilePath
            var packageInfoFilePath = context.GetValue(PackageInfoFilePath);
            
            var count = Execute(packageInfoFilePath);

            PackageInfoElementsCount.Set(context, count);
            
        }

        /// <summary>
        /// Gets the count of the XML file and returns it
        /// </summary>
        /// <param name="packageInfoFilePath"></param>
        /// <returns></returns>
        public int Execute(string packageInfoFilePath)
        {
            if (!File.Exists(packageInfoFilePath)) throw new ArgumentException("XML Project file: " + packageInfoFilePath + " does not exist.");

            var xdoc = XDocument.Load(packageInfoFilePath);

            var count = xdoc.Elements("NuGetterPackages").Elements("NuGetterPackage").Count();

            return count;
        }

    }
}