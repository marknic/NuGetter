using System.IO;

namespace MyTestNuGetApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var fileIndex = string.Empty;
            var commandLine = "NuGet.exe";

            if (args.Length == 0)
            {
                commandLine += " NoArgs";
            }
            else
            {
                for (var index = 0; index < args.Length; index++)
                {
                    if (index == 0)
                    {
                        var values = args[index].Split('-');

                        fileIndex = values[1];

                        commandLine += string.Format(" {0}", values[0]);
                    }
                    else
                    {
                        var arg = args[index];
                        commandLine += string.Format(" {0}", arg);
                    }
                }
            }

            //File.WriteAllText(string.Format("MyTestNuGetApp-{0}.data", fileIndex), commandLine);
        }
    }
}
