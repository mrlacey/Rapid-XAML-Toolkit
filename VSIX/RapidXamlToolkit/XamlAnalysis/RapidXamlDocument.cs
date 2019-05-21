// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.Text;
using Newtonsoft.Json;
using RapidXamlToolkit.Resources;
using RapidXamlToolkit.XamlAnalysis.Processors;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.XamlAnalysis
{
    public class RapidXamlDocument
    {
        public RapidXamlDocument()
        {
            this.Tags = new TagList();
        }

        public string RawText { get; set; }

        public TagList Tags { get; set; }

        private static Dictionary<string, (DateTime timeStamp, List<TagSuppression> suppressions)> SuppressionsCache { get; }
            = new Dictionary<string, (DateTime, List<TagSuppression>)>();

        public static RapidXamlDocument Create(ITextSnapshot snapshot, string fileName)
        {
            var result = new RapidXamlDocument();

            try
            {
                var text = snapshot.GetText();

                // TODO: ISSUE#165 review when to redo tags, etc, while invalid, or remove any tags created previously
                if (text.IsValidXml())
                {
                    result.RawText = text;

                    var suppressions = GetSuppressions(fileName);

                    // If suppressing all tags in file, don't bother parsing the file
                    if (suppressions == null || suppressions?.Any(s => string.IsNullOrWhiteSpace(s.TagErrorCode)) == false)
                    {
                        XamlElementExtractor.Parse(fileName, snapshot, text, GetAllProcessors(), result.Tags, suppressions);
                    }
                }
            }
            catch (Exception e)
            {
                result.Tags.Add(new UnexpectedErrorTag(new Span(0, 0), snapshot, fileName)
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

        private static List<TagSuppression> GetSuppressions(string fileName)
        {
            List<TagSuppression> result = null;

            try
            {
                var proj = ProjectHelpers.Dte.Solution.GetProjectContainingFile(fileName);

                var suppressionsFile = Path.Combine(Path.GetDirectoryName(proj.FullName), "suppressions.xamlAnalysis");

                if (File.Exists(suppressionsFile))
                {
                    List<TagSuppression> allSuppressions = null;
                    var fileTime = File.GetLastWriteTimeUtc(suppressionsFile);

                    if (SuppressionsCache.ContainsKey(suppressionsFile))
                    {
                        if (SuppressionsCache[suppressionsFile].timeStamp == fileTime)
                        {
                            allSuppressions = SuppressionsCache[suppressionsFile].suppressions;
                        }
                    }

                    if (allSuppressions == null)
                    {
                        var json = File.ReadAllText(suppressionsFile);
                        allSuppressions = JsonConvert.DeserializeObject<List<TagSuppression>>(json);
                    }

                    SuppressionsCache[suppressionsFile] = (fileTime, allSuppressions);

                    result = allSuppressions.Where(s => string.IsNullOrWhiteSpace(s.FileName) || fileName.EndsWith(s.FileName)).ToList();
                }
            }
            catch (Exception exc)
            {
                RapidXamlPackage.Logger?.RecordError("Failed to load 'suppressions.xamlAnalysis' file.");
                RapidXamlPackage.Logger?.RecordException(exc);
            }

            return result;
        }
    }
}
