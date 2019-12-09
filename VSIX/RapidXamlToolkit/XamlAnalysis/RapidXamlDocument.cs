// Copyright (c) Matt Lacey Ltd.. All rights reserved.
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
                result.Tags.Add(new UnexpectedErrorTag(new Span(0, 0), snapshot, fileName)
                {
                    Description = StringRes.Error_XamlAnalysisDescription,
                    ExtendedMessage = StringRes.Error_XamlAnalysisExtendedMessage.WithParams(e),
                });

                RapidXamlPackage.Logger?.RecordException(e);
            }

            return result;
        }

        public static List<(string, XamlElementProcessor)> GetAllProcessors(ProjectType projType)
        {
            return new List<(string, XamlElementProcessor)>
                    {
                        (Elements.Grid, new GridProcessor(projType, RapidXamlPackage.Logger)),
                        (Elements.TextBlock, new TextBlockProcessor(projType, RapidXamlPackage.Logger)),
                        (Elements.TextBox, new TextBoxProcessor(projType, RapidXamlPackage.Logger)),
                        (Elements.Button, new ButtonProcessor(projType, RapidXamlPackage.Logger)),
                        (Elements.Entry, new EntryProcessor(projType, RapidXamlPackage.Logger)),
                        (Elements.AppBarButton, new AppBarButtonProcessor(projType, RapidXamlPackage.Logger)),
                        (Elements.AppBarToggleButton, new AppBarToggleButtonProcessor(projType, RapidXamlPackage.Logger)),
                        (Elements.AutoSuggestBox, new AutoSuggestBoxProcessor(projType, RapidXamlPackage.Logger)),
                        (Elements.CalendarDatePicker, new CalendarDatePickerProcessor(projType, RapidXamlPackage.Logger)),
                        (Elements.CheckBox, new CheckBoxProcessor(projType, RapidXamlPackage.Logger)),
                        (Elements.ComboBox, new ComboBoxProcessor(projType, RapidXamlPackage.Logger)),
                        (Elements.DatePicker, new DatePickerProcessor(projType, RapidXamlPackage.Logger)),
                        (Elements.TimePicker, new TimePickerProcessor(projType, RapidXamlPackage.Logger)),
                        (Elements.Hub, new HubProcessor(projType, RapidXamlPackage.Logger)),
                        (Elements.HubSection, new HubSectionProcessor(projType, RapidXamlPackage.Logger)),
                        (Elements.HyperlinkButton, new HyperlinkButtonProcessor(projType, RapidXamlPackage.Logger)),
                        (Elements.RepeatButton, new RepeatButtonProcessor(projType, RapidXamlPackage.Logger)),
                        (Elements.Pivot, new PivotProcessor(projType, RapidXamlPackage.Logger)),
                        (Elements.PivotItem, new PivotItemProcessor(projType, RapidXamlPackage.Logger)),
                        (Elements.MenuFlyoutItem, new MenuFlyoutItemProcessor(projType, RapidXamlPackage.Logger)),
                        (Elements.MenuFlyoutSubItem, new MenuFlyoutSubItemProcessor(projType, RapidXamlPackage.Logger)),
                        (Elements.ToggleMenuFlyoutItem, new ToggleMenuFlyoutItemProcessor(projType, RapidXamlPackage.Logger)),
                        (Elements.RichEditBox, new RichEditBoxProcessor(projType, RapidXamlPackage.Logger)),
                        (Elements.ToggleSwitch, new ToggleSwitchProcessor(projType, RapidXamlPackage.Logger)),
                        (Elements.Slider, new SliderProcessor(projType, RapidXamlPackage.Logger)),
                        (Elements.Label, new LabelProcessor(projType, RapidXamlPackage.Logger)),
                        (Elements.PasswordBox, new PasswordBoxProcessor(projType, RapidXamlPackage.Logger)),
                        (Elements.MediaElement, new MediaElementProcessor(projType, RapidXamlPackage.Logger)),
                        (Elements.ListView, new SelectedItemAttributeProcessor(projType, RapidXamlPackage.Logger)),
                        (Elements.DataGrid, new SelectedItemAttributeProcessor(projType, RapidXamlPackage.Logger)),
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
