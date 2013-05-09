using System.Activities;
using System.Diagnostics;
using Microsoft.TeamFoundation.Build.Client;

namespace TfsBuild.NuGetter.Activities
{
    public class NuGetProcess : INuGetProcess
    {
        public bool RunNuGetProcess(string nuGetFilePath, string arguments, CodeActivityContext context)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var proc = new Process
            {
                StartInfo =
                {
                    FileName = nuGetFilePath,
                    Arguments = arguments,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardError = true
                }
            };

            proc.Start();

            var errorOutput = proc.StandardError.ReadToEnd();

            // Block for now but force release of the process after 5 minutes - Just in case NuGet fails to return
            //  otherwise it will return as soon as NuGet is done.
            var result = proc.WaitForExit(300000);

            stopwatch.Stop();

            if (context != null)
            {
                if (!string.IsNullOrWhiteSpace(errorOutput))
                {
                    result = false;
                    context.WriteBuildMessage(string.Format("Error reported in the NuGet Process: {0}", errorOutput), BuildMessageImportance.High);
                }

                context.WriteBuildMessage(string.Format("NuGet Process execution time: {0}", stopwatch.Elapsed.Seconds), BuildMessageImportance.High);
            }

            return result;
        }
    }
}
