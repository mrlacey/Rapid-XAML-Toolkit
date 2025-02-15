// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using EnvDTE;
using Microsoft.VisualStudio.Text;

namespace RapidXamlToolkit.VisualStudioIntegration
{
    public class VisualStudioTextManipulation : IVisualStudioTextManipulation
    {
        public VisualStudioTextManipulation(DTE dte)
        {
            this.Dte = dte ?? throw new ArgumentNullException(nameof(dte));
        }

        protected DTE Dte { get; }

        public void RemoveInActiveDocOnLine(string find, int lineNumber)
        {
            this.ReplaceInActiveDocOnLine(find, string.Empty, lineNumber);
        }

        public void ReplaceInActiveDocOnLine(string find, string replace, int lineNumber)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();

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
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();

            if (this.Dte.ActiveDocument.Object("TextDocument") is EnvDTE.TextDocument txtDoc)
            {
                var matches = TextDocumentHelper.FindMatches(txtDoc, find);

                foreach (var matchPoint in matches.Reverse())
                {
                    if (matchPoint.Line > lineNumber)
                    {
                        continue;
                    }
                    else
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

        public void ReplaceInActiveDoc(string find, string replace, int startIndex, int endIndex)
        {
            this.ReplaceInActiveDoc(new List<(string Find, string Replace)> { (find, replace) }, startIndex, endIndex, null);
        }

        public void ReplaceInActiveDoc(List<(string Find, string Replace)> replacements, int startIndex, int endIndex, Dictionary<int, int> exclusions = null)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();

            if (this.Dte.ActiveDocument.Object("TextDocument") is EnvDTE.TextDocument txtDoc)
            {
                // Have to implement search and replace directly as built-in functionality doesn't provide the control to only replace within the desired area
                // Plus need to allow areas (exclusions) where replacement shouldn't occur.
                foreach (var (find, replace) in replacements)
                {
                    var matches = TextDocumentHelper.FindMatches(txtDoc, find, new Span(startIndex, endIndex - startIndex));

                    foreach (var matchPoint in matches)
                    {
                        var searchAgain = false;

                        // if in exclusion area then search again
                        if (exclusions != null)
                        {
                            foreach (var exclusion in exclusions)
                            {
                                if (matchPoint.AbsoluteCharOffset >= exclusion.Key && matchPoint.AbsoluteCharOffset <= exclusion.Value)
                                {
                                    searchAgain = true;
                                    break; // Foreach
                                }
                            }
                        }

                        if (!searchAgain)
                        {
                            if (!TextDocumentHelper.MakeReplacements(txtDoc, matchPoint, find, replace))
                            {
                                System.Diagnostics.Debug.WriteLine($"Failed to find '{find}' in {matchPoint}.");
                            }
                        }
                    }
                }
            }
        }

        public void InsertIntoActiveDocumentOnNextLine(string text, int pos)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();

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
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();

            if (this.Dte.ActiveDocument.Object("TextDocument") is TextDocument txtDoc)
            {
                var matches = TextDocumentHelper.FindMatches(txtDoc, ">");

                var matchPoint = matches.FirstOrDefault(m =>
                {
                    Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
                    return m.Line >= openingAngleBracketLineNumber;
                });

                if (matchPoint is not null)
                {
                    TextDocumentHelper.MakeReplacements(txtDoc, matchPoint, ">", $">{Environment.NewLine}{toInsert}");
                }
            }
        }

        // Returns false if an UndoContext is already open.
        // Track the return value to know whether to end/close the UndoContext.
        public bool StartSingleUndoOperation(string name)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();

            if (!this.Dte.UndoContext.IsOpen)
            {
                this.Dte.UndoContext.Open(name);
                return true;
            }

            return false;
        }

        public void EndSingleUndoOperation()
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();

            this.Dte.UndoContext.Close();
        }

        public void InsertAtEndOfLine(int lineNumber, string toInsert)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();

            if (this.Dte.ActiveDocument.Object("TextDocument") is EnvDTE.TextDocument txtDoc)
            {
                txtDoc.Selection.MoveToLineAndOffset(lineNumber, 1);
                txtDoc.Selection.EndOfLine();
                txtDoc.Selection.Insert(toInsert);
            }
        }

        public void DeleteFromEndOfLine(int lineNumber, int charsToDelete)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();

            if (this.Dte.ActiveDocument.Object("TextDocument") is EnvDTE.TextDocument txtDoc)
            {
                txtDoc.Selection.MoveToLineAndOffset(lineNumber, 1);
                txtDoc.Selection.EndOfLine();
                txtDoc.Selection.DeleteLeft(charsToDelete);
            }
        }

        public void AddXmlnsAliasToActiveDoc(string alias, string value)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();

            if (this.Dte.ActiveDocument.Object("TextDocument") is EnvDTE.TextDocument txtDoc)
            {
                var matches = TextDocumentHelper.FindMatches(txtDoc, ">");

                var matchPoint = matches.FirstOrDefault();

                if (matchPoint is null)
                {
                    return;
                }

                txtDoc.Selection.MoveToAbsoluteOffset(matchPoint.AbsoluteCharOffset);

                txtDoc.Selection.MoveToLineAndOffset(1, 1, Extend: true);

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
