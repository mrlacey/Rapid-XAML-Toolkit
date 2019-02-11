// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Text;

namespace RapidXamlToolkit.XamlAnalysis
{
    public class RapidXamlDocument
    {
        public RapidXamlDocument()
        {
            this.SuggestionTags = new List<IRapidXamlTag>();
        }

        public string RawText { get; set; }

        public List<IRapidXamlTag> SuggestionTags { get; set; }

        public static RapidXamlDocument Create(ITextSnapshot snapshot)
        {
            var result = new RapidXamlDocument();

            try
            {
                var text = snapshot.GetText();

                // TODO: only try and parse doc if valid XML - need to consider when to redo tags, etc, while invalid
                if (text.IsValidXml())
                { }

                result.RawText = text;

                var processors = new List<(string, XamlElementProcessor)>
                {
                    ("Grid", new GridProcessor()),
                    ("TextBlock", new TextBlockProcessor()),
                };

                XamlElementExtractor.Parse(snapshot, text, processors, result.SuggestionTags);
            }
            catch (Exception e)
            {
                result.SuggestionTags.Add(new UnexpectedErrorTag
                {
                    Span = new Span(0, 0),
                    Message = "Unexpected error occurred while parsing XAML.",
                    ExtendedMessage = "Please log an issue to https://github.com/Microsoft/Rapid-XAML-Toolkit/issues Reason: " + e,
                });
            }

            return result;
        }
    }
}
