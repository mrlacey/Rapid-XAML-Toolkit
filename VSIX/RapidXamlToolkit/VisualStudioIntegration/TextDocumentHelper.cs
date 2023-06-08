// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.TextManager.Interop;

namespace RapidXamlToolkit.VisualStudioIntegration
{

    /// <summary>
    /// A static helper class for working with text documents.
    /// This is a slightly modified version of the TextDocumentHelper class from https://github.com/codecadwallader/codemaid
    /// </summary>
    /// <remarks>
    ///
    /// Note: All POSIXRegEx text replacements search against '\n' but insert/replace with
    ///       Environment.NewLine. This handles line endings correctly.
    /// </remarks>
    internal static class TextDocumentHelper
    {
        /// <summary>
        /// The common set of options to be used for find and replace patterns.
        /// </summary>
        internal const FindOptions StandardFindOptions = FindOptions.MatchCase;

        /// <summary>
        /// Finds all matches of the specified pattern within the specified text document.
        /// </summary>
        /// <param name="textDocument">The text document.</param>
        /// <param name="patternString">The pattern string.</param>
        /// <returns>The set of matches.</returns>
        internal static IEnumerable<EditPoint> FindMatches(TextDocument textDocument, string patternString)
        {
            var matches = new List<EditPoint>();

            ThreadHelper.JoinableTaskFactory.Run(async () =>
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                if (TryGetTextBufferAt(textDocument.Parent.FullName, out ITextBuffer textBuffer))
                {
                    IFinder finder = GetFinder(patternString, textBuffer);
                    var findMatches = finder.FindAll();
                    foreach (var match in findMatches)
                    {
                        matches.Add(GetEditPointForSnapshotPosition(textDocument, textBuffer.CurrentSnapshot, match.Start));
                    }
                }
            });

            return matches;
        }

        internal static IEnumerable<EditPoint> FindMatches(TextDocument textDocument, string patternString, Span searchRange)
        {
            var matches = new List<EditPoint>();

            ThreadHelper.JoinableTaskFactory.Run(async () =>
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                if (TryGetTextBufferAt(textDocument.Parent.FullName, out ITextBuffer textBuffer))
                {
                    IFinder finder = GetFinder(patternString, textBuffer);
                    var findMatches = finder.FindAll(searchRange);
                    foreach (var match in findMatches)
                    {
                        matches.Add(GetEditPointForSnapshotPosition(textDocument, textBuffer.CurrentSnapshot, match.Start));
                    }
                }
            });

            return matches;
        }

        /// <summary>
        /// Finds all matches of the specified pattern within the specified text selection.
        /// </summary>
        /// <param name="textSelection">The text selection.</param>
        /// <param name="patternString">The pattern string.</param>
        /// <returns>The set of matches.</returns>
        internal static IEnumerable<EditPoint> FindMatches(TextSelection textSelection, string patternString)
        {
            var matches = new List<EditPoint>();
            ThreadHelper.JoinableTaskFactory.Run(async () =>
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                if (TryGetTextBufferAt(textSelection.Parent.Parent.FullName, out ITextBuffer textBuffer))
                {
                    IFinder finder = GetFinder(patternString, textBuffer);
                    var findMatches = finder.FindAll(GetSnapshotSpanForTextSelection(textBuffer.CurrentSnapshot, textSelection));
                    foreach (var match in findMatches)
                    {
                        matches.Add(GetEditPointForSnapshotPosition(textSelection.Parent, textBuffer.CurrentSnapshot, match.Start));
                    }
                }
            });

            return matches;
        }

        /// <summary>
        /// Finds the first match of the specified pattern within the specified text document, otherwise null.
        /// </summary>
        /// <param name="textDocument">The text document.</param>
        /// <param name="patternString">The pattern string.</param>
        /// <returns>The first match, otherwise null.</returns>
        internal static EditPoint FirstOrDefaultMatch(TextDocument textDocument, string patternString)
        {
            EditPoint result = null;
            ThreadHelper.JoinableTaskFactory.Run(async () =>
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                if (TryGetTextBufferAt(textDocument.Parent.FullName, out ITextBuffer textBuffer))
                {
                    IFinder finder = GetFinder(patternString, textBuffer);
                    if (finder.TryFind(out Span match))
                    {
                        result = GetEditPointForSnapshotPosition(textDocument, textBuffer.CurrentSnapshot, match.Start);
                    }
                }
            });

            return result;
        }

        /// <summary>
        /// Attempts to find next match starting with <paramref name="startPoint"/> and if sucessful,
        /// updates <paramref name="endPoint"/> to point to the end position of the match.
        /// </summary>
        /// <returns>True if successful, false if no match found.</returns>
        internal static bool TryFindNextMatch(EditPoint startPoint, ref EditPoint endPoint, string patternString)
        {
            bool result = false;
            EditPoint resultEndPoint = null;

            ThreadHelper.JoinableTaskFactory.Run(async () =>
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                if (TryGetTextBufferAt(startPoint.Parent.Parent.FullName, out ITextBuffer textBuffer))
                {
                    IFinder finder = GetFinder(patternString, textBuffer);

                    if (finder.TryFind(GetSnapshotPositionForTextPoint(textBuffer.CurrentSnapshot, startPoint), out Span match))
                    {
                        resultEndPoint = GetEditPointForSnapshotPosition(startPoint.Parent, textBuffer.CurrentSnapshot, match.End);
                        result = true;
                    }
                }
            });

            endPoint = resultEndPoint;
            return result;
        }

        /// <summary>
        /// Gets the text between the specified start point and the first match.
        /// </summary>
        /// <param name="startPoint">The start point.</param>
        /// <param name="matchString">The match string.</param>
        /// <returns>The matching text, otherwise null.</returns>
        internal static string GetTextToFirstMatch(TextPoint startPoint, string matchString)
        {
            string result = null;
            ThreadHelper.JoinableTaskFactory.Run(async () =>
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                if (TryGetTextBufferAt(startPoint.Parent.Parent.FullName, out ITextBuffer textBuffer))
                {
                    IFinder finder = GetFinder(matchString, textBuffer);
                    if (finder.TryFind(GetSnapshotPositionForTextPoint(textBuffer.CurrentSnapshot, startPoint), out Span match))
                    {
                        result = textBuffer.CurrentSnapshot.GetText(startPoint.AbsoluteCharOffset, match.Start - startPoint.AbsoluteCharOffset);
                    }
                }
            });

            return result;
        }

        ///// <summary>
        ///// Inserts a blank line before the specified point except where adjacent to a brace.
        ///// </summary>
        ///// <param name="point">The point.</param>
        //internal static void InsertBlankLineBeforePoint(EditPoint point)
        //{
        //    if (point.Line <= 1) return;

        //    point.LineUp(1);
        //    point.StartOfLine();

        //    string text = point.GetLine();
        //    if (RegexNullSafe.IsMatch(text, @"^\s*[^\s\{]")) // If it is not a scope boundary, insert newline.
        //    {
        //        point.EndOfLine();
        //        point.Insert(Environment.NewLine);
        //    }
        //}

        ///// <summary>
        ///// Inserts a blank line after the specified point except where adjacent to a brace.
        ///// </summary>
        ///// <param name="point">The point.</param>
        //internal static void InsertBlankLineAfterPoint(EditPoint point)
        //{
        //    if (point.AtEndOfDocument) return;

        //    point.LineDown(1);
        //    point.StartOfLine();

        //    string text = point.GetLine();
        //    if (RegexNullSafe.IsMatch(text, @"^\s*[^\s\}]"))
        //    {
        //        point.Insert(Environment.NewLine);
        //    }
        //}

        /// <summary>
        /// Substitutes all occurrences in the specified text document of the specified pattern
        /// string with the specified replacement string.
        /// </summary>
        /// <param name="textDocument">The text document.</param>
        /// <param name="patternString">The pattern string.</param>
        /// <param name="replacementString">The replacement string.</param>
        internal static void SubstituteAllStringMatches(TextDocument textDocument, string patternString, string replacementString)
        {
            ThreadHelper.JoinableTaskFactory.Run(async () =>
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                if (TryGetTextBufferAt(textDocument.Parent.FullName, out ITextBuffer textBuffer))
                {
                    IFinder finder = GetFinder(patternString, replacementString, textBuffer);
                    ReplaceAll(textBuffer, finder.FindForReplaceAll());
                }
            });
        }

        /// <summary>
        /// Substitutes all occurrences in the specified text selection of the specified pattern
        /// string with the specified replacement string.
        /// </summary>
        /// <param name="textSelection">The text selection.</param>
        /// <param name="patternString">The pattern string.</param>
        /// <param name="replacementString">The replacement string.</param>
        internal static void SubstituteAllStringMatches(TextSelection textSelection, string patternString, string replacementString)
        {
            ThreadHelper.JoinableTaskFactory.Run(async () =>
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                if (TryGetTextBufferAt(textSelection.Parent.Parent.FullName, out ITextBuffer textBuffer))
                {
                    IFinder finder = GetFinder(patternString, replacementString, textBuffer);
                    ReplaceAll(textBuffer, finder.FindForReplaceAll(GetSnapshotSpanForTextSelection(textBuffer.CurrentSnapshot, textSelection)));
                }
            });
        }

        /// <summary>
        /// Substitutes all occurrences between the specified start and end points of the specified
        /// pattern string with the specified replacement string.
        /// </summary>
        /// <param name="startPoint">The start point.</param>
        /// <param name="endPoint">The end point.</param>
        /// <param name="patternString">The pattern string.</param>
        /// <param name="replacementString">The replacement string.</param>
        internal static void SubstituteAllStringMatches(EditPoint startPoint, EditPoint endPoint, string patternString, string replacementString)
        {
            ThreadHelper.JoinableTaskFactory.Run(async () =>
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                if (TryGetTextBufferAt(startPoint.Parent.Parent.FullName, out ITextBuffer textBuffer))
                {
                    IFinder finder = GetFinder(patternString, replacementString, textBuffer);
                    ReplaceAll(textBuffer, finder.FindForReplaceAll(GetSnapshotSpanForExtent(textBuffer.CurrentSnapshot, startPoint, endPoint)));
                }
            });
        }

        internal static bool MakeReplacements(TextDocument textDocument, EditPoint startPoint, string patternString, string replacementString)
        {
            var result = false;

            ThreadHelper.JoinableTaskFactory.Run(async () =>
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                if (TryGetTextBufferAt(textDocument.Parent.FullName, out ITextBuffer textBuffer))
                {
                    IFinder finder = GetFinder(patternString, replacementString, textBuffer);
                    result = ReplaceAll(textBuffer, finder.FindForReplaceAll(GetSnapshotSpanForExtent(textBuffer.CurrentSnapshot, startPoint, patternString.Length)));
                }
            });

            return result;
        }

        private static EditPoint GetEditPointForSnapshotPosition(TextDocument textDocument, ITextSnapshot textSnapshot, int position)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var editPoint = textDocument.CreateEditPoint();
            var textSnapshotLine = textSnapshot.GetLineFromPosition(position);
            editPoint.MoveToLineAndOffset(textSnapshotLine.LineNumber + 1, position - textSnapshotLine.Start.Position + 1);
            return editPoint;
        }

        private static IFindService GetFindService()
        {
            // TODO: cache these?
            var componentModel = (IComponentModel)Package.GetGlobalService(typeof(SComponentModel));
            var findService = componentModel.GetService<IFindService>();
            return findService;
        }

        private static IFinder GetFinder(string findWhat, ITextBuffer textBuffer)
        {
            var finderFactory = GetFindService().CreateFinderFactory(findWhat, StandardFindOptions);
            return finderFactory.Create(textBuffer.CurrentSnapshot);
        }

        private static IFinder GetFinder(string findWhat, string replaceWith, ITextBuffer textBuffer)
        {
            var finderFactory = GetFindService().CreateFinderFactory(findWhat, replaceWith, StandardFindOptions);
            return finderFactory.Create(textBuffer.CurrentSnapshot);
        }

        private static Span GetSnapshotSpanForTextSelection(ITextSnapshot textSnapshot, TextSelection selection)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var startPosition = GetSnapshotPositionForTextPoint(textSnapshot, selection.AnchorPoint);
            var endPosition = GetSnapshotPositionForTextPoint(textSnapshot, selection.ActivePoint);

            if (startPosition <= endPosition)
            {
                return new Span(startPosition, endPosition - startPosition);
            }
            else
            {
                return new Span(endPosition, startPosition - endPosition);
            }
        }

        private static int GetSnapshotPositionForTextPoint(ITextSnapshot textSnapshot, TextPoint textPoint)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var textSnapshotLine = textSnapshot.GetLineFromLineNumber(textPoint.Line - 1);
            return textSnapshotLine.Start.Position + textPoint.LineCharOffset - 1;
        }

        private static Span GetSnapshotSpanForExtent(ITextSnapshot textSnapshot, EditPoint startPoint, EditPoint endPoint)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var startPosition = GetSnapshotPositionForTextPoint(textSnapshot, startPoint);
            var endPosition = GetSnapshotPositionForTextPoint(textSnapshot, endPoint);

            if (startPosition <= endPosition)
            {
                return new Span(startPosition, endPosition - startPosition);
            }
            else
            {
                return new Span(endPosition, startPosition - endPosition);
            }
        }

        private static Span GetSnapshotSpanForExtent(ITextSnapshot textSnapshot, EditPoint startPoint, int length)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var startPosition = GetSnapshotPositionForTextPoint(textSnapshot, startPoint);
            var endPosition = startPosition + length;

            if (startPosition <= endPosition)
            {
                return new Span(startPosition, endPosition - startPosition);
            }
            else
            {
                return new Span(endPosition, startPosition - endPosition);
            }
        }

        private static bool ReplaceAll(ITextBuffer textBuffer, IEnumerable<FinderReplacement> replacements)
        {
            var result = false;

            if (replacements.Any())
            {
                using (var edit = textBuffer.CreateEdit())
                {
                    foreach (var match in replacements)
                    {
                        result = true;
                        edit.Replace(match.Match, match.Replace);
                    }

                    edit.Apply();
                }
            }

            return result;
        }

        private static bool TryGetTextBufferAt(string filePath, out ITextBuffer textBuffer)
        {
            IVsWindowFrame windowFrame;

            var componentModel = (IComponentModel)Package.GetGlobalService(typeof(SComponentModel));

            if (VsShellUtilities.IsDocumentOpen(
              RapidXamlPackage.Instance,
              filePath,
              Guid.Empty,
              out var _,
              out var _,
              out windowFrame))
            {
                IVsTextView view = VsShellUtilities.GetTextView(windowFrame);
                IVsTextLines lines;
                if (view.GetBuffer(out lines) == 0)
                {
                    var buffer = lines as IVsTextBuffer;
                    if (buffer != null)
                    {
                        var editorAdapterFactoryService = componentModel.GetService<IVsEditorAdaptersFactoryService>();
                        textBuffer = editorAdapterFactoryService.GetDataBuffer(buffer);
                        return true;
                    }
                }
            }

            textBuffer = null;
            return false;
        }
    }
}
