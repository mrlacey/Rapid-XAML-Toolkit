namespace RapidXamlToolkit.Tagging
{
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
}
