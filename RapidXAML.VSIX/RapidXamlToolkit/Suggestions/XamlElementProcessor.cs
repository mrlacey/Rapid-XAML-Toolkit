// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using RapidXamlToolkit.Suggestions;

namespace RapidXamlToolkit.Tagging
{
    public abstract class XamlElementProcessor
    {
        public abstract void Process(int offset, string xamlElement, List<IRapidXamlTag> tags);
    }
}
