// <copyright file="SetDatacontextCommand.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.ComponentModel.Design;
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

                        var logic = new SetDataContextCommandLogic(profile, this.Logger, new VisualStudioAbstraction(dte));

                        showCommandButton = logic.ShouldEnableCommand();
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

                var logic = new SetDataContextCommandLogic(profile, this.Logger, new VisualStudioAbstraction(dte));

                var inXamlDoc = dte.ActiveDocument.Name.EndsWith(".xaml", StringComparison.InvariantCultureIgnoreCase);

                var viewModelName = logic.InferViewModelNameFromFileName(dte.ActiveDocument.Name);

                if (inXamlDoc)
                {
                    if (profile.Datacontext.SetsXamlPageAttribute)
                    {
                        var (add, lineNo, content) = logic.GetPageAttributeToAdd(viewModelName);

                        if (add)
                        {
                            var objectDoc = dte.ActiveDocument.Object("TextDocument") as EnvDTE.TextDocument;
                            objectDoc.Selection.GotoLine(lineNo);
                            objectDoc.Selection.EndOfLine();
                            objectDoc.Selection.Insert(content);
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
                        var objectDoc = dte.ActiveDocument.Object("TextDocument") as EnvDTE.TextDocument;

                        var textView = GetTextView(Instance.ServiceProvider);
                        var caretPosition = textView.Caret.Position.BufferPosition;
                        var document = caretPosition.Snapshot.GetOpenDocumentInCurrentContextWithChanges();

                        var toAdd = logic.GetCodeBehindContentToAdd(viewModelName, document);

                        foreach (var (anything, lineNo, contentToAdd) in toAdd)
                        {
                            if (anything)
                            {
                                objectDoc.Selection.GotoLine(lineNo);
                                objectDoc.Selection.EndOfLine();
                                objectDoc.Selection.Insert(contentToAdd);
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
