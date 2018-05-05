// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RapidXamlToolkit.Tests.SetDatacontext
{
    [TestClass]
    public class SetDatacontextInCSharpTests
    {
        // TODO: TEST - C# add to CB in ctor and outside ctor
        // TODO: TEST - C# add to CB in ctor and outside ctor when ctor does not exist
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

            var fs = new TestFileSystem { };

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

            var fs = new TestFileSystem { };

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

            var fs = new TestFileSystem { };

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

            var result = sut.GetCodeBehindConstructorContentToAdd(vs.ActiveDocumentText, vs.SyntaxTree.GetRoot(), "TestPage", "TestViewModel");

            Assert.IsTrue(result.anythingToAdd);
            Assert.AreEqual(5, result.lineNoToAddAfter);
            Assert.AreEqual($"{Environment.NewLine}{Environment.NewLine}this.DataContext = this.ViewModel;", result.contentToAdd);
            Assert.IsFalse(result.constructorAdded);
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

            var result = sut.GetCodeBehindConstructorContentToAdd(vs.ActiveDocumentText, vs.SyntaxTree.GetRoot(), "TestPage", "TestViewModel");

            var expectedContent = @"
public TestPage()
{
    this.Initialize();

this.DataContext = this.ViewModel;
}
";

            Assert.IsTrue(result.anythingToAdd);
            Assert.AreEqual(2, result.lineNoToAddAfter);
            Assert.AreEqual(expectedContent, result.contentToAdd);
            Assert.IsTrue(result.constructorAdded);
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

            var fs = new TestFileSystem { };

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

            var fs = new TestFileSystem { };

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

            var result = sut.GetCodeBehindPageContentToAdd(vs.ActiveDocumentText, vs.SyntaxTree.GetRoot(), "TestViewModel");

            var expectedContent = @"

public TestViewModel ViewModel
{
    get
    {
        return new TestViewModel();
    }
}";

            Assert.IsTrue(result.anythingToAdd);
            Assert.AreEqual(6, result.lineNoToAddAfter);
            Assert.AreEqual(expectedContent, result.contentToAdd);
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

            var result = sut.GetCodeBehindPageContentToAdd(vs.ActiveDocumentText, vs.SyntaxTree.GetRoot(), "TestViewModel");

            var expectedContent = @"

public TestViewModel ViewModel
{
    get
    {
        return new TestViewModel();
    }
}";
            Assert.IsTrue(result.anythingToAdd);
            Assert.AreEqual(2, result.lineNoToAddAfter);
            Assert.AreEqual(expectedContent, result.contentToAdd);
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

            var documentRoot = CSharpSyntaxTree.ParseText(fs.FileText).GetRoot();

            var result = sut.GetCodeBehindContentToAdd("TestPage", "TestViewModel", documentRoot);

            Assert.IsTrue(result[0].anythingToAdd);
            Assert.AreEqual(5, result[0].lineNoToAddAfter);
            Assert.AreEqual($"{Environment.NewLine}{Environment.NewLine}this.DataContext = this.ViewModel;", result[0].contentToAdd);

            var expectedContent = @"

public TestViewModel ViewModel
{
    get
    {
        return new TestViewModel();
    }
}";

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

            var documentRoot = CSharpSyntaxTree.ParseText(fs.FileText).GetRoot();

            var result = sut.GetCodeBehindContentToAdd("TestPage", "TestViewModel", documentRoot);

            var expectedContent0 = @"
public TestPage()
{
    this.Initialize();

this.DataContext = this.ViewModel;
}
";

            Assert.IsTrue(result[0].anythingToAdd);
            Assert.AreEqual(2, result[0].lineNoToAddAfter);
            Assert.AreEqual(expectedContent0, result[0].contentToAdd);

            var expectedContent1 = @"

public TestViewModel ViewModel
{
    get
    {
        return new TestViewModel();
    }
}";
            Assert.IsTrue(result[1].anythingToAdd);
            Assert.AreEqual(8, result[1].lineNoToAddAfter);
            Assert.AreEqual(expectedContent1, result[1].contentToAdd);
        }
    }
}
