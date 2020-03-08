// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.Linq;

namespace RapidXaml
{
    public class RapidXamlAttribute
    {
        public RapidXamlAttribute()
        {
        }

        public RapidXamlAttribute(params RapidXamlElement[] children)
        {
            this.Children.AddRange(children);
        }

        public string Name { get; internal set; }

        public bool IsInline { get; internal set; } = true;

        public bool HasStringValue
        {
            get
            {
                return !string.IsNullOrEmpty(this.StringValue);
            }
        }

        public string StringValue { get; internal set; }

        public List<RapidXamlElement> Children { get; } = new List<RapidXamlElement>();

        public RapidXamlElement Child
        {
            get
            {
                return this.Children.First();
            }
        }

        public RapidXamlSpan Location { get; internal set; } = new RapidXamlSpan();

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
