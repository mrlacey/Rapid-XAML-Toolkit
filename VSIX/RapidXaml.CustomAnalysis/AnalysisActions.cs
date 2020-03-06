// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;

namespace RapidXaml
{
    public class AnalysisActions
    {
        public static AnalysisActions None
        {
            get
            {
                return AnalysisActions.CreateNone();
            }
        }

        public static AnalysisActions EmptyList
        {
            get
            {
                return AnalysisActions.CreateEmpty();
            }
        }

        public bool IsNone { get; private set; } = false;

        public List<AnalysisAction> Actions { get; private set; } = new List<AnalysisAction>();

        public static AnalysisActions AddAttribute(RapidXamlErrorType errorType, string code, string description, string actionText, string addAttributeName, string addAttributeValue)
        {
            var result = new AnalysisActions();

            result.AddAttribute(errorType, code, description, actionText, addAttributeName, addAttributeValue);

            return result;
        }

        public static AnalysisActions AddChild(RapidXamlErrorType errorType, string code, string description, string actionText, string elementName, List<(string name, string value)> attributes = null)
        {
            var result = new AnalysisActions();

            result.AddChild(errorType, code, description, actionText, elementName, attributes);

            return result;
        }

        public static AnalysisActions AddChildString(RapidXamlErrorType errorType, string code, string description, string actionText, string xaml)
        {
            var result = new AnalysisActions();

            result.AddChildString(errorType, code, description, actionText, xaml);

            return result;
        }

        public static AnalysisActions HighlightWithoutAction(RapidXamlErrorType errorType, string code, string description, RapidXamlElement descendant, string moreInfoUrl = null)
        {
            var result = new AnalysisActions();

            result.HighlightWithoutAction(errorType, code, description, descendant, moreInfoUrl);

            return result;
        }

        public static AnalysisActions RemoveAttribute(RapidXamlErrorType errorType, string code, string description, string actionText, RapidXamlAttribute attribute)
        {
            var result = new AnalysisActions();

            result.RemoveAttribute(errorType, code, description, actionText, attribute);

            return result;
        }

        public static AnalysisActions RemoveAttribute(RapidXamlErrorType errorType, string code, string description, string actionText, string attributeName)
        {
            var result = new AnalysisActions();

            result.RemoveAttribute(errorType, code, description, actionText, attributeName);

            return result;
        }

        public static AnalysisActions RemoveChild(RapidXamlErrorType errorType, string code, string description, string actionText, RapidXamlElement child)
        {
            var result = new AnalysisActions();

            result.RemoveChild(errorType, code, description, actionText, child);

            return result;
        }

        public static AnalysisActions ReplaceElement(RapidXamlErrorType errorType, string code, string description, string actionText, string replacementXaml)
        {
            var result = new AnalysisActions();

            result.ReplaceElement(errorType, code, description, actionText, replacementXaml);

            return result;
        }

        public static AnalysisActions RenameElement(RapidXamlErrorType errorType, string code, string description, string actionText, string newName)
        {
            var result = new AnalysisActions();

            result.RenameElement(errorType, code, description, actionText, newName);

            return result;
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
