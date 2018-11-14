// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidXamlToolkit.Commands;

namespace RapidXamlToolkit.Tests.SetDatacontext
{
    [TestClass]
    public class SetDatacontextInCSharpTests
    {
        [TestMethod]
        public void DetectsWhenConstructorContentIsAlreadySet()
        {
            var profile = TestProfile.CreateEmpty();
            profile.Datacontext.CodeBehindConstructorContent = "this.DataContext = this.ViewModel;";

            var logger = DefaultTestLogger.Create();

            var vs = new TestVisualStudioAbstraction
            {
                ActiveDocumentFileName = "test.xaml.cs",
                ActiveDocumentText = @"class TestPage
{
    public TestPage()
    {
        this.Initialize();
        this.DataContext = this.ViewModel;
    }
}",
            };

            var fs = new TestFileSystem();

            var sut = new SetDataContextCommandLogic(profile, logger, vs, fs);

            var result = sut.ShouldEnableCommand();

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void DetectsWhenConstructorContentIsAlreadySetAndIgnoresWhiteSpace()
        {
            var profile = TestProfile.CreateEmpty();
            profile.Datacontext.CodeBehindConstructorContent = "this.DataContext = this.ViewModel;";

            var logger = DefaultTestLogger.Create();

            var vs = new TestVisualStudioAbstraction
            {
                ActiveDocumentFileName = "test.xaml.cs",
                ActiveDocumentText = @"class TestPage
{
    public TestPage()
    {
        this.Initialize();
        this.DataContext  =  this.ViewModel ;
    }
}",
            };

            var fs = new TestFileSystem();

            var sut = new SetDataContextCommandLogic(profile, logger, vs, fs);

            var result = sut.ShouldEnableCommand();

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void DetectsWhenConstructorContentIsNotAlreadySet()
        {
            var profile = TestProfile.CreateEmpty();
            profile.Datacontext.CodeBehindConstructorContent = "this.DataContext = this.ViewModel;";

            var logger = DefaultTestLogger.Create();

            var vs = new TestVisualStudioAbstraction
            {
                ActiveDocumentFileName = "test.xaml.cs",
                ActiveDocumentText = @"class TestPage
{
    public TestPage()
    {
        this.Initialize();
    }
}",
            };

            var fs = new TestFileSystem();

            var sut = new SetDataContextCommandLogic(profile, logger, vs, fs);

            var result = sut.ShouldEnableCommand();

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void CanDetectWhereAndWhenToInsertConstructorContentAndConstructorExists()
        {
            var profile = TestProfile.CreateEmpty();
            profile.Datacontext.CodeBehindConstructorContent = "this.DataContext = this.ViewModel;";

            var logger = DefaultTestLogger.Create();

            var fs = new TestFileSystem
            {
                FileText = @"class TestPage
{
    public TestPage()
    {
        this.Initialize();
    }
}",
            };

            var synTree = CSharpSyntaxTree.ParseText(fs.FileText);

            var vs = new TestVisualStudioAbstraction
            {
                ActiveDocumentFileName = "test.xaml.cs",
                ActiveDocumentText = fs.FileText,
                SyntaxTree = synTree,
                DocumentIsCSharp = true,
            };

            var sut = new SetDataContextCommandLogic(profile, logger, vs, fs);

            var (anythingToAdd, lineNoToAddAfter, contentToAdd, constructorAdded)
                = sut.GetCodeBehindConstructorContentToAdd(vs.ActiveDocumentText, vs.SyntaxTree.GetRoot(), "TestPage", "TestViewModel");

            Assert.IsTrue(anythingToAdd);
            Assert.AreEqual(5, lineNoToAddAfter);
            Assert.AreEqual($"{Environment.NewLine}{Environment.NewLine}this.DataContext = this.ViewModel;", contentToAdd);
            Assert.IsFalse(constructorAdded);
        }

        [TestMethod]
        public void CanDetectWhereAndWhenToInsertConstructorContentAndConstructorDoesNotExist()
        {
            var profile = TestProfile.CreateEmpty();
            profile.Datacontext.CodeBehindConstructorContent = "this.DataContext = this.ViewModel;";
            profile.Datacontext.DefaultCodeBehindConstructor = @"public $viewclass$()
{
    this.Initialize();
}";

            var logger = DefaultTestLogger.Create();

            var fs = new TestFileSystem
            {
                FileText = @"class TestPage
{
}",
            };

            var synTree = CSharpSyntaxTree.ParseText(fs.FileText);

            var vs = new TestVisualStudioAbstraction
            {
                ActiveDocumentFileName = "test.xaml.cs",
                ActiveDocumentText = fs.FileText,
                SyntaxTree = synTree,
                DocumentIsCSharp = true,
            };

            var sut = new SetDataContextCommandLogic(profile, logger, vs, fs);

            var (anythingToAdd, lineNoToAddAfter, contentToAdd, constructorAdded)
                = sut.GetCodeBehindConstructorContentToAdd(vs.ActiveDocumentText, vs.SyntaxTree.GetRoot(), "TestPage", "TestViewModel");

            var expectedContent = ""
          + Environment.NewLine + "public TestPage()"
          + Environment.NewLine + "{"
          + Environment.NewLine + "    this.Initialize();"
          + Environment.NewLine + ""
          + Environment.NewLine + "this.DataContext = this.ViewModel;"
          + Environment.NewLine + "}"
          + Environment.NewLine + "";

            Assert.IsTrue(anythingToAdd);
            Assert.AreEqual(2, lineNoToAddAfter);
            Assert.AreEqual(expectedContent, contentToAdd);
            Assert.IsTrue(constructorAdded);
        }

        [TestMethod]
        public void DetectsWhenPageContentIsAlreadySet()
        {
            var profile = TestProfile.CreateEmpty();
            profile.ViewGeneration.XamlFileSuffix = "Page";
            profile.ViewGeneration.ViewModelFileSuffix = "ViewModel";
            profile.Datacontext.CodeBehindPageContent = @"public $viewmodelclass$ ViewModel
{
    get
    {
        return new $viewmodelclass$();
    }
}";

            var logger = DefaultTestLogger.Create();

            var vs = new TestVisualStudioAbstraction
            {
                ActiveDocumentFileName = "TestPage.xaml.cs",
                ActiveDocumentText = @"class TestPage
{
    public TestPage()
    {
        this.Initialize();
    }

    public TestViewModel ViewModel
    {
        get
        {
            return new TestViewModel();
        }
    }
}",
            };

            var fs = new TestFileSystem();

            var sut = new SetDataContextCommandLogic(profile, logger, vs, fs);

            var result = sut.ShouldEnableCommand();

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void DetectsWhenPageContentIsNotAlreadySet()
        {
            var profile = TestProfile.CreateEmpty();
            profile.ViewGeneration.XamlFileSuffix = "Page";
            profile.ViewGeneration.ViewModelFileSuffix = "ViewModel";
            profile.Datacontext.CodeBehindPageContent = @"public $viewmodelclass$ ViewModel
{
    get
    {
        return new $viewmodelclass$();
    }
}";

            var logger = DefaultTestLogger.Create();

            var vs = new TestVisualStudioAbstraction
            {
                ActiveDocumentFileName = "Testpage.xaml.cs",
                ActiveDocumentText = @"class TestPage
{
    public TestPage()
    {
        this.Initialize();
    }
}",
            };

            var fs = new TestFileSystem();

            var sut = new SetDataContextCommandLogic(profile, logger, vs, fs);

            var result = sut.ShouldEnableCommand();

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void CanDetectWhereAndWhenToInsertPageContentAndConstructorExists()
        {
            var profile = TestProfile.CreateEmpty();
            profile.ViewGeneration.XamlFileSuffix = "Page";
            profile.ViewGeneration.ViewModelFileSuffix = "ViewModel";
            profile.Datacontext.CodeBehindPageContent = @"public $viewmodelclass$ ViewModel
{
    get
    {
        return new $viewmodelclass$();
    }
}";

            var logger = DefaultTestLogger.Create();

            var fs = new TestFileSystem
            {
                FileText = @"class TestPage
{
    public TestPage()
    {
        this.Initialize();
    }
}",
            };

            var synTree = CSharpSyntaxTree.ParseText(fs.FileText);

            var vs = new TestVisualStudioAbstraction
            {
                ActiveDocumentFileName = "TestPage.xaml.cs",
                ActiveDocumentText = fs.FileText,
                SyntaxTree = synTree,
                DocumentIsCSharp = true,
            };

            var sut = new SetDataContextCommandLogic(profile, logger, vs, fs);

            var (anythingToAdd, lineNoToAddAfter, contentToAdd)
                = sut.GetCodeBehindPageContentToAdd(vs.ActiveDocumentText, vs.SyntaxTree.GetRoot(), "TestViewModel", "TestVmNamespace");

            var expectedContent = ""
          + Environment.NewLine + ""
          + Environment.NewLine + "public TestViewModel ViewModel"
          + Environment.NewLine + "{"
          + Environment.NewLine + "    get"
          + Environment.NewLine + "    {"
          + Environment.NewLine + "        return new TestViewModel();"
          + Environment.NewLine + "    }"
          + Environment.NewLine + "}";

            Assert.IsTrue(anythingToAdd);
            Assert.AreEqual(6, lineNoToAddAfter);
            Assert.AreEqual(expectedContent, contentToAdd);
        }

        [TestMethod]
        public void CanDetectWhereAndWhenToInsertPageContentAndConstructorDoesNotExist()
        {
            var profile = TestProfile.CreateEmpty();
            profile.ViewGeneration.XamlFileSuffix = "Page";
            profile.ViewGeneration.ViewModelFileSuffix = "ViewModel";
            profile.Datacontext.CodeBehindPageContent = @"public $viewmodelclass$ ViewModel
{
    get
    {
        return new $viewmodelclass$();
    }
}";

            var logger = DefaultTestLogger.Create();

            var fs = new TestFileSystem
            {
                FileText = @"class TestPage
{
}",
            };

            var synTree = CSharpSyntaxTree.ParseText(fs.FileText);

            var vs = new TestVisualStudioAbstraction
            {
                ActiveDocumentFileName = "TestPage.xaml.cs",
                ActiveDocumentText = fs.FileText,
                SyntaxTree = synTree,
                DocumentIsCSharp = true,
            };

            var sut = new SetDataContextCommandLogic(profile, logger, vs, fs);

            var (anythingToAdd, lineNoToAddAfter, contentToAdd)
                = sut.GetCodeBehindPageContentToAdd(vs.ActiveDocumentText, vs.SyntaxTree.GetRoot(), "TestViewModel", "TestVmNamespace");

            var expectedContent = ""
          + Environment.NewLine + ""
          + Environment.NewLine + "public TestViewModel ViewModel"
          + Environment.NewLine + "{"
          + Environment.NewLine + "    get"
          + Environment.NewLine + "    {"
          + Environment.NewLine + "        return new TestViewModel();"
          + Environment.NewLine + "    }"
          + Environment.NewLine + "}";

            Assert.IsTrue(anythingToAdd);
            Assert.AreEqual(2, lineNoToAddAfter);
            Assert.AreEqual(expectedContent, contentToAdd);
        }

        [TestMethod]
        public void CanDetectWhereAndWhenToInsertConstructorAndPageContentWhenConstructorExists()
        {
            var profile = TestProfile.CreateEmpty();
            profile.ViewGeneration.XamlFileSuffix = "Page";
            profile.ViewGeneration.ViewModelFileSuffix = "ViewModel";
            profile.Datacontext.CodeBehindConstructorContent = "this.DataContext = this.ViewModel;";
            profile.Datacontext.CodeBehindPageContent = @"public $viewmodelclass$ ViewModel
{
    get
    {
        return new $viewmodelclass$();
    }
}";

            var logger = DefaultTestLogger.Create();

            var fs = new TestFileSystem
            {
                FileText = @"class TestPage
{
    public TestPage()
    {
        this.Initialize();
    }
}",
            };

            var synTree = CSharpSyntaxTree.ParseText(fs.FileText);

            var vs = new TestVisualStudioAbstraction
            {
                ActiveDocumentFileName = "TestPage.xaml.cs",
                ActiveDocumentText = fs.FileText,
                SyntaxTree = synTree,
                DocumentIsCSharp = true,
            };

            var sut = new SetDataContextCommandLogic(profile, logger, vs, fs);

            var documentRoot = synTree.GetRoot();

            var result = sut.GetCodeBehindContentToAdd("TestPage", "TestViewModel", "TestVmNamespace", documentRoot);

            Assert.IsTrue(result[0].anythingToAdd);
            Assert.AreEqual(5, result[0].lineNoToAddAfter);
            Assert.AreEqual($"{Environment.NewLine}{Environment.NewLine}this.DataContext = this.ViewModel;", result[0].contentToAdd);

            var expectedContent = ""
          + Environment.NewLine + ""
          + Environment.NewLine + "public TestViewModel ViewModel"
          + Environment.NewLine + "{"
          + Environment.NewLine + "    get"
          + Environment.NewLine + "    {"
          + Environment.NewLine + "        return new TestViewModel();"
          + Environment.NewLine + "    }"
          + Environment.NewLine + "}";

            Assert.IsTrue(result[1].anythingToAdd);
            Assert.AreEqual(8, result[1].lineNoToAddAfter);
            Assert.AreEqual(expectedContent, result[1].contentToAdd);
        }

        [TestMethod]
        public void CanDetectWhereAndWhenToInsertConstructorAndPageContentWhenConstructorDoesNotExist()
        {
            var profile = TestProfile.CreateEmpty();
            profile.ViewGeneration.XamlFileSuffix = "Page";
            profile.ViewGeneration.ViewModelFileSuffix = "ViewModel";
            profile.Datacontext.CodeBehindConstructorContent = "this.DataContext = this.ViewModel;";
            profile.Datacontext.DefaultCodeBehindConstructor = @"public $viewclass$()
{
    this.Initialize();
}";
            profile.Datacontext.CodeBehindPageContent = @"public $viewmodelclass$ ViewModel
{
    get
    {
        return new $viewmodelclass$();
    }
}";

            var logger = DefaultTestLogger.Create();

            var fs = new TestFileSystem
            {
                FileText = @"class TestPage
{
}",
            };

            var synTree = CSharpSyntaxTree.ParseText(fs.FileText);

            var vs = new TestVisualStudioAbstraction
            {
                ActiveDocumentFileName = "TestPage.xaml.cs",
                ActiveDocumentText = fs.FileText,
                SyntaxTree = synTree,
                DocumentIsCSharp = true,
            };

            var sut = new SetDataContextCommandLogic(profile, logger, vs, fs);

            var documentRoot = synTree.GetRoot();

            var result = sut.GetCodeBehindContentToAdd("TestPage", "TestViewModel", "TestVmNamespace", documentRoot);

            var expectedContent0 = ""
           + Environment.NewLine + "public TestPage()"
           + Environment.NewLine + "{"
           + Environment.NewLine + "    this.Initialize();"
           + Environment.NewLine + ""
           + Environment.NewLine + "this.DataContext = this.ViewModel;"
           + Environment.NewLine + "}"
           + Environment.NewLine + "";

            Assert.IsTrue(result[0].anythingToAdd);
            Assert.AreEqual(2, result[0].lineNoToAddAfter);
            Assert.AreEqual(expectedContent0, result[0].contentToAdd);

            var expectedContent1 = ""
           + Environment.NewLine + ""
           + Environment.NewLine + "public TestViewModel ViewModel"
           + Environment.NewLine + "{"
           + Environment.NewLine + "    get"
           + Environment.NewLine + "    {"
           + Environment.NewLine + "        return new TestViewModel();"
           + Environment.NewLine + "    }"
           + Environment.NewLine + "}";

            Assert.IsTrue(result[1].anythingToAdd);
            Assert.AreEqual(8, result[1].lineNoToAddAfter);
            Assert.AreEqual(expectedContent1, result[1].contentToAdd);
        }
    }
}
