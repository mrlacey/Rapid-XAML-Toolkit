// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using RapidXaml;
using RapidXamlToolkit.XamlAnalysis.Actions;

namespace RapidXamlToolkit.XamlAnalysis.Tags
{
    public class CustomAnalysisTag : RapidXamlDisplayedTag
    {
        public CustomAnalysisTag(CustomAnalysisTagDependencies deps)
            : base(deps.ToTagDependencies(), deps.ErrorCode, deps.ErrorType)
        {
            this.SuggestedAction = typeof(CustomAnalysisAction);

            this.Dependencies = deps;

            this.Action = deps.Action.Action;
            this.ElementName = deps.ElementName;
            this.Description = deps.Action.Description;
            this.ExtendedMessage = deps.Action.ExtendedMessage;
            this.InsertPosition = deps.InsertPos;
            this.ToolTip = deps.Action.ActionText ?? deps.Action.Description;
            this.ActionText = deps.Action.ActionText;
            this.Name = deps.Action.Name;
            this.Value = deps.Action.Value;
            this.Content = deps.Action.Content;
            this.Element = deps.Action.Element;
            this.IsInlineAttribute = deps.Action.IsInlineAttribute;
            this.AnalyzedElement = deps.AnalyzedElement;
            this.SupplementaryActions = deps.Action.SupplementaryActions;
            this.AlternativeActions = deps.Action.AlternativeActions;
        }

        public ActionType Action { get; }

        public string ElementName { get; }

        public int InsertPosition { get; }

        public string ActionText { get; }

        public string Name { get; }

        public string Value { get; }

        public string Content { get; }

        public RapidXamlElement Element { get; }

        public bool? IsInlineAttribute { get; }

        public RapidXamlElement AnalyzedElement { get; }

        public List<AnalysisAction> SupplementaryActions { get; }

        public List<AnalysisAction> AlternativeActions { get; }

        private CustomAnalysisTagDependencies Dependencies { get; }

        internal CustomAnalysisTag RecreateForAlternativeAction(AnalysisAction altAction)
        {
            var deps = this.Dependencies;

            deps.Action = altAction;

            return new CustomAnalysisTag(deps);
        }
    }
}
