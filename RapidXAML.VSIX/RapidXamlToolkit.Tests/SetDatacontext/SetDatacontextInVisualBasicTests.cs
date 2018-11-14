// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidXamlToolkit.Commands;

namespace RapidXamlToolkit.Tests.SetDatacontext
{
    [TestClass]
    public class SetDatacontextInVisualBasicTests
    {
        [TestMethod]
        public void DetectsWhenConstructorContentIsAlreadySet()
        {
            var profile = TestProfile.CreateEmpty();
            profile.Datacontext.CodeBehindConstructorContent = "DataContext = ViewModel";

            var logger = DefaultTestLogger.Create();

            var vs = new TestVisualStudioAbstraction
            {
                ActiveDocumentFileName = "TestPage.xaml.vb",
                ActiveDocumentText = @"Public NotInheritable Class TestPage
    Inherits Page

    Sub New()
        InitializeComponent()

        DataContext = ViewModel
    End Sub
End Class",
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
            profile.Datacontext.CodeBehindConstructorContent = "DataContext = ViewModel";

            var logger = DefaultTestLogger.Create();

            var vs = new TestVisualStudioAbstraction
            {
                ActiveDocumentFileName = "TestPage.xaml.vb",
                ActiveDocumentText = @"Public NotInheritable Class TestPage
    Inherits Page

    Sub New()
        InitializeComponent()

        DataContext  =  ViewModel
    End Sub
End Class",
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
            profile.Datacontext.CodeBehindConstructorContent = "DataContext = ViewModel";

            var logger = DefaultTestLogger.Create();

            var vs = new TestVisualStudioAbstraction
            {
                ActiveDocumentFileName = "TestPage.xaml.vb",
                ActiveDocumentText = @"Public NotInheritable Class TestPage
    Inherits Page

    Sub New()
        InitializeComponent()
    End Sub
End Class",
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
            profile.Datacontext.CodeBehindConstructorContent = "DataContext = ViewModel";

            var logger = DefaultTestLogger.Create();

            var fs = new TestFileSystem
            {
                FileText = @"Public NotInheritable Class TestPage
    Inherits Page

    Sub New()
        InitializeComponent()
    End Sub
End Class",
            };

            var synTree = VisualBasicSyntaxTree.ParseText(fs.FileText);

            var vs = new TestVisualStudioAbstraction
            {
                ActiveDocumentFileName = "TestPage.xaml.vb",
                ActiveDocumentText = fs.FileText,
                SyntaxTree = synTree,
                DocumentIsCSharp = false,
            };

            var sut = new SetDataContextCommandLogic(profile, logger, vs, fs);

            var (anythingToAdd, lineNoToAddAfter, contentToAdd, constructorAdded)
                = sut.GetCodeBehindConstructorContentToAdd(vs.ActiveDocumentText, vs.SyntaxTree.GetRoot(), "TestPage", "TestViewModel");

            Assert.IsTrue(anythingToAdd);
            Assert.AreEqual(5, lineNoToAddAfter);
            Assert.AreEqual($"{Environment.NewLine}{Environment.NewLine}DataContext = ViewModel", contentToAdd);
            Assert.IsFalse(constructorAdded);
        }

        [TestMethod]
        public void CanDetectWhereAndWhenToInsertConstructorContentAndConstructorDoesNotExist()
        {
            var defaultConstructor = "Sub New()"
             + Environment.NewLine + "    InitializeComponent()"
             + Environment.NewLine + "End Sub";

            var profile = TestProfile.CreateEmpty();
            profile.Datacontext.CodeBehindConstructorContent = "DataContext = ViewModel";
            profile.Datacontext.DefaultCodeBehindConstructor = defaultConstructor;

            var logger = DefaultTestLogger.Create();

            var fs = new TestFileSystem
            {
                FileText = @"Public NotInheritable Class TestPage
    Inherits Page

End Class",
            };

            var synTree = VisualBasicSyntaxTree.ParseText(fs.FileText);

            var vs = new TestVisualStudioAbstraction
            {
                ActiveDocumentFileName = "TestPage.xaml.vb",
                ActiveDocumentText = fs.FileText,
                SyntaxTree = synTree,
                DocumentIsCSharp = false,
            };

            var sut = new SetDataContextCommandLogic(profile, logger, vs, fs);

            var (anythingToAdd, lineNoToAddAfter, contentToAdd, constructorAdded)
                = sut.GetCodeBehindConstructorContentToAdd(vs.ActiveDocumentText, vs.SyntaxTree.GetRoot(), "TestPage", "TestViewModel");

            var expectedContent = ""
          + Environment.NewLine + "Sub New()"
          + Environment.NewLine + "    InitializeComponent()"
          + Environment.NewLine + ""
          + Environment.NewLine + "DataContext = ViewModel"
          + Environment.NewLine + "End Sub"
          + Environment.NewLine + "";

            Assert.IsTrue(anythingToAdd);
            Assert.AreEqual(2, lineNoToAddAfter);
            StringAssert.AreEqual(expectedContent, contentToAdd);
            Assert.IsTrue(constructorAdded);
        }

        [TestMethod]
        public void DetectsWhenPageContentIsAlreadySet()
        {
            var profile = TestProfile.CreateEmpty();
            profile.ViewGeneration.XamlFileSuffix = "Page";
            profile.ViewGeneration.ViewModelFileSuffix = "ViewModel";
            profile.Datacontext.CodeBehindPageContent = @"Public ReadOnly Property ViewModel As $viewmodelclass$
    Get
        Return New $viewmodelclass$
    End Get
End Property";

            var logger = DefaultTestLogger.Create();

            var vs = new TestVisualStudioAbstraction
            {
                ActiveDocumentFileName = "TestPage.xaml.vb",
                ActiveDocumentText = @"Public NotInheritable Class TestPage
    Inherits Page

    Sub New()
        InitializeComponent()
    End Sub

    Public ReadOnly Property ViewModel As TestViewModel
        Get
            Return New TestViewModel
        End Get
    End Property
End Class",
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
            profile.Datacontext.CodeBehindPageContent = @"Public ReadOnly Property ViewModel As $viewmodelclass$
    Get
        Return New $viewmodelclass$
    End Get
End Property";

            var logger = DefaultTestLogger.Create();

            var vs = new TestVisualStudioAbstraction
            {
                ActiveDocumentFileName = "TestPage.xaml.vb",
                ActiveDocumentText = @"Public NotInheritable Class TestPage
    Inherits Page

    Sub New()
        InitializeComponent()
    End Sub
End Class",
            };

            var fs = new TestFileSystem();

            var sut = new SetDataContextCommandLogic(profile, logger, vs, fs);

            var result = sut.ShouldEnableCommand();

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void CanDetectWhereAndWhenToInsertPageContentAndConstructorExists()
        {
            var pageContent = "Public ReadOnly Property ViewModel As $viewmodelclass$"
+ Environment.NewLine + "    Get"
+ Environment.NewLine + "        Return New $viewmodelclass$"
+ Environment.NewLine + "    End Get"
+ Environment.NewLine + "End Property";

            var profile = TestProfile.CreateEmpty();
            profile.ViewGeneration.XamlFileSuffix = "Page";
            profile.ViewGeneration.ViewModelFileSuffix = "ViewModel";
            profile.Datacontext.CodeBehindPageContent = pageContent;

            var logger = DefaultTestLogger.Create();

            var fs = new TestFileSystem
            {
                FileText = @"Public NotInheritable Class TestPage
    Inherits Page

    Sub New()
        InitializeComponent()
    End Sub
End Class",
            };

            var synTree = VisualBasicSyntaxTree.ParseText(fs.FileText);

            var vs = new TestVisualStudioAbstraction
            {
                ActiveDocumentFileName = "TestPage.xaml.vb",
                ActiveDocumentText = fs.FileText,
                SyntaxTree = synTree,
                DocumentIsCSharp = false,
            };

            var sut = new SetDataContextCommandLogic(profile, logger, vs, fs);

            var (anythingToAdd, lineNoToAddAfter, contentToAdd)
                = sut.GetCodeBehindPageContentToAdd(vs.ActiveDocumentText, vs.SyntaxTree.GetRoot(), "TestViewModel", "TestVmNamespace");

            var expectedContent = ""
          + Environment.NewLine + ""
          + Environment.NewLine + "Public ReadOnly Property ViewModel As TestViewModel"
          + Environment.NewLine + "    Get"
          + Environment.NewLine + "        Return New TestViewModel"
          + Environment.NewLine + "    End Get"
          + Environment.NewLine + "End Property";

            Assert.IsTrue(anythingToAdd);
            Assert.AreEqual(6, lineNoToAddAfter);
            StringAssert.AreEqual(expectedContent, contentToAdd);
        }

        [TestMethod]
        public void CanDetectWhereAndWhenToInsertPageContentAndConstructorDoesNotExist()
        {
            var pageContent = "Public ReadOnly Property ViewModel As $viewmodelclass$"
      + Environment.NewLine + "    Get"
      + Environment.NewLine + "        Return New $viewmodelclass$"
      + Environment.NewLine + "    End Get"
      + Environment.NewLine + "End Property";

            var profile = TestProfile.CreateEmpty();
            profile.ViewGeneration.XamlFileSuffix = "Page";
            profile.ViewGeneration.ViewModelFileSuffix = "ViewModel";
            profile.Datacontext.CodeBehindPageContent = pageContent;

            var logger = DefaultTestLogger.Create();

            var fs = new TestFileSystem
            {
                FileText = @"Public NotInheritable Class TestPage
    Inherits Page

End Class",
            };

            var synTree = VisualBasicSyntaxTree.ParseText(fs.FileText);

            var vs = new TestVisualStudioAbstraction
            {
                ActiveDocumentFileName = "TestPage.xaml.vb",
                ActiveDocumentText = fs.FileText,
                SyntaxTree = synTree,
                DocumentIsCSharp = false,
            };

            var sut = new SetDataContextCommandLogic(profile, logger, vs, fs);

            var (anythingToAdd, lineNoToAddAfter, contentToAdd)
                = sut.GetCodeBehindPageContentToAdd(vs.ActiveDocumentText, vs.SyntaxTree.GetRoot(), "TestViewModel", "TestVmNamespace");

            var expectedContent = ""
          + Environment.NewLine + ""
          + Environment.NewLine + "Public ReadOnly Property ViewModel As TestViewModel"
          + Environment.NewLine + "    Get"
          + Environment.NewLine + "        Return New TestViewModel"
          + Environment.NewLine + "    End Get"
          + Environment.NewLine + "End Property";

            Assert.IsTrue(anythingToAdd);
            Assert.AreEqual(2, lineNoToAddAfter);
            StringAssert.AreEqual(expectedContent, contentToAdd);
        }

        [TestMethod]
        public void CanDetectWhereAndWhenToInsertConstructorAndPageContentWhenConstructorExists()
        {
            var pageContent = "Public ReadOnly Property ViewModel As $viewmodelclass$"
      + Environment.NewLine + "    Get"
      + Environment.NewLine + "        Return New $viewmodelclass$"
      + Environment.NewLine + "    End Get"
      + Environment.NewLine + "End Property";

            var profile = TestProfile.CreateEmpty();
            profile.ViewGeneration.XamlFileSuffix = "Page";
            profile.ViewGeneration.ViewModelFileSuffix = "ViewModel";
            profile.Datacontext.CodeBehindConstructorContent = "DataContext = ViewModel";
            profile.Datacontext.CodeBehindPageContent = pageContent;

            var logger = DefaultTestLogger.Create();

            var fs = new TestFileSystem
            {
                FileText = @"Public NotInheritable Class TestPage
    Inherits Page

    Sub New()
        InitializeComponent()
    End Sub
End Class",
            };

            var synTree = VisualBasicSyntaxTree.ParseText(fs.FileText);

            var vs = new TestVisualStudioAbstraction
            {
                ActiveDocumentFileName = "TestPage.xaml.vb",
                ActiveDocumentText = fs.FileText,
                SyntaxTree = synTree,
                DocumentIsCSharp = false,
            };

            var sut = new SetDataContextCommandLogic(profile, logger, vs, fs);

            var documentRoot = synTree.GetRoot();

            var result = sut.GetCodeBehindContentToAdd("TestPage", "TestViewModel", "TestVmNamespace", documentRoot);

            Assert.IsTrue(result[0].anythingToAdd);
            Assert.AreEqual(5, result[0].lineNoToAddAfter);
            StringAssert.AreEqual($"{Environment.NewLine}{Environment.NewLine}DataContext = ViewModel", result[0].contentToAdd);

            var expectedContent = ""
          + Environment.NewLine + ""
          + Environment.NewLine + "Public ReadOnly Property ViewModel As TestViewModel"
          + Environment.NewLine + "    Get"
          + Environment.NewLine + "        Return New TestViewModel"
          + Environment.NewLine + "    End Get"
          + Environment.NewLine + "End Property";

            Assert.IsTrue(result[1].anythingToAdd);
            Assert.AreEqual(8, result[1].lineNoToAddAfter);
            StringAssert.AreEqual(expectedContent, result[1].contentToAdd);
        }

        [TestMethod]
        public void CanDetectWhereAndWhenToInsertConstructorAndPageContentWhenConstructorDoesNotExist()
        {
            var defaultConstructor = "Sub New()"
             + Environment.NewLine + "    InitializeComponent()"
             + Environment.NewLine + "End Sub";

            var pageContent = "Public ReadOnly Property ViewModel As $viewmodelclass$"
      + Environment.NewLine + "    Get"
      + Environment.NewLine + "        Return New $viewmodelclass$"
      + Environment.NewLine + "    End Get"
      + Environment.NewLine + "End Property";

            var profile = TestProfile.CreateEmpty();
            profile.ViewGeneration.XamlFileSuffix = "Page";
            profile.ViewGeneration.ViewModelFileSuffix = "ViewModel";
            profile.Datacontext.CodeBehindConstructorContent = "DataContext = ViewModel";
            profile.Datacontext.DefaultCodeBehindConstructor = defaultConstructor;
            profile.Datacontext.CodeBehindPageContent = pageContent;

            var logger = DefaultTestLogger.Create();

            var fs = new TestFileSystem
            {
                FileText = @"Public NotInheritable Class TestPage
    Inherits Page

End Class",
            };

            var synTree = VisualBasicSyntaxTree.ParseText(fs.FileText);

            var vs = new TestVisualStudioAbstraction
            {
                ActiveDocumentFileName = "TestPage.xaml.vb",
                ActiveDocumentText = fs.FileText,
                SyntaxTree = synTree,
                DocumentIsCSharp = false,
            };

            var sut = new SetDataContextCommandLogic(profile, logger, vs, fs);

            var documentRoot = synTree.GetRoot();

            var result = sut.GetCodeBehindContentToAdd("TestPage", "TestViewModel", "TestVmNamespace", documentRoot);

            var expectedContent0 = ""
           + Environment.NewLine + "Sub New()"
           + Environment.NewLine + "    InitializeComponent()"
           + Environment.NewLine + ""
           + Environment.NewLine + "DataContext = ViewModel"
           + Environment.NewLine + "End Sub"
           + Environment.NewLine + "";

            Assert.IsTrue(result[0].anythingToAdd);
            Assert.AreEqual(2, result[0].lineNoToAddAfter);
            StringAssert.AreEqual(expectedContent0, result[0].contentToAdd);

            var expectedContent1 = ""
           + Environment.NewLine + ""
           + Environment.NewLine + "Public ReadOnly Property ViewModel As TestViewModel"
           + Environment.NewLine + "    Get"
           + Environment.NewLine + "        Return New TestViewModel"
           + Environment.NewLine + "    End Get"
           + Environment.NewLine + "End Property";

            Assert.IsTrue(result[1].anythingToAdd);
            Assert.AreEqual(7, result[1].lineNoToAddAfter);
            StringAssert.AreEqual(expectedContent1, result[1].contentToAdd);
        }
    }
}
