namespace RapidXamlToolkit.ErrorList
{
    class ErrorListService
    {
        public static void Process(ValidationResult result)
        {
            TableDataSource.Instance.CleanErrors(result.Url);
            TableDataSource.Instance.AddErrors(result);
        }
    }
}
