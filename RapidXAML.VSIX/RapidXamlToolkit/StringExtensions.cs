// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace RapidXamlToolkit
{
    public static class StringExtensions
    {
        // Convenient syntactic wrapper around string.Format
        public static string WithParams(this string value, params object[] args)
        {
            return string.Format(value, args);
        }

        public static bool ContainsAnyOf(this string value, string filters)
        {
            if (filters == null)
            {
                throw new ArgumentNullException(nameof(filters));
            }

            return value.ContainsAnyOf(filters.Split('|'));
        }

        public static bool ContainsAnyOf(this string value, string[] filters)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (filters == null)
            {
                throw new ArgumentNullException(nameof(filters));
            }

            if (filters.Length > 0)
            {
                return filters.Any(f => !string.IsNullOrWhiteSpace(f) && value.ToUpperInvariant().Contains(f.ToUpperInvariant()));
            }

            return false;
        }

        public static bool MatchesAnyOf(this string value, string options)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            var allOptions = options.Split('|');

            return allOptions.Any(o => string.Equals(value, o, StringComparison.OrdinalIgnoreCase));
        }

        public static bool MatchesAnyOfInCSharpFormat(this string value, string options)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            var allOptions = options.Split('|');

            return allOptions.Any(o => string.Equals(value, o.ToCSharpFormat(), StringComparison.OrdinalIgnoreCase));
        }

        public static string ToCSharpFormat(this string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            var result = value.Replace("(Of ", "<")
                              .Replace("(of ", "<")
                              .Replace("(OF ", "<");

            // VB array may end "()" and don't want to break that,
            //  only want to finish the replacement for closing the opening bracket replaced above.
            if (!result.Contains("("))
            {
                result = result.Replace(")", ">");
            }

            return result;
        }

        public static bool IsGenericTypeName(this string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            // Quick hacky way of doing this and relies on being in C# format
            return value.ToCSharpFormat().EndsWith(">");
        }

        public static string RemoveFromEndIfExists(this string value, string toRemove)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (toRemove == null)
            {
                throw new ArgumentNullException(nameof(toRemove));
            }

            var result = value;

            if (value.Length > toRemove.Length)
            {
                if (value.Substring(value.Length - toRemove.Length) == toRemove)
                {
                    result = value.Substring(0, value.Length - toRemove.Length);
                }
            }

            return result;
        }

        public static string RemoveAllWhitespace(this string source)
        {
            return System.Text.RegularExpressions.Regex.Replace(source, @"\s+", string.Empty);
        }

        public static string RemoveNonAlphaNumerics(this string source)
        {
            var result = new StringBuilder(source.Length);

            for (int i = 0; i < source.Length; i++)
            {
                if (char.IsLetterOrDigit(source[i]))
                {
                    result.Append(source[i]);
                }
            }

            return result.ToString();
        }

        public static string Append(this string value, string toAdd)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (toAdd == null)
            {
                throw new ArgumentNullException(nameof(toAdd));
            }

            return $"{value}{toAdd}";
        }

        public static string AsXamlFriendlyTypeArgument(this string value)
        {
            // Replace uppercase versions first to avoid double replacements
            return value.Replace("Boolean", "x:Boolean")
                        .Replace("bool", "x:Boolean")
                        .Replace("Byte", "x:Byte")
                        .Replace("byte", "x:Byte")
                        .Replace("Char", "x:Char")
                        .Replace("char", "x:Char")
                        .Replace("Decimal", "x:Decimal")
                        .Replace("decimal", "x:Decimal")
                        .Replace("Double", "x:Double")
                        .Replace("double", "x:Double")
                        .Replace("Int16", "x:Int16")
                        .Replace("short", "x:Int16")
                        .Replace("Int32", "x:Int32")
                        .Replace("Integer", "x:Int32")
                        .Replace("int", "x:Int32")
                        .Replace("Int64", "x:Int64")
                        .Replace("long", "x:Int64")
                        .Replace("Object", "x:Object")
                        .Replace("object", "x:Object")
                        .Replace("Single", "x:Single")
                        .Replace("single", "x:Single")
                        .Replace("String", "x:String")
                        .Replace("string", "x:String")
                        .Replace("TimeSpan", "x:TimeSpan")
                        .Replace("Uri", "x:Uri");
        }

        public static bool IsOneOf(this string item, params string[] options)
        {
            return options.Contains(item);
        }

        public static string AddSpacesToCamelCase(this string source)
        {
            return System.Text.RegularExpressions.Regex.Replace(source, "([a-z](?=[A-Z]|[0-9])|[A-Z](?=[A-Z][a-z]|[0-9])|[0-9](?=[^0-9]))", "$1 ");
        }

        public static int OccurrenceCount(this string source, string substring)
        {
            return source.Split(new[] { substring }, StringSplitOptions.None).Length - 1;
        }

        public static int FirstIndexOf(this string source, params string[] values)
        {
            var valuePostions = new Dictionary<string, int>();

            foreach (var value in values)
            {
                valuePostions.Add(value, source.IndexOf(value));
            }

            if (valuePostions.Any(v => v.Value > -1))
            {
                var result = valuePostions.Select(v => v.Value).Where(v => v > -1).OrderBy(v => v).FirstOrDefault();

                return result;
            }

            return -1;
        }

        public static int LastIndexOf(this string source, params string[] values)
        {
            var valuePostions = new Dictionary<string, int>();

            foreach (var value in values)
            {
                valuePostions.Add(value, source.LastIndexOf(value));
            }

            if (valuePostions.Any(v => v.Value > -1))
            {
                var result = valuePostions.Select(v => v.Value).Where(v => v > -1).OrderByDescending(v => v).FirstOrDefault();

                return result;
            }

            return -1;
        }

        // TODO: Add better checking of $nooutput$
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
                        // For scenarios like : "<x:String>$elementwithspaces$</x:String>"
                        possibleXaml = possibleXaml.Replace(placeholder, validText);
                        break;
                    case Placeholder.GeneratedXAML:
                        possibleXaml = possibleXaml.Replace(placeholder, "<GeneratedXaml />");
                        break;
                    default:
                        possibleXaml = possibleXaml.Replace(placeholder, "Something");
                        break;
                }
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

        public static bool IsValidXml(this string source)
        {
            try
            {
                var xdoc = new XmlDocument();
                xdoc.LoadXml(source);

                // If loaded OK assume it's valid
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static string FormatXaml(this string source, int indentSize)
        {
            // Use this rather than doc.LoadXml so can ignore namespace aliases
            var xtr = new XmlTextReader(new StringReader(source))
            {
                Namespaces = false,
            };

            var doc = new XmlDocument();

            bool wrapped = false;

            try
            {
                doc.Load(xtr);
            }
            catch (XmlException)
            {
                // Assume failures are due to multiple root elements
                xtr = new XmlTextReader(new StringReader($"<wrap>{source}</wrap>"))
                {
                    Namespaces = false,
                };

                doc.Load(xtr);

                wrapped = true;
            }

            string result;

            using (var sw = new StringWriter())
            {
                using (XmlTextWriter tx = new XmlTextWriter(sw))
                {
                    tx.Indentation = indentSize;
                    tx.IndentChar = ' ';
                    tx.Formatting = Formatting.Indented;

                    doc.WriteTo(tx);
                    string strXmlText = sw.ToString();
                    result = strXmlText;
                }
            }

            if (wrapped)
            {
                // remove the dummy root element, the indentation from the extra root element, and the trailing newline that the wrapping left
                result = result.Replace("<wrap>", string.Empty).Replace("</wrap>", string.Empty).Replace(Environment.NewLine + new string(' ', indentSize), Environment.NewLine).Trim();
            }

            return result;
        }

        public static string GetXamlElement(this string source)
        {
            var result = string.Empty;

            if (source.TrimStart().StartsWith("<")
             && source.TrimStart().Length > 3
             && source.TrimStart().IndexOf(' ') > 0)
            {
                result = source.TrimStart().Substring(1, source.TrimStart().IndexOf(' ') - 1);
            }

            return result;
        }

        public static List<string> GetPlaceholders(this string source)
        {
            var plchldrRgx = new Regex("([$$][\\w]+[$$])");

            var matches = plchldrRgx.Matches(source);

            var result = new List<string>();

            foreach (Match match in matches)
            {
                result.Add(match.Value);
            }

            return result;
        }
    }
}
