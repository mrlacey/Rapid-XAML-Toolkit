// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;

namespace RapidXaml
{
    public static class AnalysisActionsExtensions
    {
        /// <summary>
        /// An attribute should be added to the analyzed element.
        /// </summary>
        /// <param name="analysisActions">The object to add this action to.</param>
        /// <param name="errorType">How the response should be indicated.</param>
        /// <param name="code">A reference code for the issue being highlighted. Can be left blank.</param>
        /// <param name="description">A description of the issue. This will be displayed in the Error List.</param>
        /// <param name="actionText">The text displayed in the quick action.</param>
        /// <param name="addAttributeName">The name for the attribute to be added by the quick action.</param>
        /// <param name="addAttributeValue">The value for the attribute to be added by the quick action.</param>
        /// <param name="moreInfoUrl">(Optional) The URL linked from the error code.</param>
        /// <returns>An AnalysisActions result.</returns>
        public static AnalysisActions AddAttribute(this AnalysisActions analysisActions, RapidXamlErrorType errorType, string code, string description, string actionText, string addAttributeName, string addAttributeValue, string moreInfoUrl = null)
        {
            var result = analysisActions;

            result.Actions.Add(new AnalysisAction
            {
                Action = ActionType.AddAttribute,
                Code = code,
                Description = description,
                ErrorType = errorType,
                ActionText = actionText,
                Name = addAttributeName,
                Value = addAttributeValue,
                MoreInfoUrl = moreInfoUrl,
            });

            return result;
        }

        /// <summary>
        /// Include the addition of an attribute as part of another quick action.
        /// </summary>
        /// <param name="analysisActions">The object to add this action to.</param>
        /// <param name="addAttributeName">The name for the attribute to be added by the quick action.</param>
        /// <param name="addAttributeValue">The value for the attribute to be added by the quick action.</param>
        /// <returns>An AnalysisActions result.</returns>
        public static AnalysisActions AndAddAttribute(this AnalysisActions analysisActions, string addAttributeName, string addAttributeValue)
        {
            return analysisActions.AddSupplementaryAction(
                new AnalysisAction
                {
                    Action = ActionType.AddAttribute,
                    Name = addAttributeName,
                    Value = addAttributeValue,
                });
        }

        /// <summary>
        /// A required child element is missing from the analyzed element.
        /// </summary>
        /// <param name="analysisActions">The object to add this action to.</param>
        /// <param name="errorType">How the response should be indicated.</param>
        /// <param name="code">A reference code for the issue being highlighted. Can be left blank.</param>
        /// <param name="description">A description of the issue. This will be displayed in the Error List.</param>
        /// <param name="actionText">The text displayed in the quick action.</param>
        /// <param name="elementName">The name of the child element to add.</param>
        /// <param name="attributes">(Optional) a collection of names and values specifying attributes for the added element.</param>
        /// <param name="moreInfoUrl">(Optional) The URL linked from the error code.</param>
        /// <returns>An AnalysisActions result.</returns>
        public static AnalysisActions AddChild(this AnalysisActions analysisActions, RapidXamlErrorType errorType, string code, string description, string actionText, string elementName, List<(string name, string value)> attributes = null, string moreInfoUrl = null)
        {
            var result = analysisActions;

            var attrs = new System.Text.StringBuilder();

            if (attributes != null)
            {
                foreach (var attr in attributes)
                {
                    attrs.Append($" {attr.name}=\"{attr.value}\"");
                }
            }

            var content = $"<{elementName}{attrs} />";

            result.Actions.Add(new AnalysisAction
            {
                Action = ActionType.AddChild,
                Code = code,
                Description = description,
                ErrorType = errorType,
                ActionText = actionText,
                Content = content,
                MoreInfoUrl = moreInfoUrl,
            });

            return result;
        }

        /// <summary>
        /// Include the addition of a child element as part of another quick action.
        /// </summary>
        /// <param name="analysisActions">The object to add this action to.</param>
        /// <param name="elementName">The name of the child element to add.</param>
        /// <param name="attributes">(Optional) a collection of names and values specifying attributes for the added element.</param>
        /// <returns>An AnalysisActions result.</returns>
        public static AnalysisActions AndAddChild(this AnalysisActions analysisActions, string elementName, List<(string name, string value)> attributes = null)
        {
            var attrs = new System.Text.StringBuilder();

            foreach (var attr in attributes)
            {
                attrs.Append($" {attr.name}=\"{attr.value}\"");
            }

            var content = $"<{elementName}{attrs} />";

            return analysisActions.AddSupplementaryAction(
                new AnalysisAction
                {
                    Action = ActionType.AddChild,
                    Content = content,
                });
        }

        /// <summary>
        /// Required child content is missing from the analyzed element and the fix will inject arbitrary text.
        /// </summary>
        /// <param name="analysisActions">The object to add this action to.</param>
        /// <param name="errorType">How the response should be indicated.</param>
        /// <param name="code">A reference code for the issue being highlighted. Can be left blank.</param>
        /// <param name="description">A description of the issue. This will be displayed in the Error List.</param>
        /// <param name="actionText">The text displayed in the quick action.</param>
        /// <param name="xaml">A string to add as a child of the element.</param>
        /// <param name="moreInfoUrl">(Optional) The URL linked from the error code.</param>
        /// <returns>An AnalysisActions result.</returns>
        public static AnalysisActions AddChildString(this AnalysisActions analysisActions, RapidXamlErrorType errorType, string code, string description, string actionText, string xaml, string moreInfoUrl = null)
        {
            var result = analysisActions;

            result.Actions.Add(new AnalysisAction
            {
                Action = ActionType.AddChild,
                Code = code,
                Description = description,
                ErrorType = errorType,
                ActionText = actionText,
                Content = xaml,
                MoreInfoUrl = moreInfoUrl,
            });

            return result;
        }

        /// <summary>
        /// Add a string as a child of the element as part of another quick action.
        /// </summary>
        /// <param name="analysisActions">The object to add this action to.</param>
        /// <param name="xaml">A string to add as a child of the element.</param>
        /// <returns>An AnalysisActions result.</returns>
        public static AnalysisActions AndAddChildString(this AnalysisActions analysisActions, string xaml)
        {
            return analysisActions.AddSupplementaryAction(
                new AnalysisAction
                {
                    Action = ActionType.AddChild,
                    Content = xaml,
                });
        }

        /// <summary>
        /// Indicate an issue with the element but don't provide a quick action to fix it.
        /// </summary>
        /// <param name="analysisActions">The object to add this action to.</param>
        /// <param name="errorType">How the response should be indicated.</param>
        /// <param name="code">A reference code for the issue being highlighted. Can be left blank.</param>
        /// <param name="description">A description of the issue. This will be displayed in the Error List.</param>
        /// <param name="descendant">The element to highlight.</param>
        /// <param name="moreInfoUrl">(Optional) The URL linked from the error code.</param>
        /// <returns>An AnalysisActions result.</returns>
        public static AnalysisActions HighlightWithoutAction(this AnalysisActions analysisActions, RapidXamlErrorType errorType, string code, string description, RapidXamlElement descendant, string moreInfoUrl = null)
        {
            var result = analysisActions;

            result.Actions.Add(new AnalysisAction
            {
                Action = ActionType.HighlightWithoutAction,
                Code = code,
                Description = description,
                ErrorType = errorType,
                Location = descendant.Location,
                MoreInfoUrl = moreInfoUrl,
            });

            return result;
        }

        // No AndHighlightWithoutAction option as it does nothing.
        // If combined with Supplementary Actions they won't get executed.

        /// <summary>
        /// A specific attribute of the element should be removed.
        /// </summary>
        /// <param name="analysisActions">The object to add this action to.</param>
        /// <param name="errorType">How the response should be indicated.</param>
        /// <param name="code">A reference code for the issue being highlighted. Can be left blank.</param>
        /// <param name="description">A description of the issue. This will be displayed in the Error List.</param>
        /// <param name="actionText">The text displayed in the quick action.</param>
        /// <param name="attribute">The attribute the quick action will remove.</param>
        /// <param name="moreInfoUrl">(Optional) The URL linked from the error code.</param>
        /// <returns>An AnalysisActions result.</returns>
        public static AnalysisActions RemoveAttribute(this AnalysisActions analysisActions, RapidXamlErrorType errorType, string code, string description, string actionText, RapidXamlAttribute attribute, string moreInfoUrl = null)
        {
            var result = analysisActions;

            result.Actions.Add(new AnalysisAction
            {
                Action = ActionType.RemoveAttribute,
                Code = code,
                Description = description,
                ErrorType = errorType,
                ActionText = actionText,
                Name = attribute?.Name,
                IsInlineAttribute = attribute?.IsInline ?? true,
                Value = attribute?.StringValue,
                MoreInfoUrl = moreInfoUrl,
            });

            return result;
        }

        /// <summary>
        /// Remove an attribute from the element as part of another quick action.
        /// </summary>
        /// <param name="analysisActions">The object to add this action to.</param>
        /// <param name="attribute">The attribute the quick action will remove.</param>
        /// <returns>An AnalysisActions result.</returns>
        public static AnalysisActions AndRemoveAttribute(this AnalysisActions analysisActions, RapidXamlAttribute attribute)
        {
            return analysisActions.AddSupplementaryAction(
                new AnalysisAction
                {
                    Action = ActionType.RemoveAttribute,
                    Name = attribute.Name,
                });
        }

        /// <summary>
        /// A specific attribute of the element should be removed.
        /// </summary>
        /// <param name="analysisActions">The object to add this action to.</param>
        /// <param name="errorType">How the response should be indicated.</param>
        /// <param name="code">A reference code for the issue being highlighted. Can be left blank.</param>
        /// <param name="description">A description of the issue. This will be displayed in the Error List.</param>
        /// <param name="actionText">The text displayed in the quick action.</param>
        /// <param name="attributeName">The name of the attribute the quick action will remove.</param>
        /// <param name="moreInfoUrl">(Optional) The URL linked from the error code.</param>
        /// <returns>An AnalysisActions result.</returns>
        public static AnalysisActions RemoveAttribute(this AnalysisActions analysisActions, RapidXamlErrorType errorType, string code, string description, string actionText, string attributeName, string moreInfoUrl = null)
        {
            var result = analysisActions;

            result.Actions.Add(new AnalysisAction
            {
                Action = ActionType.RemoveAttribute,
                Code = code,
                Description = description,
                ErrorType = errorType,
                ActionText = actionText,
                Name = attributeName,
                MoreInfoUrl = moreInfoUrl,
            });

            return result;
        }

        /// <summary>
        /// Remove an attribute from the element as part of another quick action.
        /// </summary>
        /// <param name="analysisActions">The object to add this action to.</param>
        /// <param name="attributeName">The name of the attribute the quick action will remove.</param>
        /// <returns>An AnalysisActions result.</returns>
        public static AnalysisActions AndRemoveAttribute(this AnalysisActions analysisActions, string attributeName)
        {
            return analysisActions.AddSupplementaryAction(
                new AnalysisAction
                {
                    Action = ActionType.RemoveAttribute,
                    Name = attributeName,
                });
        }

        /// <summary>
        /// A specific child of the element should be removed.
        /// </summary>
        /// <param name="analysisActions">The object to add this action to.</param>
        /// <param name="errorType">How the response should be indicated.</param>
        /// <param name="code">A reference code for the issue being highlighted. Can be left blank.</param>
        /// <param name="description">A description of the issue. This will be displayed in the Error List.</param>
        /// <param name="actionText">The text displayed in the quick action.</param>
        /// <param name="child">The element the quick action will remove.</param>
        /// <param name="moreInfoUrl">(Optional) The URL linked from the error code.</param>
        /// <returns>An AnalysisActions result.</returns>
        public static AnalysisActions RemoveChild(this AnalysisActions analysisActions, RapidXamlErrorType errorType, string code, string description, string actionText, RapidXamlElement child, string moreInfoUrl = null)
        {
            var result = analysisActions;

            result.Actions.Add(new AnalysisAction
            {
                Action = ActionType.RemoveChild,
                Code = code,
                Description = description,
                ErrorType = errorType,
                ActionText = actionText,
                Element = child,
                MoreInfoUrl = moreInfoUrl,
            });

            return result;
        }

        /// <summary>
        /// Remove a specific child element as part of another quick action.
        /// </summary>
        /// <param name="analysisActions">The object to add this action to.</param>
        /// <param name="child">The element the quick action will remove.</param>
        /// <returns>An AnalysisActions result.</returns>
        public static AnalysisActions AndRemoveChild(this AnalysisActions analysisActions, RapidXamlElement child)
        {
            return analysisActions.AddSupplementaryAction(
                new AnalysisAction
                {
                    Action = ActionType.RemoveChild,
                    Element = child,
                });
        }

        /// <summary>
        /// There is an issue that requires completely replacing the XAML for this element.
        /// </summary>
        /// <param name="analysisActions">The object to add this action to.</param>
        /// <param name="errorType">How the response should be indicated.</param>
        /// <param name="code">A reference code for the issue being highlighted. Can be left blank.</param>
        /// <param name="description">A description of the issue. This will be displayed in the Error List.</param>
        /// <param name="actionText">The text displayed in the quick action.</param>
        /// <param name="replacementXaml">The XAML that the quick action should use to replace the existing element.</param>
        /// <param name="moreInfoUrl">(Optional) The URL linked from the error code.</param>
        /// <returns>An AnalysisActions result.</returns>
        public static AnalysisActions ReplaceElement(this AnalysisActions analysisActions, RapidXamlErrorType errorType, string code, string description, string actionText, string replacementXaml, string moreInfoUrl = null)
        {
            var result = analysisActions;

            result.Actions.Add(new AnalysisAction
            {
                Action = ActionType.ReplaceElement,
                Code = code,
                Description = description,
                ErrorType = errorType,
                ActionText = actionText,
                Content = replacementXaml,
                MoreInfoUrl = moreInfoUrl,
            });

            return result;
        }

        // No AndReplaceElement option as it wouldn't make sense to modify
        // an element and then remove it.

        /// <summary>
        /// The name of the element is an issue and should be changed.
        /// </summary>
        /// <param name="analysisActions">The object to add this action to.</param>
        /// <param name="errorType">How the response should be indicated.</param>
        /// <param name="code">A reference code for the issue being highlighted. Can be left blank.</param>
        /// <param name="description">A description of the issue. This will be displayed in the Error List.</param>
        /// <param name="actionText">The text displayed in the quick action.</param>
        /// <param name="newName">The name to be applied by the quick action.</param>
        /// <param name="moreInfoUrl">(Optional) The URL linked from the error code.</param>
        /// <returns>An AnalysisActions result.</returns>
        public static AnalysisActions RenameElement(this AnalysisActions analysisActions, RapidXamlErrorType errorType, string code, string description, string actionText, string newName, string moreInfoUrl = null)
        {
            var result = analysisActions;

            result.Actions.Add(new AnalysisAction
            {
                Action = ActionType.RenameElement,
                Code = code,
                Description = description,
                ErrorType = errorType,
                ActionText = actionText,
                Name = newName,
                MoreInfoUrl = moreInfoUrl,
            });

            return result;
        }

        /// <summary>
        /// Rename the element as part of another quick action.
        /// </summary>
        /// <param name="analysisActions">The object to add this action to.</param>
        /// <param name="newName">The name to be applied by the quick action.</param>
        /// <returns>An AnalysisActions result.</returns>
        public static AnalysisActions AndRenameElement(this AnalysisActions analysisActions, string newName)
        {
            return analysisActions.AddSupplementaryAction(
                new AnalysisAction
                {
                    Action = ActionType.RenameElement,
                    Name = newName,
                });
        }

        private static AnalysisActions AddSupplementaryAction(this AnalysisActions analysisActions, AnalysisAction action)
        {
            var result = analysisActions;

            var actionsCount = result.Actions.Count;

            if (actionsCount > 0)
            {
                result.Actions[actionsCount - 1].SupplementaryActions.Add(action);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("No action found to add to.");
            }

            return result;
        }
    }
}
