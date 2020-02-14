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
