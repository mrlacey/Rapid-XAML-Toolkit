// Copyright (c) Matt Lacey Ltd.. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace RapidXamlToolkit
{
    public static class ITypeSymbolExtensions
    {
        public static IEnumerable<ITypeSymbol> GetSelfAndBaseTypes(this ITypeSymbol type)
        {
            var current = type;
            while (current != null)
            {
                yield return current;
                current = current.BaseType;
            }
        }
    }
}
