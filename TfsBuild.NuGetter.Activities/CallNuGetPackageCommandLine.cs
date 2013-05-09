using System;
using System.Drawing;
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
    [ToolboxBitmap(typeof(CallNuGetPackageCommandLine), "Resources.nugetter.ico")]
    [BuildActivity(HostEnvironmentOption.All)]
    [BuildExtension(HostEnvironmentOption.All)]
    public sealed class CallNuGetPackageCommandLine : CodeActivity
    {
        #region Workflow Parameters
        /// <summary>
        /// Full filepath to NuGet.exe 
        /// </summary>
        [RequiredArgument]
        public InArgument<string> NuGetExeFilePath { get; set; }

        /// <summary>
        /// The path of the nuspec file
        /// </summary>
        [RequiredArgument]
        public InArgument<string> NuSpecFilePath { get; set; }

        /// <summary>
        /// The destination location if deployment is to be done
        /// </summary>
        [RequiredArgument]
        public InArgument<string> BasePath { get; set; }

        /// <summary>
        /// The destination location if deployment is to be done
        /// </summary>
        [RequiredArgument]
        public InArgument<string> OutputDirectory { get; set; }

        /// <summary>
        /// The destination location if deployment is to be done
        /// </summary>
        [RequiredArgument]
        public InArgument<string> VersionNumber { get; set; }

        /// <summary>
        /// Additional NuGet command line options
        /// </summary>
        [RequiredArgument]
        public InArgument<string> AdditionalOptions { get; set; }

        /// <summary>
        /// Output: Result of the CallNuGetPackageCommandLine Activity
        /// </summary>
        public OutArgument<bool> NuGetPackagingResult { get; set; }
        #endregion

        // Property that holds the link to the Process Execution method
        //  Note: this property allows dependency injection
        public INuGetProcess NuGetProcess { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            #region Extract Workflow Argument Values

            // The file path of the nuget.exe - if null or empty then 
            //  assume nuget is "installed" on the build server and in the path
            var nuGetExeFilePath = NuGetExeFilePath.Get(context);

            // The path of the nuspec file
            var nuSpecFilePath = NuSpecFilePath.Get(context);

            // The folder location of the files to be packed
            var basePath = BasePath.Get(context);

            // The folder location of the files to be packed
            var outputDirectory = OutputDirectory.Get(context);

            // The destination location if deployment is to be done
            var versionNumber = VersionNumber.Get(context);

            // command line options to append (as is) to the nuget command line
            var additionalOptions = AdditionalOptions.Get(context);

            #endregion

            context.WriteBuildMessage(string.Format("In CallNuGetPackageCommandLine:"), BuildMessageImportance.High);
            context.WriteBuildMessage(string.Format("nuGetExeFilePath: {0}", nuGetExeFilePath), BuildMessageImportance.High);
            context.WriteBuildMessage(string.Format("basePath: {0}", basePath), BuildMessageImportance.High);
            context.WriteBuildMessage(string.Format("outputDirectory: {0}", outputDirectory), BuildMessageImportance.High);
            context.WriteBuildMessage(string.Format("versionNumber: {0}", versionNumber), BuildMessageImportance.High);
            context.WriteBuildMessage(string.Format("additionalOptions: {0}", additionalOptions), BuildMessageImportance.High);

            // Don't assume that DI will have happened.  If the value is null then create the default object.);)
            if (NuGetProcess == null)
            {
                NuGetProcess = new NuGetProcess();
            }

            // Call the method that will do the work
            var results = NuGetPackaging(nuGetExeFilePath, nuSpecFilePath, outputDirectory, basePath, versionNumber, additionalOptions, context);

            // Send the result back to the workflow
            NuGetPackagingResult.Set(context, results);
        }

        /// <summary>
        /// Does the NuGet Processing work.  Formats data into an argument string and then calls NuGet.Exe
        /// </summary>
        /// <param name="nuGetExeFilePath">Full path to the nuget.exe application OR empty if nuget is in the machine's path</param>
        /// <param name="nuSpecFilePath">path to the NuSpec "manifest" file</param>
        /// <param name="outputDirectory">path to the folder where the generated NuPkg file should be placed</param>
        /// <param name="basePath">path to the folder containing all of the files that should be packaged</param>
        /// <param name="versionNumber">optional: is a version number is provided it will be used to override the version of the package</param>
        /// <param name="additionalOptions">additional NuGet command line arguments (optional)</param>
        /// <param name="context">the workflow/build context - used to send build messages</param>
        /// <returns>true if the nuget process succeeds</returns>
        public bool NuGetPackaging(string nuGetExeFilePath, string nuSpecFilePath, string outputDirectory, string basePath, string versionNumber, string additionalOptions, CodeActivityContext context)
        {
            var versionParameter = string.Empty;

            #region Parameter Validation
            if (string.IsNullOrWhiteSpace(nuSpecFilePath))
            {
                throw new ArgumentNullException("nuSpecFilePath");
            }

            if (string.IsNullOrWhiteSpace(basePath))
            {
                throw new ArgumentNullException("basePath");
            }

            if (string.IsNullOrWhiteSpace(outputDirectory))
            {
                throw new ArgumentNullException("outputDirectory");
            }
            #endregion

            // If the version number was provided, then use it
            if (!string.IsNullOrWhiteSpace(versionNumber))
            {
                versionParameter = string.Format("-version {0}", versionNumber);
            }

            var arguments = string.Format(
                "pack \"{0}\" -OutputDirectory \"{1}\" -BasePath \"{2}\" {3} {4}", nuSpecFilePath, outputDirectory, basePath, versionParameter, additionalOptions).Trim();

            if (context != null)
            {
                context.WriteBuildMessage(string.Format("CallNuGetPackageCommandLine arguments: {0}", arguments), BuildMessageImportance.High);
            }

            return NuGetProcess.RunNuGetProcess(NuGetHelper.ValidateNuGetExe(nuGetExeFilePath), arguments, context);
        }

    }
}
