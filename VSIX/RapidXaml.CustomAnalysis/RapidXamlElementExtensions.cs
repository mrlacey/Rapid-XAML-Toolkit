// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;

namespace RapidXaml.TestHelpers
{
    public static class RapidXamlElementExtensions
    {
        public static RapidXamlElement AddChildAttribute(this RapidXamlElement element, string name, string value, int startPos = -1, int length = -1)
        {
            element.Attributes.Add(new RapidXamlAttribute() { Name = name, StringValue = value, IsInline = false, Location = new RapidXamlSpan(startPos, length) });

            return element;
        }

        public static RapidXamlElement AddInlineAttribute(this RapidXamlElement element, string name, string value, int startPos = -1, int length = -1)
        {
            element.Attributes.Add(new RapidXamlAttribute() { Name = name, StringValue = value, IsInline = true, Location = new RapidXamlSpan(startPos, length) });

            return element;
        }

        public static RapidXamlElement AddChildAttribute(this RapidXamlElement element, string name, RapidXamlElement value, int startPos = -1, int length = -1)
        {
            element.Attributes.Add(new RapidXamlAttribute(value) { Name = name, IsInline = false, Location = new RapidXamlSpan(startPos, length) });

            return element;
        }

        public static RapidXamlElement AddChildrenAttribute(this RapidXamlElement element, string name, IEnumerable<RapidXamlElement> children, int startPos = -1, int length = -1)
        {
            var newAttr = new RapidXamlAttribute() { Name = name, IsInline = false, Location = new RapidXamlSpan(startPos, length) };

            newAttr.Children.AddRange(children);

            element.Attributes.Add(newAttr);

            return element;
        }

        public static RapidXamlElement AddChild(this RapidXamlElement element, string name)
        {
            element.Children.Add(new RapidXamlElement() { Name = name });

            return element;
        }

        public static RapidXamlElement AddChild(this RapidXamlElement element, RapidXamlElement child)
        {
            if (child != null)
            {
                element.Children.Add(child);
            }

            return element;
        }

        public static RapidXamlElement SetContent(this RapidXamlElement expected, string content)
        {
            expected.Content = content;

            return expected;
        }
    }
}
