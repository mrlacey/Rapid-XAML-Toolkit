namespace RapidXaml
{
    public class RapidXamlAttribute
    {
        public string Name { get; internal set; }

        public string Value { get; internal set; }

        public override string ToString()
        {
            return $"{Name}=\"{Value}\"";
        }
    }
}
