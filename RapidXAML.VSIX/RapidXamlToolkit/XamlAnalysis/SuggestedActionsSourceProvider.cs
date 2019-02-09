// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace RapidXamlToolkit.XamlAnalysis
{
    [Export(typeof(ISuggestedActionsSourceProvider))]
    [Name("Rapid XAML Suggested Actions")]
    [ContentType(KnownContentTypes.Xaml)]
    public class SuggestedActionsSourceProvider : ISuggestedActionsSourceProvider
    {
        ITextDocumentFactoryService TextDocumentFactoryService { get; set; }

        IViewTagAggregatorFactoryService ViewTagAggregatorFactoryService { get; set; }

        [ImportingConstructor]
        public SuggestedActionsSourceProvider(IViewTagAggregatorFactoryService viewTagAggregatorFactoryService, ITextDocumentFactoryService textDocumentFactoryService)
        {
            ViewTagAggregatorFactoryService = viewTagAggregatorFactoryService;
            TextDocumentFactoryService = textDocumentFactoryService;
        }

        public ISuggestedActionsSource CreateSuggestedActionsSource(ITextView textView, ITextBuffer textBuffer)
        {
            if (TextDocumentFactoryService.TryGetTextDocument(textView.TextBuffer, out var document))
            {
                return textView.Properties.GetOrCreateSingletonProperty(() =>
                    new SuggestedActionsSource(ViewTagAggregatorFactoryService, textView, textBuffer, document.FilePath));
            }

            return null;
        }
    }
}
