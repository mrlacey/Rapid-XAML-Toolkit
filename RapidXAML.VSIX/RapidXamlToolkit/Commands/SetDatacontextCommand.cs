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

            AnalyzerBase.ServiceProvider = (IServiceProvider)Instance.ServiceProvider;
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

                                if (!showCommandButton && profile.Datacontext.SetsAnyCodeBehindContent)
                                {
                                    if (profile.Datacontext.SetsCodeBehindPageContent)
                                    {
                                        // TODO: set the DC in the CB file (C# or VB) may be open and unsaved
                                    }

                                    if (!showCommandButton && profile.Datacontext.SetsCodeBehindConstructorContent)
                                    {
                                        // TODO: set the DC in the CB file (C# or VB) may be open and unsaved
                                    }
                                }
                            }
                            else
                            {
                                if (profile.Datacontext.SetsXamlPageAttribute)
                                {
                                    // TODO: set the DC in the XAML file (C# or VB) may be open and unsaved
                                }

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
                            // TODO: set the DC in the CB file (C# or VB) may be open and unsaved
                        }

                        if (profile.Datacontext.SetsCodeBehindConstructorContent)
                        {
                            // TODO: set the DC in the CB file (C# or VB) may be open and unsaved
                        }
                    }
                }
                else
                {
                    if (profile.Datacontext.SetsXamlPageAttribute)
                    {
                        // TODO: set the DC in the XAML file (C# or VB) may be open and unsaved
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

                        if (profile.Datacontext.SetsCodeBehindPageContent)
                        {
                            var contentToInsert = profile.Datacontext.CodeBehindPageContent.Replace(Placeholder.ViewModelClass, viewModelName);

                            if (!docTextWithoutWhitespace.Contains(contentToInsert.RemoveAllWhitespace()))
                            {
                                if (activeDocument.Language == "CSharp")
                                {
                                    // Get end of constructor
                                    var documentRoot = document.GetSyntaxRootAsync().Result;

                                    var allConstructors = documentRoot.DescendantNodes().OfType<Microsoft.CodeAnalysis.CSharp.Syntax.ConstructorDeclarationSyntax>().ToList();

                                    int ctorEndPos = 0;

                                    if (allConstructors.Any())
                                    {
                                        ctorEndPos = allConstructors.First().Span.End;
                                    }
                                    else
                                    {
                                        // TODO: No constructor so may need to add one.
                                    }

                                    ctorEndPosLineNo = docText.Take(ctorEndPos).Count(c => c == '\n') + 1;

                                    objectDoc.Selection.GotoLine(ctorEndPosLineNo);
                                    objectDoc.Selection.EndOfLine();
                                    objectDoc.Selection.Insert($"{Environment.NewLine}{Environment.NewLine}{contentToInsert}");
                                }
                                else
                                {
                                    // TODO: implement for VB
                                }
                            }
                        }

                        if (profile.Datacontext.SetsCodeBehindConstructorContent)
                        {
                            var ctorCodeToInsert = profile.Datacontext.CodeBehindConstructorContent.Replace(Placeholder.ViewModelClass, viewModelName);

                            if (!docTextWithoutWhitespace.Contains(ctorCodeToInsert.RemoveAllWhitespace()))
                            {
                                if (ctorEndPosLineNo == 0)
                                {
                                    if (activeDocument.Language == "CSharp")
                                    {
                                        var documentRoot = document.GetSyntaxRootAsync().Result;

                                        var allConstructors = documentRoot.DescendantNodes().OfType<Microsoft.CodeAnalysis.CSharp.Syntax.ConstructorDeclarationSyntax>().ToList();

                                        int ctorEndPos = 0;

                                        if (allConstructors.Any())
                                        {
                                            ctorEndPos = allConstructors.First().Span.End;
                                        }
                                        else
                                        {
                                            // TODO: No constructor so may need to add one.
                                        }

                                        ctorEndPosLineNo = docText.Take(ctorEndPos).Count(c => c == '\n') + 1;
                                    }
                                    else
                                    {
                                        // TODO: implement for VB
                                    }
                                }

                                objectDoc.Selection.GotoLine(ctorEndPosLineNo - 1);
                                objectDoc.Selection.EndOfLine();
                                objectDoc.Selection.Insert($"{Environment.NewLine}{Environment.NewLine}{ctorCodeToInsert}");
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
