// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidXamlToolkit.Commands;
using RapidXamlToolkit.Options;

namespace RapidXamlToolkit.Tests.CreateViews
{
    [TestClass]
    public class CreateViewVisualBasicTests
    {
        [TestMethod]
        public async Task CorrectOutputInSameFolderAsync()
        {
            var profile = this.GetDefaultTestProfile();

            profile.ViewGeneration.AllInSameProject = true;
            profile.ViewGeneration.ViewModelDirectoryName = "Files";
            profile.ViewGeneration.ViewModelFileSuffix = "ViewModel";
            profile.ViewGeneration.XamlFileDirectoryName = "Files";
            profile.ViewGeneration.XamlFileSuffix = "Page";

            var fs = new TestFileSystem
            {
                FileExistsResponse = false,
                FileText = @"Public Class TestViewModel
    Public Property OnlyProperty As String
End Class",
            };

            var synTree = VisualBasicSyntaxTree.ParseText(fs.FileText);
            var semModel = VisualBasicCompilation.Create(string.Empty).AddSyntaxTrees(synTree).GetSemanticModel(synTree, ignoreAccessibility: true);

            var vsa = new TestVisualStudioAbstraction
            {
                SyntaxTree = synTree,
                SemanticModel = semModel,
                ActiveProject = new ProjectWrapper() { Name = "App", FileName = @"C:\Test\App\App.vbproj" },
            };

            var sut = new CreateViewCommandLogic(DefaultTestLogger.Create(), vsa, fs, profile);

            await sut.ExecuteAsync(@"C:\Test\App\Files\TestViewModel.vb");

            var expectedXaml = "<Page"
       + Environment.NewLine + "    x:Class=\"App.Files.TestPage\">"
       + Environment.NewLine + "    <Grid>"
       + Environment.NewLine + "        <StackPanel>"
       + Environment.NewLine + "            <TextBlock FB=\"True\" Text=\"OnlyProperty\" />"
       + Environment.NewLine + "        </StackPanel>"
       + Environment.NewLine + "    </Grid>"
       + Environment.NewLine + "</Page>"
       + Environment.NewLine + "";

            var expectedCodeBehind = "Imports System"
             + Environment.NewLine + "Imports Windows.UI.Xaml.Controls"
             + Environment.NewLine + "Imports App.Files"
             + Environment.NewLine + ""
             + Environment.NewLine + "Namespace Files"
             + Environment.NewLine + ""
             + Environment.NewLine + "    Public NotInheritable Partial Class TestPage"
             + Environment.NewLine + "        Inherits Page"
             + Environment.NewLine + ""
             + Environment.NewLine + "        Public Property ViewModel As TestViewModel"
             + Environment.NewLine + ""
             + Environment.NewLine + "        Public Sub New()"
             + Environment.NewLine + "            Me.InitializeComponent()"
             + Environment.NewLine + "            Me.ViewModel = New TestViewModel()"
             + Environment.NewLine + "        End Sub"
             + Environment.NewLine + "    End Class"
             + Environment.NewLine + "End Namespace"
             + Environment.NewLine + "";

            Assert.IsTrue(sut.CreateView);
            Assert.AreEqual(@"C:\Test\App\Files\TestPage.xaml", sut.XamlFileName);
            Assert.AreEqual(@"C:\Test\App\Files\TestPage.xaml.vb", sut.CodeFileName);
            StringAssert.AreEqual(expectedXaml, sut.XamlFileContents);
            StringAssert.AreEqual(expectedCodeBehind, sut.CodeFileContents);
        }

        [TestMethod]
        public async Task FileJustContainsComments()
        {
            var profile = this.GetDefaultTestProfile();

            profile.ViewGeneration.AllInSameProject = true;
            profile.ViewGeneration.ViewModelDirectoryName = "Files";
            profile.ViewGeneration.ViewModelFileSuffix = "ViewModel";
            profile.ViewGeneration.XamlFileDirectoryName = "Files";
            profile.ViewGeneration.XamlFileSuffix = "Page";

            var fs = new TestFileSystem
            {
                FileExistsResponse = false,
                FileText = @" ' Just comments in this file",
            };

            var synTree = VisualBasicSyntaxTree.ParseText(fs.FileText);
            var semModel = VisualBasicCompilation.Create(string.Empty).AddSyntaxTrees(synTree).GetSemanticModel(synTree, ignoreAccessibility: true);

            var vsa = new TestVisualStudioAbstraction
            {
                SyntaxTree = synTree,
                SemanticModel = semModel,
                ActiveProject = new ProjectWrapper() { Name = "App", FileName = @"C:\Test\App\App.vbproj" },
            };

            var sut = new CreateViewCommandLogic(DefaultTestLogger.Create(), vsa, fs, profile);

            await sut.ExecuteAsync(@"C:\Test\App\Files\TestViewModel.vb");

            Assert.IsFalse(sut.CreateView);
        }

        [TestMethod]
        public async Task CorrectOutputInSameFolder_FileContainsModuleNotClassAsync()
        {
            var profile = this.GetDefaultTestProfile();

            profile.ViewGeneration.AllInSameProject = true;
            profile.ViewGeneration.ViewModelDirectoryName = "Files";
            profile.ViewGeneration.ViewModelFileSuffix = "ViewModel";
            profile.ViewGeneration.XamlFileDirectoryName = "Files";
            profile.ViewGeneration.XamlFileSuffix = "Page";

            var fs = new TestFileSystem
            {
                FileExistsResponse = false,
                FileText = @"Public Module TestViewModel
    Public Property OnlyProperty As String
    Private Property HiddenProperty As String
End Module",
            };

            var synTree = VisualBasicSyntaxTree.ParseText(fs.FileText);
            var semModel = VisualBasicCompilation.Create(string.Empty).AddSyntaxTrees(synTree).GetSemanticModel(synTree, ignoreAccessibility: true);

            var vsa = new TestVisualStudioAbstraction
            {
                SyntaxTree = synTree,
                SemanticModel = semModel,
                ActiveProject = new ProjectWrapper() { Name = "App", FileName = @"C:\Test\App\App.vbproj" },
            };

            var sut = new CreateViewCommandLogic(DefaultTestLogger.Create(), vsa, fs, profile);

            await sut.ExecuteAsync(@"C:\Test\App\Files\TestViewModel.vb");

            var expectedXaml = "<Page"
       + Environment.NewLine + "    x:Class=\"App.Files.TestPage\">"
       + Environment.NewLine + "    <Grid>"
       + Environment.NewLine + "        <StackPanel>"
       + Environment.NewLine + "            <TextBlock FB=\"True\" Text=\"OnlyProperty\" />"
       + Environment.NewLine + "        </StackPanel>"
       + Environment.NewLine + "    </Grid>"
       + Environment.NewLine + "</Page>"
       + Environment.NewLine + "";

            var expectedCodeBehind = "Imports System"
             + Environment.NewLine + "Imports Windows.UI.Xaml.Controls"
             + Environment.NewLine + "Imports App.Files"
             + Environment.NewLine + ""
             + Environment.NewLine + "Namespace Files"
             + Environment.NewLine + ""
             + Environment.NewLine + "    Public NotInheritable Partial Class TestPage"
             + Environment.NewLine + "        Inherits Page"
             + Environment.NewLine + ""
             + Environment.NewLine + "        Public Property ViewModel As TestViewModel"
             + Environment.NewLine + ""
             + Environment.NewLine + "        Public Sub New()"
             + Environment.NewLine + "            Me.InitializeComponent()"
             + Environment.NewLine + "            Me.ViewModel = New TestViewModel()"
             + Environment.NewLine + "        End Sub"
             + Environment.NewLine + "    End Class"
             + Environment.NewLine + "End Namespace"
             + Environment.NewLine + "";

            Assert.IsTrue(sut.CreateView);
            Assert.AreEqual(@"C:\Test\App\Files\TestPage.xaml", sut.XamlFileName);
            Assert.AreEqual(@"C:\Test\App\Files\TestPage.xaml.vb", sut.CodeFileName);
            StringAssert.AreEqual(expectedXaml, sut.XamlFileContents);
            StringAssert.AreEqual(expectedCodeBehind, sut.CodeFileContents);
        }

        [TestMethod]
        public async Task CorrectOutputInSameProjectAsync()
        {
            var profile = this.GetDefaultTestProfile();

            profile.ViewGeneration.AllInSameProject = true;
            profile.ViewGeneration.ViewModelDirectoryName = "ViewModels";
            profile.ViewGeneration.ViewModelFileSuffix = "ViewModel";
            profile.ViewGeneration.XamlFileDirectoryName = "Views";
            profile.ViewGeneration.XamlFileSuffix = "Page";

            var fs = new TestFileSystem
            {
                FileExistsResponse = false,
                FileText = @"Public Class TestViewModel
    Public Property OnlyProperty As String
End Class",
            };

            var synTree = VisualBasicSyntaxTree.ParseText(fs.FileText);
            var semModel = VisualBasicCompilation.Create(string.Empty).AddSyntaxTrees(synTree).GetSemanticModel(synTree, ignoreAccessibility: true);

            var vsa = new TestVisualStudioAbstraction
            {
                SyntaxTree = synTree,
                SemanticModel = semModel,
                ActiveProject = new ProjectWrapper() { Name = "App", FileName = @"C:\Test\App\App.vbproj" },
            };

            var sut = new CreateViewCommandLogic(DefaultTestLogger.Create(), vsa, fs, profile);

            await sut.ExecuteAsync(@"C:\Test\App\ViewModels\TestViewModel.vb");

            var expectedXaml = "<Page"
       + Environment.NewLine + "    x:Class=\"App.Views.TestPage\">"
       + Environment.NewLine + "    <Grid>"
       + Environment.NewLine + "        <StackPanel>"
       + Environment.NewLine + "            <TextBlock FB=\"True\" Text=\"OnlyProperty\" />"
       + Environment.NewLine + "        </StackPanel>"
       + Environment.NewLine + "    </Grid>"
       + Environment.NewLine + "</Page>"
       + Environment.NewLine + "";

            var expectedCodeBehind = "Imports System"
             + Environment.NewLine + "Imports Windows.UI.Xaml.Controls"
             + Environment.NewLine + "Imports App.ViewModels"
             + Environment.NewLine + ""
             + Environment.NewLine + "Namespace Views"
             + Environment.NewLine + ""
             + Environment.NewLine + "    Public NotInheritable Partial Class TestPage"
             + Environment.NewLine + "        Inherits Page"
             + Environment.NewLine + ""
             + Environment.NewLine + "        Public Property ViewModel As TestViewModel"
             + Environment.NewLine + ""
             + Environment.NewLine + "        Public Sub New()"
             + Environment.NewLine + "            Me.InitializeComponent()"
             + Environment.NewLine + "            Me.ViewModel = New TestViewModel()"
             + Environment.NewLine + "        End Sub"
             + Environment.NewLine + "    End Class"
             + Environment.NewLine + "End Namespace"
             + Environment.NewLine + "";

            Assert.IsTrue(sut.CreateView);
            Assert.AreEqual(@"C:\Test\App\Views\TestPage.xaml", sut.XamlFileName);
            Assert.AreEqual(@"C:\Test\App\Views\TestPage.xaml.vb", sut.CodeFileName);
            StringAssert.AreEqual(expectedXaml, sut.XamlFileContents);
            StringAssert.AreEqual(expectedCodeBehind, sut.CodeFileContents);
        }

        [TestMethod]
        public async Task CorrectOutputInOtherProjectAsync()
        {
            var profile = this.GetDefaultTestProfile();

            profile.ViewGeneration.AllInSameProject = false;

            profile.ViewGeneration.XamlProjectSuffix = string.Empty;
            profile.ViewGeneration.ViewModelProjectSuffix = ".ViewModels";

            profile.ViewGeneration.ViewModelDirectoryName = "";
            profile.ViewGeneration.ViewModelFileSuffix = "ViewModel";
            profile.ViewGeneration.XamlFileDirectoryName = "Views";
            profile.ViewGeneration.XamlFileSuffix = "Page";

            var fs = new TestFileSystem
            {
                FileExistsResponse = false,
                FileText = @"Public Class TestViewModel
    Public Property OnlyProperty As String
End Class",
            };

            var synTree = VisualBasicSyntaxTree.ParseText(fs.FileText);
            var semModel = VisualBasicCompilation.Create(string.Empty).AddSyntaxTrees(synTree).GetSemanticModel(synTree, ignoreAccessibility: true);

            var vsa = new TestVisualStudioAbstraction
            {
                SyntaxTree = synTree,
                SemanticModel = semModel,
                ActiveProject = new ProjectWrapper { Name = "App.ViewModels", FileName = @"C:\Test\App.ViewModels\App.ViewModels.vbproj" },
                NamedProject = new ProjectWrapper { Name = "App", FileName = @"C:\Test\App\App.vbproj" },
            };

            var sut = new CreateViewCommandLogic(DefaultTestLogger.Create(), vsa, fs, profile);

            await sut.ExecuteAsync(@"C:\Test\App.ViewModels\TestViewModel.vb");

            var expectedXaml = "<Page"
       + Environment.NewLine + "    x:Class=\"App.Views.TestPage\">"
       + Environment.NewLine + "    <Grid>"
       + Environment.NewLine + "        <StackPanel>"
       + Environment.NewLine + "            <TextBlock FB=\"True\" Text=\"OnlyProperty\" />"
       + Environment.NewLine + "        </StackPanel>"
       + Environment.NewLine + "    </Grid>"
       + Environment.NewLine + "</Page>"
       + Environment.NewLine + "";

            var expectedCodeBehind = "Imports System"
             + Environment.NewLine + "Imports Windows.UI.Xaml.Controls"
             + Environment.NewLine + "Imports App.ViewModels"
             + Environment.NewLine + ""
             + Environment.NewLine + "Namespace Views"
             + Environment.NewLine + ""
             + Environment.NewLine + "    Public NotInheritable Partial Class TestPage"
             + Environment.NewLine + "        Inherits Page"
             + Environment.NewLine + ""
             + Environment.NewLine + "        Public Property ViewModel As TestViewModel"
             + Environment.NewLine + ""
             + Environment.NewLine + "        Public Sub New()"
             + Environment.NewLine + "            Me.InitializeComponent()"
             + Environment.NewLine + "            Me.ViewModel = New TestViewModel()"
             + Environment.NewLine + "        End Sub"
             + Environment.NewLine + "    End Class"
             + Environment.NewLine + "End Namespace"
             + Environment.NewLine + "";

            Assert.IsTrue(sut.CreateView);
            Assert.AreEqual(@"C:\Test\App\Views\TestPage.xaml", sut.XamlFileName);
            Assert.AreEqual(@"C:\Test\App\Views\TestPage.xaml.vb", sut.CodeFileName);
            StringAssert.AreEqual(expectedXaml, sut.XamlFileContents);
            StringAssert.AreEqual(expectedCodeBehind, sut.CodeFileContents);
        }

        private Profile GetDefaultTestProfile()
        {
            var xamlPlaceholder = "<Page"
          + Environment.NewLine + "    x:Class=\"$viewproject$.$viewns$.$viewclass$\">"
          + Environment.NewLine + "    <Grid>"
          + Environment.NewLine + "        $genxaml$"
          + Environment.NewLine + "    </Grid>"
          + Environment.NewLine + "</Page>"
          + Environment.NewLine + "";

            var codePlaceholder = "Imports System"
          + Environment.NewLine + "Imports Windows.UI.Xaml.Controls"
          + Environment.NewLine + "Imports $viewmodelns$"
          + Environment.NewLine + ""
          + Environment.NewLine + "Namespace $viewns$"
          + Environment.NewLine + ""
          + Environment.NewLine + "    Public NotInheritable Partial Class $viewclass$"
          + Environment.NewLine + "        Inherits Page"
          + Environment.NewLine + ""
          + Environment.NewLine + "        Public Property ViewModel As $viewmodelclass$"
          + Environment.NewLine + ""
          + Environment.NewLine + "        Public Sub New()"
          + Environment.NewLine + "            Me.InitializeComponent()"
          + Environment.NewLine + "            Me.ViewModel = New $viewmodelclass$()"
          + Environment.NewLine + "        End Sub"
          + Environment.NewLine + "    End Class"
          + Environment.NewLine + "End Namespace"
          + Environment.NewLine + "";

            return new Profile
            {
                Name = "TestProfile",
                ClassGrouping = "StackPanel",
                FallbackOutput = "<TextBlock FB=\"True\" Text=\"$name$\" />",
                SubPropertyOutput = "<TextBlock SP=\"True\" Text=\"$name$\" />",
                Mappings = new ObservableCollection<Mapping>(),
                ViewGeneration = new ViewGenerationSettings
                {
                    XamlPlaceholder = xamlPlaceholder,
                    CodePlaceholder = codePlaceholder,
                    XamlFileSuffix = "Page",
                    ViewModelFileSuffix = "ViewModel",

                    XamlFileDirectoryName = "Views",
                    ViewModelDirectoryName = "ViewModels",

                    AllInSameProject = true,

                    XamlProjectSuffix = "n/a",
                    ViewModelProjectSuffix = "n/a",
                },
                Datacontext = new DatacontextSettings
                {
                    XamlPageAttribute = string.Empty,
                    CodeBehindPageContent = "private $viewmodelclass$ ViewModel { get { return DataContext as $viewmodelclass$; } }",
                    CodeBehindConstructorContent = "this.DataContext = this.ViewModel;",
                    DefaultCodeBehindConstructor = string.Empty,
                },
            };
        }
    }
}
