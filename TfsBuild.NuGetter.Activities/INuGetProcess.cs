using System.Activities;

namespace TfsBuild.NuGetter.Activities
{
    public interface INuGetProcess
    {
        bool RunNuGetProcess(string nuGetFilePath, string arguments, CodeActivityContext context);
    }
}
