// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.TextManager.Interop;
using RapidXamlToolkit.ErrorList;
using RapidXamlToolkit.Logging;
using RapidXamlToolkit.Resources;
using RapidXamlToolkit.Telemetry;
using RapidXamlToolkit.VisualStudioIntegration;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.XamlAnalysis
{
    public static class RapidXamlDocumentCache
    {
        private static readonly object CacheLock = new object();
        private static readonly Dictionary<string, RapidXamlDocument> Cache = new Dictionary<string, RapidXamlDocument>();
        private static readonly List<string> CurrentlyProcessing = new List<string>();
        private static AsyncPackage package;
        private static IVisualStudioAbstraction vsa;

        private static TimeSpan lastAnalysisTime = TimeSpan.Zero;
        private static bool havePromptedForSaveAnalysisPerformance = false;

        public static event EventHandler<RapidXamlParsingEventArgs> Parsed;

        public static void Initialize(AsyncPackage rxPackage, ILogger logger)
        {
            package = rxPackage;
            vsa = new VisualStudioAbstraction(logger, rxPackage, ProjectHelpers.Dte);
        }

        public static void Add(string file, ITextSnapshot snapshot)
        {
            var fileInCache = false;

            lock (CacheLock)
            {
                fileInCache = Cache.ContainsKey(file);

                if (fileInCache)
                {
                    Update(file, snapshot);
                }
                else
                {
                    // Don't worry about timing this call as it's only repeated calls to analyze a document that might cause a user prompt.
                    // This only happens on document open. Repeated analysis of a document will happen through TryUpdate.
                    var doc = RapidXamlDocument.Create(new VsTextSnapshot(snapshot), file, vsa, string.Empty, SharedRapidXamlPackage.Logger);

                    Cache.Add(file, doc);

                    Parsed?.Invoke(null, new RapidXamlParsingEventArgs(doc, file, snapshot, ParsedAction.Add));
                }
            }
        }

        // For when don't have access to an ITextSnapshot but the document is open.
        public static void TryUpdate(string file)
        {
            if (package == null)
            {
                return;
            }

            bool DocIsOpenInLogicalView(string path, Guid logicalView, out IVsWindowFrame windowFrame)
            {
                if (package != null)
                {
                    return VsShellUtilities.IsDocumentOpen(
                        package,
                        path,
                        logicalView,
                        out var _,  // dummyHierarchy2
                        out var _,  // dummyItemId2
                        out windowFrame);
                }
                else
                {
                    windowFrame = null;
                    return false;
                }
            }

            var docIsOpenInTextView =
                DocIsOpenInLogicalView(file, VSConstants.LOGVIEWID_TextView, out var windowFrameForTextView) ||
                DocIsOpenInLogicalView(file, VSConstants.LOGVIEWID_Code, out windowFrameForTextView);

            if (docIsOpenInTextView && windowFrameForTextView != null)
            {
                var view = VsShellUtilities.GetTextView(windowFrameForTextView);

                if (view != null)
                {
                    view.GetBuffer(out var curDocTextLines);

                    var componentModel = (IComponentModel)Package.GetGlobalService(typeof(SComponentModel));

                    var adaptersFactory = componentModel.GetService<IVsEditorAdaptersFactoryService>();

                    var docBuffer = adaptersFactory.GetDocumentBuffer(curDocTextLines as IVsTextBuffer);

                    RapidXamlDocumentCache.Update(file, docBuffer.CurrentSnapshot);
                }
            }
        }

        public static void RemoveTags(string file, string errorCode)
        {
            lock (CacheLock)
            {
                for (int i = Cache[file].Tags.Count - 1; i >= 0; i--)
                {
                    if (Cache[file].Tags[i] is RapidXamlDisplayedTag rxdt && rxdt.ErrorCode == errorCode)
                    {
                        Cache[file].Tags.RemoveAt(i);
                    }
                }
            }
        }

        public static void Invalidate(string file)
        {
            lock (CacheLock)
            {
                if (Cache.ContainsKey(file))
                {
                    Cache[file].Clear();
                }
            }

            TableDataSource.Instance.CleanErrors(file);
        }

        public static void Update(string file, ITextSnapshot snapshot)
        {
            var snapshotText = snapshot.GetText();

            bool alreadyCached = false;

            lock (CacheLock)
            {
                alreadyCached = Cache.ContainsKey(file) && Cache[file].RawText == snapshotText;
            }

            if (!alreadyCached)
            {
                if (!CurrentlyProcessing.Contains(snapshotText))
                {
                    try
                    {
                        CurrentlyProcessing.Add(snapshotText);

                        RapidXamlDocument doc;

                        var sw = new Stopwatch();
                        try
                        {
                            sw.Start();

                            doc = RapidXamlDocument.Create(new VsTextSnapshot(snapshot), file, vsa, string.Empty, SharedRapidXamlPackage.Logger);
                        }
                        finally
                        {
                            sw.Stop();

                            var elapsed = sw.Elapsed;
                            var threshold = TimeSpan.FromSeconds(1.5);

                            Debug.WriteLine($"Document anlaysis took:  {elapsed}");

                            var analyzeOnSave = true;
#if VSIXNOTEXE
                            analyzeOnSave = RapidXamlPackage.AnalysisOptions?.AnalyzeWhenDocumentSaved ?? false;
#endif

                            // Don't prompt about a single execution time greater than the threshold
                            if (elapsed > threshold
                             && lastAnalysisTime > threshold
                             && analyzeOnSave == true
                             && !havePromptedForSaveAnalysisPerformance)
                            {
                                SharedRapidXamlPackage.Logger?.RecordFeatureUsage(MiscellaneousFeatures.PromptToDisableAnalysisOnSave, quiet: true);
                                RxtOutputPane.Instance.Write(StringRes.Info_PromptToDisableAnalysisOnSave);
                                RxtOutputPane.Instance.Activate(); // To increase the likelihood that it's seen

#pragma warning disable VSTHRD104 // Offer async methods - as can't make the full stack to here async
                                ThreadHelper.JoinableTaskFactory.Run(async () =>
#pragma warning restore VSTHRD104 // Offer async methods
                                {
                                    var infoBar = new AnalysisSpeedWarningInfoBar();

                                    await infoBar.ShowInfoBarAsync();
                                });

                                havePromptedForSaveAnalysisPerformance = true;
                            }

                            lastAnalysisTime = elapsed;
                        }

                        lock (CacheLock)
                        {
                            Cache[file] = doc;
                        }

                        Parsed?.Invoke(null, new RapidXamlParsingEventArgs(doc, file, snapshot, ParsedAction.Update));
                    }
                    finally
                    {
                        CurrentlyProcessing.Remove(snapshotText);
                    }
                }
            }
        }

        public static List<IRapidXamlAdornmentTag> AdornmentTags(string fileName)
        {
            var result = new List<IRapidXamlAdornmentTag>();

            lock (CacheLock)
            {
                if (Cache.ContainsKey(fileName))
                {
                    try
                    {
                        result.AddRange(
                            Cache[fileName].Tags.Where(
                                t => (t as RapidXamlDisplayedTag == null)
                                  || (t as RapidXamlDisplayedTag).ConfiguredErrorType != TagErrorType.Hidden));
                    }
                    catch (Exception exc)
                    {
                        System.Diagnostics.Debug.WriteLine(exc);
                    }
                }
            }

            return result;
        }

        public static List<RapidXamlDisplayedTag> ErrorListTags(string fileName)
        {
            var result = new List<RapidXamlDisplayedTag>();

            lock (CacheLock)
            {
                if (Cache.ContainsKey(fileName))
                {
                    result.AddRange(Cache[fileName].Tags.OfType<RapidXamlDisplayedTag>());
                }
            }

            return result;
        }
    }
}
