// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.Threading;
using RapidXamlToolkit.Resources;
using RapidXamlToolkit.VisualStudioIntegration;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.XamlAnalysis.Actions
{
    public class AddEntryKeyboardAction : BaseSuggestedAction
    {
        public AddEntryKeyboardAction(string file, AddEntryKeyboardTag tag, string keyboardName)
            : base(file)
        {
            this.DisplayText = StringRes.UI_AddEntryKeyboard.WithParams(keyboardName);
            this.Tag = tag;
            this.KeyBoardName = keyboardName;
        }

        public AddEntryKeyboardTag Tag { get; private set; }

        public string KeyBoardName { get; private set; }

        public static AddEntryKeyboardAction[] Create(AddEntryKeyboardTag tag, string file)
        {
            var result = new List<AddEntryKeyboardAction>
            {
                new AddEntryKeyboardAction(file, tag, "Default"),
            };

            if (!string.IsNullOrWhiteSpace(tag.NonDefaultKeyboardSuggestion))
            {
                result.Add(new AddEntryKeyboardAction(file, tag, tag.NonDefaultKeyboardSuggestion));
            }

            return result.ToArray();
        }

        public override void Execute(CancellationToken cancellationToken)
        {
            var vs = new VisualStudioTextManipulation(ProjectHelpers.Dte);
            vs.StartSingleUndoOperation(StringRes.UI_UndoContextAddEntryKeyboard);
            try
            {
                var lineNumber = this.Tag.Snapshot.GetLineNumberFromPosition(this.Tag.InsertPosition) + 1;

                vs.ReplaceInActiveDocOnLine("<Entry ", $"<Entry Keyboard=\"{this.KeyBoardName}\" ", lineNumber);

                RapidXamlDocumentCache.TryUpdate(this.File);
            }
            finally
            {
                vs.EndSingleUndoOperation();
            }
        }
    }

    public class CustomAnalysisAction : BaseSuggestedAction
    {
        public CustomAnalysisAction(string file, CustomAnalysisTag tag)
            : base(file)
        {
            this.Tag = tag;
            this.DisplayText = tag.ActionText;
        }

        public CustomAnalysisTag Tag { get; }

        public static CustomAnalysisAction Create(CustomAnalysisTag tag, string file)
        {
            return new CustomAnalysisAction(file, tag);
        }

        public override void Execute(CancellationToken cancellationToken)
        {
            var vs = new VisualStudioTextManipulation(ProjectHelpers.Dte);
            vs.StartSingleUndoOperation(this.DisplayText);
            try
            {
                switch (this.Tag.Action)
                {
                    case RapidXaml.ActionType.AddAttribute:
                        var lineNumber = this.Tag.Snapshot.GetLineNumberFromPosition(this.Tag.InsertPostion) + 1;

                        var before = $"<{this.Tag.ElementName} ";
                        var after = $"<{this.Tag.ElementName} {this.Tag.Name}=\"{this.Tag.Value}\" ";

                        vs.ReplaceInActiveDocOnLine(before, after, lineNumber);

                        break;
                }

                RapidXamlDocumentCache.TryUpdate(this.File);
            }
            finally
            {
                vs.EndSingleUndoOperation();
            }
        }
    }
}
