// <copyright file="GetXamlFromCodeWindowBaseCommand.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.Linq;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.TextManager.Interop;

namespace RapidXamlToolkit
{
    internal class GetXamlFromCodeWindowBaseCommand
    {
        public void GetXaml(Microsoft.VisualStudio.Shell.IAsyncServiceProvider serviceProvider, Action<string> ifContent)
        {
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

                AnalyzerOutput analyzerOutput = null;

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
                    analyzerOutput = analyzer.GetSelectionOutput(document.GetSyntaxRootAsync().Result, semanticModel, selection.Start.Position, selection.End.Position);
                }
                else
                {
                    analyzerOutput = analyzer.GetSingleItemOutput(document.GetSyntaxRootAsync().Result, semanticModel, caretPosition.Position);
                }

                if (analyzerOutput != null && analyzerOutput.OutputType != AnalyzerOutputType.None)
                {
                    var message = analyzerOutput.Output;

                    if (!string.IsNullOrWhiteSpace(message))
                    {
                        ifContent?.Invoke(message);
                    }
                    else
                    {
                        // Log no output
                    }

                    ShowStatusBarMessage(serviceProvider, $"Copied XAML for {analyzerOutput.OutputType}: {analyzerOutput.Name}");
                }
                else
                {
                    // TODO [vNext]: play error noise/beep if can't do something or error?
                    ShowStatusBarMessage(serviceProvider, "No XAML copied.");
                }
            }
            else
            {
                ShowStatusBarMessage(serviceProvider, "No XAML copied. No profiles configured.");
            }
        }

        private static void ShowStatusBarMessage(Microsoft.VisualStudio.Shell.IAsyncServiceProvider serviceProvider, string message)
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

        private static Microsoft.VisualStudio.Text.Editor.IWpfTextView GetTextView(Microsoft.VisualStudio.Shell.IAsyncServiceProvider serviceProvider)
        {
            var textManager = (IVsTextManager)serviceProvider.GetServiceAsync(typeof(SVsTextManager)).Result;

            if (textManager == null)
            {
                return null;
            }

            textManager.GetActiveView(1, null, out IVsTextView textView);

            if (textView == null)
            {
                return null;
            }

            return GetEditorAdaptersFactoryService(serviceProvider).GetWpfTextView(textView);
        }

        private static IVsEditorAdaptersFactoryService GetEditorAdaptersFactoryService(Microsoft.VisualStudio.Shell.IAsyncServiceProvider serviceProvider)
        {
            IComponentModel componentModel = (IComponentModel)serviceProvider.GetServiceAsync(typeof(SComponentModel)).Result;
            return componentModel.GetService<IVsEditorAdaptersFactoryService>();
        }
    }
}
