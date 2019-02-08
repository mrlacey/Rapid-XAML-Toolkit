// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using RapidXamlToolkit.Suggestions;
using RapidXamlToolkit.Tagging;

namespace RapidXamlToolkit.Tests.XamlAnalysis
{
    // Used to record/track/assert the values passed to a processor during testing
    public class FakeXamlElementProcessor : XamlElementProcessor
    {
        public bool ProcessCalled { get; set; } = false;

        public string XamlElement { get; set; } = string.Empty;

        public int Offset { get; set; } = -1;

        public override void Process(int offset, string xamlElement, List<IRapidXamlTag> tags)
        {
            this.ProcessCalled = true;
            this.Offset = offset;
            this.XamlElement = xamlElement;
        }
    }
}
