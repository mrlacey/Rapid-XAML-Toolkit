// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.VisualStudio.Text;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.XamlAnalysis
{
    public abstract class XamlElementProcessor
    {
        public abstract void Process(int offset, string xamlElement, ITextSnapshot snapshot, List<IRapidXamlAdornmentTag> tags);

        protected static bool TryGetAttribute(string xaml, string attributeName, out int index, out int length, out string value)
        {
            var searchText = $"{attributeName}=\"";

            var tbIndex = xaml.IndexOf(searchText, StringComparison.Ordinal);

            if (tbIndex >= 0)
            {
                var tbEnd = xaml.IndexOf("\"", tbIndex + searchText.Length, StringComparison.Ordinal);

                index = tbIndex;
                length = tbEnd - tbIndex + 1;
                value = xaml.Substring(tbIndex + searchText.Length, tbEnd - tbIndex - searchText.Length);
                return true;
            }
            else
            {
                index = -1;
                length = 0;
                value = string.Empty;
                return false;
            }
        }
    }
}
