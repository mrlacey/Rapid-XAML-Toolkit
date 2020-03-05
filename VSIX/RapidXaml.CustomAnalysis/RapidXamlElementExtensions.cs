// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

namespace RapidXaml
{
    public static class RapidXamlElementExtensions
    {
        public static RapidXamlElement AddChildAttribute(this RapidXamlElement expected, string name, string value)
        {
            expected.Attributes.Add(new RapidXamlAttribute() { Name = name, StringValue = value, IsInline = false });

            return expected;
        }

        public static RapidXamlElement AddInlineAttribute(this RapidXamlElement expected, string name, string value)
        {
            expected.Attributes.Add(new RapidXamlAttribute() { Name = name, StringValue = value, IsInline = true });

            return expected;
        }

        public static RapidXamlElement AddChildAttribute(this RapidXamlElement expected, string name, RapidXamlElement value)
        {
            expected.Attributes.Add(new RapidXamlAttribute() { Name = name, ElementValue = value, IsInline = false });

            return expected;
        }

        public static RapidXamlElement AddChild(this RapidXamlElement expected, string name)
        {
            expected.Children.Add(new RapidXamlElement() { Name = name });

            return expected;
        }

        public static RapidXamlElement AddChild(this RapidXamlElement expected, RapidXamlElement child)
        {
            if (child != null)
            {
                expected.Children.Add(child);
            }

            return expected;
        }

        public static RapidXamlElement SetContent(this RapidXamlElement expected, string content)
        {
            expected.Content = content;

            return expected;
        }
    }
}
