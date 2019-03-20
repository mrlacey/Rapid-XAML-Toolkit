// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace RapidXamlToolkit.Parsers
{
    public class AttributeDetails
    {
        public string Name { get; set; }

        public List<AttributeArgumentDetails> Arguments { get; set; } = new List<AttributeArgumentDetails>();
    }
}
