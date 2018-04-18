// <copyright file="GetXamlFromCodeWindowBaseCommand.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.Linq;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TextManager.Interop;

namespace RapidXamlToolkit
{
    internal class GetXamlFromCodeWindowBaseCommand : BaseCommand
    {
        public GetXamlFromCodeWindowBaseCommand(AsyncPackage package, ILogger logger)
            : base(package, logger)
        {
        }

        public AnalyzerOutput GetXaml(Microsoft.VisualStudio.Shell.IAsyncServiceProvider serviceProvider)
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
                    analyzer = new CSharpAnalyzer();
                }
                else if (activeDocument.Language == "Basic")
                {
                    analyzer = new VisualBasicAnalyzer();
                }

                if (isSelection)
                {
                    result = analyzer.GetSelectionOutput(document.GetSyntaxRootAsync().Result, semanticModel, selection.Start.Position, selection.End.Position);
                }
                else
                {
                    result = analyzer.GetSingleItemOutput(document.GetSyntaxRootAsync().Result, semanticModel, caretPosition.Position);
                }
            }
            else
            {
                ShowStatusBarMessage(serviceProvider, "No XAML copied. No profiles configured.");
            }

            return result;
        }

        protected static void ShowStatusBarMessage(Microsoft.VisualStudio.Shell.IAsyncServiceProvider serviceProvider, string message)
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
