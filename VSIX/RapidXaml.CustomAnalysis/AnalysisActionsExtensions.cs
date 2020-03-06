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
            return analysisActions.AddSupplementaryAction(
                new AnalysisAction
                {
                    Action = ActionType.AddAttribute,
                    Name = addAttributeName,
                    Value = addAttributeValue,
                });
        }

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

        public static AnalysisActions AndAddChildString(this AnalysisActions analysisActions, string xaml)
        {
            return analysisActions.AddSupplementaryAction(
                new AnalysisAction
                {
                    Action = ActionType.AddChild,
                    Content = xaml,
                });
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
                IsInlineAttribute = attribute.IsInline,
                Value = attribute.StringValue,
            });

            return result;
        }

        public static AnalysisActions AndRemoveAttribute(this AnalysisActions analysisActions, RapidXamlAttribute attribute)
        {
            return analysisActions.AddSupplementaryAction(
                new AnalysisAction
                {
                    Action = ActionType.RemoveAttribute,
                    Name = attribute.Name,
                });
        }

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

        public static AnalysisActions AndRemoveAttribute(this AnalysisActions analysisActions, string attributeName)
        {
            return analysisActions.AddSupplementaryAction(
                new AnalysisAction
                {
                    Action = ActionType.RemoveAttribute,
                    Name = attributeName,
                });
        }

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

        public static AnalysisActions AndRemoveChild(this AnalysisActions analysisActions, RapidXamlElement child)
        {
            return analysisActions.AddSupplementaryAction(
                new AnalysisAction
                {
                    Action = ActionType.RemoveChild,
                    Element = child,
                });
        }

        // No AndReplaceElement option as it wouldn't make sense to modify an element and then remove it.
        public static AnalysisActions ReplaceElement(this AnalysisActions analysisActions, RapidXamlErrorType errorType, string code, string description, string actionText, string replacementXaml)
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
            });

            return result;
        }

        public static AnalysisActions RenameElement(this AnalysisActions analysisActions, RapidXamlErrorType errorType, string code, string description, string actionText, string newName)
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
            });

            return result;
        }

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
