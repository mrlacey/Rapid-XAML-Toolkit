// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
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

        public void ReplaceInActiveDoc(List<(string find, string replace)> replacements, int startIndex, int endIndex, Dictionary<int, int> exclusions = null)
        {
            if (this.Dte.ActiveDocument.Object("TextDocument") is EnvDTE.TextDocument txtDoc)
            {
                // Have to implement search and replace directly as built-in functionality doesn't provide the control to only replace within the desired area
                // Plus need to allow areas (exclusions) where replacement shouldn't occur.
                foreach (var (find, replace) in replacements)
                {
                    // move to startindex
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
                                // The find call above selected the search text so this insert pastes over the top of it
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

        public void InsertIntoActiveDocument(string text, int pos)
        {
            if (this.Dte.ActiveDocument.Object("TextDocument") is EnvDTE.TextDocument txtDoc)
            {
                txtDoc.Selection.MoveToAbsoluteOffset(pos);
                txtDoc.Selection.Insert(text);
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

        public void StartSingleUndoOperation(string name)
        {
            if (!this.Dte.UndoContext.IsOpen)
            {
                this.Dte.UndoContext.Open(name);
            }
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
    }
}
