using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TfsBuild.NuGetter.Activities.Tests
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class ConvertVersionPatternTests
    {
        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }


        [TestMethod]
        public void ConvertVersionPatternTests_WhenConvertingLiteralVersionShouldNotChangeAnything()
        {
            var convertVersionPattern = new ConvertVersionPattern();

            var versionPattern = convertVersionPattern.DoConvertVersion("1.2.3.9876", "NuGet Update Test_20111221.4", 0);

            Assert.AreEqual("1.2.3.9876", versionPattern);
        }



        [TestMethod]
        public void ConvertVersionPatternTests_WhenConvertingYYYYNoSemanticShouldReturnFullYear()
        {
            var convertVersionPattern = new ConvertVersionPattern();

            var versionPattern = convertVersionPattern.DoConvertVersion("1.2.yyyy", "NuGet Update Test_20111221.4", 0);

            Assert.AreEqual("1.2." + DateTime.Today.Year, versionPattern);
        }



        [TestMethod]
        public void ConvertVersionPatternTests_WhenConvertingYYYYMMNoSemanticShouldReturnAllDateInfo()
        {
            var convertVersionPattern = new ConvertVersionPattern();

            var versionPattern = convertVersionPattern.DoConvertVersion("1.2.yyyymm", "NuGet Update Test_20111221.4", 0);

            Assert.AreEqual(string.Format("1.2.{0}{1:00}", DateTime.Today.ToString("yyyy"), DateTime.Today.Month), versionPattern);
        }

        [TestMethod]
        public void ConvertVersionPatternTests_WhenConvertingYYYYMMBNoSemanticShouldReturnAllDateInfoPlusBuildNumber()
        {
            var convertVersionPattern = new ConvertVersionPattern();

            var versionPattern = convertVersionPattern.DoConvertVersion("1.2.yyyymmb", "NuGet Update Test_20111221.4", 0);

            Assert.AreEqual(string.Format("1.2.{0}{1:00}4", DateTime.Today.ToString("yyyy"), DateTime.Today.Month), versionPattern);
        }

        [TestMethod]
        public void ConvertVersionPatternTests_WhenConvertingYYMMNoSemanticShouldReturnAllDateInfoSmallYear()
        {
            var convertVersionPattern = new ConvertVersionPattern();

            var versionPattern = convertVersionPattern.DoConvertVersion("1.2.yymm", "NuGet Update Test_20111221.4", 0);

            Assert.AreEqual(string.Format("1.2.{0}{1:00}", DateTime.Today.ToString("yy"), DateTime.Today.Month), versionPattern);
        }

        [TestMethod]
        public void ConvertVersionPatternTests_WhenConvertingYYMMBNoSemanticShouldReturnAllDateInfoSmallYearPlusBuildNumber()
        {
            var convertVersionPattern = new ConvertVersionPattern();

            var versionPattern = convertVersionPattern.DoConvertVersion("1.2.yymmb", "NuGet Update Test_20111221.4", 0);

            Assert.AreEqual(string.Format("1.2.{0}{1:00}4", DateTime.Today.ToString("yy"), DateTime.Today.Month), versionPattern);
        }








        [TestMethod]
        public void ConvertVersionPatternTests_WhenConvertingYYYYMMDDNoSemanticShouldReturnAllDateInfo()
        {
            var convertVersionPattern = new ConvertVersionPattern();

            var versionPattern = convertVersionPattern.DoConvertVersion("1.2.yyyymmdd", "NuGet Update Test_20111221.4", 0);

            Assert.AreEqual(string.Format("1.2.{0}{1:00}{2:00}", DateTime.Today.ToString("yyyy"), DateTime.Today.Month, DateTime.Today.Day), versionPattern);
        }

        [TestMethod]
        public void ConvertVersionPatternTests_WhenConvertingYYYYMMDDBNoSemanticShouldReturnAllDateInfoPlusBuildNumber()
        {
            var convertVersionPattern = new ConvertVersionPattern();

            var versionPattern = convertVersionPattern.DoConvertVersion("1.2.yyyymmddb", "NuGet Update Test_20111221.4", 0);

            Assert.AreEqual(string.Format("1.2.{0}{1:00}{2:00}4", DateTime.Today.ToString("yyyy"), DateTime.Today.Month, DateTime.Today.Day), versionPattern);
        }

        [TestMethod]
        public void ConvertVersionPatternTests_WhenConvertingYYMMDDNoSemanticShouldReturnAllDateInfoSmallYear()
        {
            var convertVersionPattern = new ConvertVersionPattern();

            var versionPattern = convertVersionPattern.DoConvertVersion("1.2.yymmdd", "NuGet Update Test_20111221.4", 0);

            Assert.AreEqual(string.Format("1.2.{0}{1:00}{2:00}", DateTime.Today.ToString("yy"), DateTime.Today.Month, DateTime.Today.Day), versionPattern);
        }

        [TestMethod]
        public void ConvertVersionPatternTests_WhenConvertingYYMMDDBNoSemanticShouldReturnAllDateInfoSmallYearPlusBuildNumber()
        {
            var convertVersionPattern = new ConvertVersionPattern();

            var versionPattern = convertVersionPattern.DoConvertVersion("1.2.yymmddb", "NuGet Update Test_20111221.4", 0);

            Assert.AreEqual(string.Format("1.2.{0}{1:00}{2:00}4", DateTime.Today.ToString("yy"), DateTime.Today.Month, DateTime.Today.Day), versionPattern);
        }

        [TestMethod]
        public void ConvertVersionPatternTests_WhenConvertingJBShouldNoSemanticReturnJulianDatePlusBuildNumber()
        {
            var convertVersionPattern = new ConvertVersionPattern();

            var versionPattern = convertVersionPattern.DoConvertVersion("1.2.jb", "NuGet Update Test_20111221.4", 0);

            Assert.AreEqual(string.Format("1.2.{0}{1:000}4", DateTime.Today.ToString("yy"), DateTime.Today.DayOfYear), versionPattern);
        }





        [TestMethod]
        public void ConvertVersionPatternTests_WhenConvertingYYYYPlusSemanticShouldReturnFullYear()
        {
            var convertVersionPattern = new ConvertVersionPattern();

            var versionPattern = convertVersionPattern.DoConvertVersion("1.2.yyyy-beta", "NuGet Update Test_20111221.4", 0);

            Assert.AreEqual("1.2." + DateTime.Today.Year + "-beta", versionPattern);
        }

        [TestMethod]
        public void ConvertVersionPatternTests_WhenConvertingYYYYMMDDPlusSemanticShouldReturnAllDateInfo()
        {
            var convertVersionPattern = new ConvertVersionPattern();

            var versionPattern = convertVersionPattern.DoConvertVersion("1.2.yyyymmdd-beta", "NuGet Update Test_20111221.4", 0);

            Assert.AreEqual(string.Format("1.2.{0}{1:00}{2:00}-beta", DateTime.Today.ToString("yyyy"), DateTime.Today.Month, DateTime.Today.Day), versionPattern);
        }

        [TestMethod]
        public void ConvertVersionPatternTests_WhenConvertingYYYYMMDDBPlusSemanticShouldReturnAllDateInfoPlusBuildNumber()
        {
            var convertVersionPattern = new ConvertVersionPattern();

            var versionPattern = convertVersionPattern.DoConvertVersion("1.2.yyyymmddb-beta", "NuGet Update Test_20111221.4", 0);

            Assert.AreEqual(string.Format("1.2.{0}{1:00}{2:00}4-beta", DateTime.Today.ToString("yyyy"), DateTime.Today.Month, DateTime.Today.Day), versionPattern);
        }

        [TestMethod]
        public void ConvertVersionPatternTests_WhenConvertingYYMMDDPlusSemanticShouldReturnAllDateInfoSmallYear()
        {
            var convertVersionPattern = new ConvertVersionPattern();

            var versionPattern = convertVersionPattern.DoConvertVersion("1.2.yymmdd-beta", "NuGet Update Test_20111221.4", 0);

            Assert.AreEqual(string.Format("1.2.{0}{1:00}{2:00}-beta", DateTime.Today.ToString("yy"), DateTime.Today.Month, DateTime.Today.Day), versionPattern);
        }

        [TestMethod]
        public void ConvertVersionPatternTests_WhenConvertingYYMMDDBPlusSemanticShouldReturnAllDateInfoSmallYearPlusBuildNumber()
        {
            var convertVersionPattern = new ConvertVersionPattern();

            var versionPattern = convertVersionPattern.DoConvertVersion("1.2.yymmddb-beta", "NuGet Update Test_20111221.4", 0);

            Assert.AreEqual(string.Format("1.2.{0}{1:00}{2:00}4-beta", DateTime.Today.ToString("yy"), DateTime.Today.Month, DateTime.Today.Day), versionPattern);
        }

        [TestMethod]
        public void ConvertVersionPatternTests_WhenConvertingJBShouldPlusSemanticReturnJulianDatePlusBuildNumber()
        {
            var convertVersionPattern = new ConvertVersionPattern();

            var versionPattern = convertVersionPattern.DoConvertVersion("1.2.jb-beta", "NuGet Update Test_20111221.4", 0);

            Assert.AreEqual(string.Format("1.2.{0}{1:000}4-beta", DateTime.Today.ToString("yy"), DateTime.Today.DayOfYear), versionPattern);
        }


        [TestMethod]
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.CSV", "|DataDirectory|\\SemanticVersionPatterns.csv", "SemanticVersionPatterns#csv", DataAccessMethod.Sequential)]
        [DeploymentItem("TfsBuild.NuGetter.Activities.Tests\\TestData\\SemanticVersionPatterns.csv")]
        public void ConvertVersionPatternTests_WhenTestingGoodPatternsShouldValidateProperly()
        {
            var patternToTest = TestContext.DataRow["VersionPattern"].ToString();
            var expectedResult = TestContext.DataRow["ExpectedResult"].ToString();

            var convertVersionPattern = new ConvertVersionPattern();

            var versionPattern = convertVersionPattern.DoConvertVersion(patternToTest, "NuGet Update Test_20111221.4", 0);

            Assert.AreEqual(expectedResult, versionPattern);
        }

        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.CSV", "|DataDirectory|\\VersionPatternsWithBuildNumberPrefix.csv", "VersionPatternsWithBuildNumberPrefix#csv", DataAccessMethod.Sequential)]
        [DeploymentItem("TfsBuild.NuGetter.Activities.Tests\\TestData\\VersionPatternsWithBuildNumberPrefix.csv")]
        [TestMethod]
        public void ConvertVersionPatternTests_WhenPatternContainsBuildNumberPrefixShouldAddPrefixToLastVersionValue()
        {
            //VersionPattern, BuildNumber, BuildNumberPrefix, ExpectedResult
            var patternToTest = TestContext.DataRow["VersionPattern"].ToString();
            var buildNumber = Int32.Parse(TestContext.DataRow["BuildNumber"].ToString());
            var buildNumberPrefix = Int32.Parse(TestContext.DataRow["BuildNumberPrefix"].ToString());
            var expectedResult = TestContext.DataRow["ExpectedResult"].ToString();

            var convertVersionPattern = new ConvertVersionPattern();

            var versionPattern = convertVersionPattern.DoConvertVersion(patternToTest, "NuGet Update Test_20111221." + buildNumber, buildNumberPrefix);

            Assert.AreEqual(expectedResult, versionPattern);
        }


        //[DataSource("Microsoft.VisualStudio.TestTools.DataSource.CSV", "|DataDirectory|\\VersionPatterns.csv", "VersionPatterns#csv", DataAccessMethod.Sequential)]
        //[DeploymentItem("TfsBuild.NuGetter.Activities.Tests\\TestData\\VersionPatterns.csv")]
        //[TestMethod]
        //public void ConvertVersionPatternTests_WhenPatternProvidedShouldReturnSamePattern()
        //{
        //    var patternToTest = TestContext.DataRow["VersionPattern"].ToString();
        //    var expectedResult = bool.Parse(TestContext.DataRow["ExpectedResult"].ToString());

        //    var getVersionPattern = new GetVersionPattern();

        //    var versionPattern = getVersionPattern.ExtractVersion(patternToTest, "MarkNicNuGetLib", GetVersionPattern.QueryPackageId, TestContext.DeploymentDirectory);

        //    if (expectedResult)
        //    {
        //        Assert.AreEqual(patternToTest, versionPattern);
        //    }
        //    else
        //    {
        //        Assert.AreEqual(string.Empty, versionPattern);
        //    }
        //}
    }
}
