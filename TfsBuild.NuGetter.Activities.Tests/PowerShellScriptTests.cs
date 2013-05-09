using System.Activities;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TfsBuild.NuGetter.Activities.Tests
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class PowerShellScriptTests
    {
        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void PowerShellScriptTestsInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void PowerShellScriptTestsCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void PowerShellScriptTestsTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void PowerShellScriptTestsTestCleanup() { }
        //
        #endregion

        //[TestMethod]
        //[DeploymentItem("TfsBuild.NuGetter.Activities.Tests\\TestData\\PSTest1CreateFolders.ps1")]
        //public void PowerShellTests_WhenPassingVariableToScriptShouldReceiveValue()
        //{
        //    using (var myRunSpace = RunspaceFactory.CreateRunspace())
        //    {
        //        myRunSpace.Open();

        //        myRunSpace.SessionStateProxy.SetVariable("ss", "Hello World");

        //        using (var cmdPipeline = myRunSpace.CreatePipeline(@".\PSTest1CreateFolders.ps1 $ss"))
        //        {
        //            var results = cmdPipeline.Invoke();

        //            if (results.Count > 0)
        //            {
        //                var result = results[0].ImmediateBaseObject;
        //            }
        //        }
        //    }
        //}

        [TestMethod]
        [DeploymentItem("TfsBuild.NuGetter.Activities.Tests\\TestData\\PSTest1CreateFolders.ps1")]
        public void PowerShellTests_WhenInvokingPowerShellScriptThroughWorkflowShouldCreateFoldersAsDirectedByScript()
        {
            // Create an instance of our test workflow
            var workflow = new InvokeNugetterPowerShellScriptWorkflow();

            // Create the workflow run-time environment
            var workflowInvoker = new WorkflowInvoker(workflow);

            const string testPowerShellScript = "PSTest1CreateFolders.ps1";

            var newDir = Path.Combine(TestContext.DeploymentDirectory, @"..\..", "TestDir");
            var directoryInfo = Directory.CreateDirectory(newDir);

            var srcFile = Path.Combine(TestContext.DeploymentDirectory, testPowerShellScript);
            var destFile = Path.Combine(directoryInfo.FullName, testPowerShellScript);

            if (File.Exists(destFile))
            {
                File.Delete(destFile);
            }

            File.Copy(srcFile, destFile);

            // Set the workflow arguments
            workflow.DropFolder = directoryInfo.FullName;
            workflow.BinariesFolder = directoryInfo.FullName;
            workflow.SourcesFolder = directoryInfo.FullName;
            workflow.NuGetPrePackageFolder = "NuGetPackage";
            workflow.PowerShellScriptFilePath = Path.Combine(directoryInfo.FullName, testPowerShellScript);

            // Invoke the workflow and capture the outputs
            var output = workflowInvoker.Invoke();

            var result = (string)output["PowerShellTestResult"];

            Assert.AreEqual("NGPS-Success", result);
            Assert.IsTrue(Directory.Exists(Path.Combine(directoryInfo.FullName, "TestFolder")), "TestFolder Creation Failed");
            Assert.IsTrue(Directory.Exists(Path.Combine(directoryInfo.FullName, "TestFolder\\TestFolder2")), "TestFolder\\TestFolder2 Creation Failed");
        }


        [TestMethod]
        [DeploymentItem("TfsBuild.NuGetter.Activities.Tests\\TestData\\PSTest2MoveFiles.ps1")]
        [DeploymentItem("TfsBuild.NuGetter.Activities.Tests\\TestData\\DummyTestFile01.dat")]
        public void PowerShellTests_WhenInvokingMoveFilesPowerShellScriptThroughWorkflowShouldCreateFoldersAndMoveFiles()
        {
            // Create an instance of our test workflow
            var workflow = new InvokeNugetterPowerShellScriptWorkflow();

            // Create the workflow run-time environment
            var workflowInvoker = new WorkflowInvoker(workflow);

            const string testPowerShellScript = "PSTest2MoveFiles.ps1";
            const string dummyTestFile01 = "DummyTestFile01.dat";

            var newDir = Path.Combine(TestContext.DeploymentDirectory, @"..\..", "TestDir");
            DirectoryInfo directoryInfo = Directory.CreateDirectory(newDir);
 
            var srcFile = Path.Combine(TestContext.DeploymentDirectory, testPowerShellScript);
            var destFile = Path.Combine(directoryInfo.FullName, testPowerShellScript);

            if (File.Exists(destFile))
            {
                File.Delete(destFile);
            }

            File.Copy(srcFile, destFile);

            srcFile = Path.Combine(TestContext.DeploymentDirectory, dummyTestFile01);
            destFile = Path.Combine(directoryInfo.FullName, dummyTestFile01);

            if (File.Exists(destFile))
            {
                File.Delete(destFile);
            }

            File.Copy(srcFile, destFile);

            
            // Set the workflow arguments
            workflow.DropFolder = directoryInfo.FullName;
            workflow.BinariesFolder = directoryInfo.FullName;
            workflow.SourcesFolder = directoryInfo.FullName;
            workflow.PowerShellScriptFilePath = Path.Combine(directoryInfo.FullName, testPowerShellScript);

            // Invoke the workflow and capture the outputs
            var output = workflowInvoker.Invoke();

            var result = (string)output["PowerShellTestResult"];

            Assert.AreEqual("NGPS-Success", result);

            var packageFolder = Path.Combine(directoryInfo.FullName, "PackageFolder");

            Assert.IsTrue(Directory.Exists(packageFolder), "TestFolder2 Creation Failed");
            Assert.IsTrue(File.Exists(Path.Combine(packageFolder, "DummyTestFile01.dat")), "DummyTestFile01.dat Move Failed");
        }

    }
}
