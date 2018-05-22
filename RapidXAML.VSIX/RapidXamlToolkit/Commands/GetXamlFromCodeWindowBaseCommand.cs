// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.Shell;
using RapidXamlToolkit.Analyzers;
using RapidXamlToolkit.Logging;

namespace RapidXamlToolkit.Commands
{
    internal class GetXamlFromCodeWindowBaseCommand : BaseCommand
    {
        public GetXamlFromCodeWindowBaseCommand(AsyncPackage package, ILogger logger)
            : base(package, logger)
        {
        }

        public async Task<AnalyzerOutput> GetXamlAsync(IAsyncServiceProvider serviceProvider)
        {
            AnalyzerOutput result = null;

            if (AnalyzerBase.GetSettings().Profiles.Any())
            {
                var dte = await serviceProvider.GetServiceAsync(typeof(EnvDTE.DTE)) as EnvDTE.DTE;
                var activeDocument = dte.ActiveDocument;

                var textView = await GetTextViewAsync(serviceProvider);

                var selection = textView.Selection;

                bool isSelection = selection.Start.Position != selection.End.Position;

                var caretPosition = textView.Caret.Position.BufferPosition;

                var document = caretPosition.Snapshot.GetOpenDocumentInCurrentContextWithChanges();

                var semanticModel = await document.GetSemanticModelAsync();

                IDocumentAnalyzer analyzer = null;

                if (activeDocument.Language == "CSharp")
                {
                    analyzer = new CSharpAnalyzer(this.Logger);
                }
                else if (activeDocument.Language == "Basic")
                {
                    analyzer = new VisualBasicAnalyzer(this.Logger);
                }

                result = isSelection
                    ? analyzer?.GetSelectionOutput(await document.GetSyntaxRootAsync(), semanticModel, selection.Start.Position, selection.End.Position)
                    : analyzer?.GetSingleItemOutput(await document.GetSyntaxRootAsync(), semanticModel, caretPosition.Position);
            }
            else
            {
                await ShowStatusBarMessageAsync(serviceProvider, "No XAML copied. No profiles configured.");
            }

            return result;
        }

        protected static async System.Threading.Tasks.Task ShowStatusBarMessageAsync(IAsyncServiceProvider serviceProvider, string message)
        {
            try
            {
                var dte = await serviceProvider.GetServiceAsync(typeof(EnvDTE.DTE)) as EnvDTE.DTE;
                dte.StatusBar.Text = message;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        protected void MenuItem_BeforeQueryStatus(object sender, EventArgs e)
        {
            try
            {
                if (sender is OleMenuCommand menuCmd)
                {
                    menuCmd.Visible = menuCmd.Enabled = false;

                    if (AnalyzerBase.GetSettings().IsActiveProfileSet)
                    {
                        menuCmd.Visible = menuCmd.Enabled = true;
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
