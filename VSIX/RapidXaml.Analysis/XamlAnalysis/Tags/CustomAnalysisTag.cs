// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using RapidXaml;
using RapidXamlToolkit.XamlAnalysis.Actions;

namespace RapidXamlToolkit.XamlAnalysis.Tags
{
    public class CustomAnalysisTag : RapidXamlDisplayedTag
    {
        public CustomAnalysisTag(CustomAnalysisTagDependencies deps)
            : base(deps.Span, deps.Snapshot, deps.FileName, deps.ErrorCode, deps.ErrorType, deps.Logger)
        {
            this.SuggestedAction = typeof(CustomAnalysisAction);

            this.Action = deps.Action.Action;
            this.ElementName = deps.ElementName;
            this.Description = deps.Action.Description;
            this.InsertPostion = deps.InsertPos;
            this.ToolTip = deps.Action.ActionText;
            this.ActionText = deps.Action.ActionText;
            this.Name = deps.Action.Name;
            this.Value = deps.Action.Value;
        }

        public ActionType Action { get; }

        public string ElementName { get; }

        public int InsertPostion { get; }

        public string ActionText { get; }

        public string Name { get; }

        public string Value { get; }
    }
}
