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
        protected InjectFixedXamlSuggestedAction(string file)
            : base(file)
        {
        }

        // Injected XAML should not be fully indented to allow for it to be used in previews.
        public string InjectedXaml { get; protected set; }

        public string UndoOperationName { get; protected set; }

        public InsertionTag Tag { get; set; }

        public override bool HasPreview => true;

        public override Task<object> GetPreviewAsync(CancellationToken cancellationToken)
        {
            var textBlock = new TextBlock { Padding = new Thickness(5) };
            textBlock.Inlines.Add(new Run() { Text = this.InjectedXaml });

            return Task.FromResult<object>(textBlock);
        }

        public override void Execute(CancellationToken cancellationToken)
        {
            var vs = new VisualStudioTextManipulation(ProjectHelpers.Dte);
            vs.StartSingleUndoOperation(this.UndoOperationName);
            try
            {
                var lineNumber = this.Tag.Snapshot.GetLineNumberFromPosition(this.Tag.InsertPosition) + 1;

                string toInsert;
                if (this.Tag.GridNeedsExpanding)
                {
                    vs.RemoveInActiveDocOnLine(" />", lineNumber);

                    toInsert = $">{Environment.NewLine}{this.InjectedXaml}";

                    toInsert = toInsert.Replace("\n", "\n" + this.Tag.LeftPad);

                    string shortPad = this.Tag.LeftPad.EndsWith("\t")
                        ? this.Tag.LeftPad.Substring(0, this.Tag.LeftPad.Length - 1)
                        : this.Tag.LeftPad.Substring(0, this.Tag.LeftPad.Length - 4);

                    toInsert += $"{Environment.NewLine}{shortPad}</Grid>";
                }
                else
                {
                    toInsert = Environment.NewLine + this.InjectedXaml;
                    toInsert = toInsert.Replace("\n", "\n" + this.Tag.LeftPad);
                }

                vs.InsertAtEndOfLine(lineNumber, toInsert);

                RapidXamlDocumentCache.TryUpdate(this.File);
            }
            finally
            {
                vs.EndSingleUndoOperation();
            }
        }
    }
}
