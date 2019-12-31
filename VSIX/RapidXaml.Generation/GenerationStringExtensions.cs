// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using RapidXamlToolkit.Options;

namespace RapidXamlToolkit
{
    public static class GenerationStringExtensions
    {
        public static bool IsValidXamlOutput(this string source)
        {
            const string validText = "ValidText";
            var placeholders = source.GetPlaceholders();

            var possibleXaml = source;

            // Ensure all placeholders are known
            foreach (var placeholder in placeholders)
            {
                if (!Placeholder.All().Contains(placeholder))
                {
                    return false;
                }

                // remove placeholders for the XML Parse check
                switch (placeholder)
                {
                    case Placeholder.SubProperties:
                        possibleXaml = possibleXaml.Replace(placeholder, "<SubProps />");
                        break;
                    case Placeholder.EnumMembers:
                        possibleXaml = possibleXaml.Replace(placeholder, "<EnumMembers />");
                        break;
                    case Placeholder.EnumElementWithSpaces:
                    case Placeholder.EnumElement:
                        // For scenarios like : "<x:String>$elementwithspaces$</x:String>" or "<x:String>$element$</x:String>"
                        possibleXaml = possibleXaml.Replace(placeholder, validText);
                        break;
                    default:
                        possibleXaml = possibleXaml.Replace(placeholder, "Something");
                        break;
                }
            }

            var attributes = possibleXaml.GetAllAttributes();

            foreach (var attribute in attributes)
            {
                possibleXaml = possibleXaml.Replace(attribute, string.Empty);
            }

            // Check XAML formatting
            try
            {
                // ignore whitespace at start or end
                // ignore placeholders and check is valid XML
                // Wrap in case there are multiple elements
                // Remove known prefix so it's ignored
                XElement element;

                if (possibleXaml.TrimStart().StartsWith("<?xml "))
                {
                    element = XElement.Parse(possibleXaml.Trim().Replace("x:", string.Empty));
                }
                else
                {
                    element = XElement.Parse($"<wrapper>{possibleXaml.Trim().Replace("x:", string.Empty)}</wrapper>");
                }

                foreach (var descendantNode in element.DescendantNodes())
                {
                    if (descendantNode.NodeType != XmlNodeType.Element && descendantNode.ToString() != validText)
                    {
                        System.Diagnostics.Debug.WriteLine(descendantNode.ToString());
                        return false;
                    }
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
                return false;
            }

            return true;
        }

        public static List<string> GetAllAttributes(this string source)
        {
            var attRgx = new Regex(AllowedPlaceholderValidator.AttributePlaceholderPattern);

            var matches = attRgx.Matches(source);

            var result = new List<string>();

            foreach (Match match in matches)
            {
                result.Add(match.Value);
            }

            return result;
        }
    }
}
