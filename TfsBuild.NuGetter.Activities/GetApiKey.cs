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
    [ToolboxBitmap(typeof(GetApiKey), "Resources.nugetter.ico")]
    [BuildActivity(HostEnvironmentOption.All)]
    [BuildExtension(HostEnvironmentOption.All)]
    public sealed class GetApiKey : CodeActivity
    {
        #region Workflow Arguments

        /// <summary>
        /// The path to the file to be checked out
        /// </summary>
        [RequiredArgument]
        public InArgument<string> ApiKeyOrFile { get; set; }

        /// <summary>
        /// The path to the local build Sources folder
        /// </summary>
        [RequiredArgument]
        public InArgument<string> SourcesDirectory { get; set; }

        /// <summary>
        /// Full path to the file retrieved from TFS
        /// </summary>
        public OutArgument<string> ApiKeyValue { get; set; }

        #endregion

        protected override void Execute(CodeActivityContext context)
        {
            #region Workflow Arguments

            // This will be the api key or a path to the file with the key in it (or blank)
            var apiKeyOrFile = ApiKeyOrFile.Get(context);

            // The local build directory
            var sourcesDirectory = SourcesDirectory.Get(context);

            #endregion

            string resultMessage;
            var apiKey = ExtractApiKey(apiKeyOrFile, sourcesDirectory, out resultMessage);

            // Write to the log
            context.WriteBuildMessage(resultMessage, BuildMessageImportance.High);
            
            // Return the value back to the workflow
            context.SetValue(ApiKeyValue, apiKey);
        }

        public string ExtractApiKey(string apiKeyOrFile, string sourcesDirectory, out string resultMessage)
        {
            var apiKey = string.Empty;

            // is there an api key value?
            if (string.IsNullOrWhiteSpace(apiKeyOrFile))
            {
                resultMessage = string.Format("No API key or file was provided.");
            }
            else
            {
                var regex = new Regex(@"[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}");

                var match = regex.Match(apiKeyOrFile);

                // Was the value passed in actually an API key)
                if (match.Success)
                {
                    // Write to the log
                    resultMessage = string.Format("API key was provided in the build definition.");

                    apiKey = match.Value;
                }
                else // 
                {
                    // full path or do i have to create one?
                    var apiKeyFilePath = Path.IsPathRooted(apiKeyOrFile) ? apiKeyOrFile : Path.Combine(sourcesDirectory, apiKeyOrFile);

                    var apiFileData = File.ReadAllText(apiKeyFilePath);

                    match = regex.Match(apiFileData);

                    if (match.Success)
                    {
                        // Write to the log
                        resultMessage = string.Format("Found the API key in the file: {0}", apiKeyOrFile);

                        apiKey = match.Value;
                    }
                    else
                    {
                        // Write to the log
                        resultMessage = string.Format("Did not find an API key in the file: {0} - was looking in this path: {1}",
                            apiKeyOrFile, apiKeyFilePath);
                    }
                }
            }

            return apiKey;
        }
    }
}
