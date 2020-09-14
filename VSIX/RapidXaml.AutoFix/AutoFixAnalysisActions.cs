// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

namespace RapidXaml
{
    public class AutoFixAnalysisActions : AnalysisActions
    {
        public static AnalysisActions RenameElement(string newName)
        {
            return RenameElement(
                RapidXamlErrorType.Error,
                string.Empty,
                string.Empty,
                string.Empty,
                newName);
        }

        /// <summary>
        /// Add an attribute to the analyzed element.
        /// </summary>
        /// <param name="addAttributeName">The name for the attribute to be added.</param>
        /// <param name="addAttributeValue">The value for the attribute to be added.</param>
        /// <returns>An AnalysisActions result.</returns>
        public static AnalysisActions AddAttribute(string addAttributeName, string addAttributeValue)
        {
            return AddAttribute(
                RapidXamlErrorType.Error,
                string.Empty,
                string.Empty,
                string.Empty,
                addAttributeName,
                addAttributeValue);
        }

        /// <summary>
        /// Add the provided string as a child of the element.
        /// </summary>
        /// <param name="xaml">A string to add as a child of the element.</param>
        /// <returns>An AnalysisActions result.</returns>
        public static AnalysisActions AddChildString(string xaml)
        {
            return AddChildString(
                RapidXamlErrorType.Error,
                string.Empty,
                string.Empty,
                string.Empty,
                xaml);
        }

        /// <summary>
        /// Add an XML Namespace (and alias) to the document.
        /// </summary>
        /// <param name="alias">The alias to use for the XML Namespace.</param>
        /// <param name="value">The XML Namespace to add.</param>
        /// <returns>An AnalysisActions result.</returns>
        public static AnalysisActions AddXmlns(string alias, string value)
        {
            return AddXmlns(
                RapidXamlErrorType.Error,
                string.Empty,
                string.Empty,
                string.Empty,
                alias,
                value);
        }

        /// <summary>
        /// Remove the specified attribute from the element.
        /// </summary>
        /// <param name="attributeName">The attribute to remove.</param>
        /// <returns>An AnalysisActions result.</returns>
        public static AnalysisActions RemoveAttribute(string attributeName)
        {
            return RemoveAttribute(
                RapidXamlErrorType.Error,
                string.Empty,
                string.Empty,
                string.Empty,
                attributeName);
        }

        /// <summary>
        /// Remove the specific child of the element.
        /// </summary>
        /// <param name="child">The element to remove.</param>
        /// <returns>An AnalysisActions result.</returns>
        public static AnalysisActions RemoveChild(RapidXamlElement child)
        {
            return RemoveChild(
                RapidXamlErrorType.Error,
                string.Empty,
                string.Empty,
                string.Empty,
                child);
        }

        /// <summary>
        /// Replace the element with the specified XAML string.
        /// </summary>
        /// <param name="replacementXaml">The XAML to use instead of the existing element.</param>
        /// <returns>An AnalysisActions result.</returns>
        public static AnalysisActions ReplaceElement(string replacementXaml)
        {
            return ReplaceElement(
                RapidXamlErrorType.Error,
                string.Empty,
                string.Empty,
                string.Empty,
                replacementXaml);
        }
    }
}
