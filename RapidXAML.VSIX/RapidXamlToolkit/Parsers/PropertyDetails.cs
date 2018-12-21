// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.CodeAnalysis;

namespace RapidXamlToolkit.Parsers
{
    public class PropertyDetails
    {
        public string Name { get; set; }

        public string PropertyType { get; set; }

        public bool IsReadOnly { get; set; }

        public ITypeSymbol Symbol { get; set; }
    }
}
