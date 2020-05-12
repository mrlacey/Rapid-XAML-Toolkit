// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using Microsoft.Language.Xml;
using RapidXaml.CustomAnalysis;

namespace RapidXaml.TestHelpers
{
    /// <summary>
    /// Helper class for testing custom analyzers.
    /// </summary>
    public static class CustomAnalysisTestHelper
    {
        /// <summary>
        /// Convert a string containing a single XAML object into a RapidXamlElement.
        /// </summary>
        /// <param name="xaml">The XAML to convert into a RapidXamlElement.</param>
        /// <returns>An element representing the XAML string.</returns>
        public static RapidXamlElement StringToElement(string xaml)
        {
            if (string.IsNullOrWhiteSpace(xaml)
             || !xaml.TrimStart().StartsWith("<")
             || !xaml.TrimEnd().EndsWith(">")
             || !IsValidXml(xaml))
            {
                throw new ArgumentException(Resources.InvalidXamlInputMessage, nameof(xaml));
            }

            try
            {
                return RapidXamlToolkit.XamlAnalysis.RapidXamlElementExtractor.GetElement(xaml);
            }
            catch (Exception exc)
            {
                // It would be really good to know about these exceptions but can't log them ourselves :(
                System.Diagnostics.Debug.WriteLine(exc);
                throw;
            }
        }

        private static bool IsValidXml(string input)
        {
            try
            {
                var parsed = Parser.ParseText(input);

                if (parsed.Kind == SyntaxKind.XmlDocument
                    && !(parsed.Body is XmlTextSyntax))
                {
                    return true;
                }
            }
            catch (Exception exc)
            {
                System.Diagnostics.Debug.WriteLine(exc);
            }

            return false;
        }
    }
}
