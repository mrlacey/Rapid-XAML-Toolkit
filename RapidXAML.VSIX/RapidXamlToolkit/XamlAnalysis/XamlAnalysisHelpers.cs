// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Text;

namespace RapidXamlToolkit.XamlAnalysis
{
    public static class XamlAnalysisHelpers
    {
        public static bool IsSelfClosing(string xaml, int startPoint = 0)
        {
            var foundSelfCloser = false;

            for (int i = startPoint; i < xaml.Length; i++)
            {
                switch (xaml[i])
                {
                    case '/':
                        foundSelfCloser = true;
                        break;
                    case '>':
                        return foundSelfCloser;
                    default:
                        break;
                }
            }

            // Shouldn't ever get here if passed valid XAML and startPoint is valid
            return false;
        }

        public static bool HasAttribute(string attributeName, string xaml, int startPoint = 0)
        {
            var searchText = $"{attributeName}=\"";

            var tbIndex = xaml.IndexOf(searchText, StringComparison.Ordinal);

            if (tbIndex == -1)
            {
                if (IsSelfClosing(xaml, startPoint))
                {
                    return false;
                }
                else
                {
                    var elementNameEndPos = xaml.Substring(startPoint).FirstIndexOf(" ", ">");
                    var elementName = xaml.Substring(startPoint + 1, elementNameEndPos - 1);

                    return xaml.Contains($"<{elementName}.{attributeName}>");
                }
            }
            else
            {
                return true;
            }
        }
    }
}
