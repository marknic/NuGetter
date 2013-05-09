using System;
using System.Activities;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using Microsoft.TeamFoundation.Build.Client;

// ==============================================================================================
// http://tfsversioning.codeplex.com/
//
// Author: Mark S. Nichols
//
// Copyright (c) 2013 Mark Nichols
//
// This source is subject to the Microsoft Permissive License. 
// ==============================================================================================

namespace TfsBuild.NuGetter.Activities
{
    /// <summary>
    /// Used to search the version "seed file" and return the value
    /// </summary>
    [ToolboxBitmap(typeof(XmlGetElement), "Resources.version.ico")]
    [BuildActivity(HostEnvironmentOption.All)]
    [BuildExtension(HostEnvironmentOption.All)]
    public sealed class XmlGetElement : CodeActivity
    {
        #region Workflow Arguments

        /// <summary>
        /// Path of the xml file to search
        /// </summary>
        [RequiredArgument]
        public InArgument<string> FilePath { get; set; }

        /// <summary>
        /// XPath expression to search for and replace in the specified
        /// text file.
        /// </summary>
        [RequiredArgument]
        public InArgument<string> XPathExpression { get; set; }

        /// <summary>
        /// The namespace within the XML doc that contains the element to change.  
        /// Leave empty if no namespace is to be used
        /// </summary>
        [RequiredArgument]
        public InArgument<string> XmlNamespace { get; set; }

        /// <summary>
        /// The namespace within the XML doc that contains the element to change.  
        /// Leave empty if no namespace is to be used
        /// </summary>
        [RequiredArgument]
        public InArgument<string> XmlNamespacePrefix { get; set; }

        /// <summary>
        /// Value is returned throught this property
        /// </summary>
        public OutArgument<string> ElementValue { get; set; }

        #endregion

        /// <summary>
        /// Searches an XML file with an XPath expression
        /// </summary>
        /// <param name="context"></param>
        protected override void Execute(CodeActivityContext context)
        {
            // get the value of the XPathExpression
            var xpathExpression = context.GetValue(XPathExpression);

            // get the value of the FilePath
            var filePath = context.GetValue(FilePath);

            // get the value of the Namespace
            var xmlNamespace = context.GetValue(XmlNamespace);

            // get the value of the Namespace
            var xmlNamespacePrefix = context.GetValue(XmlNamespacePrefix);

            string elementValue;

            try
            {
                elementValue = GetXmlElementValue(filePath, xpathExpression, xmlNamespace, xmlNamespacePrefix);
            }
            catch (FileNotFoundException fileNotFoundException)
            {
                context.WriteBuildMessage(string.Format("In XmlGetElement - {0} - File: {1}", 
                    fileNotFoundException.Message, filePath), BuildMessageImportance.High);
                throw;
            }

            // return the value 
            ElementValue.Set(context, elementValue);
        }

        /// <summary>
        /// Performs recursive search for namespace information and removes it
        /// </summary>
        /// <param name="xElement">XML Element to search</param>
        /// <returns></returns>
        private static XElement RemoveNamespaceInformation(XElement xElement)
        {
            if (!xElement.HasElements)
            {
                var newChildElement = new XElement(xElement.Name.LocalName) { Value = xElement.Value };

                newChildElement = RemoveNamespaceAttributes(xElement, newChildElement);

                return newChildElement;
            }

            var newParentElement = new XElement(xElement.Name.LocalName, xElement.Elements().Select(RemoveNamespaceInformation));

            newParentElement = RemoveNamespaceAttributes(xElement, newParentElement);

            return newParentElement;
        }

        /// <summary>
        /// Remove the namespace elements ONLY
        /// </summary>
        /// <param name="originalElement">element with the attributes still attached</param>
        /// <param name="updatedElement">element with all but namespace attributes attached</param>
        /// <returns></returns>
        private static XElement RemoveNamespaceAttributes(XElement originalElement, XElement updatedElement)
        {
            if (originalElement.HasAttributes)
            {
                foreach (var attribute in originalElement.Attributes().Where(attribute => !attribute.Name.LocalName.StartsWith("xml")))
                {
                    updatedElement.Add(attribute);
                }
            }

            return updatedElement;
        }


        /// <summary>
        /// Gets an element from an XML file based on an XPath expressiong.
        /// This method has been changed from the original that required namespace information.  It does
        /// not require that information any more.  The signature remains the same for compatibility reasons.
        /// </summary>
        /// <param name="filePath">full file path to the XML file</param>
        /// <param name="xpathExpression">XPath search expression</param>
        /// <param name="xmlNamespace">Not required</param>
        /// <param name="xmlNamespacePrefix">Not required</param>
        /// <returns></returns>
        public string GetXmlElementValue(string filePath, string xpathExpression, string xmlNamespace, string xmlNamespacePrefix)
        {
            #region Parameter Validation

            // validate that there is a filename to work with
            if (String.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException("FilePath");
            }

            // validate that there is a file
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("File not found", filePath);
            }

            // Validate that there is an XPath expression for the search
            if (String.IsNullOrWhiteSpace(xpathExpression))
            {
                throw new ArgumentException("xpathExpression");
            }

            #endregion

            var xmlDocString = File.ReadAllText(filePath);

            var updatedXDoc = RemoveNamespaceInformation(XElement.Parse(xmlDocString));

            // Create an XML document
            var document = new XmlDocument();

            // Load the document
            document.LoadXml(updatedXDoc.ToString());

            // Create regex to remove namespace prefix if there are any
            var regex = new Regex(@"\w+[^:]:");
            var newXpathExpression = regex.Replace(xpathExpression, string.Empty);

            // Do the search
            var elementToRetrieve = document.SelectSingleNode(newXpathExpression);

            // Replace the value in the node that was found
            if (elementToRetrieve == null)
            {
                throw new ArgumentException(string.Format("The element '{0}' in the XML file was not found.",
                                                          xpathExpression));
            }

            // No exception occurred so return the value
            return elementToRetrieve.InnerText;
        }
    }
}