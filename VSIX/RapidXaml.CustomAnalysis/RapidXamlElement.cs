// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;

namespace RapidXaml
{
    public class RapidXamlElement
    {
        public string Name { get; internal set; } = string.Empty;

        public string Content { get; internal set; } = string.Empty;

        public RapidXamlSpan Location { get; internal set; } = new RapidXamlSpan();

        public List<RapidXamlAttribute> Attributes { get; } = new List<RapidXamlAttribute>();

        public List<RapidXamlElement> Children { get; } = new List<RapidXamlElement>();

        public static RapidXamlElement Build(string name, int start = -1, int length = -1)
        {
            return new RapidXamlElement { Name = name, Location = new RapidXamlSpan(start, length) };
        }

        public override string ToString()
        {
            return $"{this.Name} ({this.Attributes.Count} attributes, {this.Children.Count} children)";
        }

        public string ToXamlString()
        {
            // TODO: implement RapidXamlElement.ToXamlString() - As may be useful for some replacement tasks.
            return string.Empty;
        }

        // Utility methods to simplify usage
        public bool ContainsAttribute(string attributeName)
        {
            return this.Attributes.Any(
                a => a.Name.Equals(attributeName, StringComparison.InvariantCultureIgnoreCase));
        }

        public bool ContainsChild(string childName)
        {
            return this.Children.Any(
                c => c.Name.Equals(childName, StringComparison.InvariantCultureIgnoreCase)
                  || c.Name.EndsWith($":{childName}", StringComparison.InvariantCultureIgnoreCase));
        }

        public bool ContainsChildOrAttribute(string childName)
        {
            return this.Children.Any(
                c => c.Name.Equals(childName, StringComparison.InvariantCultureIgnoreCase)
                  || c.Name.EndsWith($":{childName}", StringComparison.InvariantCultureIgnoreCase))
                || this.Attributes.Any(
                    a => !a.HasStringValue
                      && (a.ElementValue.Name.Equals(childName, StringComparison.InvariantCultureIgnoreCase)
                       || a.ElementValue.Name.EndsWith($":{childName}", StringComparison.InvariantCultureIgnoreCase)));
        }

        public bool ContainsDescendant(string childName)
        {
            if (this.ContainsChildOrAttribute(childName))
            {
                return true;
            }

            foreach (var attr in this.Attributes)
            {
                if (!attr.HasStringValue)
                {
                    if (attr.ElementValue.ContainsDescendant(childName))
                    {
                        return true;
                    }
                }
            }

            foreach (var child in this.Children)
            {
                if (child.ContainsDescendant(childName))
                {
                    return true;
                }
            }

            return false;
        }

        // This returns an enumerable because attribute element may have multiple content values
        public IEnumerable<RapidXamlAttribute> GetAttributes(string attributeName)
        {
            foreach (var attr in this.Attributes)
            {
                if (attr.Name.Equals(attributeName, StringComparison.InvariantCultureIgnoreCase))
                {
                    yield return attr;
                }
            }
        }

        public IEnumerable<RapidXamlElement> GetChildren(string childName)
        {
            foreach (var child in this.Children)
            {
                if (child.Name.Equals(childName, StringComparison.InvariantCultureIgnoreCase)
                 || child.Name.EndsWith($":{childName}", StringComparison.InvariantCultureIgnoreCase))
                {
                    yield return child;
                }
            }
        }

        public IEnumerable<RapidXamlElement> GetDescendants(string childName)
        {
            foreach (var attr in this.Attributes)
            {
                if (!attr.HasStringValue)
                {
                    if (attr.ElementValue.Name.Equals(childName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        yield return attr.ElementValue;
                    }

                    foreach (var innerChild in attr.ElementValue.GetDescendants(childName))
                    {
                        yield return innerChild;
                    }
                }
            }

            foreach (var child in this.GetChildren(childName))
            {
                yield return child;
            }

            foreach (var child in this.Children)
            {
                foreach (var innerChild in child.GetDescendants(childName))
                {
                    yield return innerChild;
                }
            }
        }
    }
}
