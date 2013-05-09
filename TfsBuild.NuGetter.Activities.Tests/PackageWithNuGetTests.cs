using System;
using System.Activities;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TfsBuild.NuGetter.Activities.Tests
{
    [DeploymentItem("TfsBuild.NuGetter.Activities.Tests\\TestData\\NuGet.exe")]
    [TestClass]
    public class CallNuGetPackageCommandLineTests
    {
        public TestContext TestContext { get; set; }

        public const string NuGetFile = "NuGet.exe";
        public const string NuGetPackBasePath = "C:\\BuildFolder\\PackFolder";
        public const string NuGetSpecFilePath = "C:\\BuildFolder\\myproject.nuspec";
        public const string NuGetOutputPath = "C:\\BuildFolder\\NuGetPackage";
        public const string NuGetPackageLocation = "C:\\BuildFolder\\NuGetPackage\\mynugetlib.nupkg";
        public const string NuGetPushDestination = "http://localhost/InternalNuget";
        public const string ApiKey = "C4E9B6FC-FFFF-FFFF-FFFF-7BFEF689B33C";
        public const string NuGetVersion = "1.2.3.4";

        private static string GetNugetFilePath(string folderName)
        {
            return Path.Combine(folderName, NuGetFile);
        }

        [DeploymentItem("TfsBuild.NuGetter.Activities.Tests\\TestData\\PackPushTestData.xml")]
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.XML", "|DataDirectory|\\PackPushTestData.xml", "PackTest", DataAccessMethod.Sequential)]
        [TestMethod]
        public void CallNuGetPackageCommandLineTests_WhenCallingPackagingMethodShouldFormatPackParametersCorrectly()
        {
            var nuGetFilePath = string.Empty;

            if (!string.IsNullOrWhiteSpace(TestContext.DataRow["NuGetFilePath"].ToString()))
            {
                nuGetFilePath = GetNugetFilePath(TestContext.DeploymentDirectory);
            }

            var nuGetPackBasePath = TestContext.DataRow["NuGetPackBasePath"].ToString();
            var nuGetSpecFilePath = TestContext.DataRow["NuGetSpecFilePath"].ToString();
            var nuGetOutputPath = TestContext.DataRow["NuGetOutputPath"].ToString();
            var nuGetVersion = TestContext.DataRow["NuGetVersion"].ToString();
 
            var testNuGetProcess = new TestNuGetProcess();

            var callNuGetPackageCommandLine = new CallNuGetPackageCommandLine { NuGetProcess = testNuGetProcess };

            callNuGetPackageCommandLine.NuGetPackaging(nuGetFilePath, nuGetSpecFilePath, nuGetOutputPath, nuGetPackBasePath, nuGetVersion, string.Empty, null);

            const string expectedResult = "pack \"C:\\BuildFolder\\myproject.nuspec\" -OutputDirectory \"C:\\BuildFolder\\NuGetPackage\" -BasePath \"C:\\BuildFolder\\PackFolder\" -version 1.2.3.4";

            //Assert.AreEqual(NuGetFilePath, testNuGetProcess.NuGetFilePath);
            Assert.AreEqual(expectedResult, testNuGetProcess.Arguments.Trim());
        }


        [DeploymentItem("TfsBuild.NuGetter.Activities.Tests\\TestData\\PackPushTestData.xml")]
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.XML", "|DataDirectory|\\PackPushTestData.xml", "PushTest", DataAccessMethod.Sequential)]
        [TestMethod]
        public void CallNuGetPackageCommandLineTests_WhenCallingPushMethodShouldFormatPushParametersCorrectly()
        {
            var nuGetFilePath = string.Empty;

            if (!string.IsNullOrWhiteSpace(TestContext.DataRow["NuGetFilePath"].ToString()))
            {
                nuGetFilePath = GetNugetFilePath(TestContext.DeploymentDirectory);
            }

            var nuGetPushDestination = TestContext.DataRow["NuGetPushDestination"].ToString();
            var nuGetPackageLocation = TestContext.DataRow["NuGetOutputPath"].ToString();
            var apiKey = TestContext.DataRow["ApiKey"].ToString();

            var testNuGetProcess = new TestNuGetProcess();

            var pushWithNuGet = new PushWithNuGet {NuGetProcess = testNuGetProcess};

            if (string.IsNullOrWhiteSpace(nuGetFilePath))
            {
                nuGetFilePath = "nuget.exe";
            }

            pushWithNuGet.NuGetPublishing(nuGetFilePath, nuGetPackageLocation, nuGetPushDestination, apiKey, null);

            string expectedResult = string.Format("push \"{0}\" {1} -s \"{2}\"",
                nuGetPackageLocation, apiKey, nuGetPushDestination);

            //Assert.AreEqual(nuGetFilePath, testNuGetProcess.NuGetFilePath);
            Assert.AreEqual(expectedResult, testNuGetProcess.Arguments);
        }


        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CallNuGetPackageCommandLineTests_WhenCallingPackMethodShouldThrowExceptionWithEmptyNuSpecFilePath()
        {
            var testNuGetProcess = new TestNuGetProcess();

            var callNuGetPackageCommandLine = new CallNuGetPackageCommandLine { NuGetProcess = testNuGetProcess };

            callNuGetPackageCommandLine.NuGetPackaging(NuGetFile, null, NuGetOutputPath, NuGetPackBasePath, NuGetVersion, string.Empty, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CallNuGetPackageCommandLineTests_WhenCallingPackMethodShouldThrowExceptionWithEmptyNuGetOutputPath()
        {
            var testNuGetProcess = new TestNuGetProcess();

            var callNuGetPackageCommandLine = new CallNuGetPackageCommandLine { NuGetProcess = testNuGetProcess };

            callNuGetPackageCommandLine.NuGetPackaging(NuGetFile, NuGetSpecFilePath, null, NuGetPackBasePath, NuGetVersion, string.Empty, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CallNuGetPackageCommandLineTests_WhenCallingPackMethodShouldThrowExceptionWithEmptyNuGetPackBasePath()
        {
            var testNuGetProcess = new TestNuGetProcess();

            var callNuGetPackageCommandLine = new CallNuGetPackageCommandLine { NuGetProcess = testNuGetProcess };

            callNuGetPackageCommandLine.NuGetPackaging(NuGetFile, NuGetSpecFilePath, NuGetOutputPath, null, NuGetVersion, string.Empty, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CallNuGetPackageCommandLineTests_WhenCallingPackMethodShouldThrowExceptionWithEmptyNuGetPackageLocation()
        {
            var testNuGetProcess = new TestNuGetProcess();

            var pushWithNuGet = new PushWithNuGet { NuGetProcess = testNuGetProcess };

            pushWithNuGet.NuGetPublishing(NuGetFile, null, NuGetPushDestination, ApiKey, null);
        }

        // This requirement was removed
        [TestMethod]
        public void CallNuGetPackageCommandLineTests_WhenCallingPackMethodWithGoodUrlShouldPushWithoutApiKey()
        {
            var testNuGetProcess = new TestNuGetProcess();

            var pushWithNuGet = new PushWithNuGet() { NuGetProcess = testNuGetProcess };

            var result = pushWithNuGet.NuGetPublishing(NuGetFile, NuGetPackageLocation, "https://nuget.org/", "", null);
        }

    }

    class TestNuGetProcess : INuGetProcess 
    {
        public string NuGetFilePath { get; private set; }
        public string Arguments { get; private set; }

        public bool RunNuGetProcess(string nuGetFilePath, string arguments, CodeActivityContext context)
        {
            NuGetFilePath = nuGetFilePath;
            Arguments = arguments;

            return true;
        }
    }
}
