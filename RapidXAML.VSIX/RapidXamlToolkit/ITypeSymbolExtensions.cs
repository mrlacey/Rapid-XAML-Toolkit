// <copyright file="ITypeSymbolExtensions.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace RapidXamlToolkit
{
    public static class ITypeSymbolExtensions
    {
        public static IEnumerable<ITypeSymbol> GetBaseTypes(this ITypeSymbol type)
        {
            var current = type.BaseType;
            while (current != null)
            {
                yield return current;
                current = current.BaseType;
            }
        }
    }
}
