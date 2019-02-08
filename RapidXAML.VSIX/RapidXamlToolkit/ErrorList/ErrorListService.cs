namespace RapidXamlToolkit.ErrorList
{
    class ErrorListService
    {
        public static void Process(ValidationResult result)
        {
            // TODO: need to update this for Rapid XAML warnings
            TableDataSource.Instance.CleanErrors(result.Url);
            TableDataSource.Instance.AddErrors(result);
        }
    }
}
