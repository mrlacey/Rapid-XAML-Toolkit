// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Text;
using RapidXamlToolkit.XamlAnalysis.Processors;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.XamlAnalysis
{
    public class RapidXamlDocument
    {
        public RapidXamlDocument()
        {
            this.Tags = new List<IRapidXamlAdornmentTag>();
        }

        public string RawText { get; set; }

        public List<IRapidXamlAdornmentTag> Tags { get; set; }

        public static RapidXamlDocument Create(ITextSnapshot snapshot)
        {
            var result = new RapidXamlDocument();

            try
            {
                var text = snapshot.GetText();

                // TODO: review when to redo tags, etc, while invalid, or remove any tags created previously
                if (text.IsValidXml())
                {
                    result.RawText = text;

                    // TODO: Need to limit processors to only run on appropriate platform (UWP/WPF/XF)
                    var processors = new List<(string, XamlElementProcessor)>
                    {
                        (Elements.Grid, new GridProcessor()),
                        (Elements.TextBlock, new TextBlockProcessor()),
                        (Elements.Button, new ButtonProcessor()),
                        (Elements.Entry, new EntryProcessor()),
                    };

                    XamlElementExtractor.Parse(snapshot, text, processors, result.Tags);
                }
            }
            catch (Exception e)
            {
                // TODO: localize this content
                result.Tags.Add(new UnexpectedErrorTag(new Span(0, 0), snapshot)
                {
                    Description = "Unexpected error occurred while parsing XAML.",
                    ExtendedMessage = "Please log an issue to https://github.com/Microsoft/Rapid-XAML-Toolkit/issues Reason: " + e,
                });

                RapidXamlPackage.Logger?.RecordException(e);
            }

            return result;
        }
    }
}
