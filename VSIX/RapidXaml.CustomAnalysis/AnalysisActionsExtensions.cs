// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

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

        public static AnalysisActions AddInvalidDescendant(this AnalysisActions analysisActions, RapidXamlErrorType errorType, string code, string description, RapidXamlElement descendant)
        {
            var result = analysisActions;

            result.Actions.Add(new AnalysisAction
            {
                Action = ActionType.HighlightWithoutAction,
                Code = code,
                Description = description,
                ErrorType = errorType,
                Location = descendant.Location,
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
    }
}
