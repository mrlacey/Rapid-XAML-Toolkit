// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Shell.TableManager;

namespace RapidXamlToolkit.ErrorList
{
    public class SinkManager : IDisposable
    {
        private readonly ITableDataSink _sink;
        private TableDataSource _errorList;
        private List<TableEntriesSnapshot> _snapshots = new List<TableEntriesSnapshot>();

        internal SinkManager(TableDataSource errorList, ITableDataSink sink)
        {
            this._sink = sink;
            this._errorList = errorList;

            errorList.AddSinkManager(this);
        }

        internal void Clear()
        {
            this._sink.RemoveAllSnapshots();
        }

        internal void UpdateSink(IEnumerable<TableEntriesSnapshot> snapshots)
        {
            foreach (var snapshot in snapshots)
            {
                var existing = this._snapshots.FirstOrDefault(s => s.FilePath == snapshot.FilePath);

                if (existing != null)
                {
                    this._snapshots.Remove(existing);
                    this._sink.ReplaceSnapshot(existing, snapshot);
                }
                else
                {
                    this._sink.AddSnapshot(snapshot);
                }

                this._snapshots.Add(snapshot);
            }
        }

        internal void RemoveSnapshots(IEnumerable<string> urls)
        {
            foreach (string url in urls)
            {
                var existing = this._snapshots.FirstOrDefault(s => s.FilePath == url);

                if (existing != null)
                {
                    this._snapshots.Remove(existing);
                    this._sink.RemoveSnapshot(existing);
                }
            }
        }

        public void Dispose()
        {
            // Called when the person who subscribed to the data source disposes of the cookie (== this object) they were given.
            this._errorList.RemoveSinkManager(this);
        }
    }
}
