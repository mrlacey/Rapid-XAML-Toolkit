using System.Collections.Generic;
using Microsoft.VisualStudio.Text;
using RapidXamlToolkit.Suggestions;

namespace RapidXamlToolkit.Tagging
{
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
}
