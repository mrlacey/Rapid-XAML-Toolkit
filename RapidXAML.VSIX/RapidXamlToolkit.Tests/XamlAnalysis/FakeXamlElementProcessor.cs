// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using RapidXamlToolkit.Tagging;

namespace RapidXamlToolkit.Tests.XamlAnalysis
{
    // Used to record/track/assert the values passed to a processor during testing
    public class FakeXamlElementProcessor : XamlElementProcessor
    {
        public string XamlElement { get; set; }

        public int Offset { get; set; }

        public void Process(int offset, string xamlElement)
        {
            this.Offset = offset;
            this.XamlElement = xamlElement;
        }
    }
}
