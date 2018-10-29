// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using EnvDTE;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.LanguageServices;
using Microsoft.VisualStudio.Text.Editor;
using TextDocument = EnvDTE.TextDocument;

namespace RapidXamlToolkit.Commands
{
    public class VisualStudioAbstraction : IVisualStudioAbstraction
    {
        private readonly DTE dte;
        private readonly IComponentModel componentModel;
        private readonly IWpfTextView textView;

        public VisualStudioAbstraction(DTE dte, IComponentModel componentModel = null, IWpfTextView textView = null)
        {
            this.dte = dte ?? throw new ArgumentNullException(nameof(dte));
            this.componentModel = componentModel;
            this.textView = textView;
        }

        public string GetActiveDocumentFileName()
        {
            return this.dte.ActiveDocument.Name;
        }

        public string GetActiveDocumentText()
        {
            var activeDoc = this.dte.ActiveDocument;

            if (activeDoc.Object("TextDocument") is TextDocument objectDoc)
            {
                var docText = objectDoc.StartPoint.CreateEditPoint().GetText(objectDoc.EndPoint);

                return docText;
            }

            return null;
        }

        public ProjectWrapper GetActiveProject()
        {
            return new ProjectWrapper(((Array)this.dte.ActiveSolutionProjects).GetValue(0) as EnvDTE.Project);
        }

        public async Task<(SyntaxTree syntaxTree, SemanticModel semModel)> GetDocumentModelsAsync(string fileName)
        {
            var visualStudioWorkspace = this.componentModel?.GetService<VisualStudioWorkspace>();

            if (visualStudioWorkspace != null)
            {
                var solution = visualStudioWorkspace.CurrentSolution;
                var documentId = solution.GetDocumentIdsWithFilePath(fileName).FirstOrDefault();
                var document = solution.GetDocument(documentId);

                return await this.GetDocumentModelsAsync(document);
            }

            return (null, null);
        }

        public async Task<(SyntaxTree syntaxTree, SemanticModel semModel)> GetDocumentModelsAsync(Microsoft.CodeAnalysis.Document document)
        {
            var root = await document.GetSyntaxRootAsync();
            var syntaxTree = root.SyntaxTree;

            var semModel = await document.GetSemanticModelAsync();

            return (syntaxTree, semModel);
        }

        public IWpfTextView GetActiveTextView()
        {
            return this.textView;
        }

        public ProjectWrapper GetProject(string projectName)
        {
            foreach (var project in this.dte.Solution.GetAllProjects())
            {
                if (project.Name == projectName)
                {
                    return new ProjectWrapper(project);
                }
            }

            return null;
        }

        public bool UserConfirms(string title, string message)
        {
            var msgResult = MessageBox.Show(
                                            message,
                                            title,
                                            MessageBoxButton.YesNo,
                                            MessageBoxImage.Warning);

            return msgResult == MessageBoxResult.Yes;
        }

        public bool ActiveDocumentIsCSharp()
        {
            return this.dte.ActiveDocument.Language == "CSharp";
        }

        public int GetCursorPosition()
        {
            var (offset, _) = this.GetCursorPositionAndLineNumber();

            return offset;
        }

        public (int, int) GetCursorPositionAndLineNumber()
        {
            var offset = ((TextSelection)this.dte.ActiveDocument.Selection).AnchorPoint.AbsoluteCharOffset;
            var lineNo = ((TextSelection)this.dte.ActiveDocument.Selection).CurrentLine;

            return (offset, lineNo);
        }

        public void ReplaceInActiveDoc(List<(string find, string replace)> replacements, int startIndex, int endIndex, Dictionary<int, int> exclusions = null)
        {
            if (this.dte.ActiveDocument.Object("TextDocument") is TextDocument txtDoc)
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

        public void InsertIntoActiveDocumentOnNextLine(string text, int pos)
        {
            if (this.dte.ActiveDocument.Object("TextDocument") is TextDocument txtDoc)
            {
                txtDoc.Selection.MoveToAbsoluteOffset(pos);
                txtDoc.Selection.EndOfLine();
                txtDoc.Selection.NewLine();
                txtDoc.Selection.Insert(text);
            }
        }

        public void StartSingleUndoOperation(string name)
        {
            if (!this.dte.UndoContext.IsOpen)
            {
                this.dte.UndoContext.Open(name);
            }
        }

        public void EndSingleUndoOperation()
        {
            this.dte.UndoContext.Close();
        }
    }
}
