// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Text;
using RapidXamlToolkit.Resources;
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

                    XamlElementExtractor.Parse(snapshot, text, GetAllProcessors(), result.Tags);
                }
            }
            catch (Exception e)
            {
                result.Tags.Add(new UnexpectedErrorTag(new Span(0, 0), snapshot)
                {
                    Description = StringRes.Error_XamlAnalysisDescription,
                    ExtendedMessage = StringRes.Error_XamlAnalysisExtendedMessage.WithParams(e),
                });

                RapidXamlPackage.Logger?.RecordException(e);
            }

            return result;
        }

        public static List<(string, XamlElementProcessor)> GetAllProcessors()
        {
            // TODO: Issue#134 - Need to limit processors to only run on appropriate platform (UWP/WPF/XF)
            return new List<(string, XamlElementProcessor)>
                    {
                        (Elements.Grid, new GridProcessor()),
                        (Elements.TextBlock, new TextBlockProcessor()),
                        (Elements.TextBox, new TextBoxProcessor()),
                        (Elements.Button, new ButtonProcessor()),
                        (Elements.Entry, new EntryProcessor()),
                        (Elements.AppBarButton, new AppBarButtonProcessor()),
                        (Elements.AppBarToggleButton, new AppBarToggleButtonProcessor()),
                        (Elements.AutoSuggestBox, new AutoSuggestBoxProcessor()),
                        (Elements.CalendarDatePicker, new CalendarDatePickerProcessor()),
                    };
        }
    }
}
