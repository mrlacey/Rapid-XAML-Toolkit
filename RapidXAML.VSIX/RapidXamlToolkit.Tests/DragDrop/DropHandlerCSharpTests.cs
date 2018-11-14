// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidXamlToolkit.Commands;
using RapidXamlToolkit.DragDrop;

namespace RapidXamlToolkit.Tests.DragDrop
{
    [TestClass]
    public class DropHandlerCSharpTests : DropHandlerTestsBase
    {
        [TestMethod]
        public async Task FileDoesNotContainClass()
        {
            var profile = this.GetProfileForTesting();

            var fileContents = " // Just a comment";

            (IFileSystemAbstraction fs, IVisualStudioAbstraction vsa) = this.GetAbstractions(fileContents);

            var sut = new DropHandlerLogic(profile, DefaultTestLogger.Create(), vsa, fs);

            var actual = await sut.ExecuteAsync("C:\\Tests\\SomeFile.cs", 8);

            string expected = null;

            StringAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public async Task FileContainsClassButNoProperties()
        {
            var profile = this.GetProfileForTesting();

            var fileContents = "public class TestViewModel"
       + Environment.NewLine + "{"
       + Environment.NewLine + "    private string FirstProperty { get; set; }"
       + Environment.NewLine + "    private string SecondProperty { get; set; }"
       + Environment.NewLine + "}";

            (IFileSystemAbstraction fs, IVisualStudioAbstraction vsa) = this.GetAbstractions(fileContents);

            var sut = new DropHandlerLogic(profile, DefaultTestLogger.Create(), vsa, fs);

            var actual = await sut.ExecuteAsync("C:\\Tests\\SomeFile.cs", 8);

            var expected = "<StackPanel>"
   + Environment.NewLine + "            <!-- No accessible properties when copying as XAML -->"
   + Environment.NewLine + "        </StackPanel>";

            StringAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public async Task FileContainsClassAndPublicProperties()
        {
            var profile = this.GetProfileForTesting();

            var fileContents = "public class TestViewModel"
       + Environment.NewLine + "{"
       + Environment.NewLine + "    public string FirstProperty { get; set; }"
       + Environment.NewLine + "    public string SecondProperty { get; set; }"
       + Environment.NewLine + "}";

            (IFileSystemAbstraction fs, IVisualStudioAbstraction vsa) = this.GetAbstractions(fileContents);

            var sut = new DropHandlerLogic(profile, DefaultTestLogger.Create(), vsa, fs);

            var actual = await sut.ExecuteAsync("C:\\Tests\\SomeFile.cs", 8);

            var expected = "<StackPanel>"
   + Environment.NewLine + "            <TextBlock Text=\"FirstProperty\" />"
   + Environment.NewLine + "            <TextBlock Text=\"SecondProperty\" />"
   + Environment.NewLine + "        </StackPanel>";

            StringAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public async Task FileContainsClassAndPublicPropertiesButNoClassGrouping()
        {
            var profile = this.GetProfileForTesting();
            profile.ClassGrouping = string.Empty;

            var fileContents = "public class TestViewModel"
       + Environment.NewLine + "{"
       + Environment.NewLine + "    public string FirstProperty { get; set; }"
       + Environment.NewLine + "    public string SecondProperty { get; set; }"
       + Environment.NewLine + "}";

            (IFileSystemAbstraction fs, IVisualStudioAbstraction vsa) = this.GetAbstractions(fileContents);

            var sut = new DropHandlerLogic(profile, DefaultTestLogger.Create(), vsa, fs);

            var actual = await sut.ExecuteAsync("C:\\Tests\\SomeFile.cs", 8);

            var expected = "<TextBlock Text=\"FirstProperty\" />"
   + Environment.NewLine + "        <TextBlock Text=\"SecondProperty\" />";

            StringAssert.AreEqual(expected, actual);
        }

        private (IFileSystemAbstraction fs, IVisualStudioAbstraction vsa) GetAbstractions(string fileContents)
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
    }
}
