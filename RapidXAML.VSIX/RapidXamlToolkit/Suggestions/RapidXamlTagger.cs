using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using RapidXamlToolkit.Suggestions;

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

            RapidXamlAnalyzerFactory.Parsed += XamlParsed;

            // Init parsing
            var doc = _buffer.CurrentSnapshot.ParseToXaml(_file);
            _errors = doc.Validate(_file);
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        public IEnumerable<ITagSpan<IErrorTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            if (spans.Count == 0 || _errors == null)
            {
                yield break;
            }

            // Check cache
            var isCached = _errorsCached != null;
            var errors = isCached ? _errorsCached : _errors;
            if (!isCached) _errorsCached = new List<XamlError>();

            foreach (var error in errors)
            {
                if (!isCached) _errorsCached.Add(error);
                var errorTag = GenerateTag(error);

                if (errorTag != null)
                {
                    yield return errorTag;
                }
            }
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


    public class ParsingEventArgs : EventArgs
    {
        public ParsingEventArgs(XamlDocument document, string file, ITextSnapshot snapshot)
        {
            Document = document;
            File = file;
            Snapshot = snapshot;
        }

        public XamlDocument Document { get; set; }

        public string File { get; set; }

        public ITextSnapshot Snapshot { get; set; }
    }

    public class XamlDocument
    {
        private DataEntry[] attachedDatas;
        private int count;

        public XamlDocument()
        {
            this.Span = default(Span);
            this.SuggestionTags = new List<IRapidXamlTag>();
        }

        public Span Span { get; set; }

        public List<IRapidXamlTag> SuggestionTags { get; set; }

        public void SetData(string key, object value)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (this.attachedDatas == null)
            {
                this.attachedDatas = new DataEntry[1];
            }
            else
            {
                for (int index = 0; index < this.count; ++index)
                {
                    if (this.attachedDatas[index].Key == key)
                    {
                        this.attachedDatas[index].Value = value;
                        return;
                    }
                }

                if (this.count == this.attachedDatas.Length)
                {
                    DataEntry[] dataEntryArray = new DataEntry[this.attachedDatas.Length + 1];
                    Array.Copy((Array)this.attachedDatas, 0, (Array)dataEntryArray, 0, this.count);
                    this.attachedDatas = dataEntryArray;
                }
            }

            this.attachedDatas[this.count] = new DataEntry(key, value);
            ++this.count;
        }

        /// <summary>
        /// Determines whether this instance contains the specified key data.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns><c>true</c> if a data with the key is stored</returns>
        /// <exception cref="T:System.ArgumentNullException">if key is null</exception>
        public bool ContainsData(object key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (this.attachedDatas == null)
            {
                return false;
            }

            for (int index = 0; index < this.count; ++index)
            {
                if (this.attachedDatas[index].Key == key)
                {
                    return true;
                }
            }

            return false;
        }

        public object GetData(object key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (this.attachedDatas == null)
            {
                return (object)null;
            }

            for (int index = 0; index < this.count; ++index)
            {
                if (this.attachedDatas[index].Key == key)
                {
                    return this.attachedDatas[index].Value;
                }
            }

            return (object)null;
        }
    }

    internal struct DataEntry
    {
        public readonly object Key;
        public object Value;

        public DataEntry(object key, object value)
        {
            this.Key = key;
            this.Value = value;
        }
    }

    public static class RapidXamlAnalyzerFactory
    {
        private const string AttachedExceptionKey = "attached-exception";
        public static object _syncRoot = new object();
        private static readonly ConditionalWeakTable<ITextSnapshot, XamlDocument> CachedDocuments = new ConditionalWeakTable<ITextSnapshot, XamlDocument>();

        public static event EventHandler<ParsingEventArgs> Parsed;

        public static IEnumerable<XamlError> Validate(this XamlDocument doc, string file)
        {
            var exception = doc.GetAttachedException();
            if (exception != null)
            {
                yield return new XamlError
                {
                    File = file,
                    Message = "Unexpected error occurred while parsing XAML. Please log an issue to https://github.com/Microsoft/Rapid-XAML-Toolkit/issues Reason: " + exception,
                    Line = 0,
                    Column = 0,
                    ErrorCode = "RXT0000",
                    Fatal = true,
                    Span = new Span(doc.Span.Start, doc.Span.Length),
                };
            }
        }

        public static Exception GetAttachedException(this XamlDocument xamlDocument)
        {
            if (xamlDocument.ContainsData("RowDef"))
            {
                return xamlDocument.GetData("RowDef") as Exception;
            }

            return xamlDocument.GetData(AttachedExceptionKey) as Exception;
        }

        public static XamlDocument ParseToXaml(this ITextSnapshot snapshot, string file = null)
        {
            lock (_syncRoot)
            {
                return CachedDocuments.GetValue(snapshot, key =>
                {
                    var text = key.GetText();
                    var xamlDocument = ParseToXaml(text);
                    Parsed?.Invoke(snapshot, new ParsingEventArgs(xamlDocument, file, snapshot));
                    return xamlDocument;
                });
            }
        }

        public static XamlDocument ParseToXaml(string text)
        {
            XamlDocument xamlDocument;

            try
            {
             //   xamlDocument = RapidXamlDocument.Create(text);
                xamlDocument = new XamlDocument();
            }
            catch (Exception ex)
            {
                xamlDocument = new XamlDocument();

                xamlDocument.Span = new Span(0, text.Length - 1);

                // we attach the exception to the document that will be later displayed to the user
                xamlDocument.SetData(AttachedExceptionKey, ex);
            }

            return xamlDocument;
        }
    }

    public class XamlError
    {
        public string Project { get; set; }

        public string File { get; set; }

        public string Message { get; set; }

        public int Line { get; set; }

        public int Column { get; set; }

        public string ErrorCode { get; set; }

        public Span Span { get; set; }

        public bool Fatal { get; set; }

        public virtual IErrorTag CreateTag()
        {
            return new ErrorTag("Intellisense", Message);
        }
    }
}
