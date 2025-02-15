﻿// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.VisualBasic;
using RapidXamlToolkit.Options;
using RapidXamlToolkit.Utils.IO;
using RapidXamlToolkit.VisualStudioIntegration;

namespace RapidXamlToolkit.Tests.DragDrop
{
    public class DropHandlerTestsBase
    {
        protected Profile GetProfileForTesting()
        {
            var profile = TestProfile.CreateEmpty();
            profile.ClassGrouping = "StackPanel";
            profile.FallbackOutput = "<TextBlock Text=\"$name$\" />";

            return profile;
        }

        protected (IFileSystemAbstraction Fs, IVisualStudioAbstractionAndDocumentModelAccess Vsa) GetCSAbstractions(string fileContents)
        {
            var fs = new TestFileSystem
            {
                FileExistsResponse = true,
                FileText = fileContents,
            };

            var synTree = CSharpSyntaxTree.ParseText(fs.FileText);
            var semModel = CSharpCompilation.Create(string.Empty).AddSyntaxTrees(synTree).GetSemanticModel(synTree, ignoreAccessibility: true);

            var vsa = new TestVisualStudioAbstraction
            {
                SyntaxTree = synTree,
                SemanticModel = semModel,
                XamlIndent = 4,
            };

            return (fs, vsa);
        }

        protected (IFileSystemAbstraction Fs, IVisualStudioAbstractionAndDocumentModelAccess Vsa) GetVbAbstractions(string fileContents)
        {
            var fs = new TestFileSystem
            {
                FileExistsResponse = true,
                FileText = fileContents,
            };

            var synTree = VisualBasicSyntaxTree.ParseText(fs.FileText);
            var semModel = VisualBasicCompilation.Create(string.Empty).AddSyntaxTrees(synTree).GetSemanticModel(synTree, ignoreAccessibility: true);

            var vsa = new TestVisualStudioAbstraction
            {
                SyntaxTree = synTree,
                SemanticModel = semModel,
                XamlIndent = 4,
            };

            return (fs, vsa);
        }
    }
}
