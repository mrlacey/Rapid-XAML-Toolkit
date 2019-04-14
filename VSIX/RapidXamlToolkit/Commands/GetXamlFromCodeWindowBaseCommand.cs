// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Linq;
using System.Threading.Tasks;
using EnvDTE;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.Shell;
using RapidXamlToolkit.Logging;
using RapidXamlToolkit.Parsers;
using RapidXamlToolkit.Resources;
using RapidXamlToolkit.VisualStudioIntegration;
using Task = System.Threading.Tasks.Task;

namespace RapidXamlToolkit.Commands
{
    internal class GetXamlFromCodeWindowBaseCommand : BaseCommand
    {
        public GetXamlFromCodeWindowBaseCommand(AsyncPackage package, ILogger logger)
            : base(package, logger)
        {
        }

        public async Task<ParserOutput> GetXamlAsync(IAsyncServiceProvider serviceProvider)
        {
            ParserOutput result = null;

            if (CodeParserBase.GetSettings().Profiles.Any())
            {
                if (!(await serviceProvider.GetServiceAsync(typeof(EnvDTE.DTE)) is DTE dte))
                {
                    RapidXamlPackage.Logger?.RecordError("Failed to get DTE in GetXamlFromCodeWindowBaseCommand.GetXamlAsync");
                }
                else
                {
                    var activeDocument = dte.ActiveDocument;

                    var textView = await GetTextViewAsync(serviceProvider);

                    var selection = textView.Selection;

                    bool isSelection = selection.Start.Position != selection.End.Position;

                    var caretPosition = textView.Caret.Position.BufferPosition;

                    var document = caretPosition.Snapshot.GetOpenDocumentInCurrentContextWithChanges();

                    var semanticModel = await document.GetSemanticModelAsync();

                    var vs = new VisualStudioAbstraction(this.Logger, this.ServiceProvider, dte);
                    var xamlIndent = await vs.GetXamlIndentAsync();

                    System.Diagnostics.Debug.WriteLine(vs.GetProjectType(vs.GetActiveProject().Project));

                    IDocumentParser parser = null;

                    if (activeDocument.Language == "CSharp")
                    {
                        parser = new CSharpParser(this.Logger, xamlIndent);
                    }
                    else if (activeDocument.Language == "Basic")
                    {
                        parser = new VisualBasicParser(this.Logger, xamlIndent);
                    }

                    result = isSelection
                        ? parser?.GetSelectionOutput(await document.GetSyntaxRootAsync(), semanticModel, selection.Start.Position, selection.End.Position)
                        : parser?.GetSingleItemOutput(await document.GetSyntaxRootAsync(), semanticModel, caretPosition.Position);
                }
            }
            else
            {
                await ShowStatusBarMessageAsync(serviceProvider, StringRes.UI_NoXamlCopiedNoProfilesConfigured);
            }

            return result;
        }

        protected static async Task ShowStatusBarMessageAsync(IAsyncServiceProvider serviceProvider, string message)
        {
            try
            {
                if (await serviceProvider.GetServiceAsync(typeof(EnvDTE.DTE)) is DTE dte)
                {
                    dte.StatusBar.Text = message;
                }
                else
                {
                    RapidXamlPackage.Logger?.RecordError("Failed to get DTE in GetXamlFromCodeWindowBaseCommand.ShowStatusBarMessageAsync");
                }
            }
            catch (Exception exc)
            {
                RapidXamlPackage.Logger?.RecordException(exc);
            }
        }

        protected void MenuItem_BeforeQueryStatus(object sender, EventArgs e)
        {
            try
            {
                if (sender is OleMenuCommand menuCmd)
                {
                    menuCmd.Visible = menuCmd.Enabled = false;

                    if (CodeParserBase.GetSettings().IsActiveProfileSet)
                    {
                        menuCmd.Visible = menuCmd.Enabled = true;
                    }
                }
            }
            catch (Exception exc)
            {
                this.Logger.RecordException(exc);
                throw;  // Remove for launch. see issue #90
            }
        }
    }
}
