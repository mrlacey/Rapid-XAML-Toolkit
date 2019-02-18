// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using Microsoft.VisualStudio.Text;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.XamlAnalysis.Processors
{
    public class EntryProcessor : XamlElementProcessor
    {
        public override void Process(int offset, string xamlElement, string linePadding, ITextSnapshot snapshot, List<IRapidXamlAdornmentTag> tags)
        {
            if (!XamlAnalysisHelpers.HasAttribute(Attributes.Keyboard, xamlElement))
            {
                // TODO: Create tag that indicates adding a Keyboard attribute
                // Get Tag insert point
                // TODO: create action to implement the tag
            }

            // TODO: implement advanced entry processor - suggested keyboard based on other info in the element (e.g. bound property name, header, etc.)
        }
    }
}
