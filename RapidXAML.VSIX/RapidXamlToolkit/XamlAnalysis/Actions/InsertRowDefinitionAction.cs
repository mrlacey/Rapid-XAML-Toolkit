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
using RapidXamlToolkit.Resources;
using RapidXamlToolkit.VisualStudioIntegration;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.XamlAnalysis.Actions
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
            get { return $"Insert new definition for row {this.tag.RowId}"; }  // TODO: localize
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

        // TODO: add more tests for grid nesting
        public static string GetPreviewText(string original, List<(string find, string replace)> replacements, Dictionary<int, int> exclusions, InsertRowDefinitionTag tag)
        {
            var withReplacements = SwapReplacements(original, replacements, exclusions);

            var insertLineStart = withReplacements.Substring(tag.GridStartPos, tag.InsertPoint - tag.GridStartPos).LastIndexOf('\n') + 1;

            var toInsert = tag.XamlTag + Environment.NewLine + new string(' ', tag.InsertPoint - tag.GridStartPos - insertLineStart);
            var withInsertion = withReplacements.Insert(tag.InsertPoint, toInsert);

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
}
