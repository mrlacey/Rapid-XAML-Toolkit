// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using Microsoft.Language.Xml;

namespace RapidXaml
{
    public static class CustomAnalysisTestHelper
    {
        public static RapidXamlElement StringToElement(string xaml)
        {
            if (string.IsNullOrWhiteSpace(xaml)
             || !xaml.TrimStart().StartsWith("<")
             || !xaml.TrimEnd().EndsWith(">")
             || !IsValidXml(xaml))
            {
                // TODO: Localize this response
                throw new ArgumentException("Input must be a valid XAML string.", nameof(xaml));
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
