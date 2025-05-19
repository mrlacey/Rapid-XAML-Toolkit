// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;

namespace RapidXaml
{
    // TODO: Change all parameter orders so that extendedMessage always comes before moreInfoUrl
    /// <summary>
    /// Result object for custom analyzer responses.
    /// </summary>
    public class AnalysisActions
    {
        private bool isNone = false;

        /// <summary>
        /// Gets an indicator that the analyzer requires no action.
        /// </summary>
        public static AnalysisActions None
        {
            get
            {
                return AnalysisActions.CreateNone();
            }
        }

        /// <summary>
        /// Gets an AnalysisActions object to which actions can be added.
        /// </summary>
        public static AnalysisActions EmptyList
        {
            get
            {
                return AnalysisActions.CreateEmpty();
            }
        }

        /// <summary>
        /// Gets a value indicating whether there are actions to take as a result of the analysis.
        /// </summary>
        public bool IsNone
        {
            get
            {
                return this.isNone && this.Actions.Count == 0;
            }

            private set
            {
                this.isNone = value;
            }
        }

        /// <summary>
        /// Gets a list of the actions that should be performed as a result of the analysis.
        /// </summary>
        public List<AnalysisAction> Actions { get; private set; } = new List<AnalysisAction>();

        /// <summary>
        /// An attribute should be added to the analyzed element.
        /// </summary>
        /// <param name="errorType">How the response should be indicated.</param>
        /// <param name="code">A reference code for the issue being highlighted. Can be left blank.</param>
        /// <param name="description">A description of the issue. This will be displayed in the Error List.</param>
        /// <param name="actionText">The text displayed in the quick action.</param>
        /// <param name="addAttributeName">The name for the attribute to be added by the quick action.</param>
        /// <param name="addAttributeValue">The value for the attribute to be added by the quick action.</param>
        /// <param name="moreInfoUrl">(Optional) The URL linked from the error code.</param>
        /// <param name="extendedMessage">(Optional) Additional explanatory information about why the error is displayed.</param>
        /// <returns>An AnalysisActions result.</returns>
        public static AnalysisActions AddAttribute(RapidXamlErrorType errorType, string code, string description, string actionText, string addAttributeName, string addAttributeValue, string moreInfoUrl = null, string extendedMessage = null)
        {
            var result = new AnalysisActions();

            result.AddAttribute(errorType, code, description, actionText, addAttributeName, addAttributeValue, extendedMessage: extendedMessage, moreInfoUrl: moreInfoUrl);

            return result;
        }

        /// <summary>
        /// A required child element is missing from the analyzed element.
        /// </summary>
        /// <param name="errorType">How the response should be indicated.</param>
        /// <param name="code">A reference code for the issue being highlighted. Can be left blank.</param>
        /// <param name="description">A description of the issue. This will be displayed in the Error List.</param>
        /// <param name="actionText">The text displayed in the quick action.</param>
        /// <param name="elementName">The name of the child element to add.</param>
        /// <param name="attributes">(Optional) a collection of names and values specifying attributes for the added element.</param>
        /// <param name="moreInfoUrl">(Optional) The URL linked from the error code.</param>
        /// <param name="extendedMessage">(Optional) Additional explanatory information about why the error is displayed.</param>
        /// <returns>An AnalysisActions result.</returns>
        public static AnalysisActions AddChild(RapidXamlErrorType errorType, string code, string description, string actionText, string elementName, List<(string Name, string Value)> attributes = null, string moreInfoUrl = null, string extendedMessage = null)
        {
            var result = new AnalysisActions();

            result.AddChild(errorType, code, description, actionText, elementName, attributes, extendedMessage: extendedMessage, moreInfoUrl: moreInfoUrl);

            return result;
        }

        /// <summary>
        /// Required child content is missing from the analyzed element and the fix will inject arbitrary text.
        /// </summary>
        /// <param name="errorType">How the response should be indicated.</param>
        /// <param name="code">A reference code for the issue being highlighted. Can be left blank.</param>
        /// <param name="description">A description of the issue. This will be displayed in the Error List.</param>
        /// <param name="actionText">The text displayed in the quick action.</param>
        /// <param name="xaml">A string to add as a child of the element.</param>
        /// <param name="moreInfoUrl">(Optional) The URL linked from the error code.</param>
        /// <param name="extendedMessage">(Optional) Additional explanatory information about why the error is displayed.</param>
        /// <returns>An AnalysisActions result.</returns>
        public static AnalysisActions AddChildString(RapidXamlErrorType errorType, string code, string description, string actionText, string xaml, string moreInfoUrl = null, string extendedMessage = null)
        {
            var result = new AnalysisActions();

            result.AddChildString(errorType, code, description, actionText, xaml, extendedMessage: extendedMessage, moreInfoUrl: moreInfoUrl);

            return result;
        }

        /// <summary>
        /// Add an XML Namespace (and alias) to the document.
        /// </summary>
        /// <param name="errorType">How the response should be indicated.</param>
        /// <param name="code">A reference code for the issue being highlighted. Can be left blank.</param>
        /// <param name="description">A description of the issue. This will be displayed in the Error List.</param>
        /// <param name="actionText">The text displayed in the quick action.</param>
        /// <param name="alias">The alias to use for the XML Namespace.</param>
        /// <param name="value">The XML Namespace to add.</param>
        /// <param name="moreInfoUrl">(Optional) The URL linked from the error code.</param>
        /// <param name="extendedMessage">(Optional) Additional explanatory information about why the error is displayed.</param>
        /// <returns>An AnalysisActions result.</returns>
        public static AnalysisActions AddXmlns(RapidXamlErrorType errorType, string code, string description, string actionText, string alias, string value, string moreInfoUrl = null, string extendedMessage = null)
        {
            var result = new AnalysisActions();

            result.AddXmlns(errorType, code, description, actionText, alias, value, extendedMessage: extendedMessage, moreInfoUrl: moreInfoUrl);

            return result;
        }

        /// <summary>
        /// Create a resource file entry.
        /// </summary>
        /// <param name="errorType">How the response should be indicated.</param>
        /// <param name="code">A reference code for the issue being highlighted. Can be left blank.</param>
        /// <param name="description">A description of the issue. This will be displayed in the Error List.</param>
        /// <param name="actionText">The text displayed in the quick action.</param>
        /// <param name="resFilePath">The path to the resx or resw file.</param>
        /// <param name="resourceKey">The key (or name) for the entry in the resource file.</param>
        /// <param name="resourceValue">The value for the entry in the resource file.</param>
        /// <param name="moreInfoUrl">(Optional) The URL linked from the error code.</param>
        /// <param name="extendedMessage">(Optional) Additional explanatory information about why the error is displayed.</param>
        /// <returns>An AnalysisActions result.</returns>
        public static AnalysisActions CreateResource(RapidXamlErrorType errorType, string code, string description, string actionText, string resFilePath, string resourceKey, string resourceValue, string moreInfoUrl = null, string extendedMessage = null)
        {
            var result = new AnalysisActions();

            result.CreateResource(errorType, code, description, actionText, resFilePath, resourceKey, resourceValue, extendedMessage: extendedMessage, moreInfoUrl: moreInfoUrl);

            return result;
        }

        /// <summary>
        /// Indicate an issue with the element but don't provide a quick action to fix it.
        /// </summary>
        /// <param name="errorType">How the response should be indicated.</param>
        /// <param name="code">A reference code for the issue being highlighted. Can be left blank.</param>
        /// <param name="description">A description of the issue. This will be displayed in the Error List.</param>
        /// <param name="descendant">The element to highlight.</param>
        /// <param name="moreInfoUrl">(Optional) The URL linked from the error code.</param>
        /// <param name="extendedMessage">(Optional) Additional explanatory information about why the error is displayed.</param>
        /// <returns>An AnalysisActions result.</returns>
        public static AnalysisActions HighlightDescendantWithoutAction(RapidXamlErrorType errorType, string code, string description, RapidXamlElement descendant, string moreInfoUrl = null, string extendedMessage = null)
        {
            var result = new AnalysisActions();

            result.HighlightDescendantWithoutAction(errorType, code, description, descendant, extendedMessage: extendedMessage, moreInfoUrl: moreInfoUrl);

            return result;
        }

        /// <summary>
        /// Indicate an issue with the element but don't provide a quick action to fix it.
        /// </summary>
        /// <param name="errorType">How the response should be indicated.</param>
        /// <param name="code">A reference code for the issue being highlighted. Can be left blank.</param>
        /// <param name="description">A description of the issue. This will be displayed in the Error List.</param>
        /// <param name="moreInfoUrl">(Optional) The URL linked from the error code.</param>
        /// <param name="extendedMessage">(Optional) Additional explanatory information about why the error is displayed.</param>
        /// <returns>An AnalysisActions result.</returns>
        public static AnalysisActions HighlightWithoutAction(RapidXamlErrorType errorType, string code, string description, string moreInfoUrl = null, string extendedMessage = null)
        {
            var result = new AnalysisActions();

            result.HighlightWithoutAction(errorType, code, description, extendedMessage: extendedMessage, moreInfoUrl: moreInfoUrl);

            return result;
        }

        /// <summary>
        /// Indicate an issue with the element but don't provide a quick action to fix it.
        /// </summary>
        /// <param name="errorType">How the response should be indicated.</param>
        /// <param name="code">A reference code for the issue being highlighted. Can be left blank.</param>
        /// <param name="description">A description of the issue. This will be displayed in the Error List.</param>
        /// <param name="attribute">The attribute to highlight.</param>
        /// <param name="moreInfoUrl">(Optional) The URL linked from the error code.</param>
        /// <param name="extendedMessage">(Optional) Additional explanatory information about why the error is displayed.</param>
        /// <returns>An AnalysisActions result.</returns>
        public static AnalysisActions HighlightAttributeWithoutAction(RapidXamlErrorType errorType, string code, string description, RapidXamlAttribute attribute, string moreInfoUrl = null, string extendedMessage = null)
        {
            var result = new AnalysisActions();

            result.HighlightAttributeWithoutAction(errorType, code, description, attribute, extendedMessage: extendedMessage, moreInfoUrl: moreInfoUrl);

            return result;
        }

        /// <summary>
        /// A specific attribute of the element should be removed.
        /// </summary>
        /// <param name="errorType">How the response should be indicated.</param>
        /// <param name="code">A reference code for the issue being highlighted. Can be left blank.</param>
        /// <param name="description">A description of the issue. This will be displayed in the Error List.</param>
        /// <param name="actionText">The text displayed in the quick action.</param>
        /// <param name="attribute">The attribute the quick action will remove.</param>
        /// <param name="moreInfoUrl">(Optional) The URL linked from the error code.</param>
        /// <param name="extendedMessage">(Optional) Additional explanatory information about why the error is displayed.</param>
        /// <returns>An AnalysisActions result.</returns>
        public static AnalysisActions RemoveAttribute(RapidXamlErrorType errorType, string code, string description, string actionText, RapidXamlAttribute attribute, string moreInfoUrl = null, string extendedMessage = null)
        {
            var result = new AnalysisActions();

            result.RemoveAttribute(errorType, code, description, actionText, attribute, extendedMessage: extendedMessage, moreInfoUrl: moreInfoUrl);

            return result;
        }

        /// <summary>
        /// A specific attribute of the element should be removed.
        /// </summary>
        /// <param name="errorType">How the response should be indicated.</param>
        /// <param name="code">A reference code for the issue being highlighted. Can be left blank.</param>
        /// <param name="description">A description of the issue. This will be displayed in the Error List.</param>
        /// <param name="actionText">The text displayed in the quick action.</param>
        /// <param name="attributeName">The name of the attribute the quick action will remove.</param>
        /// <param name="moreInfoUrl">(Optional) The URL linked from the error code.</param>
        /// <param name="extendedMessage">(Optional) Additional explanatory information about why the error is displayed.</param>
        /// <returns>An AnalysisActions result.</returns>
        public static AnalysisActions RemoveAttribute(RapidXamlErrorType errorType, string code, string description, string actionText, string attributeName, string moreInfoUrl = null, string extendedMessage = null)
        {
            var result = new AnalysisActions();

            result.RemoveAttribute(errorType, code, description, actionText, attributeName, extendedMessage: extendedMessage, moreInfoUrl: moreInfoUrl);

            return result;
        }

        /// <summary>
        /// A specific child of the element should be removed.
        /// </summary>
        /// <param name="errorType">How the response should be indicated.</param>
        /// <param name="code">A reference code for the issue being highlighted. Can be left blank.</param>
        /// <param name="description">A description of the issue. This will be displayed in the Error List.</param>
        /// <param name="actionText">The text displayed in the quick action.</param>
        /// <param name="child">The element the quick action will remove.</param>
        /// <param name="moreInfoUrl">(Optional) The URL linked from the error code.</param>
        /// <param name="extendedMessage">(Optional) Additional explanatory information about why the error is displayed.</param>
        /// <returns>An AnalysisActions result.</returns>
        public static AnalysisActions RemoveChild(RapidXamlErrorType errorType, string code, string description, string actionText, RapidXamlElement child, string moreInfoUrl = null, string extendedMessage = null)
        {
            var result = new AnalysisActions();

            result.RemoveChild(errorType, code, description, actionText, child, extendedMessage: extendedMessage, moreInfoUrl: moreInfoUrl);

            return result;
        }

        /// <summary>
        /// There is an issue that requires completely replacing the XAML for this element.
        /// </summary>
        /// <param name="errorType">How the response should be indicated.</param>
        /// <param name="code">A reference code for the issue being highlighted. Can be left blank.</param>
        /// <param name="description">A description of the issue. This will be displayed in the Error List.</param>
        /// <param name="actionText">The text displayed in the quick action.</param>
        /// <param name="replacementXaml">The XAML that the quick action should use to replace the existing element.</param>
        /// <param name="moreInfoUrl">(Optional) The URL linked from the error code.</param>
        /// <param name="extendedMessage">(Optional) Additional explanatory information about why the error is displayed.</param>
        /// <returns>An AnalysisActions result.</returns>
        public static AnalysisActions ReplaceElement(RapidXamlErrorType errorType, string code, string description, string actionText, string replacementXaml, string moreInfoUrl = null, string extendedMessage = null)
        {
            var result = new AnalysisActions();

            result.ReplaceElement(errorType, code, description, actionText, replacementXaml, extendedMessage: extendedMessage, moreInfoUrl: moreInfoUrl);

            return result;
        }

        /// <summary>
        /// Replace the value of an attribute.
        /// </summary>
        /// <param name="errorType">How the response should be indicated.</param>
        /// <param name="code">A reference code for the issue being highlighted. Can be left blank.</param>
        /// <param name="description">A description of the issue. This will be displayed in the Error List.</param>
        /// <param name="actionText">The text displayed in the quick action.</param>
        /// <param name="attributeName">The name of the attribute.</param>
        /// <param name="replacementValue">The new value.</param>
        /// <param name="moreInfoUrl">(Optional) The URL linked from the error code.</param>
        /// <param name="extendedMessage">(Optional) Additional explanatory information about why the error is displayed.</param>
        /// <returns>An AnalysisActions result.</returns>
        public static AnalysisActions ReplaceAttributeValue(RapidXamlErrorType errorType, string code, string description, string actionText, string attributeName, string replacementValue, string moreInfoUrl = null, string extendedMessage = null)
        {
            var result = new AnalysisActions();

            result.ReplaceAttributeValue(errorType, code, description, actionText, attributeName, replacementValue, extendedMessage: extendedMessage, moreInfoUrl: moreInfoUrl);

            return result;
        }

        /// <summary>
        /// The name of the element is an issue and should be changed.
        /// </summary>
        /// <param name="errorType">How the response should be indicated.</param>
        /// <param name="code">A reference code for the issue being highlighted. Can be left blank.</param>
        /// <param name="description">A description of the issue. This will be displayed in the Error List.</param>
        /// <param name="actionText">The text displayed in the quick action.</param>
        /// <param name="newName">The name to be applied by the quick action.</param>
        /// <param name="moreInfoUrl">(Optional) The URL linked from the error code.</param>
        /// <param name="extendedMessage">(Optional) Additional explanatory information about why the error is displayed.</param>
        /// <returns>An AnalysisActions result.</returns>
        public static AnalysisActions RenameElement(RapidXamlErrorType errorType, string code, string description, string actionText, string newName, string moreInfoUrl = null, string extendedMessage = null)
        {
            var result = new AnalysisActions();

            result.RenameElement(errorType, code, description, actionText, newName, extendedMessage: extendedMessage, moreInfoUrl: moreInfoUrl);

            return result;
        }

        /// <summary>
        /// Add the actions from another AnalysisActions object.
        /// </summary>
        /// <param name="otherActions">Another custom anlaysis response.</param>
        public void Add(AnalysisActions otherActions)
        {
            if (otherActions != null && !otherActions.IsNone)
            {
                this.Actions.AddRange(otherActions.Actions);
            }
        }

        private static AnalysisActions CreateNone()
        {
            return new AnalysisActions { IsNone = true };
        }

        private static AnalysisActions CreateEmpty()
        {
            return new AnalysisActions();
        }
    }
}
