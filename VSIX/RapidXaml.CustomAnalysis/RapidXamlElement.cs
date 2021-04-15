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
        private readonly int? startChange = null;
        private List<RapidXamlAttribute> attributes;
        private List<RapidXamlElement> children;
        private bool attributesUpdated = false;
        private bool childrenUpdated = false;
        private bool? isSelfClosing = null;

        public RapidXamlElement()
        {
        }

        internal RapidXamlElement(int startChange)
        {
            this.startChange = startChange;
        }

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
        public List<RapidXamlAttribute> Attributes
        {
            get
            {
                if (this.attributes == null)
                {
                    this.attributes = new List<RapidXamlAttribute>();
                }

                if (this.startChange.HasValue && !this.attributesUpdated)
                {
                    var newAttributes = new List<RapidXamlAttribute>();

                    for (int i = 0; i < this.attributes.Count; i++)
                    {
                        newAttributes.Add(this.attributes[i].CloneWithAdjustedLocationStart(this.startChange.Value));
                    }

                    this.attributes = newAttributes;
                    this.attributesUpdated = true;
                }

                return this.attributes;
            }

            internal set
            {
                this.attributes = value;
            }
        }

        /// <summary>
        /// Gets a list of child elements specified for th element.
        /// </summary>
        public List<RapidXamlElement> Children
        {
            get
            {
                if (this.children == null)
                {
                    this.children = new List<RapidXamlElement>();
                }

                if (this.startChange.HasValue && !this.childrenUpdated)
                {
                    var newChildren = new List<RapidXamlElement>();

                    for (int i = 0; i < this.children.Count; i++)
                    {
                        newChildren.Add(this.children[i].CloneWithAdjustedLocationStart(this.startChange.Value));
                    }

                    this.children = newChildren;
                    this.childrenUpdated = true;
                }

                return this.children;
            }

            internal set
            {
                this.children = value;
            }
        }

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

                yield break;
            }
        }

        /// <summary>
        /// Gets all attributes that are not specified as child elements.
        /// </summary>
        public IEnumerable<RapidXamlAttribute> InlineAttributes
        {
            get
            {
                foreach (var attr in this.Attributes)
                {
                    if (attr.HasStringValue)
                    {
                        yield return attr;
                    }
                }

                yield break;
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

            yield break;
        }

        /// <summary>
        /// Get attributes of the element that have the specified names.
        /// </summary>
        /// <param name="attributeNames">The names of the attributes to get.</param>
        /// <returns>Attributes with the specified names.</returns>
        public IEnumerable<RapidXamlAttribute> GetAttributes(params string[] attributeNames)
        {
            foreach (var attr in this.Attributes)
            {
                foreach (var attName in attributeNames)
                {
                    if (attr.Name.Equals(attName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        yield return attr;
                    }
                }
            }

            yield break;
        }

        /// <summary>
        /// Tries to get the string value of the specified attribtue.
        /// </summary>
        /// <param name="attributeName">The name of the desired attribute.</param>
        /// <param name="value">The string value (if found.)</param>
        /// <returns>True if there was an attribute with the specified name and it has a srting value.</returns>
        public bool TryGetAttributeStringValue(string attributeName, out string value)
        {
            var attr = this.GetAttributes(attributeName).FirstOrDefault();

            if (attr != null && attr.HasStringValue)
            {
                value = attr.StringValue;
                return true;
            }

            value = null;
            return false;
        }

        /// <summary>
        /// Gets a value indicating if the element has an attribute with the specified name.
        /// </summary>
        /// <param name="attributeName">The name of the desired attribute.</param>
        /// <returns>True if the element has an attribute with that name.</returns>
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

            yield break;
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

            yield break;
        }

        public RapidXamlElement WithUpdatedLocationStart(int newLocationStart)
        {
            var change = newLocationStart - this.Location.Start;

            var result = this.CloneWithAdjustedLocationStart(change);

            return result;
        }

        public RapidXamlElement CloneWithAdjustedLocationStart(int startChange)
        {
            var result = new RapidXamlElement
            {
                Content = this.Content,
                Location = this.Location.CloneWithAdjustedLocationStart(startChange),
                Name = this.Name,
                OriginalString = this.OriginalString,
            };

            for (int i = 0; i < this.Children.Count; i++)
            {
                result.Children.Add(this.Children[i].CloneWithAdjustedLocationStart(startChange));
            }

            for (int i = 0; i < this.Attributes.Count; i++)
            {
                result.Attributes.Add(this.Attributes[i].CloneWithAdjustedLocationStart(startChange));
            }

            return result;
        }

        public bool IsSelfClosing()
        {
            if (!this.isSelfClosing.HasValue)
            {
                this.isSelfClosing = this.OriginalString.EndsWith("/>");
            }

            return this.isSelfClosing.Value;
        }

        public void OverrideIsSelfClosing(bool newValue)
        {
            this.isSelfClosing = newValue;
        }

        private bool ContainsChildOrAttribute(string name)
        {
            return this.Children.Any(
                    c => c.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase)
                      || c.Name.EndsWith($":{name}", StringComparison.InvariantCultureIgnoreCase))
                || this.Attributes.Any(
                    a => !a.HasStringValue
                      && a.Children.Any(c => c?.ContainsDescendant(name) ?? false));
        }
    }
}
