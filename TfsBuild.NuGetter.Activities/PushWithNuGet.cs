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
    [ToolboxBitmap(typeof(PushWithNuGet), "Resources.nugetter.ico")]
    [BuildActivity(HostEnvironmentOption.All)]
    [BuildExtension(HostEnvironmentOption.All)]
    public sealed class PushWithNuGet : CodeActivity
    {
        #region Workflow Parameters
        /// <summary>
        /// Full filepath to NuGet.exe 
        /// </summary>
        [RequiredArgument]
        public InArgument<string> NuGetExeFilePath { get; set; }

        /// <summary>
        /// The path of the nupkg file
        /// </summary>
        [RequiredArgument]
        public InArgument<string> PackageLocation { get; set; }

        /// <summary>
        /// The API key for pushing/publishing
        /// </summary>
        [RequiredArgument]
        public InArgument<string> ApiKey { get; set; }

        /// <summary>
        /// The destination location if deployment is to be done
        /// </summary>
        [RequiredArgument]
        public InArgument<string> PushDestination { get; set; }

        public OutArgument<bool> NuGetPushResult { get; set; }
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
            var packageLocation = PackageLocation.Get(context);

            // The API Key for pushing
            var apiKey = ApiKey.Get(context);

            // The destination location if deployment is to be done
            var pushDestination = PushDestination.Get(context);

            #endregion

            // Don't assume that DI will have happened.  If the value is null then create the default object.)
            if (NuGetProcess == null)
            {
                NuGetProcess = new NuGetProcess();
            }

            // Call the method that will do the work
            var results = NuGetPublishing(nuGetExeFilePath, packageLocation, pushDestination, apiKey, context);

            // Send the result back to the workflow
            NuGetPushResult.Set(context, results);
        }


        /// <summary>
        /// Does the NuGet Processing work.  Formats data into an argument string and then calls NuGet.Exe to do the Push or Push/Publish
        /// </summary>
        /// <param name="nuGetExeFilePath">Full path to the nuget.exe application OR empty if nuget is in the machine's path</param>
        /// <param name="packageLocation">Full path to the NuPkg file that is to be pushed</param>
        /// <param name="pushDestination">Address of the gallery that nuget.exe will push the package to</param>
        /// <param name="apiKey">Optional: (if required) the key used to push the package to the nuget gallery</param>
        /// <param name="context">the workflow/build context - used to send build messages</param>
        /// <returns>true if the nuget process succeeds</returns>
        public bool NuGetPublishing(string nuGetExeFilePath, string packageLocation, string pushDestination, string apiKey, CodeActivityContext context)
        {
            #region Parameter Validation
            if (string.IsNullOrWhiteSpace(packageLocation))
            {
                throw new ArgumentNullException("packageLocation");
            }

            if (pushDestination == null)
            {
                // Assume that we will push to the NuGet Gallery
                pushDestination = string.Empty;
            }
            #endregion

            // Match a unc or drive based location - Do a copy if found
            var regex = new Regex(@"^((\\\\[a-zA-Z0-9-]+\\[a-zA-Z0-9`~!@#$%^&(){}'._-]+([ ]+[a-zA-Z0-9`~!@#$%^&(){}'._-]+)*)|([a-zA-Z]:))(\\[^ \\/:*?""<>|]+([ ]+[^ \\/:*?""<>|]+)*)*\\?$");

            if (!regex.Match(pushDestination).Success)
            {
                var pushDestinationArgument = string.IsNullOrWhiteSpace(pushDestination) ?
                   string.Empty : string.Format("-s \"{0}\"", pushDestination);

                var arguments = string.Format("push \"{0}\" {1} {2}",
                                             packageLocation, apiKey, pushDestinationArgument);

                if (context != null)
                {
                    context.WriteBuildMessage(string.Format("Push Arguments: {0}", arguments), BuildMessageImportance.High);
                }

                return NuGetProcess.RunNuGetProcess(NuGetHelper.ValidateNuGetExe(nuGetExeFilePath), arguments, context);
            }

            var fileName = Path.GetFileName(packageLocation);

            if (fileName != null)
            {
                var destinationFilePath = Path.Combine(pushDestination, fileName);

                File.Copy(packageLocation, destinationFilePath, true);

                return true;
            }

            return false;

        }
    }
}
