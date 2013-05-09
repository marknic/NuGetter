using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace TfsBuild.NuGetter.Activities.Tests
{
    [TestClass]
    public class GetApiKeyTests
    {
        public TestContext TestContext { get; set; }

        [DeploymentItem("TfsBuild.NuGetter.Activities.Tests\\TestData\\ApiKeyDataFile.txt")]
        [DeploymentItem("TfsBuild.NuGetter.Activities.Tests\\TestData\\GetApiKeyData.xml")]
        [DeploymentItem("TfsBuild.NuGetter.Activities.Tests\\TestData\\ApiKeyDataFileNoValidKey.txt")]
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.XML", "|DataDirectory|\\GetApiKeyData.xml", "Test", DataAccessMethod.Sequential)]
        [TestMethod]
        public void GetApiKeyTests_WhenGettingTheApiKeyShouldWorkWithFilesAndDirectKeyData()
        {
            var apiKeyOrFile = TestContext.DataRow["ApiKeyOrFile"].ToString();
            var passFullPath = bool.Parse(TestContext.DataRow["PassFullPath"].ToString());
            var apiKeyValue = TestContext.DataRow["ApiKeyValue"].ToString();
            var expectedResult = bool.Parse(TestContext.DataRow["ExpectedResult"].ToString());

            var getApiKey = new GetApiKey();

            string result;
            if (passFullPath && !string.IsNullOrWhiteSpace(apiKeyOrFile))
            {
                apiKeyOrFile = Path.Combine(TestContext.DeploymentDirectory, apiKeyOrFile);
            }

            var keyValueReturned = getApiKey.ExtractApiKey(apiKeyOrFile, TestContext.DeploymentDirectory, out result);

            if (expectedResult)
            {
                Assert.AreEqual(apiKeyValue, keyValueReturned);
            }
            else
            {
                Assert.AreEqual(string.Empty, keyValueReturned);
            }
        }
    }
}
