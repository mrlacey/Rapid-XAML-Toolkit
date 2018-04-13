// <copyright file="PropertyDetails.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using Microsoft.CodeAnalysis;

namespace RapidXamlToolkit
{
    public class PropertyDetails
    {
        public string Name { get; set; }

        public string PropertyType { get; set; }

        public bool IsReadOnly { get; set; }

        public ITypeSymbol Symbol { get; set; }
    }
}
