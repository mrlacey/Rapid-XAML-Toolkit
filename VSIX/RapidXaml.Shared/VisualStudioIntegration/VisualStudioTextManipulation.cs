// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Xml.Linq;
using EnvDTE;

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
            if (this.Dte.ActiveDocument.Object("TextDocument") is EnvDTE.TextDocument txtDoc)
            {
                txtDoc.Selection.MoveToLineAndOffset(lineNumber, 1);

                if (txtDoc.Selection.FindText(find, (int)vsFindOptions.vsFindOptionsMatchCase))
                {
                    // The FindText call selected the search text so this insert pastes over the top of it
                    txtDoc.Selection.Insert(replace);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"Failed to find '{find}' on line {lineNumber}.");
                }
            }
        }

        public void ReplaceInActiveDocOnLineOrAbove(string find, string replace, int lineNumber)
        {
            if (this.Dte.ActiveDocument.Object("TextDocument") is EnvDTE.TextDocument txtDoc)
            {
                txtDoc.Selection.MoveToLineAndOffset(lineNumber, 1);

                txtDoc.Selection.FindText(find, (int)vsFindOptions.vsFindOptionsMatchCase);

                var lineToSearch = lineNumber;

                var keepLooking = true;

                while (keepLooking)
                {
                    if (txtDoc.Selection.ActivePoint.Line == lineToSearch)
                    {
                        // The FindText call selected the search text so this insert pastes over the top of it
                        txtDoc.Selection.Insert(replace);
                        keepLooking = false;
                    }
                    else
                    {
                        lineToSearch -= 1;
                        txtDoc.Selection.MoveToLineAndOffset(lineToSearch, 1);
                        txtDoc.Selection.FindText(find, (int)vsFindOptions.vsFindOptionsMatchCase);
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
            var xdoc = XDocument.Load(resPath);

            // Don't want to create a duplicate entry.
            var alreadyExists = false;

            foreach (var element in xdoc.Descendants("data"))
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
    }
}
