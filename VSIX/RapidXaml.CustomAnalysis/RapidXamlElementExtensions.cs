// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

namespace RapidXaml
{
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
