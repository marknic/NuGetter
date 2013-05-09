using System;
using System.Drawing;
using System.Management.Automation.Runspaces;
using System.Activities;
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

    [ToolboxBitmap(typeof(InvokeNugetterPowerShellScript), "Resources.nugetter.ico")]
    [BuildActivity(HostEnvironmentOption.All)]
    [BuildExtension(HostEnvironmentOption.All)]
    public sealed class InvokeNugetterPowerShellScript : CodeActivity
    {
        #region Workflow Arguments

        /// <summary>
        /// This is the Drop Folder where the application resides after the compilation process
        /// </summary>
        [RequiredArgument]
        public InArgument<string> DropFolder { get; set; }

        /// <summary>
        /// This is the Sources Folder where the source files are copied for the compilation process
        /// </summary>
        [RequiredArgument]
        public InArgument<string> SourcesFolder { get; set; }

        /// <summary>
        /// This is the Binaries Folder where the application binaries are created during the compilation process
        /// </summary>
        [RequiredArgument]
        public InArgument<string> BinariesFolder { get; set; }

        /// <summary>
        /// The path to the PowerShell script
        /// </summary>
        [RequiredArgument]
        public InArgument<string> PowerShellScriptFilepath { get; set; }

        /// <summary>
        /// The path to the pre-package folder.  This is where NuGet will expect to find the files for the packaging process
        /// </summary>
        [RequiredArgument]
        public InArgument<string> NuGetPrePackageFolder { get; set; }

        /// <summary>
        /// Result of running the PowerShell script (0 == Success)
        /// </summary>
        public OutArgument<string> Result { get; set; }

        #endregion

        // If your activity returns a value, derive from CodeActivity<TResult>
        // and return the value from the Execute method.
        protected override void Execute(CodeActivityContext context)
        {
            var dropFolder = DropFolder.Get(context);
            var sourcesFolder = SourcesFolder.Get(context);
            var binariesFolder = BinariesFolder.Get(context);
            var powerShellScriptFilepath = PowerShellScriptFilepath.Get(context);
            var nuGetPrePackageFolder = NuGetPrePackageFolder.Get(context);

            context.WriteBuildMessage(string.Format("powerShellScriptFilepath: {0}", powerShellScriptFilepath), BuildMessageImportance.High);
            context.WriteBuildMessage(string.Format("dropFolder: {0}", dropFolder), BuildMessageImportance.High);
            context.WriteBuildMessage(string.Format("binariesFolder: {0}", binariesFolder), BuildMessageImportance.High);
            context.WriteBuildMessage(string.Format("sourcesFolder: {0}", sourcesFolder), BuildMessageImportance.High);

            // Call the PowerShell script
            var result = ExecutePowerShellScript(powerShellScriptFilepath, dropFolder, binariesFolder, sourcesFolder, nuGetPrePackageFolder);
            
            // Return the value back to the workflow
            context.SetValue(Result, result);
        }

        public string ExecutePowerShellScript(string powerShellScriptFilepath, string dropFolder, string binariesFolder, 
            string sourcesFolder, string nuGetPrePackageFolder)
        {
            var result = "NGPS-Success";

            // Create the PowerShell "RunSpace"
            using (var myRunSpace = RunspaceFactory.CreateRunspace())
            {
                // Open the runspace
                myRunSpace.Open();

                // Set the variable so that it is available when the script runs
                myRunSpace.SessionStateProxy.SetVariable("tfsDropFolder", dropFolder);
                myRunSpace.SessionStateProxy.SetVariable("tfsSourcesFolder", sourcesFolder);
                myRunSpace.SessionStateProxy.SetVariable("tfsBinariesFolder", binariesFolder);
                myRunSpace.SessionStateProxy.SetVariable("tfsNuGetPrePackageFolder", nuGetPrePackageFolder);

                // Create the PowerShell pipeline for executing the script.
                using (var cmdPipeline = myRunSpace.CreatePipeline(string.Format("&\"{0}\"", powerShellScriptFilepath)))
                {
                    try
                    {
                        var invokeResults = cmdPipeline.Invoke();
                    }
                    catch (Exception exception)
                    {
                        result = exception.Message;
                    }
                }
            }

            return result;
        }
    }
}
