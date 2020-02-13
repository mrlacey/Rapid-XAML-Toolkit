using System;
using System.Collections.Generic;
using System.Linq;

namespace RapidXaml
{
    public class RapidXamlElement
    {
        public string Name { get; internal set; } = string.Empty;

        public string Content { get; internal set; } = string.Empty;

        public List<RapidXamlAttribute> Attributes { get; internal set; } = new List<RapidXamlAttribute>();

        public List<RapidXamlElement> Children { get; internal set; } = new List<RapidXamlElement>();

        public static RapidXamlElement Build(string name)
        {
            return new RapidXamlElement { Name = name };
        }

        // Utility method to simplify usage
        public bool ContainsAttribute(string attributeName)
        {
            return this.Attributes.Any(
                a => a.Name.Equals(attributeName, StringComparison.InvariantCultureIgnoreCase));
        }

        public override string ToString()
        {
            return $"{Name} ({Attributes.Count} attributes, {Children.Count} children)";
        }
    }

    public static class RapidXamlElementExtensions
    {
        public static RapidXamlElement AddAttribute(this RapidXamlElement expected, string name, string value)
        {
            expected.Attributes.Add(new RapidXamlAttribute() { Name = name, Value = value });

            return expected;
        }
        public static RapidXamlElement SetContent(this RapidXamlElement expected, string content)
        {
            expected.Content = content;

            return expected;
        }
    }
}
