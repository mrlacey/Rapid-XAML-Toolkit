// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;

namespace RapidXamlToolkit.XamlAnalysis
{
    public abstract class XamlElementProcessor
    {
        public abstract void Process(int offset, string xamlElement, List<IRapidXamlTag> tags);
    }
}
