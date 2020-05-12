// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace RapidXaml
{
    /// <summary>
    /// A representation of a XAML element.
    /// </summary>
    [DebuggerDisplay("{OriginalString}")]
    public class RapidXamlElement
    {
        /// <summary>
        /// Gets the name of the element.
        /// </summary>
        public string Name { get; internal set; } = string.Empty;

        /// <summary>
        /// Gets the inner content of the element. Or empty if none.
        /// </summary>
        public string Content { get; internal set; } = string.Empty;

        /// <summary>
        /// Gets the original text that the element was created from.
        /// May be empty if the object was created directly.
        /// </summary>
        public string OriginalString { get; internal set; } = string.Empty;

        /// <summary>
        /// Gets the location of the element in the original document.
        /// </summary>
        public RapidXamlSpan Location { get; internal set; } = new RapidXamlSpan();

        /// <summary>
        /// Gets a list of the attributes assigned to the element.
        /// This includes attributes specified inline or as children.
        /// </summary>
        public List<RapidXamlAttribute> Attributes { get; } = new List<RapidXamlAttribute>();

        /// <summary>
        /// Gets a list of child elements specified for th element.
        /// </summary>
        public List<RapidXamlElement> Children { get; } = new List<RapidXamlElement>();

        /// <summary>
        /// Gets all attributes that are specified as child elements.
        /// </summary>
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

        /// <summary>
        /// A builder method to create an element of the specified name.
        /// </summary>
        /// <param name="name">The name of the element.</param>
        /// <param name="start">(Optional) the starting position of element in the containing doc.</param>
        /// <param name="length">(Optional) the length of the element in the containing doc.</param>
        /// <param name="originalXaml">(Optional) The original text the element represents.</param>
        /// <returns>An element representing XAML text.</returns>
        public static RapidXamlElement Build(string name, int start = -1, int length = -1, string originalXaml = "")
        {
            return new RapidXamlElement { Name = name, Location = new RapidXamlSpan(start, length), OriginalString = originalXaml };
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{this.Name} ({this.Attributes.Count} attributes, {this.Children.Count} children)";
        }

        /// <summary>
        /// Does the element contain an attribute with the specified name.
        /// </summary>
        /// <param name="attributeName">The name of the attribute to check for.</param>
        /// <returns>True if the element has an attribute with the specified name.</returns>
        public bool ContainsAttribute(string attributeName)
        {
            return this.Attributes.Any(
                a => a.Name.Equals(attributeName, StringComparison.InvariantCultureIgnoreCase));
        }

        /// <summary>
        /// Does the element contain a child with the specified name.
        /// </summary>
        /// <param name="childName">The name of the child to check for.</param>
        /// <returns>True if the element contains a child with the specified name.</returns>
        public bool ContainsChild(string childName)
        {
            return this.Children.Any(
                c => c.Name.Equals(childName, StringComparison.InvariantCultureIgnoreCase)
                  || c.Name.EndsWith($":{childName}", StringComparison.InvariantCultureIgnoreCase));
        }

        /// <summary>
        /// Does the element have a child or any descendant with the specified name.
        /// </summary>
        /// <param name="elementName">The name of the element to check for.</param>
        /// <returns>True if the element contains a child or any descendant with the specified name.</returns>
        public bool ContainsDescendant(string elementName)
        {
            if (this.ContainsChildOrAttribute(elementName))
            {
                return true;
            }

            foreach (var attr in this.Attributes)
            {
                if (!attr.HasStringValue)
                {
                    foreach (var child in attr.Children)
                    {
                        if (child.Name.Equals(elementName, StringComparison.InvariantCultureIgnoreCase)
                         || child.Name.EndsWith($":{elementName}", StringComparison.InvariantCultureIgnoreCase)
                         || child.ContainsDescendant(elementName))
                        {
                            return true;
                        }
                    }
                }
            }

            foreach (var child in this.Children)
            {
                if (child.Name.Equals(elementName, StringComparison.InvariantCultureIgnoreCase)
                 || child.Name.EndsWith($":{elementName}", StringComparison.InvariantCultureIgnoreCase)
                 || child.ContainsDescendant(elementName))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Get attributes of the element that have the specified name.
        /// </summary>
        /// <param name="attributeName">The name of the attributes to get.</param>
        /// <returns>Attributes with the specified name.</returns>
        public IEnumerable<RapidXamlAttribute> GetAttributes(string attributeName)
        {
            // This returns an enumerable because attribute element may have been set multiple times. It shouldn't be valid but may happen.
            foreach (var attr in this.Attributes)
            {
                if (attr.Name.Equals(attributeName, StringComparison.InvariantCultureIgnoreCase))
                {
                    yield return attr;
                }
            }
        }

        public bool HasAttribute(string attributeName)
        {
            return this.Attributes.Any(a => a.Name == attributeName);
        }

        /// <summary>
        /// Get child elements that have the specified name.
        /// </summary>
        /// <param name="childName">The name of the children to get.</param>
        /// <returns>Child elements with the specified name.</returns>
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

        /// <summary>
        /// Get child elements and any descendants that have the specified name.
        /// </summary>
        /// <param name="elementName">The name of elements to get.</param>
        /// <returns>All child or descendant eements with the specified name.</returns>
        public IEnumerable<RapidXamlElement> GetDescendants(string elementName)
        {
            foreach (var attr in this.Attributes)
            {
                if (!attr.HasStringValue)
                {
                    foreach (var attrChild in attr.Children)
                    {
                        if (attrChild.Name.Equals(elementName, StringComparison.InvariantCultureIgnoreCase))
                        {
                            yield return attrChild;
                        }
                    }

                    foreach (var innerChild in attr.Child.GetDescendants(elementName))
                    {
                        yield return innerChild;
                    }
                }
            }

            foreach (var child in this.GetChildren(elementName))
            {
                yield return child;
            }

            foreach (var child in this.Children)
            {
                foreach (var innerChild in child.GetDescendants(elementName))
                {
                    yield return innerChild;
                }
            }
        }

        private bool ContainsChildOrAttribute(string name)
        {
            return this.Children.Any(
                c => c.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase)
                  || c.Name.EndsWith($":{name}", StringComparison.InvariantCultureIgnoreCase))
                || this.Attributes.Any(
                    a => !a.HasStringValue
                      && a.Children.Any(c => c.ContainsDescendant(name)));
        }
    }
}
