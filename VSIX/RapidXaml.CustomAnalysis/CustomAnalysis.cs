
using System.Collections.Generic;

namespace RapidXaml
{
    public abstract class CustomAnalysis
    {
        public abstract string TargetType();

        public abstract AnalysisActions Analyze(RapidXamlElement element);
    }

    public class AnalysisActions
    {
        public bool IsNone { get; private set; } = false;

        public List<AnalysisAction> Actions { get; private set; } = new List<AnalysisAction>();

        public static AnalysisActions None = AnalysisActions.CreateNone();

        public static AnalysisActions AddAttribute(RapidXamlErrorType errorType, string code, string description, string actionText, string addAttributeName, string addAttributeValue)
        {
            var result = new AnalysisActions();

            result.AddAttribute(errorType, code, description, actionText, addAttributeName, addAttributeValue);

            return result;
        }

        private static AnalysisActions CreateNone()
        {
            return new AnalysisActions { IsNone = true };
        }
    }

    public enum RapidXamlErrorType
    {
        Suggestion,
        Warning,
        Error,
    }

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
    }

    public class AnalysisAction
    {
        public RapidXamlErrorType ErrorType { get; internal set; }

        public ActionType Action { get; internal set; }

        public string Code { get; internal set; }

        public string Description { get; internal set; }

        public string ActionText { get; internal set; }

        public string Name { get; internal set; }

        public string Value { get; internal set; }
    }

    public enum ActionType
    {
        AddAttribute,
    }
}
