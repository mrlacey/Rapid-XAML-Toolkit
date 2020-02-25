// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.Text;
using Newtonsoft.Json;
using RapidXamlToolkit.Logging;
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

        public static RapidXamlDocument Create(ITextSnapshot snapshot, string fileName, IVisualStudioAbstraction vsa)
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
                        var vsAbstraction = vsa;

                        // This will happen if open a project with open XAML files before the package is initialized.
                        if (vsAbstraction == null)
                        {
                            vsAbstraction = new VisualStudioAbstraction(new RxtLogger(), null, ProjectHelpers.Dte);
                        }

                        var projType = vsAbstraction.GetProjectType(ProjectHelpers.Dte.Solution.GetProjectContainingFile(fileName));

                        XamlElementExtractor.Parse(projType, fileName, snapshot, text, GetAllProcessors(projType), result.Tags, suppressions);
                    }
                }
            }
            catch (Exception e)
            {
                result.Tags.Add(new UnexpectedErrorTag(new Span(0, 0), snapshot, fileName, SharedRapidXamlPackage.Logger)
                {
                    Description = StringRes.Error_XamlAnalysisDescription,
                    ExtendedMessage = StringRes.Error_XamlAnalysisExtendedMessage.WithParams(e),
                });

                SharedRapidXamlPackage.Logger?.RecordException(e);
            }

            return result;
        }

        public static List<(string, XamlElementProcessor)> GetAllProcessors(ProjectType projType, ILogger logger = null)
        {
            logger = logger ?? SharedRapidXamlPackage.Logger;

            var processors = new List<(string, XamlElementProcessor)>
                    {
                        (Elements.Grid, new GridProcessor(projType, logger)),
                        (Elements.TextBlock, new TextBlockProcessor(projType, logger)),
                        (Elements.TextBox, new TextBoxProcessor(projType, logger)),
                        (Elements.Button, new ButtonProcessor(projType, logger)),
                        (Elements.Entry, new EntryProcessor(projType, logger)),
                        (Elements.AppBarButton, new AppBarButtonProcessor(projType, logger)),
                        (Elements.AppBarToggleButton, new AppBarToggleButtonProcessor(projType, logger)),
                        (Elements.AutoSuggestBox, new AutoSuggestBoxProcessor(projType, logger)),
                        (Elements.CalendarDatePicker, new CalendarDatePickerProcessor(projType, logger)),
                        (Elements.CheckBox, new CheckBoxProcessor(projType, logger)),
                        (Elements.ComboBox, new ComboBoxProcessor(projType, logger)),
                        (Elements.DatePicker, new DatePickerProcessor(projType, logger)),
                        (Elements.TimePicker, new TimePickerProcessor(projType, logger)),
                        (Elements.Hub, new HubProcessor(projType, logger)),
                        (Elements.HubSection, new HubSectionProcessor(projType, logger)),
                        (Elements.HyperlinkButton, new HyperlinkButtonProcessor(projType, logger)),
                        (Elements.RepeatButton, new RepeatButtonProcessor(projType, logger)),
                        (Elements.Pivot, new PivotProcessor(projType, logger)),
                        (Elements.PivotItem, new PivotItemProcessor(projType, logger)),
                        (Elements.MenuFlyoutItem, new MenuFlyoutItemProcessor(projType, logger)),
                        (Elements.MenuFlyoutSubItem, new MenuFlyoutSubItemProcessor(projType, logger)),
                        (Elements.ToggleMenuFlyoutItem, new ToggleMenuFlyoutItemProcessor(projType, logger)),
                        (Elements.RichEditBox, new RichEditBoxProcessor(projType, logger)),
                        (Elements.ToggleSwitch, new ToggleSwitchProcessor(projType, logger)),
                        (Elements.Slider, new SliderProcessor(projType, logger)),
                        (Elements.Label, new LabelProcessor(projType, logger)),
                        (Elements.PasswordBox, new PasswordBoxProcessor(projType, logger)),
                        (Elements.MediaElement, new MediaElementProcessor(projType, logger)),
                        (Elements.ListView, new SelectedItemAttributeProcessor(projType, logger)),
                        (Elements.DataGrid, new SelectedItemAttributeProcessor(projType, logger)),
                    };

            var customProcessors = GetCustomProcessors();

            foreach (var customProcessor in customProcessors)
            {
                processors.Add((customProcessor.TargetType(), new CustomProcessorWrapper(customProcessor, projType, logger)));
            }

            return processors;
        }

        public static List<RapidXaml.CustomAnalyzer> GetCustomProcessors()
        {
            var result = new List<RapidXaml.CustomAnalyzer>();

            //// TODO: Do MEF magic to find other custom processors here

            result.Add(new CustomAnalysis.FooAnalysis());
            result.Add(new CustomAnalysis.TwoPaneViewAnalyzer());

            return result;
        }

        public void Clear()
        {
            this.RawText = string.Empty;
            this.Tags.Clear();
            SuppressionsCache.Clear();
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
                SharedRapidXamlPackage.Logger?.RecordError("Failed to load 'suppressions.xamlAnalysis' file.");
                SharedRapidXamlPackage.Logger?.RecordException(exc);
            }

            return result;
        }
    }
}
