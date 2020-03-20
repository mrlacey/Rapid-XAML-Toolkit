// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.Linq;

namespace RapidXaml
{
    /// <summary>
    /// A representation of an attribute of a XAML element.
    /// </summary>
    public class RapidXamlAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RapidXamlAttribute"/> class.
        /// </summary>
        public RapidXamlAttribute()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RapidXamlAttribute"/> class.
        /// </summary>
        /// <param name="children">Child elements of the attribute.</param>
        public RapidXamlAttribute(params RapidXamlElement[] children)
        {
            this.Children.AddRange(children);
        }

        /// <summary>
        /// Gets the name of the attribute.
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether this is an inline attribute.
        /// </summary>
        public bool IsInline { get; internal set; } = true;

        /// <summary>
        /// Gets a value indicating whether the attribute has a string value.
        /// </summary>
        public bool HasStringValue
        {
            get
            {
                return !string.IsNullOrEmpty(this.StringValue);
            }
        }

        /// <summary>
        /// Gets the string value of the attribute, if it has one.
        /// </summary>
        public string StringValue { get; internal set; }

        /// <summary>
        /// Gets all elements set as children of the attribute.
        /// </summary>
        public List<RapidXamlElement> Children { get; } = new List<RapidXamlElement>();

        /// <summary>
        /// Gets the first (or only) child element assigned to the attribute.
        /// </summary>
        public RapidXamlElement Child
        {
            get
            {
                return this.Children.FirstOrDefault();
            }
        }

        /// <summary>
        /// Gets the location of the attribute in the original string.
        /// </summary>
        public RapidXamlSpan Location { get; internal set; } = new RapidXamlSpan();

        /// <inheritdoc/>
        public override string ToString()
        {
            if (this.HasStringValue)
            {
                return $"{this.Name}=\"{this.StringValue}\"";
            }
            else
            {
                return $"{this.Name}=\"{this.Children.Count} child element{(this.Children.Count > 1 ? "s" : string.Empty)}\"";
            }
        }
    }
}
