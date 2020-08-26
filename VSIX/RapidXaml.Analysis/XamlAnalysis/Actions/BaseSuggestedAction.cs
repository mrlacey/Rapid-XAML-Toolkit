// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Imaging.Interop;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

namespace RapidXamlToolkit.XamlAnalysis.Actions
{
    public abstract class BaseSuggestedAction : ISuggestedAction
    {
        protected BaseSuggestedAction(string file)
        {
            this.File = file;
        }

        public string DisplayText { get; protected set; }

        public virtual bool IsEnabled { get; protected set; } = true;

        public virtual bool HasActionSets
        {
            get { return false; }
        }

        public virtual bool HasPreview
        {
            get { return false; }
        }

        public string IconAutomationText
        {
            get { return null; }
        }

        public virtual ImageMoniker IconMoniker
        {
            get { return default; }
        }

        public string InputGestureText
        {
            get { return null; }
        }

        public string CustomFeatureUsageOverride { get; protected set; }

        public string File { get; }

        protected ITextView View { get; set; }

        public virtual void Dispose()
        {
            // nothing to dispose
        }

        public Task<IEnumerable<SuggestedActionSet>> GetActionSetsAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult<IEnumerable<SuggestedActionSet>>(null);
        }

        public virtual Task<object> GetPreviewAsync(CancellationToken cancellationToken)
        {
            return null;
        }

        public void Invoke(CancellationToken cancellationToken)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();

            if (!this.IsEnabled)
            {
                return;
            }

            var undoContext = ProjectHelpers.Dte2.UndoContext;

            try
            {
                undoContext.Open(this.DisplayText);
                this.Execute(cancellationToken);

                if (string.IsNullOrWhiteSpace(this.CustomFeatureUsageOverride))
                {
                    SharedRapidXamlPackage.Logger?.RecordFeatureUsage(this.GetType().Name);
                }
                else
                {
                    SharedRapidXamlPackage.Logger?.RecordFeatureUsage(this.CustomFeatureUsageOverride.Trim());
                }

                RapidXamlDocumentCache.TryUpdate(this.File);
            }
            catch (Exception exc)
            {
                SharedRapidXamlPackage.Logger?.RecordException(exc);
            }
            finally
            {
                if (undoContext.IsOpen)
                {
                    undoContext.Close();
                }
            }
        }

        public abstract void Execute(CancellationToken cancellationToken);

        public bool TryGetTelemetryId(out Guid telemetryId)
        {
            telemetryId = Guid.Empty;
            return false;
        }

        protected static IEnumerable<ITextSnapshotLine> GetSelectedLines(SnapshotSpan span, out SnapshotSpan wholeSpan)
        {
            var startLine = span.Start.GetContainingLine();
            var endLine = span.End.GetContainingLine();

            wholeSpan = new SnapshotSpan(startLine.Start, endLine.End);
            return span.Snapshot.Lines.Where(l => l.LineNumber >= startLine.LineNumber && l.LineNumber <= endLine.LineNumber);
        }

        // Call this after having made the change if need to force reevaluation of actions
        private void RaiseBufferChange()
        {
            // Adding and deleting a char in order to force taggers re-evaluation
            string text = " ";
            this.View?.TextBuffer.Insert(0, text);
            this.View?.TextBuffer.Delete(new Span(0, text.Length));
        }
    }
}
