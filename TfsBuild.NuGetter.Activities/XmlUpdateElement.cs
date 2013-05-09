using System;
using System.Activities;
using System.Drawing;
using System.IO;
using System.Xml;
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
    [ToolboxBitmap(typeof(XmlUpdateElement), "Resources.version.ico")]
    [BuildActivity(HostEnvironmentOption.All)]
    [BuildExtension(HostEnvironmentOption.All)]
    public sealed class XmlUpdateElement : CodeActivity
    {
        #region Workflow Arguments

        /// <summary>
        /// Specify the path of the file to replace occurences of the regular 
        /// expression with the replacement text
        /// </summary>
        [RequiredArgument]
        public InArgument<string> FilePath { get; set; }

        /// <summary>
        /// Regular expression to search for and replace in the specified
        /// text file.
        /// </summary>
        [RequiredArgument]
        public InArgument<string> XPathExpression { get; set; }

        /// <summary>
        /// The new value to insert into the element
        /// </summary>
        [RequiredArgument]
        public InArgument<string> ReplacementValue { get; set; }

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

            // get the value of the FilePath
            var replacementValue = context.GetValue(ReplacementValue);

            // get the value of the Namespace
            var xmlNamespace = context.GetValue(XmlNamespace);

            // get the value of the Namespace
            var xmlNamespacePrefix = context.GetValue(XmlNamespacePrefix);

            ReplaceXmlElementValue(filePath, xpathExpression, replacementValue, xmlNamespace, xmlNamespacePrefix);
        }

        public void ReplaceXmlElementValue(string filePath, string xpathExpression, string replacementValue, string xmlNamespace, string xmlNamespacePrefix)
        {
            XmlNode elementToChange;

            #region Parameter Validation

            // validate that there is a filename to work with
            if (String.IsNullOrEmpty(filePath))
            {
                throw new ArgumentException("FilePath");
            }

            // validate that there is a file
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("File not found", filePath);
            }

            // Validate that there is an XPath expression for the search
            if (String.IsNullOrEmpty(xpathExpression))
            {
                throw new ArgumentException("xpathExpression");
            }

            if (String.IsNullOrEmpty(replacementValue))
            {
                throw new ArgumentException("replacementValue");
            }

            if (((String.IsNullOrEmpty(xmlNamespace)) && (!String.IsNullOrEmpty(xmlNamespacePrefix))) ||
                ((!String.IsNullOrEmpty(xmlNamespace)) && (String.IsNullOrEmpty(xmlNamespacePrefix))))
            {
                throw new ArgumentException(
                    "To use a namespace, XmlNamespace and XmlNamespacePrefix must both have values");
            }

            #endregion
            
            // Create an XML document
            var document = new XmlDocument();

            // Load the document
            document.Load(filePath);

            // Do we need to process using an XML Namespace?
            if (!string.IsNullOrEmpty(xmlNamespace))
            {
                var xmlnsManager = new XmlNamespaceManager(document.NameTable);

                if (xmlNamespacePrefix != null) xmlnsManager.AddNamespace(xmlNamespacePrefix, xmlNamespace);

                // Do the search
                elementToChange = document.SelectSingleNode(xpathExpression, xmlnsManager);
            }
            else
            {
                // No namespace so just do the search
                elementToChange = document.SelectSingleNode(xpathExpression);
            }

            // Replace the value in the node that was found
            if (elementToChange == null)
            {
                throw new ArgumentException(string.Format("The element '{0}' in the XML file was not found.",
                                                          xpathExpression));
            }

            // No exception occurred so replace the value
            elementToChange.InnerText = replacementValue;
            document.Save(filePath);
        }
    }
}