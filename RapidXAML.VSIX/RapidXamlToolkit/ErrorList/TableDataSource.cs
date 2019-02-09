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
        private static TableDataSource _instance;
        private readonly List<SinkManager> _managers = new List<SinkManager>();
        private static Dictionary<string, TableEntriesSnapshot> _snapshots = new Dictionary<string, TableEntriesSnapshot>();

        [Import]
        private ITableManagerProvider TableManagerProvider { get; set; } = null;

        private TableDataSource()
        {
            var compositionService = ServiceProvider.GlobalProvider.GetService(typeof(SComponentModel)) as IComponentModel;
            compositionService.DefaultCompositionService.SatisfyImportsOnce(this);

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
                if (_instance == null)
                {
                    _instance = new TableDataSource();
                }

                return _instance;
            }
        }

        public bool HasErrors
        {
            get { return _snapshots.Any(); }
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

        public IDisposable Subscribe(ITableDataSink sink)
        {
            return new SinkManager(this, sink);
        }

        public void AddSinkManager(SinkManager manager)
        {
            // This call can, in theory, happen from any thread so be appropriately thread safe.
            // In practice, it will probably be called only once from the UI thread (by the error list tool window).
            lock (_managers)
            {
                _managers.Add(manager);
            }
        }

        public void RemoveSinkManager(SinkManager manager)
        {
            // This call can, in theory, happen from any thread so be appropriately thread safe.
            // In practice, it will probably be called only once from the UI thread (by the error list tool window).
            lock (_managers)
            {
                _managers.Remove(manager);
            }
        }

        public void UpdateAllSinks()
        {
            lock (_managers)
            {
                foreach (var manager in _managers)
                {
                    manager.UpdateSink(_snapshots.Values);
                }
            }
        }

        public void AddErrors(FileErrorCollection result)
        {
            if (result == null || !result.Errors.Any())
            {
                return;
            }

            result.Errors = result.Errors.Where(v => !_snapshots.Any(s => s.Value.Errors.Contains(v))).ToList();

            var snapshot = new TableEntriesSnapshot(result);
            _snapshots[result.FilePath] = snapshot;

            this.UpdateAllSinks();
        }

        public void CleanErrors(params string[] urls)
        {
            foreach (string url in urls)
            {
                if (_snapshots.ContainsKey(url))
                {
                    _snapshots[url].Dispose();
                    _snapshots.Remove(url);
                }
            }

            lock (_managers)
            {
                foreach (var manager in _managers)
                {
                    manager.RemoveSnapshots(urls);
                }
            }

            this.UpdateAllSinks();
        }

        public void CleanAllErrors()
        {
            foreach (string url in _snapshots.Keys)
            {
                var snapshot = _snapshots[url];
                if (snapshot != null)
                {
                    snapshot.Dispose();
                }
            }

            _snapshots.Clear();

            lock (_managers)
            {
                foreach (var manager in _managers)
                {
                    manager.Clear();
                }
            }

            this.UpdateAllSinks();
        }
    }
}
