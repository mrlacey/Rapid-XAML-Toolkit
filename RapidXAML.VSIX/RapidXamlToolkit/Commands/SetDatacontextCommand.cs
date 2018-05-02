// <copyright file="SetDatacontextCommand.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.ComponentModel.Design;
using System.Linq;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.Shell;
using Task = System.Threading.Tasks.Task;

namespace RapidXamlToolkit
{
    internal sealed class SetDatacontextCommand : BaseCommand
    {
        public const int CommandId = 4132;

        private SetDatacontextCommand(AsyncPackage package, OleMenuCommandService commandService, ILogger logger)
            : base(package, logger)
        {
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuItem = new OleMenuCommand(this.Execute, menuCommandID);
            menuItem.BeforeQueryStatus += this.MenuItem_BeforeQueryStatus;
            commandService.AddCommand(menuItem);
        }

        public static SetDatacontextCommand Instance
        {
            get;
            private set;
        }

        public static async Task InitializeAsync(AsyncPackage package, ILogger logger)
        {
            // Verify the current thread is the UI thread - the call to AddCommand in SetDatacontextCommand's constructor requires
            // the UI thread.
            ThreadHelper.ThrowIfNotOnUIThread();

            OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Instance = new SetDatacontextCommand(package, commandService, logger);
        }

        private void MenuItem_BeforeQueryStatus(object sender, EventArgs e)
        {
            try
            {
                if (sender is OleMenuCommand menuCmd)
                {
                    bool showCommandButton = false;

                    var settings = AnalyzerBase.GetSettings();

                    if (settings.IsActiveProfileSet)
                    {
                        var profile = settings.GetActiveProfile();

                        var dte = (EnvDTE.DTE)Instance.ServiceProvider.GetServiceAsync(typeof(EnvDTE.DTE)).Result;
                        var activeDocument = dte.ActiveDocument;

                        // Is a XAML or code-behind file
                        if (activeDocument.Name.ToLowerInvariant().Contains(".xaml"))
                        {
                            var inXamlDoc = activeDocument.Name.EndsWith(".xaml", StringComparison.InvariantCultureIgnoreCase);

                            var viewModelName = System.IO.Path.GetFileNameWithoutExtension(activeDocument.Name)
                                                              .RemoveFromEndIfExists(".xaml") // to allow for double extensions
                                                              .RemoveFromEndIfExists(profile.ViewGeneration.XamlFileSuffix)
                                                              .Append(profile.ViewGeneration.ViewModelFileSuffix);

                            // Only show based on current doc - will need to switch to other doc if not set there
                            if (inXamlDoc)
                            {
                                if (profile.Datacontext.SetsXamlPageAttribute)
                                {
                                    var objectDoc = activeDocument.Object("TextDocument") as EnvDTE.TextDocument;
                                    var docText = objectDoc.StartPoint.CreateEditPoint().GetText(objectDoc.EndPoint);

                                    // Get formatted content to insert
                                    var contentToInsert = profile.Datacontext.XamlPageAttribute.Replace(Placeholder.ViewModelClass, viewModelName);

                                    if (!docText.Contains(contentToInsert))
                                    {
                                        showCommandButton = true;
                                    }
                                }
                            }
                            else
                            {
                                if (!showCommandButton && profile.Datacontext.SetsAnyCodeBehindContent)
                                {
                                    var objectDoc = activeDocument.Object("TextDocument") as EnvDTE.TextDocument;
                                    var docText = objectDoc.StartPoint.CreateEditPoint().GetText(objectDoc.EndPoint);

                                    // Compare without whitespace to allow for VS reformatting the code we add
                                    var docTextWithoutWhitespace = docText.RemoveAllWhitespace();

                                    if (profile.Datacontext.SetsCodeBehindPageContent)
                                    {
                                        var contentToInsert = profile.Datacontext.CodeBehindPageContent.Replace(Placeholder.ViewModelClass, viewModelName);

                                        if (!docTextWithoutWhitespace.Contains(contentToInsert.RemoveAllWhitespace()))
                                        {
                                            showCommandButton = true;
                                        }
                                    }

                                    if (!showCommandButton && profile.Datacontext.SetsCodeBehindConstructorContent)
                                    {
                                        var ctorCodeToInsert = profile.Datacontext.CodeBehindConstructorContent.Replace(Placeholder.ViewModelClass, viewModelName);

                                        if (!docTextWithoutWhitespace.Contains(ctorCodeToInsert.RemoveAllWhitespace()))
                                        {
                                            showCommandButton = true;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    menuCmd.Visible = menuCmd.Enabled = showCommandButton;
                }
            }
            catch (Exception exc)
            {
                this.Logger.RecordException(exc);
                throw;
            }
        }

        private void Execute(object sender, EventArgs e)
        {
            try
            {
                ThreadHelper.ThrowIfNotOnUIThread();

                this.Logger?.RecordFeatureUsage(nameof(SetDatacontextCommand));

                var settings = AnalyzerBase.GetSettings();
                var profile = settings.GetActiveProfile();

                var dte = (EnvDTE.DTE)Instance.ServiceProvider.GetServiceAsync(typeof(EnvDTE.DTE)).Result;
                var activeDocument = dte.ActiveDocument;

                var inXamlDoc = activeDocument.Name.EndsWith(".xaml", StringComparison.InvariantCultureIgnoreCase);

                var viewModelName = System.IO.Path.GetFileNameWithoutExtension(activeDocument.Name)
                                                  .RemoveFromEndIfExists(".xaml") // to allow for double extensions
                                                  .RemoveFromEndIfExists(profile.ViewGeneration.XamlFileSuffix)
                                                  .Append(profile.ViewGeneration.ViewModelFileSuffix);

                if (inXamlDoc)
                {
                    if (profile.Datacontext.SetsXamlPageAttribute)
                    {
                        var objectDoc = activeDocument.Object("TextDocument") as EnvDTE.TextDocument;
                        var docText = objectDoc.StartPoint.CreateEditPoint().GetText(objectDoc.EndPoint);
                        var docLines = docText.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

                        var contentToInsert = profile.Datacontext.XamlPageAttribute.Replace(Placeholder.ViewModelClass, viewModelName);

                        if (!docText.Contains(contentToInsert))
                        {
                            var pageOpeningTagEnd = docText.IndexOf(">");

                            // Assume attributes are on different lines
                            var openingPageTag = docText.Substring(0, pageOpeningTagEnd);

                            var lineNumberToInsertAfter = openingPageTag.Select((c, i) => openingPageTag.Substring(i)).Count(sub => sub.StartsWith(Environment.NewLine));

                            var lineToAddAfter = docLines[lineNumberToInsertAfter];

                            var whitespaceLength = lineToAddAfter.Length - lineToAddAfter.TrimStart().Length;

                            objectDoc.Selection.GotoLine(lineNumberToInsertAfter);
                            objectDoc.Selection.EndOfLine();
                            objectDoc.Selection.Insert($"{Environment.NewLine}{new string(' ', whitespaceLength)}{contentToInsert}");
                        }
                    }

                    if (profile.Datacontext.SetsAnyCodeBehindContent)
                    {
                        if (profile.Datacontext.SetsCodeBehindPageContent)
                        {
                            // TODO: ISSUE#22 - set the DC in the CB file (C# or VB) may be open and unsaved
                        }

                        if (profile.Datacontext.SetsCodeBehindConstructorContent)
                        {
                            // TODO: ISSUE#22 - set the DC in the CB file (C# or VB) may be open and unsaved
                        }
                    }
                }
                else
                {
                    if (profile.Datacontext.SetsXamlPageAttribute)
                    {
                        // TODO: ISSUE#22 - set the DC in the XAML file (C# or VB) may be open and unsaved
                    }

                    if (profile.Datacontext.SetsAnyCodeBehindContent)
                    {
                        var objectDoc = activeDocument.Object("TextDocument") as EnvDTE.TextDocument;
                        var docText = objectDoc.StartPoint.CreateEditPoint().GetText(objectDoc.EndPoint);

                        // Compare without whitespace to allow for VS reformatting the code we add
                        var docTextWithoutWhitespace = docText.RemoveAllWhitespace();

                        var textView = GetTextView(Instance.ServiceProvider);

                        var caretPosition = textView.Caret.Position.BufferPosition;

                        var document = caretPosition.Snapshot.GetOpenDocumentInCurrentContextWithChanges();

                        var semanticModel = document.GetSemanticModelAsync().Result;

                        int ctorEndPosLineNo = 0;

                        var documentRoot = document.GetSyntaxRootAsync().Result;

                        int ctorAddedLineNo = -1;

                        if (profile.Datacontext.SetsCodeBehindConstructorContent)
                        {
                            var ctorCodeToInsert = profile.Datacontext.CodeBehindConstructorContent.Replace(Placeholder.ViewModelClass, viewModelName);

                            if (!docTextWithoutWhitespace.Contains(ctorCodeToInsert.RemoveAllWhitespace()))
                            {
                                int ctorEndPos = 0;

                                if (activeDocument.Language == "CSharp")
                                {
                                    var allConstructors = documentRoot.DescendantNodes().OfType<Microsoft.CodeAnalysis.CSharp.Syntax.ConstructorDeclarationSyntax>().ToList();

                                    if (allConstructors.Any())
                                    {
                                        ctorEndPos = allConstructors.First().Span.End;
                                    }
                                    else
                                    {
                                        var classDeclaration = documentRoot.DescendantNodes().OfType<Microsoft.CodeAnalysis.CSharp.Syntax.ClassDeclarationSyntax>().FirstOrDefault();

                                        if (classDeclaration != null)
                                        {
                                            ctorEndPos = classDeclaration.OpenBraceToken.Span.End;
                                        }

                                        ctorCodeToInsert = profile.Datacontext.DefaultCodeBehindConstructor.Insert(profile.Datacontext.DefaultCodeBehindConstructor.LastIndexOf("}"), $"{ctorCodeToInsert}{Environment.NewLine}");
                                    }

                                    ctorEndPosLineNo = docText.Take(ctorEndPos).Count(c => c == '\n');

                                    // Add 2 at the end to account for the 2 lines added below during insertion
                                    ctorAddedLineNo = ctorEndPosLineNo + System.Text.RegularExpressions.Regex.Matches(ctorCodeToInsert, Environment.NewLine).Count + 2;
                                }
                                else
                                {
                                    var allConstructors = documentRoot.DescendantNodes().OfType<Microsoft.CodeAnalysis.VisualBasic.Syntax.ConstructorBlockSyntax>().ToList();

                                    if (allConstructors.Any())
                                    {
                                        ctorEndPos = allConstructors.First().Span.End;
                                        ctorEndPosLineNo = docText.Take(ctorEndPos).Count(c => c == '\n');
                                    }
                                    else
                                    {
                                        var classBlock = documentRoot.DescendantNodes().OfType<Microsoft.CodeAnalysis.VisualBasic.Syntax.ClassBlockSyntax>().FirstOrDefault();

                                        var classStatement = classBlock?.DescendantNodes().OfType<Microsoft.CodeAnalysis.VisualBasic.Syntax.ClassStatementSyntax>().FirstOrDefault();

                                        if (classStatement != null)
                                        {
                                            ctorEndPos = classStatement.Span.End;

                                            var implementsStatement = classBlock.DescendantNodes().OfType<Microsoft.CodeAnalysis.VisualBasic.Syntax.ImplementsStatementSyntax>().LastOrDefault();

                                            if (implementsStatement != null)
                                            {
                                                ctorEndPos = implementsStatement.Span.End;
                                            }
                                            else
                                            {
                                                var inheritsStatement = classBlock.DescendantNodes().OfType<Microsoft.CodeAnalysis.VisualBasic.Syntax.InheritsStatementSyntax>().FirstOrDefault();

                                                if (inheritsStatement != null)
                                                {
                                                    ctorEndPos = inheritsStatement.Span.End;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            var moduleStatement = documentRoot.DescendantNodes().OfType<Microsoft.CodeAnalysis.VisualBasic.Syntax.ModuleStatementSyntax>().FirstOrDefault();

                                            if (moduleStatement != null)
                                            {
                                                ctorEndPos = moduleStatement.Span.End;
                                            }
                                        }

                                        ctorCodeToInsert = profile.Datacontext.DefaultCodeBehindConstructor.Insert(profile.Datacontext.DefaultCodeBehindConstructor.LastIndexOf("End "), $"{ctorCodeToInsert}{Environment.NewLine}");

                                        if (ctorEndPos == 0)
                                        {
                                            // TODO: handle not finding anywhere to add the content? Or should it just go at the top of the file?
                                        }

                                        ctorEndPosLineNo = docText.Take(ctorEndPos).Count(c => c == '\n') + 1;
                                    }

                                    // Add 2 at the end to account for the 2 lines added below during insertion
                                    ctorAddedLineNo = ctorEndPosLineNo + System.Text.RegularExpressions.Regex.Matches(ctorCodeToInsert, Environment.NewLine).Count + 2;
                                }

                                objectDoc.Selection.GotoLine(ctorEndPosLineNo);
                                objectDoc.Selection.EndOfLine();
                                objectDoc.Selection.Insert($"{Environment.NewLine}{Environment.NewLine}{ctorCodeToInsert}");
                            }
                        }

                        if (profile.Datacontext.SetsCodeBehindPageContent)
                        {
                            var contentToInsert = profile.Datacontext.CodeBehindPageContent.Replace(Placeholder.ViewModelClass, viewModelName);

                            if (!docTextWithoutWhitespace.Contains(contentToInsert.RemoveAllWhitespace()))
                            {
                                int ctorEndPos = 0;

                                // Get end of constructor
                                if (activeDocument.Language == "CSharp")
                                {
                                    var allConstructors = documentRoot.DescendantNodes().OfType<Microsoft.CodeAnalysis.CSharp.Syntax.ConstructorDeclarationSyntax>().ToList();

                                    if (allConstructors.Any())
                                    {
                                        ctorEndPos = allConstructors.First().Span.End;
                                    }
                                    else
                                    {
                                        var classDeclaration = documentRoot.DescendantNodes().OfType<Microsoft.CodeAnalysis.CSharp.Syntax.ClassDeclarationSyntax>().FirstOrDefault();

                                        if (classDeclaration != null)
                                        {
                                            ctorEndPos = classDeclaration.OpenBraceToken.Span.End;
                                        }
                                    }
                                }
                                else
                                {
                                    var allConstructors = documentRoot.DescendantNodes().OfType<Microsoft.CodeAnalysis.VisualBasic.Syntax.ConstructorBlockSyntax>().ToList();

                                    if (allConstructors.Any())
                                    {
                                        ctorEndPos = allConstructors.First().Span.End;
                                    }
                                    else
                                    {
                                        // If the constructor was added above it won't be in the SyntaxNode (documentRoot)
                                        if (ctorAddedLineNo == -1)
                                        {
                                            var classBlock = documentRoot.DescendantNodes().OfType<Microsoft.CodeAnalysis.VisualBasic.Syntax.ClassBlockSyntax>().FirstOrDefault();

                                            var classStatement = classBlock?.DescendantNodes().OfType<Microsoft.CodeAnalysis.VisualBasic.Syntax.ClassStatementSyntax>().FirstOrDefault();

                                            if (classStatement != null)
                                            {
                                                ctorEndPos = classStatement.Span.End;

                                                var implementsStatement = classBlock.DescendantNodes().OfType<Microsoft.CodeAnalysis.VisualBasic.Syntax.ImplementsStatementSyntax>().LastOrDefault();

                                                if (implementsStatement != null)
                                                {
                                                    ctorEndPos = implementsStatement.Span.End;
                                                }
                                                else
                                                {
                                                    var inheritsStatement = classBlock.DescendantNodes().OfType<Microsoft.CodeAnalysis.VisualBasic.Syntax.InheritsStatementSyntax>().FirstOrDefault();

                                                    if (inheritsStatement != null)
                                                    {
                                                        ctorEndPos = inheritsStatement.Span.End;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                var moduleStatement = documentRoot.DescendantNodes().OfType<Microsoft.CodeAnalysis.VisualBasic.Syntax.ModuleBlockSyntax>().FirstOrDefault();

                                                if (moduleStatement != null)
                                                {
                                                    ctorEndPos = moduleStatement.Span.End;
                                                }
                                            }
                                        }
                                    }
                                }

                                if (ctorEndPos == 0 && ctorAddedLineNo == -1)
                                {
                                    // TODO: handle not finding anywhere to add the content? Or should it just go at the top of the file?
                                }

                                if (ctorAddedLineNo > -1)
                                {
                                    ctorEndPosLineNo = ctorAddedLineNo;
                                }
                                else
                                {
                                    ctorEndPosLineNo = docText.Take(ctorEndPos).Count(c => c == '\n');
                                }

                                objectDoc.Selection.GotoLine(ctorEndPosLineNo + 1);
                                objectDoc.Selection.EndOfLine();
                                objectDoc.Selection.Insert($"{Environment.NewLine}{Environment.NewLine}{contentToInsert}");
                            }
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                this.Logger.RecordException(exc);
                throw;
            }
        }
    }
}
