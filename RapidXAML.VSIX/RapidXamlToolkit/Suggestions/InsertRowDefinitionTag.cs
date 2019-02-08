using Microsoft.VisualStudio.Text;
using RapidXamlToolkit.Suggestions;

public class InsertRowDefinitionTag : IRapidXamlTag
{
    public ActionTypes ActionType => ActionTypes.InsertRowDefinition;

    // Used for text in suggested action ("Insert new row {RowId}")
    public int RowId { get; set; }

    public Span Span { get; set; }
}
