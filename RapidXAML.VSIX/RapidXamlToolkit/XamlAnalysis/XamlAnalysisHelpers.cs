// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

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

        // TODO: Add Tests for XAH.HasAttribute
        public static bool HasAttribute(string attributeName)
        {
            // TODO: implement HasAttribute
            return true;
        }
    }
}
