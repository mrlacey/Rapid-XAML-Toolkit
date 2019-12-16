// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using Microsoft.CodeAnalysis;

namespace RapidXamlToolkit.Parsers
{
    public interface IDocumentParser
    {
        ParserOutput GetSingleItemOutput(SyntaxNode documentRoot, SemanticModel semModel, int caretPosition);

        ParserOutput GetSelectionOutput(SyntaxNode documentRoot, SemanticModel semModel, int selStart, int selEnd);
    }
}
