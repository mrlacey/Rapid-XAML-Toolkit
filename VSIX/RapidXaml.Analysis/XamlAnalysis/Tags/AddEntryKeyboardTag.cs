// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.Text;
using RapidXaml;
using RapidXamlToolkit.Logging;
using RapidXamlToolkit.Resources;
using RapidXamlToolkit.XamlAnalysis.Actions;

namespace RapidXamlToolkit.XamlAnalysis.Tags
{
    public class AddEntryKeyboardTag : RapidXamlDisplayedTag
    {
        public AddEntryKeyboardTag(Span span, ITextSnapshot snapshot, string fileName, string originalXaml, ILogger logger)
            : base(span, snapshot, fileName, "RXT300", TagErrorType.Suggestion, logger)
        {
            this.SuggestedAction = typeof(AddEntryKeyboardAction);
            this.Description = StringRes.UI_XamlAnalysisEntryWithoutKeyboardDescription;
            this.ExtendedMessage = StringRes.UI_XamlAnalysisEntryWithoutKeyboardExtendedMessage;

            var xaml = originalXaml.ToLowerInvariant();
            if (xaml.Contains("email"))
            {
                this.NonDefaultKeyboardSuggestion = "Email";
            }
            else if (xaml.Contains("phone") || xaml.Contains("cell") || xaml.Contains("mobile"))
            {
                this.NonDefaultKeyboardSuggestion = "Telephone";
            }
            else if (xaml.Contains("url"))
            {
                this.NonDefaultKeyboardSuggestion = "Url";
            }
        }

        public int InsertPosition { get; set; }

        public string NonDefaultKeyboardSuggestion { get; internal set; }
    }

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

    public class CustomAnalysisTagDependencies
    {
        public Span Span { get; set; }

        public ITextSnapshot Snapshot { get; set; }

        public string FileName { get; set; }

        public int InsertPos { get; set; }

        public AnalysisAction Action { get; set; }

        public ILogger Logger { get; set; }

        public string ErrorCode { get; set; }

        public TagErrorType ErrorType { get; set; }

        public string ElementName { get; set; }
    }
}
