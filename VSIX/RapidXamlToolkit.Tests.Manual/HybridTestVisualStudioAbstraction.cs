// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System.IO;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RapidXamlToolkit.Tests.Manual
{
    public class HybridTestVisualStudioAbstraction : TestVisualStudioAbstraction
    {
        public override async Task<(SyntaxTree syntaxTree, SemanticModel semModel)> GetDocumentModelsAsync(string fileName)
        {
            await Task.CompletedTask;
            var code = File.ReadAllText(fileName);

            var isCSharp = Path.GetExtension(fileName) == ".cs";

            var syntaxTree = isCSharp ? CSharpSyntaxTree.ParseText(code)
                                      : VisualBasicSyntaxTree.ParseText(code);

            Assert.IsNotNull(syntaxTree);

            var semModel = isCSharp ? CSharpCompilation.Create(string.Empty).AddSyntaxTrees(syntaxTree).GetSemanticModel(syntaxTree, ignoreAccessibility: true)
                                    : VisualBasicCompilation.Create(string.Empty).AddSyntaxTrees(syntaxTree).GetSemanticModel(syntaxTree, ignoreAccessibility: true);

            return (syntaxTree, semModel);
        }
    }
}
