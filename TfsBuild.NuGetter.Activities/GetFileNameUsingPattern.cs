using System;
using System.Activities;
using System.Drawing;
using System.IO;
using System.Linq;
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
    [ToolboxBitmap(typeof(GetFileNameUsingPattern), "Resources.version.ico")]
    [BuildActivity(HostEnvironmentOption.All)]
    [BuildExtension(HostEnvironmentOption.All)]
    public sealed class GetFileNameUsingPattern : CodeActivity
    {
        #region Workflow Arguments

        /// <summary>
        /// Pattern to use for retrieving an actual filename
        /// </summary>
        [RequiredArgument]
        public InArgument<string> FileNamePattern { get; set; }

        /// <summary>
        /// Folder to search for the file using the pattern
        /// text file.
        /// </summary>
        [RequiredArgument]
        public InArgument<string> SearchFolder { get; set; }

        /// <summary>
        /// The full file name and path from a successful search
        /// </summary>
        public OutArgument<string> FullFilePath { get; set; }

        #endregion

        /// <summary>
        /// Searches an XML file with an XPath expression
        /// </summary>
        /// <param name="context"></param>
        protected override void Execute(CodeActivityContext context)
        {
            // get the value of the XPathExpression
            var fileNamePattern = FileNamePattern.Get(context);

            // get the value of the FilePath
            var searchFolder = SearchFolder.Get(context);

            var filePath = FindFile(fileNamePattern, searchFolder);

            context.WriteBuildMessage(string.Format("Path found: {0}", filePath), BuildMessageImportance.High);

            // return the value 
            FullFilePath.Set(context, filePath);
        }

        public string FindFile(string fileNamePattern, string searchFolder)
        {
            #region Parameter Validation

            // validate that there is a file pattern to work with
            if (String.IsNullOrWhiteSpace(fileNamePattern))
            {
                throw new ArgumentException("FileNamePattern must contain a search pattern");
            }

            // Validate that there is a search folder for the search
            if (String.IsNullOrWhiteSpace(searchFolder))
            {
                throw new ArgumentException("searchFolder");
            }

            #endregion

            var fileList = Directory.EnumerateFiles(searchFolder, fileNamePattern).ToArray();

            if (fileList.Length == 1)
            {
                return fileList[0];
            }

            var exMessage = fileList.Length > 1 ? 
                string.Format("Search pattern '{0}' retrieved more than one file at: {1}", fileNamePattern, searchFolder) :
                string.Format("Search pattern '{0}' did not find any files at: {1}", fileNamePattern, searchFolder);

            throw new ArgumentException(exMessage);
        }
    }
}