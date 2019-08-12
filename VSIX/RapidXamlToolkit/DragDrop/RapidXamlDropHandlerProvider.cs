// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.ComponentModel.Composition;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Editor.DragDrop;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.Utilities;
using RapidXamlToolkit.Commands;
using RapidXamlToolkit.Logging;
using RapidXamlToolkit.Resources;
using RapidXamlToolkit.VisualStudioIntegration;
using RapidXamlToolkit.XamlAnalysis;
using Task = System.Threading.Tasks.Task;

namespace RapidXamlToolkit.DragDrop
{
    [Export(typeof(IDropHandlerProvider))]
    [DropFormat("CF_VSSTGPROJECTITEMS")]
    [Name("RapidXamlDropHandler")]
    [ContentType(KnownContentTypes.Xaml)]
    [Order(Before = "DefaultFileDropHandler")]
    internal class RapidXamlDropHandlerProvider : IDropHandlerProvider
    {
        private static DTE dte;

        private static AsyncPackage Package { get; set; }

        private static ILogger Logger { get; set; }

        [Import(typeof(ITextBufferUndoManagerProvider))]
        private ITextBufferUndoManagerProvider UndoProvider { get; set; }

        [Import(typeof(ITextDocumentFactoryService))]
        private ITextDocumentFactoryService TextDocumentFactoryService { get; set; }

        public static async Task InitializeAsync(AsyncPackage package, ILogger logger)
        {
            Logger = logger;
            Package = package;

            dte = await package.GetServiceAsync(typeof(DTE)) as DTE;
        }

        public IDropHandler GetAssociatedDropHandler(IWpfTextView view)
        {
            ITextBufferUndoManager undoManager = this.UndoProvider.GetTextBufferUndoManager(view.TextBuffer);

            var vsa = new VisualStudioAbstraction(Logger, Package, dte);

            var projType = ProjectType.Unknown;

            if (this.TextDocumentFactoryService.TryGetTextDocument(view.TextBuffer, out ITextDocument textDocument))
            {
                var proj = ProjectHelpers.Dte.Solution.GetProjectContainingFile(textDocument.FilePath);
                projType = vsa.GetProjectType(proj);

                Logger?.RecordInfo(StringRes.Info_DetectedProjectType.WithParams(projType.GetDescription()));
            }

            return view.Properties.GetOrCreateSingletonProperty(() => new RapidXamlDropHandler(Logger, view, undoManager, vsa, projType));
        }
    }
}
