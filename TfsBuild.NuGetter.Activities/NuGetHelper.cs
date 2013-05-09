using System;
using System.IO;

namespace TfsBuild.NuGetter.Activities
{
    public static class NuGetHelper
    {
        public static string ValidateNuGetExe(string nuGetExeFilePath)
        {
            if (string.IsNullOrWhiteSpace(nuGetExeFilePath))
            {
                nuGetExeFilePath = "nuget.exe";
            }
            else
            {
                if (!File.Exists(nuGetExeFilePath))
                {
                    throw new ArgumentException(string.Format("The NuGet application could not be found in the provided location: {0}", nuGetExeFilePath));
                }
            }

            return nuGetExeFilePath;
        }
    }
}
