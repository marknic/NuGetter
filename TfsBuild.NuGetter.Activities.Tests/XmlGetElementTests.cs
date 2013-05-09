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
    public class XmlGetElementTests
    {
        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        [TestMethod]
        [DeploymentItem("TfsBuild.NuGetter.Activities.Tests\\TestData\\TestLib.nuspec")]
        public void XmlGetElementTests_WhenValuesAreValidShouldGetXmlValueProperly()
        {
            var nuGetterActivities = new XmlGetElement();

            var elementValue = nuGetterActivities.GetXmlElementValue("TestLib.nuspec", "ns:package/ns:metadata/ns:version", "http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd", "ns");

            Assert.AreEqual("0.9.4156.32354", elementValue);
        }

        [TestMethod]
        [DeploymentItem("TfsBuild.NuGetter.Activities.Tests\\TestData\\TestLib.nuspec")]
        public void XmlGetElementTests_WhenValuesAreValidUsingWorkflowShouldReplaceXmlValueProperly()
        {
            const string xmlNamespace = "http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd";
            const string xpathExpression = "ns:package/ns:metadata/ns:version";
            const string xmlNamespacePrefix = "ns";
 
            // Create an instance of our test workflow
            var workflow = new CallXmlGetElementWorkflow();

            // Create the workflow run-time environment
            var workflowInvoker = new WorkflowInvoker(workflow);

            // Set the workflow arguments
            workflow.FilePath = "TestLib.nuspec";
            workflow.XmlNamespace = xmlNamespace;
            workflow.XmlNamespacePrefix = xmlNamespacePrefix;
            workflow.XPathExpression = xpathExpression;

            // Invoke the workflow and capture the outputs
            var output = workflowInvoker.Invoke();

            var elementValue = (string)output["ElementValue"];

            Assert.AreEqual("0.9.4156.32354", elementValue);
        }
            
        [TestMethod]
        [DeploymentItem("TfsBuild.NuGetter.Activities.Tests\\TestData\\TestLib.nuspec")]
        public void XmlGetElementTests_WhenUsingV14NuspecFileCheckedShouldGetValueProperly()
        {
            const string xpathExpression = "ns:package/ns:metadata/ns:version";

            var nuGetterActivities = new XmlGetElement();

            var elementValue = nuGetterActivities.GetXmlElementValue("TestLib.nuspec", xpathExpression, "", "");

            Assert.AreEqual("0.9.4156.32354", elementValue);
        }
    

        [TestMethod]
        [DeploymentItem("TfsBuild.NuGetter.Activities.Tests\\TestData\\TestLibV15.nuspec")]
        public void XmlGetElementTests_WhenUsingV15NuspecFileCheckedShouldGetValueProperly()
        {
            const string xpathExpression = "ns:package/ns:metadata/ns:version";

            var nuGetterActivities = new XmlGetElement();

            var elementValue = nuGetterActivities.GetXmlElementValue("TestLibV15.nuspec", xpathExpression, "", "");

            Assert.AreEqual("1.5.4156.32354", elementValue);
        }

        [TestMethod]
        [DeploymentItem("TfsBuild.NuGetter.Activities.Tests\\TestData\\TestLibNoNamespace.nuspec")]
        public void XmlGetElementTests_WhenUsingV15NuspecFileWithNoNamespaceShouldGetValueProperly()
        {
            const string xpathExpression = "ns:package/ns:metadata/ns:version";

            var nuGetterActivities = new XmlGetElement();

            var elementValue = nuGetterActivities.GetXmlElementValue("TestLibNoNamespace.nuspec", xpathExpression, "", "");

            Assert.AreEqual("1.5.0.0", elementValue);
        }

    }
}
