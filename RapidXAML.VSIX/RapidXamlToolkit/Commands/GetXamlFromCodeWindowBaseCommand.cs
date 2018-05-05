// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Linq;
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

        public AnalyzerOutput GetXaml(IAsyncServiceProvider serviceProvider)
        {
            AnalyzerOutput result = null;

            if (AnalyzerBase.GetSettings().Profiles.Any())
            {
                var dte = (EnvDTE.DTE)serviceProvider.GetServiceAsync(typeof(EnvDTE.DTE)).Result;
                var activeDocument = dte.ActiveDocument;

                var textView = GetTextView(serviceProvider);

                var selection = textView.Selection;

                bool isSelection = selection.Start.Position != selection.End.Position;

                var caretPosition = textView.Caret.Position.BufferPosition;

                var document = caretPosition.Snapshot.GetOpenDocumentInCurrentContextWithChanges();

                var semanticModel = document.GetSemanticModelAsync().Result;

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
                    ? analyzer?.GetSelectionOutput(document.GetSyntaxRootAsync().Result, semanticModel, selection.Start.Position, selection.End.Position)
                    : analyzer?.GetSingleItemOutput(document.GetSyntaxRootAsync().Result, semanticModel, caretPosition.Position);
            }
            else
            {
                ShowStatusBarMessage(serviceProvider, "No XAML copied. No profiles configured.");
            }

            return result;
        }

        protected static void ShowStatusBarMessage(IAsyncServiceProvider serviceProvider, string message)
        {
            try
            {
                ((EnvDTE.DTE)serviceProvider.GetServiceAsync(typeof(EnvDTE.DTE)).Result).StatusBar.Text = message;
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
