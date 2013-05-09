using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TfsBuild.NuGetter.Activities.Tests
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class GetVersionPatternTests
    {
        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }


        [TestMethod]
        [DeploymentItem("TfsBuild.NuGetter.Activities.Tests\\TestData\\PackageVersionSeedFile.xml")]
        public void GetVersionPatternTests_WhenFullPathProvidedShouldFindVersionPattern()
        {
            var getVersionPattern = new GetVersionPattern();

            var versionPattern = getVersionPattern.ExtractVersion(Path.Combine(TestContext.DeploymentDirectory, "PackageVersionSeedFile.xml"), "MarkNicNuGetLib", GetVersionPattern.QueryPackageId, TestContext.DeploymentDirectory);

            Assert.AreEqual("7.6.5.4", versionPattern);
        }

        [TestMethod]
        [DeploymentItem("TfsBuild.NuGetter.Activities.Tests\\TestData\\PackageVersionSeedFile.xml")]
        public void GetVersionPatternTests_WhenRelativePathProvidedShouldFindVersionPattern()
        {
            var getVersionPattern = new GetVersionPattern();

            var newPath = Path.Combine(TestContext.DeploymentDirectory, "RelativePath");
            var newFile = Path.Combine(newPath, "PackageVersionSeedFile.xml");

            Directory.CreateDirectory(newPath);
            File.Copy("PackageVersionSeedFile.xml", newFile);

            var versionPattern = getVersionPattern.ExtractVersion("RelativePath\\PackageVersionSeedFile.xml", "MarkNicNuGetLib", GetVersionPattern.QueryPackageId, TestContext.DeploymentDirectory);

            Assert.AreEqual("7.6.5.4", versionPattern);
        }

        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.CSV", "|DataDirectory|\\VersionPatterns.csv", "VersionPatterns#csv", DataAccessMethod.Sequential)]
        [DeploymentItem("TfsBuild.NuGetter.Activities.Tests\\TestData\\VersionPatterns.csv")]
        [TestMethod]
        public void GetVersionPatternTests_WhenPatternProvidedShouldReturnSamePattern()
        {
            var patternToTest = TestContext.DataRow["VersionPattern"].ToString();
            var expectedResult = bool.Parse(TestContext.DataRow["ExpectedResult"].ToString());

            var getVersionPattern = new GetVersionPattern();

            var versionPattern = getVersionPattern.ExtractVersion(patternToTest, "MarkNicNuGetLib", GetVersionPattern.QueryPackageId, TestContext.DeploymentDirectory);

            Assert.AreEqual(expectedResult ? patternToTest : string.Empty, versionPattern);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetVersionPatternTests_WhenPassingNullVersionPatternOrSeedFilePathShouldThrowException()
        {
            var getVersionPattern = new GetVersionPattern();

            getVersionPattern.ExtractVersion(null, "MarkNicNuGetLib", GetVersionPattern.QueryPackageId, TestContext.DeploymentDirectory);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetVersionPatternTests_WhenPassingNullPackageIdPathShouldThrowException()
        {
            var getVersionPattern = new GetVersionPattern();

            getVersionPattern.ExtractVersion("1.2.3.4", null, GetVersionPattern.QueryPackageId, TestContext.DeploymentDirectory);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetVersionPatternTests_WhenPassingNullSourcesDirectoryShouldThrowException()
        {
            var getVersionPattern = new GetVersionPattern();

            getVersionPattern.ExtractVersion("2.3.4.5", "MarkNicNuGetLib", GetVersionPattern.QueryPackageId, null);
        }
    }
}
