// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using RapidXamlToolkit.Resources;
using RapidXamlToolkit.VisualStudioIntegration;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.XamlAnalysis.Actions
{
    public abstract class InjectFixedXamlSuggestedAction : BaseSuggestedAction
    {
        public string InjectedXaml { get; protected set; }

        public string UndoOperationName { get; protected set; }

        public LineInsertionTag Tag { get; set; }

        public override bool HasPreview => true;

        public override string DisplayText { get; }

        public override Task<object> GetPreviewAsync(CancellationToken cancellationToken)
        {
            var textBlock = new TextBlock();
            textBlock.Padding = new Thickness(5);
            textBlock.Inlines.Add(new Run() { Text = this.InjectedXaml });

            return Task.FromResult<object>(textBlock);
        }

        public override void Execute(CancellationToken cancellationToken)
        {
            var vs = new VisualStudioTextManipulation(ProjectHelpers.Dte);
            vs.StartSingleUndoOperation(StringRes.Info_UndoContextIndertRowDef);
            try
            {
                // TODO: pad lines with appropriate whitespace
                vs.InsertAtEndOfLine(this.Tag.InsertLine, Environment.NewLine + this.InjectedXaml);
            }
            finally
            {
                vs.EndSingleUndoOperation();
            }
        }
    }
}
