// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using RapidXamlToolkit.ErrorList;
using RapidXamlToolkit.Resources;

namespace RapidXamlToolkit.XamlAnalysis
{
    public class RapidXamlTagger : ITagger<IErrorTag>
    {
        private readonly ITextBuffer buffer;
        private readonly string file;

        public RapidXamlTagger(ITextBuffer buffer, string file)
        {
            this.buffer = buffer;
            this.file = file;

            RapidXamlDocumentCache.Parsed += this.OnXamlDocParsed;

            // Docs may have already been parsed when we get here (will happen if document was opened with the project) so process what has been parsed
            this.OnXamlDocParsed(this, new RapidXamlParsingEventArgs(null, this.file, this.buffer.CurrentSnapshot, ParsedAction.Unknown));
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        // This creates the underlying tags that are shown on the designer.
        public IEnumerable<ITagSpan<IErrorTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            if (spans.Count == 0)
            {
                yield break;
            }

            var errors = RapidXamlDocumentCache.AdornmentTags(this.file);

            foreach (var viewTag in errors)
            {
                foreach (var span in spans)
                {
                    if (span.IntersectsWith(viewTag.Span))
                    {
                        if (span.Snapshot.TextBuffer != viewTag.Snapshot.TextBuffer)
                        {
                            //// SharedRapidXamlPackage.Logger?.RecordInfo(StringRes.Info_TextBufferChanged.WithParams(this.file));
                            System.Diagnostics.Debug.WriteLine(StringRes.Info_TextBufferChanged.WithParams(this.file));
                            RapidXamlDocumentCache.Invalidate(this.file);
                            break;
                        }

                        if (span.Snapshot.Version.VersionNumber != viewTag.Snapshot.Version.VersionNumber)
                        {
                            //// SharedRapidXamlPackage.Logger?.RecordInfo(StringRes.Info_SnapshotVersionChanged.WithParams(this.file));
                            System.Diagnostics.Debug.WriteLine(StringRes.Info_SnapshotVersionChanged.WithParams(this.file));
                            RapidXamlDocumentCache.Invalidate(this.file);
                            break;
                        }

                        yield return viewTag.AsErrorTag();
                    }
                }
            }
        }

        public void RaiseTagsChanged()
        {
            var args = new RapidXamlParsingEventArgs(null, this.file, this.buffer.CurrentSnapshot, ParsedAction.Update);
            this.OnXamlDocParsed(this, args);
        }

        // This handles adding and removing things from the error list
        private void OnXamlDocParsed(object sender, RapidXamlParsingEventArgs e)
        {
            var visibleErrors = RapidXamlDocumentCache.ErrorListTags(this.file);

            var projectName = this.GetProjectName(this.file);

            var result = new FileErrorCollection { Project = projectName, FilePath = this.file };

            foreach (var viewTag in visibleErrors)
            {
                result.Errors.Add(viewTag.AsErrorRow());
            }

            ErrorListService.Process(result);

            // As the tags that are shown in the error list might have changed, trigger that to be updated too.
            if (e != null)
            {
                var span = new SnapshotSpan(e.Snapshot, 0, e.Snapshot.Length);
                this.TagsChanged?.Invoke(this, new SnapshotSpanEventArgs(span));
            }
        }

        private string GetProjectName(string fileName)
        {
            try
            {
                return ProjectHelpers.Dte2.Solution.FindProjectItem(fileName)?.ContainingProject?.Name ?? string.Empty;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
                return string.Empty;
            }
        }
    }
}
