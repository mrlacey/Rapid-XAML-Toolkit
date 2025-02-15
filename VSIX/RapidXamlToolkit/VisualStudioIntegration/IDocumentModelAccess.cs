// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;

namespace RapidXamlToolkit.VisualStudioIntegration
{
    public interface IDocumentModelAccess
    {
        Task<(SyntaxTree SyntaxTree, SemanticModel SemModel)> GetDocumentModelsAsync(string fileName);

        Task<(SyntaxTree SyntaxTree, SemanticModel SemModel)> GetDocumentModelsAsync(Document document);
    }
}
