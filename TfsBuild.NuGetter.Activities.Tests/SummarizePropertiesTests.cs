using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TfsBuild.NuGetter.Activities.Tests
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class SummarizePropertiesTests
    {
        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }


        [TestMethod]
        [DeploymentItem("TfsBuild.NuGetter.Activities.Tests\\TestData\\PSTest1CreateFolders.ps1")]
        public void SummarizePropertiesTests_WhenShould()
        {
            var summarizeProperties = new SummarizeProperties();

            var resultString = summarizeProperties.SummarizePropertyValues(true, "relativepath\\powershellscript.ps1",
                                                        "relativepath\\nuget.exe",
                                                        "relativepath\\project.nuspec", "BasePathFolder",
                                                        "OutputDirectory", "1.2.3.4", true,
                                                        "{E043FC83-D120-4A3D-AD6D-6B79BE430350}",
                                                        "relativepathdestination", string.Empty);

            Assert.IsNotNull(resultString);

            resultString = summarizeProperties.SummarizePropertyValues(true, "relativepath\\powershellscript.ps1",
                                                        "",
                                                        "relativepath\\project.nuspec", "BasePathFolder",
                                                        "OutputDirectory", "1.2.3.4", true,
                                                        "{E043FC83-D120-4A3D-AD6D-6B79BE430350}",
                                                        "relativepathdestination", "-NoDefaultExcludes");

            Assert.IsNotNull(resultString);
        }


    }
}
