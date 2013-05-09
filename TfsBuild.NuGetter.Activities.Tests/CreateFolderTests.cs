using System.Activities;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TfsBuild.NuGetter.Activities.Tests
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class CreateFolderTests
    {
        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }


        [TestMethod]
        public void CreateFoldersTests_WhenCreatingRelativePathShouldUseDropLocation()
        {
            var createFolder = new CreateFolder();

            var folderCreated = createFolder.DoCreateFolder(TestContext.DeploymentDirectory, "TestFolder1");
            var testFolderPath = Path.Combine(TestContext.DeploymentDirectory, "TestFolder1");

            Assert.IsTrue(Directory.Exists(testFolderPath));
            Assert.AreEqual(testFolderPath, folderCreated);
        }

        [TestMethod]
        public void CreateFoldersTests_WhenCreatingRootedPathShouldNotUseDropLocation()
        {
            var createFolder = new CreateFolder();
            var testFolderPath = Path.Combine(TestContext.DeploymentDirectory, "TestFolder2");

            var folderCreated = createFolder.DoCreateFolder(TestContext.DeploymentDirectory, testFolderPath);

            Assert.IsTrue(Directory.Exists(testFolderPath));
            Assert.AreEqual(testFolderPath, folderCreated);
        }

        [TestMethod]
        public void CreateFolderTests_WhenCreatingRelativePathUsingWorkflowShouldUseDropLocation()
        {
            var workflow = new TestCreateFolderWorkflow();

            // Create the workflow run-time environment
            var workflowInvoker = new WorkflowInvoker(workflow);

            // Set the workflow arguments
            workflow.DropLocation = TestContext.DeploymentDirectory;
            workflow.FolderName = "TestFolder1";

            // Invoke the workflow and capture the outputs
            var output = workflowInvoker.Invoke();

            var folderCreated = (string)output["FolderCreated"];

            var expectedResult = Path.Combine(TestContext.DeploymentDirectory, "TestFolder1");

            Assert.IsTrue(Directory.Exists(expectedResult));
            Assert.AreEqual(expectedResult, folderCreated);
        }

    }
}
