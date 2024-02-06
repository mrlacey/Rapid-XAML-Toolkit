namespace RapidXaml;

public partial class PickerOptions : BindableObject
{
    public static readonly BindableProperty CsvProperty = BindableProperty.CreateAttached("Csv", typeof(string), typeof(PickerOptions), string.Empty, propertyChanged: OnCsvChanged);

    private static void OnCsvChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is Picker picker)
        {
            if (newValue is string value)
            {
                var options = value.Split(',');
                picker.ItemsSource = options;
            }
        }
    }

    public static string GetCsv(BindableObject view)
    {
        return (string)view.GetValue(CsvProperty);
    }

    public static void SetStrings(BindableObject view, string value)
    {
        view.SetValue(CsvProperty, value);
    }
}
