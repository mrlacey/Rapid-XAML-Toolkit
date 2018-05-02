// <copyright file="IVisualStudioAbstraction.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using Microsoft.CodeAnalysis;

namespace RapidXamlToolkit
{
    public interface IVisualStudioAbstraction
    {
        bool UserConfirms(string title, string message);

        EnvDTE.Project GetActiveProject();

        EnvDTE.Project GetProject(string projectName);

        (SyntaxTree syntaxTree, SemanticModel semModel) GetDocumentModels(string fileName);
    }
}
