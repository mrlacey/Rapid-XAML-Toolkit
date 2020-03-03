// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;

namespace RapidXaml
{
    public static class AnalysisActionsExtensions
    {
        public static AnalysisActions AddAttribute(this AnalysisActions analysisActions, RapidXamlErrorType errorType, string code, string description, string actionText, string addAttributeName, string addAttributeValue)
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
            });

            return result;
        }

        public static AnalysisActions AndAddAttribute(this AnalysisActions analysisActions, string addAttributeName, string addAttributeValue)
        {
            var result = analysisActions;

            var actionsCount = result.Actions.Count;

            if (actionsCount > 0)
            {
                result.Actions[actionsCount - 1].SupplementaryActions.Add(
                    new AnalysisAction
                    {
                        Action = ActionType.AddAttribute,
                        Name = addAttributeName,
                        Value = addAttributeValue,
                    });
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("No action found to add to.");
            }

            return result;
        }

        // TODO: Need to add AndAddChild
        public static AnalysisActions AddChild(this AnalysisActions analysisActions, RapidXamlErrorType errorType, string code, string description, string actionText, string elementName, List<(string name, string value)> attributes = null)
        {
            var result = analysisActions;

            var attrs = new System.Text.StringBuilder();

            foreach (var attr in attributes)
            {
                attrs.Append($" {attr.name}=\"{attr.value}\"");
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
            });

            return result;
        }

        // TODO: Need to add AndChildString
        public static AnalysisActions AddChildString(this AnalysisActions analysisActions, RapidXamlErrorType errorType, string code, string description, string actionText, string xaml)
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
            });

            return result;
        }

        // No AndIndicateInvalidDescendant option as it does nothing.
        // If combined with Supplementary Actions they won't get executed.
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

        // TODO: Need to add AndRemoveAttribute
        public static AnalysisActions RemoveAttribute(this AnalysisActions analysisActions, RapidXamlErrorType errorType, string code, string description, string actionText, RapidXamlAttribute attribute)
        {
            var result = analysisActions;

            result.Actions.Add(new AnalysisAction
            {
                Action = ActionType.RemoveAttribute,
                Code = code,
                Description = description,
                ErrorType = errorType,
                ActionText = actionText,
                Name = attribute.Name,
            });

            return result;
        }

        // TODO: Need to add AndRemoveAttribute
        public static AnalysisActions RemoveAttribute(this AnalysisActions analysisActions, RapidXamlErrorType errorType, string code, string description, string actionText, string attributeName)
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
            });

            return result;
        }

        // TODO: Need to add AndRemoveChild
        public static AnalysisActions RemoveChild(this AnalysisActions analysisActions, RapidXamlErrorType errorType, string code, string description, string actionText, RapidXamlElement child)
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
            });

            return result;
        }

        // No AndRemoveElement option as it wouldn't make sense to modify an element and then remove it.
        public static AnalysisActions RemoveElement(this AnalysisActions analysisActions, RapidXamlErrorType errorType, string code, string description, string actionText)
        {
            var result = analysisActions;

            result.Actions.Add(new AnalysisAction
            {
                Action = ActionType.RemoveElement,
                Code = code,
                Description = description,
                ErrorType = errorType,
                ActionText = actionText,
            });

            return result;
        }

        // TODO: Need to add AndRenameElement
        public static AnalysisActions RenameElement(this AnalysisActions analysisActions, RapidXamlErrorType errorType, string code, string description, string actionText, RapidXamlElement element, string newName)
        {
            var result = analysisActions;

            result.Actions.Add(new AnalysisAction
            {
                Action = ActionType.RemoveElement,
                Code = code,
                Description = description,
                ErrorType = errorType,
                ActionText = actionText,
                Name = newName,
            });

            return result;
        }
    }
}
