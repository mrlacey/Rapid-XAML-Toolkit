using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Adornments;
using Microsoft.VisualStudio.Text.Tagging;
using RapidXamlToolkit.Suggestions;
using RapidXamlToolkit.Tagging;

namespace RapidXamlToolkit.Tagging
{
    public static class RapidXamlDocumentCache
    {
        private static Dictionary<string, RapidXamlDocument> cache = new Dictionary<string, RapidXamlDocument>();

        public static event EventHandler<RapidXamlParsingEventArgs> Parsed;

        public static void Add(string file, ITextSnapshot snapshot)
        {
            if (cache.ContainsKey(file))
            {
                Update(file, snapshot);
            }
            else
            {
                var doc = RapidXamlDocument.Create(snapshot);
                cache.Add(file, doc);

                Parsed?.Invoke(null, new RapidXamlParsingEventArgs(doc, file, snapshot, ParsedAction.Add));
            }
        }

        public static void Update(string file, ITextSnapshot snapshot)
        {
            if (cache[file].RawText != snapshot.GetText())
            {
                var doc = RapidXamlDocument.Create(snapshot);
                cache[file] = doc;

                Parsed?.Invoke(null, new RapidXamlParsingEventArgs(doc, file, snapshot, ParsedAction.Update));
            }
        }

        public static List<IRapidXamlTag> Tags(string fileName)
        {
            var result = new List<IRapidXamlTag>();

            if (cache.ContainsKey(fileName))
            {
                result.AddRange(cache[fileName].SuggestionTags);
            }

            return result;
        }

        public static List<IRapidXamlViewTag> ViewTags(string fileName)
        {
            var result = new List<IRapidXamlViewTag>();

            if (cache.ContainsKey(fileName))
            {
                result.AddRange(cache[fileName].SuggestionTags.OfType<IRapidXamlViewTag>());
            }

            return result;
        }
    }

    public enum ParsedAction
    {
        Add,
        Update,
        Unknown
    }

    public class RapidXamlParsingEventArgs : EventArgs
    {
        public RapidXamlParsingEventArgs(RapidXamlDocument document, string file, ITextSnapshot snapshot, ParsedAction action)
        {
            this.Document = document;
            this.File = file;
            this.Snapshot = snapshot;
            this.Action = action;
        }

        public RapidXamlDocument Document { get; private set; }

        public string File { get; private set; }

        public ITextSnapshot Snapshot { get; private set; }

        public ParsedAction Action { get; private set; }
    }
}

public class RapidXamlDocument
{
    public RapidXamlDocument()
    {
        this.SuggestionTags = new List<IRapidXamlTag>();
    }

    public string RawText { get; set; }

    public List<IRapidXamlTag> SuggestionTags { get; set; }

    public static RapidXamlDocument Create(ITextSnapshot snapshot)
    {
        var result = new RapidXamlDocument();

        var text = snapshot.GetText();
        result.RawText = text;

        // TODO: offload the creation of tags to separate classes for handling each XAML element
        var count = 0;

        var rowDefIndex = text.IndexOf("<RowDefinition");
        while (rowDefIndex >= 0)
        {
            var endPos = text.IndexOf('>', rowDefIndex);

            var tag = new InsertRowDefinitionTag
            {
                Span = new Span(rowDefIndex, endPos - rowDefIndex),
                RowId = count,
            };

            result.SuggestionTags.Add(tag);

            count = count + 1;

            rowDefIndex = text.IndexOf("<RowDefinition", endPos);
        }


        var tbIndex = text.IndexOf("<TextBlock Text=\"");

        if (tbIndex >= 0)
        {
            var tbEnd = text.IndexOf(">", tbIndex);

            var line = snapshot.GetLineFromPosition(tbIndex);
            var col = tbEnd - line.Start.Position;

            result.SuggestionTags.Add(new HardCodedStringTag
            {
                Span = new Span(tbIndex, tbEnd - tbIndex),
                Line = line.LineNumber,
                Column = col,
                Snapshot = snapshot,
            });
        }

        return result;
    }
}

public class InsertRowDefinitionTag : IRapidXamlTag
{
    public ActionTypes ActionType => ActionTypes.InsertRowDefinition;

    // Used for text in suggested action ("Insert new row {RowId}")
    public int RowId { get; set; }

    public Span Span { get; set; }
}
public class HardCodedStringTag : IRapidXamlViewTag
{
    public ActionTypes ActionType => ActionTypes.HardCodedString;

    public Span Span { get; set; }

    public int Line { get; set; }

    public int Column { get; set; }

    public ITextSnapshot Snapshot { get; set; }

    public ITagSpan<IErrorTag> AsErrorTag()
    {
        var span = new SnapshotSpan(this.Snapshot, this.Span);
        return new TagSpan<IErrorTag>(span, new ErrorTag(PredefinedErrorTypeNames.Warning, "HardCoded string message"));
    }

    public XamlWarning AsXamlWarning()
    {
        var span = new SnapshotSpan(this.Snapshot, this.Span);
        var result = new XamlWarning(span);

        return result;
    }
}
