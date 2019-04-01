// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.IO;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextManager.Interop;

namespace RapidXamlToolkit.XamlAnalysis
{
    internal class RapidXamlRunningDocTableEvents : IVsRunningDocTableEvents
    {
        private readonly RapidXamlPackage package;
        private readonly RunningDocumentTable runningDocumentTable;

        public RapidXamlRunningDocTableEvents(RapidXamlPackage package, RunningDocumentTable runningDocumentTable)
        {
            this.package = package;
            this.runningDocumentTable = runningDocumentTable;
        }

        public int OnAfterFirstDocumentLock(uint docCookie, uint dwRDTLockType, uint dwReadLocksRemaining, uint dwEditLocksRemaining)
        {
            return VSConstants.S_OK;
        }

        public int OnBeforeLastDocumentUnlock(uint docCookie, uint dwRDTLockType, uint dwReadLocksRemaining, uint dwEditLocksRemaining)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterSave(uint docCookie)
        {
            var documentInfo = this.runningDocumentTable.GetDocumentInfo(docCookie);

            var documentPath = documentInfo.Moniker;

            if (Path.GetExtension(documentPath) == ".xaml")
            {
                RapidXamlDocumentCache.TryUpdate(documentPath);
            }

            return VSConstants.S_OK;
        }

        public int OnAfterAttributeChange(uint docCookie, uint grfAttribs)
        {
            return VSConstants.S_OK;
        }

        public int OnBeforeDocumentWindowShow(uint docCookie, int fFirstShow, IVsWindowFrame pFrame)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterDocumentWindowHide(uint docCookie, IVsWindowFrame pFrame)
        {
            return VSConstants.S_OK;
        }
    }
}
