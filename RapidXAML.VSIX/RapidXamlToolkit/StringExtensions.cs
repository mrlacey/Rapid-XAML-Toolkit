// <copyright file="StringExtensions.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

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

        public static string ToCSharpFormat(this string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            return value.Replace("(Of ", "<").Replace(")", ">");
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
    }
}
