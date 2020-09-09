// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;

using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Editor.DragDrop;
using Microsoft.VisualStudio.Text.Operations;

using Newtonsoft.Json;

using RapidXamlToolkit.Commands;
using RapidXamlToolkit.Logging;
using RapidXamlToolkit.Resources;
using RapidXamlToolkit.VisualStudioIntegration;

namespace RapidXamlToolkit.DragDrop
{
    internal class RapidXamlDropHandler : IDropHandler
    {
        private readonly ILogger logger;
        private readonly IWpfTextView view;
        private readonly ITextBufferUndoManager undoManager;
        private readonly IFileSystemAbstraction fileSystem;
        private readonly IVisualStudioAbstraction vs;
        private readonly ProjectType projectType;
        private readonly IVsSolution solution;
        private string draggedFilename;

        public RapidXamlDropHandler(ILogger logger, IWpfTextView view, ITextBufferUndoManager undoManager, IVisualStudioAbstraction vs, ProjectType projectType, IVsSolution solution, IFileSystemAbstraction fileSystem = null)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.view = view;
            this.undoManager = undoManager;
            this.vs = vs ?? throw new ArgumentNullException(nameof(vs));
            this.projectType = projectType;
            this.solution = solution;
            this.fileSystem = fileSystem ?? new WindowsFileSystem();
        }

        public DragDropPointerEffects HandleDataDropped(DragDropInfo dragDropInfo)
        {
            var position = dragDropInfo.VirtualBufferPosition.Position;

            // Get left padding (allowing for drop postion to not be on the start/end of a line)
            var insertLineStart = this.view.GetTextViewLineContainingBufferPosition(position).Start.Position;

            var insertLinePadding = position.Position - insertLineStart;

            ThreadHelper.JoinableTaskFactory.Run(async () =>
            {
                try
                {
                    this.logger?.RecordFeatureUsage(nameof(RapidXamlDropHandler));
                    this.logger?.RecordInfo(StringRes.Info_FileDropped.WithParams(this.draggedFilename));

                    var logic = new DropHandlerLogic(this.logger, this.vs, this.fileSystem);

                    var textOutput = await logic.ExecuteAsync(this.draggedFilename, insertLinePadding, this.projectType);

                    if (!string.IsNullOrEmpty(textOutput))
                    {
                        this.view.TextBuffer.Insert(position.Position, textOutput);
                    }
                }
                catch (Exception exc)
                {
                    this.logger?.RecordException(exc);
                }
            });

            return DragDropPointerEffects.Copy;
        }

        public void HandleDragCanceled()
        {
        }

        public DragDropPointerEffects HandleDragStarted(DragDropInfo dragDropInfo)
        {
            return DragDropPointerEffects.All;
        }

        public DragDropPointerEffects HandleDraggingOver(DragDropInfo dragDropInfo)
        {
            return DragDropPointerEffects.All;
        }

        public bool IsDropEnabled(DragDropInfo dragDropInfo)
        {
            var fileName = GetFilename(dragDropInfo);

            this.draggedFilename = fileName;

            return File.Exists(fileName) && (fileName.EndsWith(".cs") || fileName.EndsWith(".vb"));
        }

        private string GetFilename(DragDropInfo info)
        {
            MemoryStream stream = info.Data.GetData("CF_VSSTGPROJECTITEMS") as MemoryStream;
            if (stream != null)
            {
                string reference = EnumerateDroppedFiles(stream).FirstOrDefault();
                if (ErrorHandler.Succeeded(this.solution.GetItemOfProjref(reference, out IVsHierarchy hierarchy, out uint itemId, out _, new VSUPDATEPROJREFREASON[1])))
                {
                    if (hierarchy is IVsProject project && ErrorHandler.Succeeded(project.GetMkDocument(itemId, out string fileName)))
                    {
                        return fileName;
                    }
                }
            }

            return null;
        }

        private static IEnumerable<string> EnumerateDroppedFiles(MemoryStream stream)
        {
            const uint MAX_PATH = 260; // windef.h
            char[] buffer = new char[MAX_PATH];

            var handle = GCHandle.Alloc(stream.ToArray(), GCHandleType.Pinned);

            try
            {
                uint length = DragQueryFile(handle.AddrOfPinnedObject(), 0xFFFFFFFF, null, 0);

                for (uint i = 0; i < length; i++)
                {
                    uint charCount = DragQueryFile(handle.AddrOfPinnedObject(), i, buffer, (uint)buffer.Length);
                    yield return new string(buffer, 0, (int)charCount);
                }
            }
            finally
            {
                handle.Free();
            }
        }

        [DllImport("shell32.dll", EntryPoint = "DragQueryFileW", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        private static extern uint DragQueryFile(IntPtr hDrop, uint iFile, char[] lpszFile, uint cch);
    }
}
