// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.CodeAnalysis;
using RapidXamlToolkit.Options;

namespace RapidXamlToolkit.Analyzers
{
    public interface IDocumentAnalyzer
    {
        AnalyzerOutput GetSingleItemOutput(SyntaxNode documentRoot, SemanticModel semModel, int caretPosition, int indent, Profile profileOverload = null);

        AnalyzerOutput GetSelectionOutput(SyntaxNode documentRoot, SemanticModel semModel, int selStart, int selEnd, int indent, Profile profileOverload = null);
    }
}
