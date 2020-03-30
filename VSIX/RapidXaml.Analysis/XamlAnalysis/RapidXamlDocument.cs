// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.Text;
using Newtonsoft.Json;
using RapidXaml;
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

            List<(string, XamlElementProcessor)> processors = null;

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

                        var proj = ProjectHelpers.Dte.Solution.GetProjectContainingFile(fileName);
                        var projType = vsAbstraction.GetProjectType(proj);
                        var projDir = Path.GetDirectoryName(proj.FileName);

                        processors = GetAllProcessors(projType, projDir);

                        // May need to tidy-up-release processors after this - depending on caching. X-Ref http://www.visualstudioextensibility.com/2013/03/17/the-strange-case-of-quot-loaderlock-was-detected-quot-with-a-com-add-in-written-in-net/
                        XamlElementExtractor.Parse(projType, fileName, snapshot, text, processors, result.Tags, suppressions);
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

        public static List<(string, XamlElementProcessor)> GetAllProcessors(ProjectType projType, string projectPath, ILogger logger = null)
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

            if (!string.IsNullOrWhiteSpace(projectPath))
            {
                var customProcessors = GetCustomProcessors(projectPath);

#if DEBUG
                // These types exists for testing only and so are only referenced during Debug
                customProcessors.Add(new CustomAnalysis.FooAnalysis());
                customProcessors.Add(new CustomAnalysis.BadCustomAnalyzer());
                customProcessors.Add(new CustomAnalysis.InternalBadCustomAnalyzer());
                customProcessors.Add(new CustomAnalysis.CustomGridDefinitionAnalyzer());
                customProcessors.Add(new CustomAnalysis.RenameElementTestAnalyzer());
                customProcessors.Add(new CustomAnalysis.ReplaceElementTestAnalyzer());
                customProcessors.Add(new CustomAnalysis.AddChildTestAnalyzer());
                customProcessors.Add(new CustomAnalysis.RemoveFirstChildAnalyzer());
#endif
                customProcessors.Add(new CustomAnalysis.TwoPaneViewAnalyzer());

                foreach (var customProcessor in customProcessors)
                {
                    processors.Add((customProcessor.TargetType(), new CustomProcessorWrapper(customProcessor, projType, logger)));
                }
            }

            return processors;
        }

        public static List<ICustomAnalyzer> GetCustomProcessors(string projectPath)
        {
            try
            {
                // Start searching one directory higher to allow for multi-project solutions.
                var pathToSearch = Path.GetDirectoryName(projectPath);

                // Only load custom analyzers when VS has finished starting up.
                // We may get here before the package is loaded if a XAML doc is opened with the solution.
                if (RapidXamlAnalysisPackage.IsLoaded)
                {
                    if (RapidXamlAnalysisPackage.Options.EnableCustomAnalysis)
                    {
                        return GetCustomAnalyzers(pathToSearch);
                    }
                }
            }
            catch (Exception exc)
            {
                SharedRapidXamlPackage.Logger?.RecordError(StringRes.Error_FailedToImportCustomAnalyzers);
                SharedRapidXamlPackage.Logger?.RecordException(exc);
            }

            // If package not loaded, setting not enabled, or error.
            return new List<ICustomAnalyzer>();
        }

        // TODO: ISSUE#331 cache this response so don't need to look up again if files haven't changed.
        public static List<ICustomAnalyzer> GetCustomAnalyzers(string dllPath)
        {
            var result = new List<ICustomAnalyzer>();

            bool FileFilter(string fileName)
            {
                var filterResult = !fileName.Contains("/obj/")
                                && !fileName.Contains("\\obj\\")
                                && !fileName.Contains(".resources")
                                && !fileName.Contains(".Tests")
                                && !Path.GetFileName(fileName).StartsWith("Microsoft.")
                                && !Path.GetFileName(fileName).StartsWith("System.")
                                && !Path.GetFileName(fileName).StartsWith("Xamarin.")
                                && !Path.GetFileName(fileName).Equals("clrcompression.dll")
                                && !Path.GetFileName(fileName).Equals("mscorlib.dll")
                                && !Path.GetFileName(fileName).Equals("ucrtbased.dll")
                                && !Path.GetFileName(fileName).Equals("netstandard.dll")
                                && !Path.GetFileName(fileName).Equals("WindowsBase.dll")
                                && !Path.GetFileName(fileName).Equals("RapidXaml.CustomAnalysis.dll");

#if DEBUG
                // Avoid trying to load self while debugging
                filterResult = filterResult
                            && !Path.GetFileName(fileName).Equals("RapidXaml.Analysis.dll");
#endif

                return filterResult;
            }

            // Keep track of what's been loaded so don't load duplicates.
            // Duplicates are likely if the custom analyzer project is in a parallel project in the same solution.
            var loadedAssemblies = new List<string>();

            // Skip anything (esp. comon files) that definitely won't contain custom analyzers
            foreach (var file in Directory.GetFiles(dllPath, "*.dll", SearchOption.AllDirectories)
                                          .Where(f => FileFilter(f)))
            {
                try
                {
                    // Only load assemblies that are in the same folder as the library containing the interface
                    // This library is distributed with custom analyzers so it's a goog indication of assemblies that def don't contain analyzers.
                    // This is also necessary for assembly resolution.
                    if (!File.Exists(Path.Combine(Path.GetDirectoryName(file), "RapidXaml.CustomAnalysis.dll")))
                    {
                        continue;
                    }

                    var fileName = Path.GetFileName(file);

                    if (loadedAssemblies.Contains(fileName))
                    {
                        continue;
                    }

                    // Make an in-memory copy of the file to avoid locking, or needing multiple AppDomains.
                    byte[] assemblyBytes = File.ReadAllBytes(file);
                    var asmbly = Assembly.Load(assemblyBytes);

                    var analyzerInterface = typeof(ICustomAnalyzer);

                    var customAnalyzers = asmbly.GetTypes()
                                                .Where(t => analyzerInterface.IsAssignableFrom(t)
                                                         && t.IsClass
                                                         && t.IsPublic);

                    foreach (var ca in customAnalyzers)
                    {
                        result.Add((ICustomAnalyzer)Activator.CreateInstance(ca));
                    }

                    loadedAssemblies.Add(fileName);
                }
                catch (ReflectionTypeLoadException ex)
                {
                    var sb = new System.Text.StringBuilder();

                    foreach (Exception exSub in ex.LoaderExceptions)
                    {
                        sb.AppendLine(exSub.Message);

                        if (exSub is FileNotFoundException exFileNotFound)
                        {
                            if (!string.IsNullOrEmpty(exFileNotFound.FusionLog))
                            {
                                sb.AppendLine("Fusion Log:");
                                sb.AppendLine(exFileNotFound.FusionLog);
                            }
                        }

                        sb.AppendLine();
                    }

                    SharedRapidXamlPackage.Logger?.RecordInfo(StringRes.Error_FailedToLoadAssemblyMEF.WithParams(file));
                    SharedRapidXamlPackage.Logger?.RecordInfo(ex.ToString());
                    SharedRapidXamlPackage.Logger?.RecordInfo(ex.Source);
                    SharedRapidXamlPackage.Logger?.RecordInfo(ex.Message);
                    SharedRapidXamlPackage.Logger?.RecordInfo(ex.StackTrace);
                    SharedRapidXamlPackage.Logger?.RecordInfo(sb.ToString());
                }
                catch (Exception exc)
                {
                    // As these may happen a lot (i.e. if trying to load a file but can't) treat as info only.
                    SharedRapidXamlPackage.Logger?.RecordInfo(StringRes.Error_FailedToLoadAssemblyMEF.WithParams(file));
                    SharedRapidXamlPackage.Logger?.RecordInfo(exc.ToString());
                    SharedRapidXamlPackage.Logger?.RecordInfo(exc.Source);
                    SharedRapidXamlPackage.Logger?.RecordInfo(exc.Message);
                    SharedRapidXamlPackage.Logger?.RecordInfo(exc.StackTrace);
                }
            }

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
                SharedRapidXamlPackage.Logger?.RecordError(StringRes.Error_FailedToLoadSuppressionsAnalysisFile);
                SharedRapidXamlPackage.Logger?.RecordException(exc);
            }

            return result;
        }
    }
}
