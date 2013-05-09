using System;
using System.Drawing;
using System.Activities;
using System.IO;
using Microsoft.TeamFoundation.Build.Client;
using Microsoft.TeamFoundation.VersionControl.Client;

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
    [ToolboxBitmap(typeof(GetFile), "Resources.nugetter.ico")]
    [BuildActivity(HostEnvironmentOption.All)]
    [BuildExtension(HostEnvironmentOption.All)]
    public sealed class GetFile : CodeActivity
    {
        #region Workflow Arguments

        /// <summary>
        /// This is the workspace used by the project being built
        /// </summary>
        [RequiredArgument]
        public InArgument<Workspace> Workspace { get; set; }

        /// <summary>
        /// The path to the file to be checked out
        /// </summary>
        [RequiredArgument]
        public InArgument<string> FileToGet { get; set; }

        /// <summary>
        /// The path to the local build folder
        /// </summary>
        [RequiredArgument]
        public InArgument<string> BuildDirectory { get; set; }

        /// <summary>
        /// The path to the local build folder
        /// </summary>
        [RequiredArgument]
        public InArgument<string> DestinationSubfolderName { get; set; }

        /// <summary>
        /// Full path to the file retrieved from TFS
        /// </summary>
        public OutArgument<string> FullPathToFile { get; set; }

        #endregion

        protected override void Execute(CodeActivityContext context)
        {
            #region Workflow Arguments

            // The TFS source location of the file to get
            var fileToGet = context.GetValue(FileToGet);

            // The current workspace - used to create a new workspace for the get
            var workspace = context.GetValue(Workspace);

            // The local build directory
            var buildDirectory = context.GetValue(BuildDirectory);

            var destinationSubfolderName = context.GetValue(DestinationSubfolderName);

            #endregion

            // File and path
            var versionFileDirectory = string.Format("{0}\\{1}", buildDirectory, destinationSubfolderName);
            var filename = Path.GetFileName(fileToGet);

            if (filename == null)
            {
                throw new ArgumentException("Filename must not be null");
            }

            var fullPathToFile = Path.Combine(versionFileDirectory, filename);

            // Write to the log
            context.WriteBuildMessage(string.Format("Getting file from Source: {0}", fileToGet), BuildMessageImportance.High);
            
            // Create workspace and working folder
            var tempWorkspace = workspace.VersionControlServer.CreateWorkspace("NuGetterTemp");
            var workingFolder = new WorkingFolder(fileToGet, fullPathToFile);

            // Map the workspace
            tempWorkspace.CreateMapping(workingFolder);

            // Get the file
            var request = new GetRequest(new ItemSpec(fileToGet, RecursionType.None), VersionSpec.Latest);
            var status = tempWorkspace.Get(request, GetOptions.GetAll | GetOptions.Overwrite); 

            if (!status.NoActionNeeded)
            {
                foreach (var failure in status.GetFailures())
                {
                    context.WriteBuildMessage(string.Format("Failed to get file from source: {0} - {1}", fileToGet, failure.GetFormattedMessage()), BuildMessageImportance.High);
                }    
            }

            // Return the value back to the workflow
            context.SetValue(FullPathToFile, fullPathToFile);

            // Get rid of the workspace
            tempWorkspace.Delete();
        }
    }
}
