// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using RapidXaml;
using RapidXamlToolkit.XamlAnalysis.Actions;

namespace RapidXamlToolkit.XamlAnalysis.Tags
{
    public class CustomAnalysisTag : RapidXamlDisplayedTag
    {
        public CustomAnalysisTag(CustomAnalysisTagDependencies deps)
            : base(deps.Span, deps.Snapshot, deps.FileName, deps.ErrorCode, deps.ErrorType, deps.Logger, deps.Action.MoreInfoUrl, deps.CustomFeatureUsageValue)
        {
            this.SuggestedAction = typeof(CustomAnalysisAction);

            this.Action = deps.Action.Action;
            this.ElementName = deps.ElementName;
            this.Description = deps.Action.Description;
            this.InsertPosition = deps.InsertPos;
            this.ToolTip = deps.Action.ActionText ?? deps.Action.Description;
            this.ActionText = deps.Action.ActionText;
            this.Name = deps.Action.Name;
            this.Value = deps.Action.Value;
            this.Content = deps.Action.Content;
            this.Element = deps.Action.Element;
            this.IsInlineAttribute = deps.Action.IsInlineAttribute;
            this.AnalyzedElement = deps.AnalyzedElement;
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
    }
}
