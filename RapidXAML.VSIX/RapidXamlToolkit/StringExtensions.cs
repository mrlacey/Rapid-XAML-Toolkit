// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Linq;

namespace RapidXamlToolkit
{
    public static class StringExtensions
    {
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

            return value.Replace("(Of ", "<")
                        .Replace("(of ", "<")
                        .Replace("(OF ", "<")
                        .Replace(")", ">");
        }

        public static bool IsGenericTypeName(this string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            // Know no better way of detecting this
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
            return source.Replace(" ", string.Empty)
                         .Replace("\t", string.Empty)
                         .Replace(Environment.NewLine, string.Empty);
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
    }
}
