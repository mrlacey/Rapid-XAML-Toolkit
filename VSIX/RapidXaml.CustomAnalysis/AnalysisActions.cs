// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
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

        public static AnalysisActions AddInvalidDescendant(RapidXamlErrorType errorType, string code, string description, RapidXamlElement descendant)
        {
            var result = new AnalysisActions();

            result.AddInvalidDescendant(errorType, code, description, descendant);

            return result;
        }

        public static AnalysisActions Highlight(RapidXamlErrorType errorType, string code, string description, string actionText)
        {
            var result = new AnalysisActions();

            result.Highlight(errorType, code, description, actionText);

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
