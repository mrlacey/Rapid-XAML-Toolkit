// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Imaging.Interop;
using Microsoft.VisualStudio.Text.Editor;
using RapidXamlToolkit.Commands;
using RapidXamlToolkit.Resources;
using RapidXamlToolkit.VisualStudioIntegration;

namespace RapidXamlToolkit.XamlAnalysis
{
    public class InsertRowDefinitionAction : BaseSuggestedAction
    {
        public InsertRowDefinitionTag tag;

        public override bool HasPreview => true;

        public List<(string find, string replace)> Replacements { get; private set; }

        public Dictionary<int, int> Exclusions { get; private set; }

        public string PreviewText { get; private set; }

        public override string DisplayText
        {
            get { return $"Insert new definition for row {tag.RowId}"; }
        }

        public override ImageMoniker IconMoniker => KnownMonikers.InsertClause;

        public static InsertRowDefinitionAction Create(InsertRowDefinitionTag tag, string file, ITextView view)
        {
            var text = view.TextSnapshot.GetText(tag.GridStartPos, tag.GridLength);
            var replacements = GetReplacements(tag.RowId, tag.RowCount);
            var exclusions = GetExclusions(text);

            var previewText = GetPreviewText(text, replacements, exclusions, tag);

            var result = new InsertRowDefinitionAction
            {
                tag = tag,
                File = file,
                View = view,
                Replacements = replacements,
                Exclusions = exclusions,
                PreviewText = previewText,
            };

            return result;
        }

        // TODO: need to allow for changing the Row of nested grids
        public static Dictionary<int, int> GetExclusions(string xaml)
        {
            const string gridOpen = "<Grid";
            const string gridOpenSpace = "<Grid ";
            const string gridOpenComplete = "<Grid>";
            const string gridClose = "</Grid>";

            var exclusions = new Dictionary<int, int>();

            var nextOpening = xaml.Substring(gridOpen.Length).FirstIndexOf(gridOpenComplete, gridOpenSpace);

            while (nextOpening > -1 && nextOpening < xaml.Length)
            {
                var endPos = xaml.IndexOf(gridClose, nextOpening, StringComparison.Ordinal) + gridClose.Length;

                exclusions.Add(nextOpening, endPos);

                var searchFrom = endPos + 1;

                nextOpening = xaml.Substring(searchFrom).FirstIndexOf(gridOpenComplete, gridOpenSpace);

                if (nextOpening > -1)
                {
                    nextOpening += searchFrom;
                }
            }

            return exclusions;
        }

        public static List<(string find, string replace)> GetReplacements(int rowNumber, int totalRows)
        {
            var result = new List<(string, string)>();

            if (rowNumber > -1)
            {
                // subtract 1 from total rows to allow for zero indexing
                for (int i = totalRows - 1; i >= rowNumber; i--)
                {
                    result.Add(($" Grid.Row=\"{i}\"", $" Grid.Row=\"{i + 1}\""));
                }
            }

            return result;
        }

        public static string GetPreviewText(string original, List<(string find, string replace)> replacements, Dictionary<int, int> exclusions, InsertRowDefinitionTag tag)
        {
            var withReplacements = SwapReplacements(original, replacements, exclusions);

            var insertLineStart = withReplacements.Substring(0, tag.InsertPoint).LastIndexOf('\n') + 1;

            var toInsert = new string(' ', tag.InsertPoint - insertLineStart) + tag.XamlTag + Environment.NewLine;
            var withInsertion = withReplacements.Insert(tag.InsertPoint - tag.GridStartPos, toInsert);

            return withInsertion;
        }

        public override Task<object> GetPreviewAsync(CancellationToken cancellationToken)
        {
            var textBlock = new TextBlock();
            textBlock.Padding = new Thickness(5);
            textBlock.Inlines.Add(new Run() { Text = this.PreviewText });

            return Task.FromResult<object>(textBlock);
        }

        // TODO: add tests for this
        public static string SwapReplacements(string originalXaml, List<(string find, string replace)> replacements, Dictionary<int, int> exclusions = null)
        {
            var result = originalXaml;

            // Have to implement search and replace directly as built-in functionality doesn't provide the control to only replace outside of exclusion areas.
            // Mimics the process in VisualStudioTextManipulation.ReplaceInActiveDoc
            foreach (var (find, replace) in replacements)
            {
                var pos = 0;

                while (true)
                {
                    pos = result.Substring(pos).IndexOf(find, StringComparison.Ordinal);

                    if (pos < 0)
                    {
                        break; // while
                    }

                    var searchAgain = false;

                    // if in exclusion area then search again
                    if (exclusions != null)
                    {
                        foreach (var exclusion in exclusions)
                        {
                            if (pos >= exclusion.Key && pos <= exclusion.Value)
                            {
                                searchAgain = true;
                                break; // Foreach
                            }
                        }
                    }

                    if (!searchAgain)
                    {
                        var before = result.Substring(0, pos);
                        var after = result.Substring(pos + find.Length);
                        result = before + replace + after;
                    }
                }
            }

            return result;
        }

        public override void Execute(CancellationToken cancellationToken)
        {
            var vs = new VisualStudioTextManipulation(ProjectHelpers.Dte);
            vs.StartSingleUndoOperation(StringRes.Info_UndoContextIndertRowDef);
            try
            {
                vs.ReplaceInActiveDoc(this.Replacements, this.tag.GridStartPos, this.tag.GridStartPos + this.tag.GridLength, this.Exclusions);
                vs.InsertIntoActiveDocumentOnNextLine(this.tag.XamlTag, this.tag.InsertPoint);
            }
            finally
            {
                vs.EndSingleUndoOperation();
            }
        }
    }
    // TODO: abstract duplication in the following actions
    public class AddRowDefinitionsAction : BaseSuggestedAction
    {
        private const string InjectedXaml = @"<Grid.RowDefinitions>
    <RowDefinition Height=""Auto"" />
    <RowDefinition Height=""*"" />
</Grid.RowDefinitions>";

        public AddRowDefinitionsTag tag;

        public override bool HasPreview => true;

        public override ImageMoniker IconMoniker => KnownMonikers.TwoRows;

        public override string DisplayText { get; } = "Add RowDefinitions";

        public static AddRowDefinitionsAction Create(AddRowDefinitionsTag tag)
        {
            var result = new AddRowDefinitionsAction
            {
                tag = tag,
            };

            return result;
        }

        public override Task<object> GetPreviewAsync(CancellationToken cancellationToken)
        {
            var textBlock = new TextBlock();
            textBlock.Padding = new Thickness(5);
            textBlock.Inlines.Add(new Run() { Text = InjectedXaml });

            return Task.FromResult<object>(textBlock);
        }

        public override void Execute(CancellationToken cancellationToken)
        {
            var vs = new VisualStudioTextManipulation(ProjectHelpers.Dte);
            vs.StartSingleUndoOperation(StringRes.Info_UndoContextIndertRowDef); // TODO: need correct resource
            try
            {
                // TODO: pad lines with appropriate whitespace
                vs.InsertAtEndOfLine(this.tag.InsertLine, Environment.NewLine + InjectedXaml);
            }
            finally
            {
                vs.EndSingleUndoOperation();
            }
        }
    }
    public class AddColumnDefinitionsAction : BaseSuggestedAction
    {
        public AddColumnDefinitionsTag tag;

        private const string InjectedXaml = @"<Grid.ColumnDefinitions>
    <ColumnDefinition Width=""*"" />
    <ColumnDefinition Width=""*"" />
</Grid.ColumnDefinitions>";

        public override bool HasPreview => true;

        public override ImageMoniker IconMoniker => KnownMonikers.TwoColumns;

        public override string DisplayText { get; } = "Add ColumnDefinitions";

        public static AddColumnDefinitionsAction Create(AddColumnDefinitionsTag tag)
        {
            var result = new AddColumnDefinitionsAction
            {
                tag = tag,
            };

            return result;
        }

        public override Task<object> GetPreviewAsync(CancellationToken cancellationToken)
        {
            var textBlock = new TextBlock();
            textBlock.Padding = new Thickness(5);
            textBlock.Inlines.Add(new Run() { Text = InjectedXaml });

            return Task.FromResult<object>(textBlock);
        }

        public override void Execute(CancellationToken cancellationToken)
        {
            var vs = new VisualStudioTextManipulation(ProjectHelpers.Dte);
            vs.StartSingleUndoOperation(StringRes.Info_UndoContextIndertRowDef); // TODO: need correct resource
            try
            {
                // TODO: pad lines with appropriate whitespace
                vs.InsertAtEndOfLine(this.tag.InsertLine, Environment.NewLine + InjectedXaml);
            }
            finally
            {
                vs.EndSingleUndoOperation();
            }
        }
    }
    public class AddRowAndColumnDefinitionsAction : BaseSuggestedAction
    {
        public AddRowAndColumnDefinitionsTag tag;

        private const string InjectedXaml = @"<Grid.RowDefinitions>
    <RowDefinition Height=""Auto"" />
    <RowDefinition Height=""*"" />
</Grid.RowDefinitions>
<Grid.ColumnDefinitions>
    <ColumnDefinition Width=""*"" />
    <ColumnDefinition Width=""*"" />
</Grid.ColumnDefinitions>";

        public override bool HasPreview => true;

        public override ImageMoniker IconMoniker => KnownMonikers.TwoRowsTwoColumns;

        public override string DisplayText { get; } = "Add RowDefinitions and ColumnDefinitions";

        public static AddRowAndColumnDefinitionsAction Create(AddRowAndColumnDefinitionsTag tag)
        {
            var result = new AddRowAndColumnDefinitionsAction
            {
                tag = tag,
            };

            return result;
        }

        public override Task<object> GetPreviewAsync(CancellationToken cancellationToken)
        {
            var textBlock = new TextBlock();
            textBlock.Padding = new Thickness(5);
            textBlock.Inlines.Add(new Run() { Text = InjectedXaml });

            return Task.FromResult<object>(textBlock);
        }

        public override void Execute(CancellationToken cancellationToken)
        {
            var vs = new VisualStudioTextManipulation(ProjectHelpers.Dte);
            vs.StartSingleUndoOperation(StringRes.Info_UndoContextIndertRowDef); // TODO: need correct resource
            try
            {
                // TODO: pad lines with appropriate whitespace
                vs.InsertAtEndOfLine(this.tag.InsertLine, Environment.NewLine + InjectedXaml);
            }
            finally
            {
                vs.EndSingleUndoOperation();
            }
        }
    }
}
