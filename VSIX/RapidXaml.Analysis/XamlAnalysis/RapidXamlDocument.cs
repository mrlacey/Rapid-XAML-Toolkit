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
using RapidXamlToolkit.Telemetry;
using RapidXamlToolkit.VisualStudioIntegration;
using RapidXamlToolkit.XamlAnalysis.Processors;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.XamlAnalysis
{
    public class RapidXamlDocument
    {
        private const string AnyOfStart = "ANYOF:";

        private static readonly Dictionary<string, (DateTime timestamp, List<ICustomAnalyzer> analyzer)> AnalyzerCache = new Dictionary<string, (DateTime timestamp, List<ICustomAnalyzer> analyzer)>();

        public RapidXamlDocument()
        {
            this.Tags = new TagList();
        }

        public string RawText { get; set; }

        public TagList Tags { get; set; }

        public IVisualStudioAbstraction VsAbstraction { get; set; }

        private static Dictionary<string, (DateTime timeStamp, List<TagSuppression> suppressions)> SuppressionsCache { get; }
            = new Dictionary<string, (DateTime, List<TagSuppression>)>();

        public static RapidXamlDocument Create(ITextSnapshot snapshot, string fileName, IVisualStudioAbstraction vsa, string projectFile)
        {
            var result = new RapidXamlDocument();

            List<(string, XamlElementProcessor)> processors = null;

            var vsAbstraction = vsa;

            // This will happen if open a project with open XAML files before the package is initialized.
            if (vsAbstraction == null)
            {
                vsAbstraction = new VisualStudioAbstraction(new RxtLogger(), null, ProjectHelpers.Dte);
            }

            try
            {
                var text = snapshot.GetText();

                if (text.IsValidXml())
                {
                    result.RawText = text;

                    var suppressions = GetSuppressions(fileName, vsAbstraction, projectFile);

                    // If suppressing all tags in file, don't bother parsing the file
                    if (suppressions == null || suppressions?.Any(s => string.IsNullOrWhiteSpace(s.TagErrorCode)) == false)
                    {
                        var (projFileName, projType) = vsAbstraction.GetNameAndTypeOfProjectContainingFile(fileName);

                        processors = GetAllProcessors(projType, projFileName, vsAbstraction);

                        // May need to tidy-up-release processors after this - depending on caching. X-Ref http://www.visualstudioextensibility.com/2013/03/17/the-strange-case-of-quot-loaderlock-was-detected-quot-with-a-com-add-in-written-in-net/
                        XamlElementExtractor.Parse(fileName, snapshot, text, processors, result.Tags, suppressions, GetEveryElementProcessor(projType, projFileName, vsAbstraction), SharedRapidXamlPackage.Logger);

                        var tagsFound = result.Tags.OfType<RapidXamlDisplayedTag>().Count();

                        SharedRapidXamlPackage.Logger?.RecordFeatureUsage(StringRes.Info_UsedFeatureParseDocument.WithParams(tagsFound), quiet: true);
                    }
                }
            }
            catch (Exception e)
            {
                var tagDeps = new TagDependencies
                {
                    Span = new Span(0, 0),
                    Snapshot = snapshot,
                    FileName = fileName,
                    Logger = SharedRapidXamlPackage.Logger,
                    VsPfp = vsAbstraction,
                    ProjectFilePath = string.Empty,
                };

                result.Tags.Add(new UnexpectedErrorTag(tagDeps)
                {
                    Description = StringRes.Error_XamlAnalysisDescription.WithParams(CoreDetails.GetVersion()),
                    ExtendedMessage = StringRes.Error_XamlAnalysisExtendedMessage.WithParams(e),
                });

                SharedRapidXamlPackage.Logger?.RecordException(e);
            }

            return result;
        }

        public static XamlElementProcessor GetEveryElementProcessor(ProjectType projectType, string projectFilePath, IVisualStudioAbstraction vsAbstraction)
        {
            return new EveryElementProcessor(new ProcessorEssentials(projectType, SharedRapidXamlPackage.Logger, projectFilePath, vsAbstraction));
        }

        public static List<(string, XamlElementProcessor)> GetAllProcessors(ProjectType projType, string projectFilePath, IVisualStudioAbstraction vsAbstraction, ILogger logger = null)
        {
            logger = logger ?? SharedRapidXamlPackage.Logger;

            var processorEssentials = new ProcessorEssentials
            {
                ProjectType = projType,
                Logger = logger,
                ProjectFilePath = projectFilePath,
            };

            var processors = new List<(string, XamlElementProcessor)>
                    {
                        (Elements.Grid, new GridProcessor(processorEssentials)),
                        (Elements.TextBlock, new TextBlockProcessor(processorEssentials)),
                        (Elements.TextBox, new TextBoxProcessor(processorEssentials)),
                        (Elements.AppBarButton, new AppBarButtonProcessor(processorEssentials)),
                        (Elements.AppBarToggleButton, new AppBarToggleButtonProcessor(processorEssentials)),
                        (Elements.AutoSuggestBox, new AutoSuggestBoxProcessor(processorEssentials)),
                        (Elements.CalendarDatePicker, new CalendarDatePickerProcessor(processorEssentials)),
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
                        (Elements.PasswordBox, new PasswordBoxProcessor(processorEssentials)),
                        (Elements.MediaElement, new MediaElementProcessor(processorEssentials)),
                        (Elements.ListView, new SelectedItemAttributeProcessor(processorEssentials)),
                        (Elements.DataGrid, new SelectedItemAttributeProcessor(processorEssentials)),
                        ////(Elements.Label, new LabelProcessor(processorEssentials)),
                    };

            if (!string.IsNullOrWhiteSpace(projectFilePath))
            {
                var customProcessors = GetCustomProcessors(Path.GetDirectoryName(projectFilePath));

#if DEBUG
                // These types exists for testing only and so are only referenced during Debug
                ////customProcessors.Add(new CustomAnalysis.FooAnalysis());
                ////customProcessors.Add(new CustomAnalysis.BadCustomAnalyzer());
                ////customProcessors.Add(new CustomAnalysis.InternalBadCustomAnalyzer(vsAbstraction));
                ////customProcessors.Add(new CustomAnalysis.CustomGridDefinitionAnalyzer());
                ////customProcessors.Add(new CustomAnalysis.RenameElementTestAnalyzer());
                ////customProcessors.Add(new CustomAnalysis.ReplaceElementTestAnalyzer());
                ////customProcessors.Add(new CustomAnalysis.AddChildTestAnalyzer());
                ////customProcessors.Add(new CustomAnalysis.RemoveFirstChildAnalyzer());
                ////customProcessors.Add(new CustomAnalysis.Issue364ExampleAnalyzer(vsAbstraction));
                ////customProcessors.Add(new CustomAnalysis.Issue364ExampleAnalyzer2(vsAbstraction));
               ////customProcessors.Add(new CustomAnalysis.AddXmlnsAnalyzer());
                customProcessors.Add(new CustomAnalysis.WebViewToWebView2Converter());
#endif
                customProcessors.Add(new CustomAnalysis.ButtonAnalyzer(vsAbstraction));
                customProcessors.Add(new CustomAnalysis.TwoPaneViewAnalyzer(vsAbstraction));
                customProcessors.Add(new CustomAnalysis.UnoIgnorablesAnalyzer(vsAbstraction));
                customProcessors.Add(new CustomAnalysis.LabelAnalyzer(vsAbstraction));
                customProcessors.Add(new CustomAnalysis.XfImageAnalyzer(vsAbstraction));
                customProcessors.Add(new CustomAnalysis.XfLineAnalyzer(vsAbstraction));
                customProcessors.Add(new CustomAnalysis.XfMapPinAnalyzer(vsAbstraction));
                customProcessors.Add(new CustomAnalysis.ImageButtonAnalyzer(vsAbstraction));
                customProcessors.Add(new CustomAnalysis.RadioButtonAnalyzer(vsAbstraction));
                customProcessors.Add(new CustomAnalysis.SearchBarAnalyzer(vsAbstraction));
                customProcessors.Add(new CustomAnalysis.EntryAnalyzer(vsAbstraction));
                customProcessors.Add(new CustomAnalysis.PickerAnalyzer(vsAbstraction));
                customProcessors.Add(new CustomAnalysis.BindingToXBindAnalyzer(vsAbstraction));
                customProcessors.Add(new CustomAnalysis.CheckBoxAnalyzer(vsAbstraction));
                customProcessors.Add(new CustomAnalysis.TableSectionAnalyzer(vsAbstraction));
                customProcessors.Add(new CustomAnalysis.EntryCellAnalyzer(vsAbstraction));
                customProcessors.Add(new CustomAnalysis.SwitchCellAnalyzer(vsAbstraction));
                customProcessors.Add(new CustomAnalysis.SwipeItemAnalyzer(vsAbstraction));
                customProcessors.Add(new CustomAnalysis.SliderAnalyzer(vsAbstraction));
                customProcessors.Add(new CustomAnalysis.StepperAnalyzer(vsAbstraction));

                for (int i = 0; i < customProcessors.Count; i++)
                {
                    ICustomAnalyzer customProcessor = customProcessors[i];
                    processors.Add(
                        (customProcessor.TargetType(),
                         new CustomProcessorWrapper(customProcessor, projType, projectFilePath, logger, vsAbstraction)));
                }
            }

            return processors;
        }

        public static IEnumerable<(string, XamlElementProcessor)> WrapCustomProcessors(List<ICustomAnalyzer> customProcessors, ProjectType projType, string projectFilePath, ILogger logger, IVisualStudioAbstraction vsAbstraction)
        {
            foreach (var customProcessor in customProcessors)
            {
                var targetType = customProcessor.TargetType();

                var wrapper = new CustomProcessorWrapper(customProcessor, projType, projectFilePath, logger, vsAbstraction);

                if (targetType.StartsWith(AnyOfStart, StringComparison.InvariantCultureIgnoreCase))
                {
                    var types = targetType.Substring(6).Split(new[] { ' ', ',', ':', ';' }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (var type in types)
                    {
                        yield return (type, wrapper);
                    }
                }
                else
                {
                    yield return (targetType, wrapper);
                }
            }

            yield break;
        }

        public static List<ICustomAnalyzer> GetCustomProcessors(string projectFileDirectory)
        {
            try
            {
                // Start searching one directory higher to allow for multi-project solutions.
                var dirToSearch = Path.GetDirectoryName(projectFileDirectory);

                var loadCustomAnalyzers = false;

#if VSIXNOTEXE
                // Only load custom analyzers when VS has finished starting up.
                // We may get here before the package is loaded if a XAML doc is opened with the solution.
                if (RapidXamlAnalysisPackage.IsLoaded)
                {
                    if (RapidXamlAnalysisPackage.Options.EnableCustomAnalysis)
                    {
                        loadCustomAnalyzers = true;
                    }
                }
#endif

#if ANALYSISEXE
                loadCustomAnalyzers = true;
#endif

                if (loadCustomAnalyzers)
                {
                    return GetCustomAnalyzers(dirToSearch);
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

        public static List<ICustomAnalyzer> GetCustomAnalyzers(string folderToSearch)
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
                                && !Path.GetFileName(fileName).StartsWith("EnvDTE")
                                && !Path.GetFileName(fileName).StartsWith("VSLangProj")
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

            // Skip anything (esp. common files) that definitely won't contain custom analyzers
            foreach (var file in Directory.GetFiles(folderToSearch, "*.dll", SearchOption.AllDirectories)
                                          .Where(f => FileFilter(f)))
            {
                try
                {
                    // It is not always the case that VS/MSBuild will copy the RX.CA assembly :(
                    ////// Only load assemblies that are in the same folder as the library containing the interface
                    ////// This library is distributed with custom analyzers so it's a good indication of assemblies that def don't contain analyzers.
                    ////// This is also necessary for assembly resolution.
                    ////if (!File.Exists(Path.Combine(Path.GetDirectoryName(file), "RapidXaml.CustomAnalysis.dll")))
                    ////{
                    ////    continue;
                    ////}

                    var fileName = Path.GetFileName(file);

                    if (loadedAssemblies.Contains(fileName))
                    {
                        continue;
                    }

                    var fileTimestamp = File.GetCreationTimeUtc(file);

                    if (AnalyzerCache.ContainsKey(file))
                    {
                        var (cacheTimestamp, cachedAnalyzers) = AnalyzerCache[file];

                        if (cacheTimestamp == fileTimestamp)
                        {
                            if (cachedAnalyzers != null)
                            {
                                result.AddRange(cachedAnalyzers);
                            }

                            continue;
                        }
                        else
                        {
                            AnalyzerCache.Remove(file);
                        }
                    }

                    // Make an in-memory copy of the file to avoid locking, or needing multiple AppDomains.
                    byte[] assemblyBytes = File.ReadAllBytes(file);
                    var asmbly = Assembly.Load(assemblyBytes);

                    var analyzerInterface = typeof(ICustomAnalyzer);

                    var customAnalyzers = asmbly.GetTypes()
                                                .Where(t => analyzerInterface.IsAssignableFrom(t)
                                                         && t.IsClass
                                                         && t.IsPublic)
                                                .ToList();

                    if (customAnalyzers.Any())
                    {
                        var cacheList = new List<ICustomAnalyzer>();

                        foreach (var ca in customAnalyzers)
                        {
                            var ica = (ICustomAnalyzer)Activator.CreateInstance(ca);
                            cacheList.Add(ica);
                            result.Add(ica);
                        }

                        AnalyzerCache.Add(file, (fileTimestamp, cacheList));
                    }
                    else
                    {
                        AnalyzerCache.Add(file, (fileTimestamp, null));
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

        private static List<TagSuppression> GetSuppressions(string fileName, IVisualStudioAbstraction vsa, string projectFileName)
        {
            List<TagSuppression> result = null;

            try
            {
                if (string.IsNullOrWhiteSpace(projectFileName))
                {
                    projectFileName = vsa.GetPathOfProjectContainingFile(fileName);
                }

                if (!string.IsNullOrWhiteSpace(projectFileName) && File.Exists(projectFileName))
                {
                    var suppressionsFile = Path.Combine(Path.GetDirectoryName(projectFileName), "suppressions.xamlAnalysis");

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
