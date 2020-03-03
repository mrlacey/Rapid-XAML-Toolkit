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

        public static AnalysisActions IndicateInvalidDescendant(this AnalysisActions analysisActions, RapidXamlErrorType errorType, string code, string description, RapidXamlElement descendant, string moreInfoUrl = null)
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

        public static AnalysisActions Highlight(this AnalysisActions analysisActions, RapidXamlErrorType errorType, string code, string description, string actionText)
        {
            var result = analysisActions;

            result.Actions.Add(new AnalysisAction
            {
                Action = ActionType.HighlightWithoutAction,
                Code = code,
                Description = description,
                ErrorType = errorType,
                ActionText = actionText,
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
            });

            return result;
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
