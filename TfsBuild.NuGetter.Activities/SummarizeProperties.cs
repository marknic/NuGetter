using System.Drawing;
using System.Activities;
using System.Text.RegularExpressions;
using Microsoft.TeamFoundation.Build.Client;
using System.Text;

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
    [ToolboxBitmap(typeof(SummarizeProperties), "Resources.nugetter.ico")]
    [BuildActivity(HostEnvironmentOption.All)]
    [BuildExtension(HostEnvironmentOption.All)]
    public sealed class SummarizeProperties : CodeActivity
    {
        #region Workflow Arguments
        
        [RequiredArgument]
        public InArgument<bool> SwitchInvokePowerShell { get; set; }

        [RequiredArgument]
        public InArgument<string> PowerShellScriptPath { get; set; }

        [RequiredArgument]
        public InArgument<string> NuGetExeFilePath { get; set; }

        [RequiredArgument]
        public InArgument<string> OutputDirectory { get; set; }

        [RequiredArgument]
        public InArgument<string> BasePath { get; set; }

        [RequiredArgument]
        public InArgument<string> NuSpecFilePath { get; set; }

        [RequiredArgument]
        public InArgument<string> Version { get; set; }

        [RequiredArgument]
        public InArgument<bool> SwitchInvokePush { get; set; }

        [RequiredArgument]
        public InArgument<string> AdditionalOptions { get; set; }

        /// <summary>
        /// The path to the file to be checked out
        /// </summary>
        [RequiredArgument]
        public InArgument<string> ApiKey { get; set; }

        [RequiredArgument]
        public InArgument<string> PushDestination { get; set; }

        #endregion

        protected override void Execute(CodeActivityContext context)
        {
            #region Workflow Arguments

            var switchInvokePowerShell = SwitchInvokePowerShell.Get(context);        
            var powerShellScriptPath = PowerShellScriptPath.Get(context);
            var nuGetExeFilePath = NuGetExeFilePath.Get(context);
            var nuSpecFilePath = NuSpecFilePath.Get(context);
            var basePath = BasePath.Get(context);
            var outputDirectory = OutputDirectory.Get(context);
            var version = Version.Get(context);
            var switchInvokePush = SwitchInvokePush.Get(context);
            var apiKey = ApiKey.Get(context);
            var pushDestination = PushDestination.Get(context);
            var additionalOptions = AdditionalOptions.Get(context);

            #endregion

            var resultMessages = SummarizePropertyValues(switchInvokePowerShell, powerShellScriptPath, nuGetExeFilePath,
                nuSpecFilePath, basePath, outputDirectory, version, switchInvokePush, apiKey,
                pushDestination, additionalOptions);

            for (var i = 0; i < resultMessages.Length - 1; i++)
            {
                // Write to the log
                context.WriteBuildMessage(resultMessages[i], BuildMessageImportance.High);
            }
        }

        private static void AppendLineAndDelimiter(StringBuilder stringBuilder, string lineToAppend)
        {
            stringBuilder.Append(lineToAppend);
            stringBuilder.Append('|');
        }

        public string[] SummarizePropertyValues(bool switchInvokePowerShell, string powerShellScriptPath, 
            string nuGetExeFilePath, string nuSpecFilePath, string basePath, string outputDirectory,
            string version, bool switchInvokePush, string apiKey, string pushDestination, 
            string additionalOptions)
        {
            var propertySummary = new StringBuilder();
            
            if (switchInvokePowerShell)
            {
                if (string.IsNullOrWhiteSpace(powerShellScriptPath))
                {
                    AppendLineAndDelimiter(propertySummary,
                        "Invoke PowerShell switch is set to True but there was no PowerShell script provided. PrePackage step will be ignored.");
                }
                else
                {
                    AppendLineAndDelimiter(propertySummary,
                        string.Format("PrePackage step will be executed using PowerShell script: {0}",
                                      powerShellScriptPath));
                }
            }
            else
            {
                AppendLineAndDelimiter(propertySummary,
                    "PrePackage step will be skipped since the Invoke PowerShell switch was set to False.");
            }

            if (string.IsNullOrWhiteSpace(nuGetExeFilePath))
            {
                AppendLineAndDelimiter(propertySummary,
                "NuGet Exe File Path is empty.  Will attempt to find NuGet.Exe within the system 'Path'.");
            }
            else
            {
                if (nuGetExeFilePath.StartsWith("$/"))
                {
                    AppendLineAndDelimiter(propertySummary,
                        string.Format("Will try to retrieve NuGet.Exe from source control: {0}", nuGetExeFilePath));
                }
                else
                {
                    AppendLineAndDelimiter(propertySummary,
                        string.Format("Will try to retrieve NuGet.Exe from the following relative path: {0}",
                                      nuGetExeFilePath));
                }
            }

            if (string.IsNullOrWhiteSpace(nuSpecFilePath))
            {
                AppendLineAndDelimiter(propertySummary,
                    "NuSpec File Path is empty.  The package step will be aborted.");
            } 
            else
            {
                if(nuSpecFilePath.StartsWith("$/"))
                {
                    AppendLineAndDelimiter(propertySummary,
                        string.Format("Will try to retrieve NuSpec file from source control: {0}", nuSpecFilePath));
                }
                else
                {
                    AppendLineAndDelimiter(propertySummary,
                        string.Format("Will try to retrieve NuSpec file from the following relative path: {0}",
                                      nuSpecFilePath));
                }
            }

            AppendLineAndDelimiter(propertySummary,
                string.Format("The following relative BasePath will be used as the file source for packaging: {0}", basePath));

            if (string.IsNullOrWhiteSpace(outputDirectory))
            {
                AppendLineAndDelimiter(propertySummary,
                    "The Output Directory (destination folder for the packaging) was not provided.  The package step will be aborted.");
            }
            else
            {
                AppendLineAndDelimiter(propertySummary,
                    string.Format("The following Output Directory will be used as the destination for the packaging process: {0}",
                                  outputDirectory));
            }

            if (string.IsNullOrWhiteSpace(version))
            {
                AppendLineAndDelimiter(propertySummary,
                    "No version number was provided as an override for the packaging process.");
            }
            else
            {
                AppendLineAndDelimiter(propertySummary,
                    string.Format("The following version number will be used in the packaging process: {0}", version));
            }

            if (string.IsNullOrWhiteSpace(additionalOptions))
            {
                AppendLineAndDelimiter(propertySummary,
                    "No additional options were provided for the packaging process.");
            }
            else
            {
                AppendLineAndDelimiter(propertySummary,
                    string.Format("The following options will included on the NuGet command line: '{0}'", additionalOptions));
            }

            if (switchInvokePush)
            {
                AppendLineAndDelimiter(propertySummary,
                    "The Invoke Push switch is set to True.  The Push process will be executed.");

                if (string.IsNullOrWhiteSpace(apiKey))
                {
                    AppendLineAndDelimiter(propertySummary,
                        "No API Key was provided.");
                }
                else
                {
                    if (apiKey.StartsWith("$/"))
                    {
                        AppendLineAndDelimiter(propertySummary,
                                string.Format(
                                    "The API Key will be retrieved from the following file in source control: {0}", apiKey));
                    }
                    else
                    {
                        var regex = new Regex(@"[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}");

                        var match = regex.Match(apiKey);

                        // Was the value passed in actually an API key)
                        if (match.Success)
                        {
                            AppendLineAndDelimiter(propertySummary,
                                "API Key was provided in the build definition.");
                        }
                        else
                        {
                            AppendLineAndDelimiter(propertySummary,
                                string.Format("The API Key will be retrieved from the file in the relative path: {0}", apiKey));
                        }
                    }
                }
            }
            else
            {
                AppendLineAndDelimiter(propertySummary,
                    "The Invoke Push switch is set to False.  The Push process will not be executed.");
            }

            return propertySummary.ToString().Split(new[] {'|'});
        }
    }
}
