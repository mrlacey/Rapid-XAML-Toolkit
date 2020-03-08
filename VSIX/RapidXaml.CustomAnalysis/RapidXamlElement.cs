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

        public string OriginalString { get; protected set; } = string.Empty;

        public RapidXamlSpan Location { get; internal set; } = new RapidXamlSpan();

        public List<RapidXamlAttribute> Attributes { get; } = new List<RapidXamlAttribute>();

        public List<RapidXamlElement> Children { get; } = new List<RapidXamlElement>();

        public IEnumerable<RapidXamlAttribute> ChildAttributes
        {
            get
            {
                foreach (var attr in this.Attributes)
                {
                    if (!attr.HasStringValue)
                    {
                        yield return attr;
                    }
                }
            }
        }

        public static RapidXamlElement Build(string name, int start = -1, int length = -1, string originalXaml = "")
        {
            return new RapidXamlElement { Name = name, Location = new RapidXamlSpan(start, length), OriginalString = originalXaml };
        }

        public override string ToString()
        {
            return $"{this.Name} ({this.Attributes.Count} attributes, {this.Children.Count} children)";
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

        // TODO: add more tests for this
        public bool ContainsChildOrAttribute(string childName)
        {
            return this.Children.Any(
                c => c.Name.Equals(childName, StringComparison.InvariantCultureIgnoreCase)
                  || c.Name.EndsWith($":{childName}", StringComparison.InvariantCultureIgnoreCase))
                || this.Attributes.Any(
                    a => !a.HasStringValue
                      && a.Children.Any(c => c.ContainsDescendant(childName)));
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
                    foreach (var child in attr.Children)
                    {
                        if (child.Name.Equals(childName, StringComparison.InvariantCultureIgnoreCase)
                         || child.Name.EndsWith($":{childName}", StringComparison.InvariantCultureIgnoreCase)
                         || child.ContainsDescendant(childName))
                        {
                            return true;
                        }
                    }
                }
            }

            foreach (var child in this.Children)
            {
                if (child.Name.Equals(childName, StringComparison.InvariantCultureIgnoreCase)
                 || child.Name.EndsWith($":{childName}", StringComparison.InvariantCultureIgnoreCase)
                 || child.ContainsDescendant(childName))
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
                    if (attr.Child.Name.Equals(childName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        yield return attr.Child;
                    }

                    foreach (var innerChild in attr.Child.GetDescendants(childName))
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
