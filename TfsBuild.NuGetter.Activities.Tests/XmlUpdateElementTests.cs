using System;
using System.Activities;
using System.IO;
using System.Xml;
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
    public class XmlUpdateElementTests
    {
        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        [TestMethod]
        [DeploymentItem("TfsBuild.NuGetter.Activities.Tests\\TestData\\TestLib.nuspec")]
        public void XmlUpdateElementTests_WhenValuesAreValidShouldReplaceXmlValueProperly()
        {
            File.Copy("TestLib.nuspec", "TestLib01.nuspec");

            var nuGetterActivities = new XmlUpdateElement();

            nuGetterActivities.ReplaceXmlElementValue("TestLib01.nuspec", "ns:package/ns:metadata/ns:version", "1.2.3.4", "http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd", "ns");
        }

        [TestMethod]
        [DeploymentItem("TfsBuild.NuGetter.Activities.Tests\\TestData\\TestLib.nuspec")]
        public void XmlUpdateElementTests_WhenValuesAreValidUsingWorkflowShouldReplaceXmlValueProperly()
        {
            const string testFileName = "TestLib02.nuspec";
            const string xmlNamespace = "http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd";
            const string xpathExpression = "ns:package/ns:metadata/ns:version";
            const string xmlNamespacePrefix = "ns";
            const string newVersionNumber = "9.8.7.6";

            File.Copy("TestLib.nuspec", testFileName);

            // Create an instance of our test workflow
            var workflow = new CallXmlUpdateElementWorkflow();

            // Create the workflow run-time environment
            var workflowInvoker = new WorkflowInvoker(workflow);

            // Set the workflow arguments
            workflow.FilePath = testFileName;
            workflow.ReplacementValue = newVersionNumber;
            workflow.XmlNamespace = xmlNamespace;
            workflow.XmlNamespacePrefix = xmlNamespacePrefix;
            workflow.XPathExpression = xpathExpression;

            // Invoke the workflow
            workflowInvoker.Invoke();

            // Create an XML document
            var document = new XmlDocument();

            // Load the document
            document.Load(testFileName);

            var xmlnsManager = new XmlNamespaceManager(document.NameTable);

            xmlnsManager.AddNamespace(xmlNamespacePrefix, xmlNamespace);

            // Do the search
            var elementToChange = document.SelectSingleNode(xpathExpression, xmlnsManager);

            Assert.IsNotNull(elementToChange);

            Assert.AreEqual(newVersionNumber, elementToChange.InnerText);
        }


        [TestMethod]
        [DeploymentItem("TfsBuild.NuGetter.Activities.Tests\\TestData\\NoNamespace.xml")]
        public void XmlUpdateElementTests_WhenValuesAreValidUsingWorkflowNoNamespaceShouldReplaceXmlValueProperly()
        {
            const string testFileName = "TestLib03.nuspec";
            const string xmlNamespace = "";
            const string xpathExpression = "package/metadata/version";
            const string xmlNamespacePrefix = "";
            const string newVersionNumber = "9.8.7.6";

            File.Copy("NoNamespace.xml", testFileName);

            // Create an instance of our test workflow
            var workflow = new CallXmlUpdateElementWorkflow();

            // Create the workflow run-time environment
            var workflowInvoker = new WorkflowInvoker(workflow);

            // Set the workflow arguments
            workflow.FilePath = testFileName;
            workflow.ReplacementValue = newVersionNumber;
            workflow.XmlNamespace = xmlNamespace;
            workflow.XmlNamespacePrefix = xmlNamespacePrefix;
            workflow.XPathExpression = xpathExpression;

            // Invoke the workflow
            workflowInvoker.Invoke();

            // Create an XML document
            var document = new XmlDocument();

            // Load the document
            document.Load(testFileName);

            var xmlnsManager = new XmlNamespaceManager(document.NameTable);

            xmlnsManager.AddNamespace(xmlNamespacePrefix, xmlNamespace);

            // Do the search
            var elementToChange = document.SelectSingleNode(xpathExpression, xmlnsManager);

            Assert.IsNotNull(elementToChange);
            Assert.AreEqual(newVersionNumber, elementToChange.InnerText);
        }



        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.XML", "|DataDirectory|\\EmptyParametersTestValues.xml", "Test", DataAccessMethod.Sequential)]
        [DeploymentItem("TfsBuild.NuGetter.Activities.Tests\\TestData\\EmptyParametersTestValues.xml")]
        [DeploymentItem("TfsBuild.NuGetter.Activities.Tests\\TestData\\TestLib.nuspec")]
        [DeploymentItem("TfsBuild.NuGetter.Activities.Tests\\TestData\\NoNamespace.xml")]
        [TestMethod]
        public void XmlUpdateElementTests_WhenEmptyParametersArePassedInShouldThrowAppropriateException()
        {
            var testFileName = TestContext.DataRow["FilePath"].ToString();
            var xmlNamespace = TestContext.DataRow["XmlNamespace"].ToString();
            var xpathExpression = TestContext.DataRow["XPathExpression"].ToString();
            var xmlNamespacePrefix = TestContext.DataRow["XmlNamespacePrefix"].ToString();
            var newVersionNumber = TestContext.DataRow["ReplacementValue"].ToString();
            var expectedResult = bool.Parse(TestContext.DataRow["ExpectedResult"].ToString());
            var expectedException = TestContext.DataRow["ExpectedException"].ToString();
            
            if (testFileName == "FileNotFound")
            {
                testFileName = "missingfilename.xml";
            }
            else if (!string.IsNullOrEmpty(testFileName))
            {
                var exeptionTestFile = Path.Combine(TestContext.DeploymentDirectory, "ExceptionTestFile.xml");

                if (File.Exists(exeptionTestFile))
                {
                    File.Delete(exeptionTestFile);
                }

                testFileName = Path.Combine(TestContext.DeploymentDirectory, testFileName);
                File.Copy(testFileName, exeptionTestFile);
                //testFileName = exeptionTestFile;
            }

            // Create an instance of our test workflow
            var workflow = new CallXmlUpdateElementWorkflow();

            // Create the workflow run-time environment
            var workflowInvoker = new WorkflowInvoker(workflow);

            // Set the workflow arguments
            workflow.FilePath = testFileName;
            workflow.ReplacementValue = newVersionNumber;
            workflow.XmlNamespace = xmlNamespace;
            workflow.XmlNamespacePrefix = xmlNamespacePrefix;
            workflow.XPathExpression = xpathExpression;

            try
            {
                workflowInvoker.Invoke();

                Assert.IsTrue(expectedResult, "Should have failed");
            }
            catch (Exception exception)
            {
                var exceptionName = exception.GetType().ToString();

                Assert.IsTrue(exceptionName.EndsWith("." + expectedException), "Should have thrown an exception");
                Assert.IsFalse(expectedResult, "Should have succeeded but an exception was thrown: " + exception.Message);
            }
        }
    }
}
