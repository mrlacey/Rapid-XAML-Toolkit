// Copyright (c) Matt Lacey Ltd.. All rights reserved.
// Licensed under the MIT license.

using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidXamlToolkit.RoslynAnalyzers;

namespace RapidXamlToolkit.Tests.RoslynAnalyzers
{
    [TestClass]
    public class PropertyDeclarationSyntaxExtensionsTests
    {
        [TestMethod]
        public void GetTypeName_PredefinedTypeSyntax()
        {
            var code = "public string Property1 { get; set; }";

            this.CheckGetTypeNameOfProperty(code, "string");
        }

        [TestMethod]
        public void GetTypeName_GenericNameSyntax()
        {
            var code = "public List<Int> Property2 { get; set; }";

            this.CheckGetTypeNameOfProperty(code, "List<Int>");
        }

        [TestMethod]
        public void GetTypeName_IdentifierNameSyntax()
        {
            var code = "public MyObject Property3 { get; set; }";

            this.CheckGetTypeNameOfProperty(code, "MyObject");
        }

        [TestMethod]
        public void GetTypeName_QualifiedNameSyntax()
        {
            var code = "public MyClass.MyType Property4 { get; set; }";

            this.CheckGetTypeNameOfProperty(code, "MyType");
        }

        [TestMethod]
        public void GetTypeName_QualifiedNameSyntax_Plus_GenericNameSyntax()
        {
            var code = "public MyClass.MyGenericType<int> Property5 { get; set; }";

            this.CheckGetTypeNameOfProperty(code, "MyGenericType<int>");
        }

        private void CheckGetTypeNameOfProperty(string code, string expectedName)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(code);

            var pds = syntaxTree.GetRoot().DescendantNodes().OfType<PropertyDeclarationSyntax>().First();

            var typeName = pds.GetTypeName();

            Assert.AreEqual(expectedName, typeName);
        }
    }
}
