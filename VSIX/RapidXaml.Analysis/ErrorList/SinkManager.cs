// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Shell.TableManager;

namespace RapidXamlToolkit.ErrorList
{
    public class SinkManager : IDisposable
    {
        private readonly ITableDataSink sink;
        private readonly TableDataSource errorList;
        private readonly List<TableEntriesSnapshot> snapshots = new List<TableEntriesSnapshot>();

        internal SinkManager(TableDataSource errorList, ITableDataSink sink)
        {
            this.sink = sink;
            this.errorList = errorList;

            errorList.AddSinkManager(this);
        }

        public void Dispose()
        {
            // Called when the person who subscribed to the data source disposes of the cookie (== this object) they were given.
            this.errorList.RemoveSinkManager(this);
        }

        internal void Clear()
        {
            this.sink.RemoveAllSnapshots();
        }

        internal void UpdateSink(IEnumerable<TableEntriesSnapshot> teSnapshots)
        {
            foreach (var snapshot in teSnapshots)
            {
                var existing = this.snapshots.FirstOrDefault(s => s.FilePath == snapshot.FilePath);

                if (existing != null)
                {
                    this.snapshots.Remove(existing);
                    this.sink.ReplaceSnapshot(existing, snapshot);
                }
                else
                {
                    this.sink.AddSnapshot(snapshot);
                }

                this.snapshots.Add(snapshot);
            }
        }

        internal void RemoveSnapshots(IEnumerable<string> urls)
        {
            foreach (string url in urls)
            {
                var existing = this.snapshots.FirstOrDefault(s => s.FilePath == url);

                if (existing != null)
                {
                    this.snapshots.Remove(existing);
                    this.sink.RemoveSnapshot(existing);
                }
            }
        }
    }
}
