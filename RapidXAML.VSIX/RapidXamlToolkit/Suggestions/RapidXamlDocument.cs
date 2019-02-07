using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Text;
using RapidXamlToolkit.Suggestions;

namespace RapidXamlToolkit.Tagging
{
    public static class RapidXamlDocumentCache
    {
        private static Dictionary<string, RapidXamlDocument> cache = new Dictionary<string, RapidXamlDocument>();

        public static void Add(string file, string text)
        {
            if (cache.ContainsKey(file))
            {
                Update(file, text);
            }
            else
            {
                cache.Add(file, RapidXamlDocument.Create(text));
            }
        }

        public static void Update(string file, string text)
        {
            if (cache[file].RawText != text)
            {
                cache[file] = RapidXamlDocument.Create(text);
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
    }

    public class RapidXamlDocument
    {
        public RapidXamlDocument()
        {
            this.SuggestionTags = new List<IRapidXamlTag>();
        }

        public string RawText { get; set; }

        public List<IRapidXamlTag> SuggestionTags { get; set; }

        public static RapidXamlDocument Create(string text)
        {
            var result = new RapidXamlDocument();

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
}
