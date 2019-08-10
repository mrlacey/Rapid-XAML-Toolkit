// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.Text;
using Newtonsoft.Json;
using RapidXamlToolkit.Resources;
using RapidXamlToolkit.VisualStudioIntegration;
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
                        (Elements.Grid, new GridProcessor(RapidXamlPackage.Logger)),
                        (Elements.TextBlock, new TextBlockProcessor(RapidXamlPackage.Logger)),
                        (Elements.TextBox, new TextBoxProcessor(RapidXamlPackage.Logger)),
                        (Elements.Button, new ButtonProcessor(RapidXamlPackage.Logger)),
                        (Elements.Entry, new EntryProcessor(RapidXamlPackage.Logger)),
                        (Elements.AppBarButton, new AppBarButtonProcessor(RapidXamlPackage.Logger)),
                        (Elements.AppBarToggleButton, new AppBarToggleButtonProcessor(RapidXamlPackage.Logger)),
                        (Elements.AutoSuggestBox, new AutoSuggestBoxProcessor(RapidXamlPackage.Logger)),
                        (Elements.CalendarDatePicker, new CalendarDatePickerProcessor(RapidXamlPackage.Logger)),
                        (Elements.CheckBox, new CheckBoxProcessor(RapidXamlPackage.Logger)),
                        (Elements.ComboBox, new ComboBoxProcessor(RapidXamlPackage.Logger)),
                        (Elements.DatePicker, new DatePickerProcessor(RapidXamlPackage.Logger)),
                        (Elements.TimePicker, new TimePickerProcessor(RapidXamlPackage.Logger)),
                        (Elements.Hub, new HubProcessor(RapidXamlPackage.Logger)),
                        (Elements.HubSection, new HubSectionProcessor(RapidXamlPackage.Logger)),
                        (Elements.HyperlinkButton, new HyperlinkButtonProcessor(RapidXamlPackage.Logger)),
                        (Elements.RepeatButton, new RepeatButtonProcessor(RapidXamlPackage.Logger)),
                        (Elements.Pivot, new PivotProcessor(RapidXamlPackage.Logger)),
                        (Elements.PivotItem, new PivotItemProcessor(RapidXamlPackage.Logger)),
                        (Elements.MenuFlyoutItem, new MenuFlyoutItemProcessor(RapidXamlPackage.Logger)),
                        (Elements.MenuFlyoutSubItem, new MenuFlyoutSubItemProcessor(RapidXamlPackage.Logger)),
                        (Elements.ToggleMenuFlyoutItem, new ToggleMenuFlyoutItemProcessor(RapidXamlPackage.Logger)),
                        (Elements.RichEditBox, new RichEditBoxProcessor(RapidXamlPackage.Logger)),
                        (Elements.ToggleSwitch, new ToggleSwitchProcessor(RapidXamlPackage.Logger)),
                        (Elements.Slider, new SliderProcessor(RapidXamlPackage.Logger)),
                        (Elements.Label, new LabelProcessor(RapidXamlPackage.Logger)),
                        (Elements.PasswordBox, new PasswordBoxProcessor(RapidXamlPackage.Logger)),
                        (Elements.MediaElement, new MediaElementProcessor(RapidXamlPackage.Logger)),
                        (Elements.ListView, new SelectedItemAttributeProcessor(RapidXamlPackage.Logger)),
                        (Elements.DataGrid, new SelectedItemAttributeProcessor(RapidXamlPackage.Logger)),
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
