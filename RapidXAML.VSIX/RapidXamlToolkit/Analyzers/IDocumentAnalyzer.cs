// <copyright file="IDocumentAnalyzer.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace RapidXamlToolkit
{
    public interface IDocumentAnalyzer
    {
        AnalyzerOutput GetSingleItemOutput(SyntaxNode documentRoot, SemanticModel semModel, int caretPosition, Profile profileOverload = null, Dictionary<string, string> referenceLibs = null);

        AnalyzerOutput GetSelectionOutput(SyntaxNode documentRoot, SemanticModel semModel, int selStart, int selEnd, Profile profileOverload = null, Dictionary<string, string> referenceLibs = null);
    }
}
