using System;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Activities;
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
    /// <summary>
    /// Takes in a version pattern and turns it into a version number.
    /// </summary>
    [ToolboxBitmap(typeof(ConvertVersionPattern), "Resources.nugetter.ico")]
    [BuildActivity(HostEnvironmentOption.All)]
    [BuildExtension(HostEnvironmentOption.All)]
    public sealed class ConvertVersionPattern : CodeActivity
    {
        #region Workflow Arguments

        /// <summary>
        /// The pattern to convert
        /// </summary>
        [RequiredArgument]
        public InArgument<string> VersionPattern { get; set; }

        /// <summary>
        /// TFS build number in case the "B" pattern is used
        /// </summary>
        [RequiredArgument]
        public InArgument<string> BuildNumber { get; set; }

        /// <summary>
        /// The prefix value to add to the build number to make it unique compared to other builds
        /// </summary>
        [RequiredArgument]
        public InArgument<int> BuildNumberPrefix { get; set; }

        /// <summary>
        /// The converted version number 
        /// </summary>
        public OutArgument<string> ConvertedVersionNumber { get; set; }

        #endregion

        /// <summary>
        /// Processes the conversion of the version number
        /// </summary>
        /// <param name="context"></param>
        protected override void Execute(CodeActivityContext context)
        {
            // Get the values passed in
            var versionPattern = context.GetValue(VersionPattern);
            var buildNumber = context.GetValue(BuildNumber);
            var buildNumberPrefix = context.GetValue(BuildNumberPrefix);

            var version = DoConvertVersion(versionPattern, buildNumber, buildNumberPrefix);

            // Return the value back to the workflow
            context.SetValue(ConvertedVersionNumber, version);
        }

        public string DoConvertVersion(string versionPattern, string buildNumber, int buildNumberPrefix)
        {
            var version = new StringBuilder();

            // Validate the version pattern
            if (string.IsNullOrEmpty(versionPattern))
            {
                throw new ArgumentException("VersionPattern must contain the versioning pattern.");
            }
            
            var regex = new Regex(@"^(\d+)(\.\d{1,5})(\.\d{1,5})(\.\d{1,5})(([-|+][a-zA-Z0-9+-]*)|(\*))?$");  //^\d{1,5}\.\d{1,5}\.\d{1,5}\.\d{1,5}$");

            var initialMatch = regex.IsMatch(versionPattern);

            if (initialMatch) return versionPattern;
            
            regex = new Regex(
                    @"^(\d+)?(\.((\d{1,5})|([a-zA-Z]{1,4})))?(\.((\d{1,5})|([a-zA-Z]{1,9})))?(\.((\d)+|(\d+[a-zA-Z0-9+-]*)|([a-zA-Z]{1,9})|([a-zA-Z]{1,9})([-|+][a-zA-Z0-9+-]*)|(\*)))?$");

                    //@"^(\d+)?(\.((\d{1,5})|([a-zA-Z]{1,4})))?(\.((\d{1,5})|([a-zA-Z]{1,4})))?(\.((\d{1,5})|(\d{1,5}[a-zA-Z0-9+-]*)|([a-zA-Z]{1,4})|([a-zA-Z]{1,9})([-|+])([a-zA-Z0-9+-]*)))$");
                    //@"^(\d+)?(\.((\d{1,5})|(\d{1,5}[a-zA-Z]{1,4})))?(\.((\d{1,5})|(\d{1,5}[a-zA-Z]{1,4})))?(\.((\d{1,5})|(\d{1,5}[a-zA-Z0-9+-]*)|([a-zA-Z]{1,4})|([a-zA-Z]{1,4})([-|+])([a-zA-Z0-9+-]*)))$");

            var match = regex.Match(versionPattern);

            // Was the value passed in actually an version pattern)
            if (match.Success)
            {
                // these are the group numbers of any interest
                var matchList = new[] {4, 5, 8, 9, 12, 13, 14, 15};

                version.Append(match.Groups[1].Value);

                foreach (var i in matchList)
                {
                    var pattern = match.Groups[i].Value;

                    if (!string.IsNullOrWhiteSpace(pattern))
                    {
                        var conversionItemUpper = pattern.ToUpper();

                        version.Append(".");

                        switch (conversionItemUpper)
                        {
                            case "YYYYMMDD":
                                version.Append(DateTime.Now.ToString("yyyy"));
                                version.Append(string.Format("{0:00}", DateTime.Now.Month));
                                version.Append(string.Format("{0:00}", DateTime.Now.Day));
                                break;

                            case "YYMMDD":
                                version.Append(DateTime.Now.ToString("yy"));
                                version.Append(string.Format("{0:00}", DateTime.Now.Month));
                                version.Append(string.Format("{0:00}", DateTime.Now.Day));
                                break;

                            case "YYYYMMDDB":
                                version.Append(DateTime.Now.ToString("yyyy"));
                                version.Append(string.Format("{0:00}", DateTime.Now.Month));
                                version.Append(string.Format("{0:00}", DateTime.Now.Day));
                                version.Append(GetBuildFromBuildNumber(buildNumber, buildNumberPrefix));
                                break;

                            case "YYMMDDB":
                                version.Append(DateTime.Now.ToString("yy"));
                                version.Append(string.Format("{0:00}", DateTime.Now.Month));
                                version.Append(string.Format("{0:00}", DateTime.Now.Day));
                                version.Append(GetBuildFromBuildNumber(buildNumber, buildNumberPrefix));
                                break;

                            case "YYYYMM":
                                version.Append(DateTime.Now.ToString("yyyy"));
                                version.Append(string.Format("{0:00}", DateTime.Now.Month));
                                break;

                            case "YYMM":
                                version.Append(DateTime.Now.ToString("yy"));
                                version.Append(string.Format("{0:00}", DateTime.Now.Month));
                                break;

                            case "YYYYMMB":
                                version.Append(DateTime.Now.ToString("yyyy"));
                                version.Append(string.Format("{0:00}", DateTime.Now.Month));
                                version.Append(GetBuildFromBuildNumber(buildNumber, buildNumberPrefix));
                                break;

                            case "YYMMB":
                                version.Append(DateTime.Now.ToString("yy"));
                                version.Append(string.Format("{0:00}", DateTime.Now.Month));
                                version.Append(GetBuildFromBuildNumber(buildNumber, buildNumberPrefix));
                                break;

                            case "JB":
                                version.Append(DateTime.Now.ToString("yy"));
                                version.Append(string.Format("{0:000}", DateTime.Now.DayOfYear));
                                version.Append(GetBuildFromBuildNumber(buildNumber, buildNumberPrefix));
                                break;

                            case "YYYY":
                                version.Append(DateTime.Now.ToString("yyyy"));
                                break;

                            case "YY":
                                version.Append(DateTime.Now.ToString("yy"));
                                break;

                            case "M":
                            case "MM":
                                version.Append(DateTime.Now.Month);
                                break;

                            case "D":
                            case "DD":
                                version.Append(DateTime.Now.Day);
                                break;

                            case "J":
                                version.Append(DateTime.Now.ToString("yy"));
                                version.Append(string.Format("{0:000}", DateTime.Now.DayOfYear));
                                break;

                            case "B":
                                if (string.IsNullOrEmpty(buildNumber))
                                {
                                    throw new ArgumentException("BuildNumber must contain the build value: use $(Rev:.r) at the end of the Build Number Format");
                                }

                                int buildNumberValue;

                                // Attempt to parse - this should probably fails since it will only work if the only thing passed 
                                //  in through the BuildNumber is a number.  This is typically something like: "Buildname.year.month.buildNumber"
                                var isNumber = int.TryParse(buildNumber, out buildNumberValue);

                                if (!isNumber)
                                {
                                    var buildNumberArray = buildNumber.Split('.');

                                    const string exceptionString = "'Build Number Format' in the build definition must end with $(Rev:.r) to use the build number in the version pattern.  Suggested pattern: $(BuildDefinitionName)_$(Year:yyyy).$(Month).$(DayOfMonth)$(Rev:.r)";

                                    if (buildNumberArray.Length < 2)
                                    {
                                        throw new ArgumentException(exceptionString);
                                    }

                                    isNumber = int.TryParse(buildNumberArray[buildNumberArray.Length - 1], out buildNumberValue);

                                    if (isNumber == false)
                                    {
                                        throw new ArgumentException(exceptionString);
                                    }
                                }

                                buildNumberValue = AddBuildNumberPrefixIfNecessary(buildNumberPrefix, buildNumberValue);
                                version.Append(buildNumberValue);

                                break;

                            default:
                                version.Append(pattern);
                                break;
                        }

                        // Add the 2 other components
                        if (i == 15)
                        {
                            for (var j = 16; j <= 17; j++)
                            {
                                if (!string.IsNullOrWhiteSpace(match.Groups[j].Value))
                                {
                                    version.Append(match.Groups[j].Value);
                                }
                            }
                        }
                    }
                }
            }

            return version.ToString();
        }

        private string GetBuildFromBuildNumber(string buildNumber, int buildNumberPrefix)
        {
            if (string.IsNullOrEmpty(buildNumber))
            {
                throw new ArgumentException("BuildNumber must contain the build value: use $(Rev:.r) at the end of the Build Number Format");
            }

            int buildNumberValue;

            // Attempt to parse - this should probably fails since it will only work if the only thing passed 
            //  in through the BuildNumber is a number.  This is typically something like: "Buildname.year.month.buildNumber"
            var isNumber = int.TryParse(buildNumber, out buildNumberValue);

            if (!isNumber)
            {
                var buildNumberArray = buildNumber.Split('.');

                const string exceptionString = "'Build Number Format' in the build definition must end with $(Rev:.r) to use the build number in the version pattern.  Suggested pattern: $(BuildDefinitionName)_$(Year:yyyy).$(Month).$(DayOfMonth)$(Rev:.r)";

                if (buildNumberArray.Length < 2)
                {
                    throw new ArgumentException(exceptionString);
                }

                isNumber = int.TryParse(buildNumberArray[buildNumberArray.Length - 1], out buildNumberValue);

                if (isNumber == false)
                {
                    throw new ArgumentException(exceptionString);
                }
            }

            buildNumberValue = AddBuildNumberPrefixIfNecessary(buildNumberPrefix, buildNumberValue);

            return buildNumberValue.ToString(CultureInfo.InvariantCulture);
        }

        private static int AddBuildNumberPrefixIfNecessary(int buildNumberPrefix, int buildNumberValue)
        {
            // If a BuildNumberPrefix is in place and the BuildNumber pattern is used then 
            // attempt to prefix the build number with the BuildNumberPrefix
            // The value of 10 is used since the prefix would have to be at least 10 to be at all useable
            if (buildNumberPrefix > 0)
            {
                if ((buildNumberValue >= buildNumberPrefix) || (buildNumberPrefix < 10))
                {
                    throw new ArgumentException("When the BuildNumberPrefix is used it must be at least 10 and also larger than the Build Number.");
                }

                // Prefix the build number to set it apart from any other build definition
                buildNumberValue += buildNumberPrefix;
            }

            return buildNumberValue;
        }
    }
}
