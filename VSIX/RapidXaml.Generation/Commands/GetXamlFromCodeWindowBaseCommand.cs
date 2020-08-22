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
                if (!(await serviceProvider.GetServiceAsync(typeof(EnvDTE.DTE)) is DTE dte))
                {
                    SharedRapidXamlPackage.Logger?.RecordError(StringRes.Error_FailedToGetDteInGetXamlAsync);
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

                    var vs = new VisualStudioAbstraction(this.Logger, this.AsyncPackage, dte);
                    var xamlIndent = await vs.GetXamlIndentAsync();

                    var proj = dte.Solution.GetProjectContainingFile(document.FilePath);

                    if (proj == null)
                    {
                        proj = vs.GetActiveProject().Project;
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
                    SharedRapidXamlPackage.Logger?.RecordError(StringRes.Error_FailedToGetDteInShowStatusBarMessageAsync);
                }
            }
            catch (Exception exc)
            {
                SharedRapidXamlPackage.Logger?.RecordException(exc);
            }
        }
    }
}
