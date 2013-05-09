using System;
using System.Activities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

// ==============================================================================================
// http://tfsversioning.codeplex.com/
//
// Author: Mark S. Nichols
//
// Copyright (c) 2013 Mark Nichols
//
// This source is subject to the Microsoft Permissive License. 
// ==============================================================================================

namespace TfsBuild.NuGetter.Activities.Tests
{
    [TestClass]
    public class XmlGetPackageNuSpecFileTests
    {
        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        [TestMethod]
        [DeploymentItem("TfsBuild.NuGetter.Activities.Tests\\TestData\\NuGetMultiProjectPkgInfo2.xml")]
        public void XmlGetPackageNuSpecFileTests_WhenValuesAreValidUsingWorkflowIndex0ShouldExtractXmlValueProperly()
        {
            var packageInfoFilename = "NuGetMultiProjectPkgInfo2.xml";

            for (var i = 0; i < 2; i++)
            {
                // Create an instance of our test workflow
                var workflow = new InvokeXmlGetPackageNuSpecFileWorkflow();

                // Create the workflow run-time environment
                var workflowInvoker = new WorkflowInvoker(workflow);

                workflow.PackageIndex = i;

                // Set the workflow arguments
                workflow.PackageInfoFilePath = packageInfoFilename;

                // Invoke the workflow and capture the outputs
                var output = workflowInvoker.Invoke();

                var nuSpecFilePathOut = output["NuSpecFilePath"];

                Assert.AreEqual("NuSpecFilePath" + i, nuSpecFilePathOut);
            }

        }


        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        [DeploymentItem("TfsBuild.NuGetter.Activities.Tests\\TestData\\NuGetMultiProjectPkgInfo2.xml")]
        public void XmlGetPackageNuSpecFileTests_WhenIndexOutOfBoundsShouldThrowArgumentException()
        {
            var packageInfoFilename = "NuGetMultiProjectPkgInfo2.xml";

            // Create an instance of our test workflow
            var workflow = new InvokeXmlGetPackageNuSpecFileWorkflow();

            // Create the workflow run-time environment
            var workflowInvoker = new WorkflowInvoker(workflow);

            workflow.PackageIndex = 4;

            // Set the workflow arguments
            workflow.PackageInfoFilePath = packageInfoFilename;

            // Invoke the workflow and capture the outputs
            var output = workflowInvoker.Invoke();

            Assert.IsNotNull(output);
        }


        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        [DeploymentItem("TfsBuild.NuGetter.Activities.Tests\\TestData\\NuGetMultiProjectPkgInfo2.xml")]
        public void XmlGetPackageNuSpecFileTests_WhenNuSpecElementNotIncludedShouldThrowArgumentException()
        {
            var packageInfoFilename = "NuGetMultiProjectPkgInfo2.xml";

            // Create an instance of our test workflow
            var workflow = new InvokeXmlGetPackageNuSpecFileWorkflow();

            // Create the workflow run-time environment
            var workflowInvoker = new WorkflowInvoker(workflow);

            workflow.PackageIndex = 3;

            // Set the workflow arguments
            workflow.PackageInfoFilePath = packageInfoFilename;

            // Invoke the workflow and capture the outputs
            var output = workflowInvoker.Invoke();

            Assert.IsNotNull(output);
        }

    }
}
