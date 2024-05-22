// Copyright (c) Matt Lacey Ltd. All rights reserved.
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
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                if (!(await serviceProvider.GetServiceAsync(typeof(EnvDTE.DTE)) is DTE dte))
                {
                    RapidXamlPackage.Logger?.RecordError(StringRes.Error_FailedToGetDteInGetXamlAsync);
                }
                else
                {
                    var activeDocument = dte.ActiveDocument;

                    var textView = await GetTextViewAsync(serviceProvider);

                    var selection = textView.Selection;

                    bool isSelection = selection.Start.Position != selection.End.Position;

                    var caretPosition = textView.Caret.Position.BufferPosition;

                    var document = caretPosition.Snapshot.GetOpenDocumentInCurrentContextWithChanges();

                    if (document == null)
                    {
                        this.Logger?.RecordInfo(StringRes.Info_UnableToRetrieveEditorDocument);
                    }
                    else
                    {
                        var semanticModel = await document.GetSemanticModelAsync();

                        var vs = new VisualStudioAbstraction(this.Logger, this.AsyncPackage, dte);
                        var xamlIndent = await vs.GetXamlIndentAsync();

                        var proj = dte.Solution.GetProjectContainingFile(document.FilePath);

                        if (proj == null)
                        {
                            // Default to the "active project" if file is not part of a known project
                            proj = ((Array)dte.ActiveSolutionProjects).GetValue(0) as Project;
                        }

                        var projType = vs.GetProjectType(proj);

                        this.Logger?.RecordInfo(StringRes.Info_DetectedProjectType.WithParams(projType.GetDescription()));

                        IDocumentParser parser = null;

                        if (activeDocument.Language == "CSharp")
                        {
                            parser = new CSharpParser(this.Logger, projType, xamlIndent);
                        }
                        else if (activeDocument.Language == "Basic")
                        {
                            parser = new VisualBasicParser(this.Logger, projType, xamlIndent);
                        }

                        result = isSelection
                            ? parser?.GetSelectionOutput(await document.GetSyntaxRootAsync(), semanticModel, selection.Start.Position, selection.End.Position)
                            : parser?.GetSingleItemOutput(await document.GetSyntaxRootAsync(), semanticModel, caretPosition.Position);
                    }
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
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            try
            {
                if (await serviceProvider.GetServiceAsync(typeof(EnvDTE.DTE)) is DTE dte)
                {
                    dte.StatusBar.Text = message;
                }
                else
                {
                    RapidXamlPackage.Logger?.RecordError(StringRes.Error_FailedToGetDteInShowStatusBarMessageAsync);
                }
            }
            catch (Exception exc)
            {
                RapidXamlPackage.Logger?.RecordException(exc);
            }
        }
    }
}
