// Copyright (c) Matt Lacey Ltd.. All rights reserved.
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
        [ImportingConstructor]
        public SuggestedActionsSourceProvider(IViewTagAggregatorFactoryService viewTagAggregatorFactoryService, ITextDocumentFactoryService textDocumentFactoryService, ISuggestedActionCategoryRegistryService suggestedActionCategoryRegistry)
        {
            this.ViewTagAggregatorFactoryService = viewTagAggregatorFactoryService;
            this.TextDocumentFactoryService = textDocumentFactoryService;
            this.SuggestedActionCategoryRegistry = suggestedActionCategoryRegistry;
        }

        public ITextDocumentFactoryService TextDocumentFactoryService { get; set; }

        public ISuggestedActionCategoryRegistryService SuggestedActionCategoryRegistry { get; }

        public IViewTagAggregatorFactoryService ViewTagAggregatorFactoryService { get; set; }

        public ISuggestedActionsSource CreateSuggestedActionsSource(ITextView textView, ITextBuffer textBuffer)
        {
            if (this.TextDocumentFactoryService.TryGetTextDocument(textView.TextBuffer, out var document))
            {
                return textView.Properties.GetOrCreateSingletonProperty(() =>
                    new SuggestedActionsSource(this.ViewTagAggregatorFactoryService, this.SuggestedActionCategoryRegistry, textView, textBuffer, document.FilePath));
            }

            return null;
        }
    }
}
