// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;

namespace RapidXamlToolkit.VisualStudioIntegration
{
    public interface IDocumentModelAccess
    {
        Task<(SyntaxTree syntaxTree, SemanticModel semModel)> GetDocumentModelsAsync(string fileName);

        Task<(SyntaxTree syntaxTree, SemanticModel semModel)> GetDocumentModelsAsync(Document document);
    }
}
