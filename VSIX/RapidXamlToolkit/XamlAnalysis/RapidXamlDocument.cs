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

                // TODO: ISSUE#165 review when to redo tags, etc, while invalid, or remove any tags created previously
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
                        (Elements.CheckBox, new CheckBoxProcessor()),
                        (Elements.ComboBox, new ComboBoxProcessor()),
                        (Elements.DatePicker, new DatePickerProcessor()),
                        (Elements.TimePicker, new TimePickerProcessor()),
                        (Elements.Hub, new HubProcessor()),
                        (Elements.HubSection, new HubSectionProcessor()),
                        (Elements.HyperlinkButton, new HyperlinkButtonProcessor()),
                        (Elements.RepeatButton, new RepeatButtonProcessor()),
                        (Elements.Pivot, new PivotProcessor()),
                        (Elements.PivotItem, new PivotItemProcessor()),
                        (Elements.MenuFlyoutItem, new MenuFlyoutItemProcessor()),
                        (Elements.MenuFlyoutSubItem, new MenuFlyoutSubItemProcessor()),
                        (Elements.ToggleMenuFlyoutItem, new ToggleMenuFlyoutItemProcessor()),
                        (Elements.RichEditBox, new RichEditBoxProcessor()),
                        (Elements.ToggleSwitch, new ToggleSwitchProcessor()),
                        (Elements.Slider, new SliderProcessor()),
                        (Elements.Label, new LabelProcessor()),
                        (Elements.PasswordBox, new PasswordBoxProcessor()),
                        (Elements.MediaElement, new MediaElementProcessor()),
                        (Elements.ListView, new SelectedItemAttributeProcessor()),
                        (Elements.DataGrid, new SelectedItemAttributeProcessor()),
                    };
        }
    }
}
