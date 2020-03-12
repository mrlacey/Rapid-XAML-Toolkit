// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
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
        // Track these references so can dispose them all later.
        private static AppDomain domain;

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

            ////aggCatalog = new AggregateCatalog();
            ////assCatalogs = new List<AssemblyCatalog>();
            ////importer = new AnalyzerImporter();

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
            finally
            {
                //// processors?.Clear();

                //// var batch = new CompositionBatch();

                //// foreach (var customAnalyzer in importer.CustomAnalyzers)
                //// {
                ////     batch.AddPart(customAnalyzer.Value);
                ////     container.ReleaseExport<ICustomAnalyzer>(customAnalyzer);
                //// }

                //// container.Compose(batch);

                //// container.ReleaseExports<ICustomAnalyzer>(importer.CustomAnalyzers);

                ////// importer.CustomAnalyzers.Clear();

                //// ////importer.CustomAnalyzers = new ICustomAnalyzer[] { };

                //// ////for (int i = importer.CustomAnalyzers.Count(); i >= 0; i--)
                //// ////{
                //// ////    importer.CustomAnalyzers[i] = null;
                //// ////}

                //// importer = null;

                //// foreach (var ac in assCatalogs)
                //// {
                ////     ac.Dispose();
                //// }

                //// foreach (var cat in aggCatalog.Catalogs)
                //// {
                ////     cat.Dispose();
                //// }

                //// aggCatalog.Dispose();
                //// container.Dispose();

                //// AppDomain.Unload(domain);
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
            var assembly = Assembly.GetExecutingAssembly();
            var codeBase = assembly.Location;
            var codeBaseDirectory = Path.GetDirectoryName(codeBase);
            var setup = new AppDomainSetup
            {
                ApplicationName = "RapidXaml.Analysis",
                ApplicationBase = codeBaseDirectory,
                DynamicBase = codeBaseDirectory,
                CachePath = Path.Combine(projectPath, "rxt-cache"),
                ShadowCopyFiles = "true",
                ShadowCopyDirectories = Path.Combine(projectPath, "bin"),
                LoaderOptimization = LoaderOptimization.MultiDomain,
            };

            domain = AppDomain.CreateDomain("Host_AppDomain", AppDomain.CurrentDomain.Evidence, setup);

            List<ICustomAnalyzer> result = new List<ICustomAnalyzer>();

            try
            {
                var loader = (CustomAnalyzerDomainLoader)domain.CreateInstanceAndUnwrap(
                    typeof(CustomAnalyzerDomainLoader).Assembly.FullName,
                    typeof(CustomAnalyzerDomainLoader).FullName);

                // TODO: have this return individual items as List<T> isn't serialized
                //result = loader.GetCustomAnalyzers(setup.ShadowCopyDirectories);

                //   var oneAnalyzer = loader.GetOneAnalyzer(setup.ShadowCopyDirectories);


                //     oneAnalyzer = domain.GetData("one") as ICustomAnalyzer;

                //    result.Add(oneAnalyzer);

                //    System.Diagnostics.Debug.WriteLine(result.Count);
                //     object x = domain.GetData("one");


                //oneAnalyzer.Analyze(RapidXaml.RapidXamlElement.Build("TestElement"));

                //var list = new CustomAnalyzerList();

                //loader.LoadOneAnalyzer(setup.ShadowCopyDirectories, list);

                //result.Add(list.First);

                var hndl = loader.GetHandleForOneAnalyzer(setup.ShadowCopyDirectories);

                var item = hndl.Unwrap() as ICustomAnalyzer;

                result.Add(item);

                System.Diagnostics.Debug.WriteLine(result);

            }
            finally
            {
                AppDomain.Unload(domain);
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

    public class CustomAnalyzerList : MarshalByRefObject
    {
        public ICustomAnalyzer First { get; set; }
    }

    public class CustomAnalyzerDomainLoader : MarshalByRefObject
    {
        private AggregateCatalog aggCatalog = new AggregateCatalog();
        private List<AssemblyCatalog> assCatalogs = new List<AssemblyCatalog>();
        private CompositionContainer container;
        private AnalyzerImporter importer = new AnalyzerImporter();


        public void LoadOneAnalyzer(string dllPath, CustomAnalyzerList list)
        {
            var one = GetOneAnalyzer(dllPath);

            list.First = one;
        }

        public System.Runtime.Remoting.ObjectHandle GetHandleForOneAnalyzer(string dllPath)
        {
            var ca = GetCustomAnalyzers(dllPath).First();

            return new System.Runtime.Remoting.ObjectHandle(ca);
        }

        public ICustomAnalyzer GetOneAnalyzer(string dllPath)
        {
            var ca = GetCustomAnalyzers(dllPath).First();

            AppDomain.CurrentDomain.SetData("one", ca);

            return ca;
        }

        public List<ICustomAnalyzer> GetCustomAnalyzers(string dllPath)
        {
            var result = new List<ICustomAnalyzer>();

            // Add specific assemblies only.
            // Don't add directories as most will fail to load and access parts.
            // Don't include '*.exe.' files as can't load from generated apps.
            // While using MEF for loading, the analyzers need to be in a separate library.
            foreach (var file in Directory.GetFiles(dllPath, "*.*", SearchOption.AllDirectories)
                                          .Where(f => !Path.GetFileName(f).StartsWith("Microsoft.")
                                                   && !Path.GetFileName(f).StartsWith("System.")
                                                   && !Path.GetFileName(f).Equals("RapidXaml.CustomAnalysis.dll")
                                                   && f.EndsWith(".dll")))
            {
                try
                {
                    var ac = new AssemblyCatalog(Assembly.LoadFile(file));
                    var parts = ac.Parts.ToArray(); // This will also throw if would throw when calling ComposeParts

                    this.assCatalogs.Add(ac);
                    this.aggCatalog.Catalogs.Add(ac);
                }
                catch (Exception exc)
                {
                    // TODO: only want this in the RXT pane when extended logging is enabled
                    SharedRapidXamlPackage.Logger?.RecordError(StringRes.Error_FailedToLoadAssemblyMEF.WithParams(file));
                    SharedRapidXamlPackage.Logger?.RecordException(exc);
                }
            }

            try
            {
                this.container = new CompositionContainer(this.aggCatalog);

                this.container.ComposeParts(this.importer);

                foreach (var importedAnalyzer in this.importer.CustomAnalyzers)
                {
                    result.Add(importedAnalyzer);
                }
            }
            catch (Exception exc)
            {
                SharedRapidXamlPackage.Logger?.RecordError(StringRes.Error_FailedToImportCustomAnalyzers);
                SharedRapidXamlPackage.Logger?.RecordException(exc);
            }

            return result;
        }
    }
}
