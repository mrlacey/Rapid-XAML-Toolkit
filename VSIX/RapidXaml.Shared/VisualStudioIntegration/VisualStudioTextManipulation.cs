// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using EnvDTE;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.TextManager.Interop;

namespace RapidXamlToolkit.VisualStudioIntegration
{
    // TODO: Issue#481 (VS2022): https://docs.microsoft.com/en-us/visualstudio/extensibility/migration/breaking-api-list?view=vs-2022#legacy-find-api-deprecation
    public class VisualStudioTextManipulation : IVisualStudioTextManipulation
    {
        public VisualStudioTextManipulation(DTE dte)
        {
            this.Dte = dte ?? throw new ArgumentNullException(nameof(dte));
        }

        protected DTE Dte { get; }

        private static IFinder GetFinder(string findWhat, ITextBuffer textBuffer)
        {
            var componentModel = (IComponentModel)Package.GetGlobalService(typeof(SComponentModel));
            var findService = componentModel.GetService<IFindService>();
            var finderFactory = findService.CreateFinderFactory(findWhat, FindOptions.UseRegularExpressions);
            return finderFactory.Create(textBuffer.CurrentSnapshot);
        }

        public void RemoveInActiveDocOnLine(string find, int lineNumber)
        {
            this.ReplaceInActiveDocOnLine(find, string.Empty, lineNumber);
        }

        public void ReplaceInActiveDocOnLine(string find, string replace, int lineNumber)
        {
            if (this.Dte.ActiveDocument.Object("TextDocument") is EnvDTE.TextDocument txtDoc)
            {
                var matches = TextDocumentHelper.FindMatches(txtDoc, find);

                foreach (var matchPoint in matches)
                {
                    if (matchPoint.Line == lineNumber)
                    {
                        if (!TextDocumentHelper.MakeReplacements(txtDoc, matchPoint, find, replace))
                        {
                            System.Diagnostics.Debug.WriteLine($"Failed to find '{find}' on line {lineNumber}.");
                        }

                        break;
                    }
                }
            }
        }

        public void ReplaceInActiveDocOnLineOrAbove(string find, string replace, int lineNumber)
        {
            if (this.Dte.ActiveDocument.Object("TextDocument") is EnvDTE.TextDocument txtDoc)
            {
                txtDoc.Selection.MoveToLineAndOffset(lineNumber, 1);

                var lineToSearch = lineNumber;

                var found = false;

                while (!found)
                {
                    // TODO: change as deprecated
                    found = txtDoc.Selection.FindText(find, (int)vsFindOptions.vsFindOptionsMatchCase);

                    if (found)
                    {
                        // The FindText call selected the search text so this insert pastes over the top of it
                        txtDoc.Selection.Insert(replace);
                    }
                    else
                    {
                        lineToSearch -= 1;
                        txtDoc.Selection.MoveToLineAndOffset(lineToSearch, 1);
                    }
                }
            }
        }

        public void ReplaceInActiveDoc(string find, string replace, int startIndex, int endIndex)
        {
            this.ReplaceInActiveDoc(new List<(string find, string replace)> { (find, replace) }, startIndex, endIndex, null);
        }

        public void ReplaceInActiveDoc(List<(string find, string replace)> replacements, int startIndex, int endIndex, Dictionary<int, int> exclusions = null)
        {
            if (this.Dte.ActiveDocument.Object("TextDocument") is EnvDTE.TextDocument txtDoc)
            {
                // Have to implement search and replace directly as built-in functionality doesn't provide the control to only replace within the desired area
                // Plus need to allow areas (exclusions) where replacement shouldn't occur.
                foreach (var (find, replace) in replacements)
                {
                    // move to startIndex
                    // find match text
                    // if > endIndex move next
                    // delete match text length
                    // insert replacement text
                    // repeat find
                    txtDoc.Selection.MoveToAbsoluteOffset(startIndex);

                    var keepSearching = true;

                    while (keepSearching)
                    {
                        // TODO: change as deprecated
                        if (!txtDoc.Selection.FindText(find, (int)vsFindOptions.vsFindOptionsMatchCase))
                        {
                            break; // while
                        }

                        var curPos = txtDoc.Selection.AnchorPoint.AbsoluteCharOffset;

                        var searchAgain = false;

                        // if in exclusion area then search again
                        if (exclusions != null)
                        {
                            foreach (var exclusion in exclusions)
                            {
                                if (curPos >= exclusion.Key && curPos <= exclusion.Value)
                                {
                                    searchAgain = true;
                                    break; // Foreach
                                }
                            }
                        }

                        if (!searchAgain)
                        {
                            if (curPos < endIndex)
                            {
                                // The FindText call above selected the search text so this insert pastes over the top of it
                                txtDoc.Selection.Insert(replace);

                                // Allow for find and replace being different lengths and adjust endpoint accordingly
                                endIndex += find.Length - replace.Length;
                            }
                            else
                            {
                                keepSearching = false;
                            }
                        }
                    }
                }
            }
        }

        public void InsertIntoActiveDocumentOnNextLine(string text, int pos)
        {
            if (this.Dte.ActiveDocument.Object("TextDocument") is EnvDTE.TextDocument txtDoc)
            {
                txtDoc.Selection.MoveToAbsoluteOffset(pos);
                txtDoc.Selection.EndOfLine();
                txtDoc.Selection.NewLine();
                txtDoc.Selection.Insert(text);
            }
        }

        public void InsertIntoActiveDocOnLineAfterClosingTag(int openingAngleBracketLineNumber, string toInsert)
        {
            if (this.Dte.ActiveDocument.Object("TextDocument") is TextDocument txtDoc)
            {
                txtDoc.Selection.MoveToLineAndOffset(openingAngleBracketLineNumber, 1);

                // TODO: change as deprecated
                // This will allow selection to move to whichever line the startTag ends on.
                txtDoc.Selection.FindText(">", (int)vsFindOptions.vsFindOptionsMatchCase);

                txtDoc.Selection.Insert($">{Environment.NewLine}{toInsert}");
            }
        }

        // Returns false if an UndoContext is already open.
        // Track the return value to know whether to end/close the UndoContext.
        public bool StartSingleUndoOperation(string name)
        {
            if (!this.Dte.UndoContext.IsOpen)
            {
                this.Dte.UndoContext.Open(name);
                return true;
            }

            return false;
        }

        public void EndSingleUndoOperation()
        {
            this.Dte.UndoContext.Close();
        }

        public void InsertAtEndOfLine(int lineNumber, string toInsert)
        {
            if (this.Dte.ActiveDocument.Object("TextDocument") is EnvDTE.TextDocument txtDoc)
            {
                txtDoc.Selection.MoveToLineAndOffset(lineNumber, 1);
                txtDoc.Selection.EndOfLine();
                txtDoc.Selection.Insert(toInsert);
            }
        }

        public void DeleteFromEndOfLine(int lineNumber, int charsToDelete)
        {
            if (this.Dte.ActiveDocument.Object("TextDocument") is EnvDTE.TextDocument txtDoc)
            {
                txtDoc.Selection.MoveToLineAndOffset(lineNumber, 1);
                txtDoc.Selection.EndOfLine();
                txtDoc.Selection.DeleteLeft(charsToDelete);
            }
        }

        public void AddXmlnsAliasToActiveDoc(string alias, string value)
        {
            if (this.Dte.ActiveDocument.Object("TextDocument") is EnvDTE.TextDocument txtDoc)
            {
                txtDoc.Selection.MoveToLineAndOffset(1, 1);


                // TODO: change as deprecated

                txtDoc.Selection.FindText(">", (int)vsFindOptions.vsFindOptionsMatchCase);

                txtDoc.Selection.MoveToLineAndOffset(1, 1, true);

                if (!txtDoc.Selection.Text.Contains($" xmlns:{alias}"))
                {
                    if (txtDoc.Selection.BottomLine > txtDoc.Selection.TopLine)
                    {
                        var lineOfInterest = txtDoc.Selection.BottomLine;

                        txtDoc.Selection.GotoLine(lineOfInterest);
                        txtDoc.Selection.SelectLine();

                        var lineText = txtDoc.Selection.Text;

                        var trimLineText = lineText.TrimStart();
                        var indent = lineText.Substring(0, lineText.Length - trimLineText.Length);

                        txtDoc.Selection.MoveToLineAndOffset(txtDoc.Selection.BottomLine - 1, 1);
                        txtDoc.Selection.Insert($"{indent}xmlns:{alias}=\"{value}\"{Environment.NewLine}");
                    }
                    else
                    {
                        txtDoc.Selection.MoveToLineAndOffset(txtDoc.Selection.TopLine, txtDoc.Selection.BottomPoint.DisplayColumn);
                        txtDoc.Selection.Insert($" xmlns:{alias}=\"{value}\"");
                    }
                }
            }
        }

        public void AddResource(string resPath, string resKey, string resValue)
        {
            if (!System.IO.File.Exists(resPath))
            {
                return;
            }

            try
            {
                var xdoc = XDocument.Load(resPath);

                // Don't want to create a duplicate entry.
                var alreadyExists = false;

                foreach (var element in xdoc?.Descendants("data"))
                {
                    if (element.Attribute("name")?.Value == resKey)
                    {
                        alreadyExists = true;
                        break;
                    }
                }

                if (!alreadyExists)
                {
                    var newData = new XElement("data");

                    newData.Add(new XAttribute("name", resKey));

                    newData.Add(new XAttribute(XNamespace.Xml + "space", "preserve"));
                    newData.Add(new XElement("value", resValue));
                    xdoc.Element("root").Add(newData);
                    xdoc.Save(resPath);
                }
            }
            catch (Exception exc)
            {
                // File locked, read-only, or corrupt are all reasons to get here.
                System.Diagnostics.Debug.WriteLine(exc);
            }
        }
    }
}




namespace RapidXamlToolkit.VisualStudioIntegration
{
    /// <summary>
    /// A static helper class for working with text documents.
    /// </summary>
    /// <remarks>
    ///
    /// Note: All POSIXRegEx text replacements search against '\n' but insert/replace with
    ///       Environment.NewLine. This handles line endings correctly.
    /// </remarks>
    internal static class TextDocumentHelper
    {
        #region Internal Constants

        /// <summary>
        /// The common set of options to be used for find and replace patterns.
        /// </summary>
        internal const FindOptions StandardFindOptions = FindOptions.MatchCase;

        #endregion Internal Constants

        #region Internal Methods

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
        /// <param name="startPoint"></param>
        /// <param name="endPoint"></param>
        /// <param name="patternString"></param>
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

        #endregion Internal Methods

        #region Private Methods

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
              SharedRapidXamlPackage.Instance,
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

        #endregion Private Methods
    }
}
