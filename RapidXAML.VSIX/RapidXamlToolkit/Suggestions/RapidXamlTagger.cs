using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using RapidXamlToolkit.ErrorList;

namespace RapidXamlToolkit.Tagging
{
    public class RapidXamlTagger : ITagger<IErrorTag>
    {
        private ITextBuffer _buffer;
        private string _file;
        private IEnumerable<XamlError> _errors;
        private List<XamlError> _errorsCached;  // Cache errors to avoid validation on GetTags

        public RapidXamlTagger(ITextBuffer buffer, string file)
        {
            _buffer = buffer;
            _file = file;

            RapidXamlDocumentCache.Parsed += this.OnXamlDocParsed;

            // Docs may have already been parsed when we get here (will happen if document was opened with the project) so process what has been parsed
            this.OnXamlDocParsed(this, new RapidXamlParsingEventArgs(
                null, _file, _buffer.CurrentSnapshot, ParsedAction.Unknown));

          //  RapidXamlAnalyzerFactory.Parsed += XamlParsed;

            // Init parsing
           // var doc = _buffer.CurrentSnapshot.ParseToXaml(_file);
          //  _errors = doc.Validate(_file);
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        private void OnXamlDocParsed(object sender, RapidXamlParsingEventArgs e)
        {

            var visibleErrors = RapidXamlDocumentCache.ViewTags(_file);

            foreach (var viewTag in visibleErrors)
            {
                var result = new ValidationResult();
                result.Project = "a-project";
                result.Url = $"{viewTag.Line} - {viewTag.Column}";
                result.Errors = new List<Error>();

                result.Errors.Add(new Error { Extract = viewTag.ActionType.ToString(), Message = "rxt - viewtag" });

                ErrorListService.Process(result);
            }

            if (e != null)
            {
                var span = new SnapshotSpan(e.Snapshot, 0, e.Snapshot.Length);
                TagsChanged?.Invoke(this, new SnapshotSpanEventArgs(span));
            }


            /*
            var span = new SnapshotSpan(e.Snapshot, 0, e.Snapshot.Length);
            TagsChanged?.Invoke(this, new SnapshotSpanEventArgs(span));







            // Translate all the spelling errors to the new snapshot (and remove anything that is a dirty region since we will need to check that again).
            var oldErrors = this.Factory.CurrentSnapshot;
            var newErrors = new ErrorsSnapshot(_file, oldErrors.VersionNumber + 1);

            // Copy all of the old errors to the new errors unless the error was affected by the text change
            foreach (var error in oldErrors.Errors)
            {
                Debug.Assert(error.NextIndex == -1);

                var newError = XamlWarning.CloneAndTranslateTo(error, e.Snapshot);

                if (newError != null)
                {
                    Debug.Assert(newError.Span.Length == error.Span.Length);

                    error.NextIndex = newErrors.Errors.Count;
                    newErrors.Errors.Add(newError);
                }
            }



            if (newErrors.Count == 0)
            {

                var errors = RapidXamlDocumentCache.ViewTags(e.File);


                foreach (var viewTag in errors)
                {
                    if (span.IntersectsWith(span))
                    {
                        newErrors.Errors.Add(viewTag.AsXamlWarning());
                    }
                }
            }

            this.Factory.UpdateErrors(newErrors);


            TagsChanged?.Invoke(this, new SnapshotSpanEventArgs(span));




            _provider.UpdateAllSinks();
            */
        }

        public IEnumerable<ITagSpan<IErrorTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            if (spans.Count == 0)
            {
                yield break;
            }

            var errors = RapidXamlDocumentCache.ViewTags(_file);

            foreach (var viewTag in errors)
            {
                foreach (var span in spans)
                {
                    if (span.IntersectsWith(viewTag.Span))
                    {
                        yield return viewTag.AsErrorTag();
                    }
                }
            }


            //   // Check cache
            //   var isCached = _errorsCached != null;
            //   var errors = isCached ? _errorsCached : _errors;
            //   if (!isCached) _errorsCached = new List<XamlError>();
            //
            //   foreach (var error in errors)
            //   {
            //       if (!isCached) _errorsCached.Add(error);
            //       var errorTag = GenerateTag(error);
            //
            //       if (errorTag != null)
            //       {
            //           yield return errorTag;
            //       }
            //   }
        }

        private TagSpan<IErrorTag> GenerateTag(XamlError error)
        {
            if (_buffer.CurrentSnapshot.Length >= error.Span.End)
            {
                var span = new SnapshotSpan(_buffer.CurrentSnapshot, error.Span);
                return new TagSpan<IErrorTag>(span, error.CreateTag());
            }

            return null;
        }

        private void XamlParsed(object sender, ParsingEventArgs e)
        {
            if (string.IsNullOrEmpty(e.File) || e.Snapshot != _buffer.CurrentSnapshot)
            {
                return;
            }

            // Clear cache if document is updated
            _errorsCached = null;
            _errors = e.Document.Validate(e.File);

            SnapshotSpan span = new SnapshotSpan(_buffer.CurrentSnapshot, 0, _buffer.CurrentSnapshot.Length);
            TagsChanged?.Invoke(this, new SnapshotSpanEventArgs(span));
        }
    }
}
