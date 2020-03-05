// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System.Threading;
using RapidXamlToolkit.VisualStudioIntegration;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.XamlAnalysis.Actions
{
    public class CustomAnalysisAction : BaseSuggestedAction
    {
        public CustomAnalysisAction(string file, CustomAnalysisTag tag)
            : base(file)
        {
            this.Tag = tag;
            this.DisplayText = tag.ActionText;

            this.CustomFeatureUsageOverride = tag.CustomFeatureUsageOverride;
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

                    case RapidXaml.ActionType.AddChild:
                        // TODO: implement AddChild functionality
                        break;

                    case RapidXaml.ActionType.HighlightWithoutAction:
                        // As the name applies, do nothing.
                        break;

                    case RapidXaml.ActionType.RemoveAttribute:
                        // TODO: implement RemoveAttribute functionality
                        break;

                    case RapidXaml.ActionType.RemoveChild:
                        // TODO: implement RemoveChild functionality
                        break;

                    case RapidXaml.ActionType.ReplaceElement:
                        // TODO: implement ReplaceElement functionality
                        break;

                    case RapidXaml.ActionType.RenameElement:
                        // TODO: implement RenameElement functionality
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
