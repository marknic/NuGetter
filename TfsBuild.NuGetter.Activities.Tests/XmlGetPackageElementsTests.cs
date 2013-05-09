using System.Activities;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TfsBuild.NuGetter.Activities.Tests.TestData;

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
    public class XmlGetPackageElementsTests
    {
        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        [TestMethod]
        [DeploymentItem("TfsBuild.NuGetter.Activities.Tests\\TestData\\NuGetMultiProjectPkgInfo.xml")]
        public void XmlGetPackageElementsTests_WhenValuesAreValidShouldGetXmlValuesProperly()
        {
            var nuGetterElements = new XmlGetPackageElements();

            Assert.IsNotNull(nuGetterElements);
            //var projectInfo = nuGetterElements.Execute("NuGetMultiProjectPkgInfo.xml", 0);
        }


        [TestMethod]
        [DeploymentItem("TfsBuild.NuGetter.Activities.Tests\\TestData\\NuGetMultiProjectPkgInfo.xml")]
        public void XmlGetPackageElementsTests_WhenValuesAreValidUsingWorkflowIndex0ShouldExtractXmlValueProperly()
        {
            var packageInfoFilename = "NuGetMultiProjectPkgInfo.xml";

            var xdoc = XDocument.Load(packageInfoFilename);

            var nuGetterPackageInfoList = (from xml in xdoc.Elements("NuGetterPackages").Elements("NuGetterPackage")
                                           let name = xml.Attribute("name")
                                           let addnlOptionsElement = xml.Element("AdditionalOptions")
                                           let basePathElement = xml.Element("BasePath")
                                           let nuspecFilePath = xml.Element("NuSpecFilePath")
                                           let powerShellScriptPath = xml.Element("PowerShellScriptPath")
                                           let version = xml.Element("Version")

                                           let outputDirectory = xml.Element("OutputDirectory")
                                           let switchInvokePush = xml.Element("SwitchInvokePush")
                                           let pushDestination = xml.Element("PushDestination")
                                           let switchInvokePowerShell = xml.Element("InvokePowerShell")


                                           select new NuGetterPackageInfoTest
                                           {
                                               Name = name == null ? string.Empty : name.Value,
                                               AdditionalOptions = addnlOptionsElement == null ? null : addnlOptionsElement.Value,
                                               BasePath = basePathElement == null ? null : basePathElement.Value,
                                               NuSpecFilePath = nuspecFilePath == null ? null : nuspecFilePath.Value,
                                               PowerShellScriptPath = powerShellScriptPath == null ? null : powerShellScriptPath.Value,
                                               Version = version == null ? null : version.Value,

                                               OutputDirectory = outputDirectory == null ? null : outputDirectory.Value,
                                               SwitchInvokePush = switchInvokePush == null ? null : switchInvokePush.Value,
                                               PushDestination = pushDestination == null ? null : pushDestination.Value,
                                               SwitchInvokePowerShell = switchInvokePowerShell == null ? null : switchInvokePowerShell.Value,

                                           }).ToList();




            for (var i = 0; i < nuGetterPackageInfoList.Count; i++)
            {
                // Create an instance of our test workflow
                var workflow = new InvokeXmlGetPackageElements();

                // Create the workflow run-time environment
                var workflowInvoker = new WorkflowInvoker(workflow);

                workflow.PackageIndex = i;

                // Set the workflow arguments
                workflow.PackageInfoFilePath = packageInfoFilename;
                workflow.OutputDirectoryFromBldDef = "BldDefOutputDirectory";
                workflow.PushDestinationFromBldDef = "BldDefPushDestination";
                workflow.SwitchInvokePowerShellFromBldDef = true;
                workflow.SwitchInvokePushFromBldDef = true;
                workflow.VersionFromBldDef = "BldDefVersion";
                workflow.AdditionalOptionsFromBldDef = "BldDefAdditionalOptions";
                workflow.BasePathFromBldDef = "BldDefBasePath";
                workflow.PushDestinationFromBldDef = "BldDefPushDestination";
                workflow.PowerShellScriptPathFromBldDef = "BldDefPowerShellScriptPath";
                // Invoke the workflow and capture the outputs
                var output = workflowInvoker.Invoke();

                var nuSpecFilePathOut = output["NuSpecFilePath"];

                string expectedAdditionalOptions;
                string expectedBasePath;

                string expectedInvokePowerShell;
                string expectedInvokePush;
                string expectedOutputDirectory;
                string expectedPowerShellScriptPath;
                string expectedPushDestination;
                string expectedVersion;



                if (i > 0)
                {
                    expectedAdditionalOptions = "AdditionalOptions" + i;
                }
                else
                {
                    expectedAdditionalOptions = "BldDefAdditionalOptions";
                }

                if (i > 1)
                {
                    expectedBasePath = "BasePath" + i;
                }
                else
                {
                    expectedBasePath = "BldDefBasePath";
                }

                expectedInvokePowerShell = i > 3 ? "false" : "true";
                expectedInvokePush = i > 4 ? "false" : "true";

                
                if (i > 5)
                {
                    expectedOutputDirectory = "OutputDirectory" + i;
                }
                else
                {
                    expectedOutputDirectory = "BldDefOutputDirectory";
                }
                
                if (i > 6)
                {
                    expectedPowerShellScriptPath = "PowerShellScriptPath" + i;
                }
                else
                {
                    expectedPowerShellScriptPath = "BldDefPowerShellScriptPath";
                }

                if (i > 7)
                {
                    expectedPushDestination = "PushDestination" + i;
                }
                else
                {
                    expectedPushDestination = "BldDefPushDestination";
                }
                
                if (i > 8)
                {
                    expectedVersion = "Version" + i;
                }
                else
                {
                    expectedVersion = "BldDefVersion";
                }

                Assert.AreEqual(expectedAdditionalOptions, output["AdditionalOptions"]);
                Assert.AreEqual(expectedBasePath, output["BasePath"]);
                Assert.AreEqual(expectedInvokePowerShell, output["SwitchInvokePowerShell"].ToString().ToLower());
                Assert.AreEqual(expectedInvokePush, output["SwitchInvokePush"].ToString().ToLower());
                Assert.AreEqual(expectedOutputDirectory, output["OutputDirectory"]);
                Assert.AreEqual(expectedPowerShellScriptPath, output["PowerShellScriptPath"]);
                Assert.AreEqual(expectedPushDestination, output["PushDestination"]);
                Assert.AreEqual(expectedVersion, output["Version"]);
                Assert.AreEqual("NuSpecFilePath" + i, nuSpecFilePathOut);
                
            }

        }


        [TestMethod]
        [DeploymentItem("TfsBuild.NuGetter.Activities.Tests\\TestData\\NuGetMultiProjectPkgInfo.xml")]
        public void XmlGetPackageElementsTests_WhenCallingElementsCountShouldReturnNumberOfNuGetterPackageElements()
        {
            var nuGetterElements = new XmlGetPackageElementsCount();

            var count = nuGetterElements.Execute("NuGetMultiProjectPkgInfo.xml");

            Assert.AreEqual(10, count);
        }

    }


    public class NuGetterPackageInfoTest
    {
        public string Name { get; set; }
        public string AdditionalOptions { get; set; }
        public string BasePath { get; set; }
        public string NuSpecFilePath { get; set; }
        public string PowerShellScriptPath { get; set; }
        public string Version { get; set; }
        public string OutputDirectory { get; set; }
        public string SwitchInvokePush { get; set; }
        public string PushDestination { get; set; }
        public string SwitchInvokePowerShell { get; set; }
    }
}
