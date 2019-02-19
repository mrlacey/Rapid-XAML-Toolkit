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
                bool DocIsOpenInLogicalView(string path, Guid logicalView, out IVsWindowFrame windowFrame)
                {
                    if (package != null)
                    {
                        return VsShellUtilities.IsDocumentOpen(
                            package,
                            path,
                            VSConstants.LOGVIEWID_TextView,
                            out var dummyHierarchy2,
                            out var dummyItemId2,
                            out windowFrame);
                    }
                    else
                    {
                        windowFrame = null;
                        return false;
                    }
                }

                var docIsOpenInTextView =
                    DocIsOpenInLogicalView(documentPath, VSConstants.LOGVIEWID_Code, out var windowFrameForTextView) ||
                    DocIsOpenInLogicalView(documentPath, VSConstants.LOGVIEWID_TextView, out windowFrameForTextView);

                if (docIsOpenInTextView && windowFrameForTextView != null)
                {
                    var view = VsShellUtilities.GetTextView(windowFrameForTextView);

                    if (view != null)
                    {
                        view.GetBuffer(out var curDocTextLines);

                        var componentModel = (IComponentModel)Package.GetGlobalService(typeof(SComponentModel));

                        var adaptersFactory = componentModel.GetService<IVsEditorAdaptersFactoryService>();

                        var docBuffer = adaptersFactory.GetDocumentBuffer(curDocTextLines as IVsTextBuffer);

                        RapidXamlDocumentCache.Update(documentPath, docBuffer.CurrentSnapshot);
                    }
                }
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
