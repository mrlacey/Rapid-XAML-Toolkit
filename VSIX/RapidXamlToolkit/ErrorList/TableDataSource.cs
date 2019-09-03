// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.TableControl;
using Microsoft.VisualStudio.Shell.TableManager;
using RapidXamlToolkit.Resources;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.ErrorList
{
    public class TableDataSource : ITableDataSource
    {
        private static readonly Dictionary<string, TableEntriesSnapshot> Snapshots = new Dictionary<string, TableEntriesSnapshot>();
        private static TableDataSource instance;
        private readonly List<SinkManager> managers = new List<SinkManager>();

        private TableDataSource()
        {
            var compositionService = ServiceProvider.GlobalProvider.GetService(typeof(SComponentModel)) as IComponentModel;
            compositionService?.DefaultCompositionService.SatisfyImportsOnce(this);

            var manager = this.TableManagerProvider.GetTableManager(StandardTables.ErrorsTable);
            manager.AddSource(
                              this,
                              StandardTableColumnDefinitions.DetailsExpander,
                              StandardTableColumnDefinitions.BuildTool,
                              StandardTableColumnDefinitions.ErrorSeverity,
                              StandardTableColumnDefinitions.ErrorCode,
                              StandardTableColumnDefinitions.ErrorSource,
                              StandardTableColumnDefinitions.ErrorCategory,
                              StandardTableColumnDefinitions.Text,
                              StandardTableColumnDefinitions.DocumentName,
                              StandardTableColumnDefinitions.Line,
                              StandardTableColumnDefinitions.Column);
        }

        public static TableDataSource Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new TableDataSource();
                }

                return instance;
            }
        }

        public bool HasErrors => Snapshots.Any();

        public string SourceTypeIdentifier => StandardTableDataSources.ErrorTableDataSource;

        public string Identifier => RapidXamlPackage.PackageGuidString;

        public string DisplayName => StringRes.VSIX__LocalizedName;

        [Import]
        private ITableManagerProvider TableManagerProvider { get; set; } = null;

        public IDisposable Subscribe(ITableDataSink sink)
        {
            return new SinkManager(this, sink);
        }

        public void AddSinkManager(SinkManager manager)
        {
            // This call can, in theory, happen from any thread so be appropriately thread safe.
            // In practice, it will probably be called only once from the UI thread (by the error list tool window).
            lock (this.managers)
            {
                this.managers.Add(manager);
            }
        }

        public void RemoveSinkManager(SinkManager manager)
        {
            // This call can, in theory, happen from any thread so be appropriately thread safe.
            // In practice, it will probably be called only once from the UI thread (by the error list tool window).
            lock (this.managers)
            {
                this.managers.Remove(manager);
            }
        }

        public void UpdateAllSinks()
        {
            lock (this.managers)
            {
                foreach (var manager in this.managers)
                {
                    manager.UpdateSink(Snapshots.Values);
                }
            }
        }

        public void AddErrors(FileErrorCollection result)
        {
            if (result == null || result.Errors.All(e => e.ErrorType == TagErrorType.Hidden))
            {
                return;
            }

            result.Errors = result.Errors.Where(v => !Snapshots.Any(s => s.Value.Errors.Contains(v)) && v.ErrorType != TagErrorType.Hidden).ToList();

            var snapshot = new TableEntriesSnapshot(result);
            Snapshots[result.FilePath] = snapshot;

            this.UpdateAllSinks();
        }

        public void CleanErrors(params string[] urls)
        {
            foreach (string url in urls)
            {
                if (url != null && Snapshots.ContainsKey(url))
                {
                    Snapshots[url].Dispose();
                    Snapshots.Remove(url);
                }
            }

            lock (this.managers)
            {
                foreach (var manager in this.managers)
                {
                    manager.RemoveSnapshots(urls);
                }
            }

            this.UpdateAllSinks();
        }

        public void CleanAllErrors()
        {
            foreach (string url in Snapshots.Keys)
            {
                var snapshot = Snapshots[url];
                snapshot?.Dispose();
            }

            Snapshots.Clear();

            lock (this.managers)
            {
                foreach (var manager in this.managers)
                {
                    manager.Clear();
                }
            }

            this.UpdateAllSinks();
        }
    }
}
