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

        public VisualStudioAbstraction VsAbstraction { get; set; }

        private static Dictionary<string, (DateTime timeStamp, List<TagSuppression> suppressions)> SuppressionsCache { get; }
            = new Dictionary<string, (DateTime, List<TagSuppression>)>();

        public static RapidXamlDocument Create(ITextSnapshot snapshot, string fileName, IVisualStudioAbstraction vsa, string projectFile)
        {
            var result = new RapidXamlDocument();

            try
            {
                var text = snapshot.GetText();

                if (text.IsValidXml())
                {
                    result.RawText = text;

                    var suppressions = GetSuppressions(fileName, vsa, projectFile);

                    // If suppressing all tags in file, don't bother parsing the file
                    if (suppressions == null || suppressions?.Any(s => string.IsNullOrWhiteSpace(s.TagErrorCode)) == false)
                    {
                        var vsAbstraction = vsa;

                        // This will happen if open a project with open XAML files before the package is initialized.
                        if (vsAbstraction == null)
                        {
                            vsAbstraction = new VisualStudioAbstraction(new RxtLogger(), null, ProjectHelpers.Dte);
                        }

                        var projType = vsAbstraction.GetProjectType(vsa.GetProjectContainingFile(fileName));

                        XamlElementExtractor.Parse(projType, fileName, snapshot, text, GetAllProcessors(projType, projectFile), result.Tags, suppressions);
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

        public static List<(string, XamlElementProcessor)> GetAllProcessors(ProjectType projType, string projectFile)
        {
            var logger = SharedRapidXamlPackage.Logger;

            var processorEssentials = new ProcessorEssentials
            {
                ProjectType = projType,
                Logger = logger,
                ProjectFilePath = projectFile,
            };

            return new List<(string, XamlElementProcessor)>
                    {
                        (Elements.Grid, new GridProcessor(processorEssentials)),
                        (Elements.TextBlock, new TextBlockProcessor(processorEssentials)),
                        (Elements.TextBox, new TextBoxProcessor(processorEssentials)),
                        (Elements.Button, new ButtonProcessor(processorEssentials)),
                        (Elements.Entry, new EntryProcessor(processorEssentials)),
                        (Elements.AppBarButton, new AppBarButtonProcessor(processorEssentials)),
                        (Elements.AppBarToggleButton, new AppBarToggleButtonProcessor(processorEssentials)),
                        (Elements.AutoSuggestBox, new AutoSuggestBoxProcessor(processorEssentials)),
                        (Elements.CalendarDatePicker, new CalendarDatePickerProcessor(processorEssentials)),
                        (Elements.CheckBox, new CheckBoxProcessor(processorEssentials)),
                        (Elements.ComboBox, new ComboBoxProcessor(processorEssentials)),
                        (Elements.DatePicker, new DatePickerProcessor(processorEssentials)),
                        (Elements.TimePicker, new TimePickerProcessor(processorEssentials)),
                        (Elements.Hub, new HubProcessor(processorEssentials)),
                        (Elements.HubSection, new HubSectionProcessor(processorEssentials)),
                        (Elements.HyperlinkButton, new HyperlinkButtonProcessor(processorEssentials)),
                        (Elements.RepeatButton, new RepeatButtonProcessor(processorEssentials)),
                        (Elements.Pivot, new PivotProcessor(processorEssentials)),
                        (Elements.PivotItem, new PivotItemProcessor(processorEssentials)),
                        (Elements.MenuFlyoutItem, new MenuFlyoutItemProcessor(processorEssentials)),
                        (Elements.MenuFlyoutSubItem, new MenuFlyoutSubItemProcessor(processorEssentials)),
                        (Elements.ToggleMenuFlyoutItem, new ToggleMenuFlyoutItemProcessor(processorEssentials)),
                        (Elements.RichEditBox, new RichEditBoxProcessor(processorEssentials)),
                        (Elements.ToggleSwitch, new ToggleSwitchProcessor(processorEssentials)),
                        (Elements.Slider, new SliderProcessor(processorEssentials)),
                        (Elements.Label, new LabelProcessor(processorEssentials)),
                        (Elements.PasswordBox, new PasswordBoxProcessor(processorEssentials)),
                        (Elements.MediaElement, new MediaElementProcessor(processorEssentials)),
                        (Elements.ListView, new SelectedItemAttributeProcessor(processorEssentials)),
                        (Elements.DataGrid, new SelectedItemAttributeProcessor(processorEssentials)),
                    };
        }

        public void Clear()
        {
            this.RawText = string.Empty;
            this.Tags.Clear();
            SuppressionsCache.Clear();
        }

        private static List<TagSuppression> GetSuppressions(string fileName, IVisualStudioAbstraction vsa, string projectFile)
        {
            List<TagSuppression> result = null;

            try
            {
                if (string.IsNullOrWhiteSpace(projectFile))
                {
                    var proj = vsa.GetProjectContainingFile(fileName);
                    projectFile = proj.FullName;
                }

                var suppressionsFile = Path.Combine(Path.GetDirectoryName(projectFile), "suppressions.xamlAnalysis");

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
