// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

namespace RapidXaml
{
    public class RapidXamlAttribute
    {
        public string Name { get; internal set; }

        // TODO: calculate attribute positions - if needed?
        public RapidXamlSpan Postion { get; internal set; }

        public bool HasStringValue
        {
            get
            {
                return !string.IsNullOrEmpty(this.StringValue);
            }
        }

        public string StringValue { get; internal set; }

        public RapidXamlElement ElementValue { get; internal set; }

        public override string ToString()
        {
            return $"{this.Name}=\"{(this.HasStringValue ? this.StringValue : this.ElementValue.ToString())}\"";
        }
    }
}
