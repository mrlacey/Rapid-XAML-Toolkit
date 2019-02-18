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

namespace RapidXamlToolkit.ErrorList
{
    public class TableDataSource : ITableDataSource
    {
        private static TableDataSource instance;
        private static Dictionary<string, TableEntriesSnapshot> snapshots = new Dictionary<string, TableEntriesSnapshot>();
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

        public bool HasErrors
        {
            get { return snapshots.Any(); }
        }

        public string SourceTypeIdentifier
        {
            get { return StandardTableDataSources.ErrorTableDataSource; }
        }

        public string Identifier
        {
            get { return RapidXamlPackage.PackageGuidString; }
        }

        public string DisplayName
        {
            get { return "Rapid XAML Toolkit"; }
        }

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
                    manager.UpdateSink(snapshots.Values);
                }
            }
        }

        public void AddErrors(FileErrorCollection result)
        {
            if (result == null || !result.Errors.Any())
            {
                return;
            }

            result.Errors = result.Errors.Where(v => !snapshots.Any(s => s.Value.Errors.Contains(v))).ToList();

            var snapshot = new TableEntriesSnapshot(result);
            snapshots[result.FilePath] = snapshot;

            this.UpdateAllSinks();
        }

        public void CleanErrors(params string[] urls)
        {
            foreach (string url in urls)
            {
                if (snapshots.ContainsKey(url))
                {
                    snapshots[url].Dispose();
                    snapshots.Remove(url);
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
            foreach (string url in snapshots.Keys)
            {
                var snapshot = snapshots[url];
                snapshot?.Dispose();
            }

            snapshots.Clear();

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
