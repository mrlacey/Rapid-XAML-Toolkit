// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RapidXamlToolkit.RoslynAnalyzers
{
    public static class PropertyDeclarationSyntaxExtensions
    {
        public static string GetTypeName(this PropertyDeclarationSyntax source)
        {
            switch (source.Type)
            {
                case GenericNameSyntax gns:
                    return gns.ToString(); // Lazy way to get generic types

                case PredefinedTypeSyntax pds:
                    return pds.Keyword.ValueText;

                case IdentifierNameSyntax ins:
                    return ins.Identifier.ValueText;

                case QualifiedNameSyntax qns:
                    var text = qns.Right.Identifier.ValueText;

                    if (qns.Right is GenericNameSyntax qgns)
                    {
                        text += qgns.TypeArgumentList.ToString();
                    }

                    return text;
            }

            return null;
        }
    }
}
