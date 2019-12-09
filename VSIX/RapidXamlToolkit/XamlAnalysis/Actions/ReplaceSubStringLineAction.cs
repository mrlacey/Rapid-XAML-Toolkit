// Copyright (c) Matt Lacey Ltd.. All rights reserved.
// Licensed under the MIT license.

using System.Threading;
using RapidXamlToolkit.VisualStudioIntegration;

namespace RapidXamlToolkit.XamlAnalysis.Actions
{
    public class ReplaceSubStringLineAction : BaseSuggestedAction
    {
        public ReplaceSubStringLineAction(string file, string undoText, string original, string replace, int lineNo)
            : base(file)
        {
            this.UndoText = undoText;
            this.Original = original;
            this.Replace = replace;
            this.LineNo = lineNo;
        }

        public string UndoText { get; }

        public string Original { get; }

        public string Replace { get; }

        public int LineNo { get; }

        public override void Execute(CancellationToken cancellationToken)
        {
            var vs = new VisualStudioTextManipulation(ProjectHelpers.Dte);
            vs.StartSingleUndoOperation(this.UndoText);
            try
            {
                vs.ReplaceInActiveDocOnLine(this.Original, this.Replace, this.LineNo);

                RapidXamlDocumentCache.TryUpdate(this.File);
            }
            finally
            {
                vs.EndSingleUndoOperation();
            }
        }
    }
}
