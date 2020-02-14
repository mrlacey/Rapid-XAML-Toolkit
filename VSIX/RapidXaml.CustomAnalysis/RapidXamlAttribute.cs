// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

namespace RapidXaml
{
    public class RapidXamlAttribute
    {
        public string Name { get; internal set; }

        public string Value { get; internal set; }

        public override string ToString()
        {
            return $"{this.Name}=\"{this.Value}\"";
        }
    }
}
