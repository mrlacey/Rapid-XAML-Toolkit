// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

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

            var sut = new CreateViewCommandLogic(profile, DefaultTestLogger.Create(), vsa, fs);

            await sut.ExecuteAsync(@"C:\Test\App\Files\TestViewModel.vb");

            var expectedXaml = @"<Page
    x:Class=""App.Files.TestPage"">
    <Grid>
        <StackPanel>
<TextBlock FB=""True"" Text=""OnlyProperty"" />
</StackPanel>
    </Grid>
</Page>
";

            var expectedCodeBehind = @"Imports System
Imports Windows.UI.Xaml.Controls
Imports App.Files

Namespace Files

    Public NotInheritable Partial Class TestPage
        Inherits Page

        Public Property ViewModel As TestViewModel

        Public Sub New()
            Me.InitializeComponent()
            Me.ViewModel = New TestViewModel()
        End Sub
    End Class
End Namespace
";

            Assert.IsTrue(sut.CreateView);
            Assert.AreEqual(@"C:\Test\App\Files\TestPage.xaml", sut.XamlFileName);
            Assert.AreEqual(@"C:\Test\App\Files\TestPage.xaml.vb", sut.CodeFileName);
            Assert.AreEqual(expectedXaml, sut.XamlFileContents);
            Assert.AreEqual(expectedCodeBehind, sut.CodeFileContents);
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

            var sut = new CreateViewCommandLogic(profile, DefaultTestLogger.Create(), vsa, fs);

            await sut.ExecuteAsync(@"C:\Test\App\ViewModels\TestViewModel.vb");

            var expectedXaml = @"<Page
    x:Class=""App.Views.TestPage"">
    <Grid>
        <StackPanel>
<TextBlock FB=""True"" Text=""OnlyProperty"" />
</StackPanel>
    </Grid>
</Page>
";

            var expectedCodeBehind = @"Imports System
Imports Windows.UI.Xaml.Controls
Imports App.ViewModels

Namespace Views

    Public NotInheritable Partial Class TestPage
        Inherits Page

        Public Property ViewModel As TestViewModel

        Public Sub New()
            Me.InitializeComponent()
            Me.ViewModel = New TestViewModel()
        End Sub
    End Class
End Namespace
";

            Assert.IsTrue(sut.CreateView);
            Assert.AreEqual(@"C:\Test\App\Views\TestPage.xaml", sut.XamlFileName);
            Assert.AreEqual(@"C:\Test\App\Views\TestPage.xaml.vb", sut.CodeFileName);
            Assert.AreEqual(expectedXaml, sut.XamlFileContents);
            Assert.AreEqual(expectedCodeBehind, sut.CodeFileContents);
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

            var sut = new CreateViewCommandLogic(profile, DefaultTestLogger.Create(), vsa, fs);

            await sut.ExecuteAsync(@"C:\Test\App.ViewModels\TestViewModel.vb");

            var expectedXaml = @"<Page
    x:Class=""App.Views.TestPage"">
    <Grid>
        <StackPanel>
<TextBlock FB=""True"" Text=""OnlyProperty"" />
</StackPanel>
    </Grid>
</Page>
";

            var expectedCodeBehind = @"Imports System
Imports Windows.UI.Xaml.Controls
Imports App.ViewModels

Namespace Views

    Public NotInheritable Partial Class TestPage
        Inherits Page

        Public Property ViewModel As TestViewModel

        Public Sub New()
            Me.InitializeComponent()
            Me.ViewModel = New TestViewModel()
        End Sub
    End Class
End Namespace
";

            Assert.IsTrue(sut.CreateView);
            Assert.AreEqual(@"C:\Test\App\Views\TestPage.xaml", sut.XamlFileName);
            Assert.AreEqual(@"C:\Test\App\Views\TestPage.xaml.vb", sut.CodeFileName);
            Assert.AreEqual(expectedXaml, sut.XamlFileContents);
            Assert.AreEqual(expectedCodeBehind, sut.CodeFileContents);
        }

        private Profile GetDefaultTestProfile()
        {
            return new Profile
            {
                Name = "TestProfile",
                ClassGrouping = "StackPanel",
                FallbackOutput = "<TextBlock FB=\"True\" Text=\"$name$\" />",
                SubPropertyOutput = "<TextBlock SP=\"True\" Text=\"$name$\" />",
                Mappings = new ObservableCollection<Mapping>(),
                ViewGeneration = new ViewGenerationSettings
                {
                    XamlPlaceholder = @"<Page
    x:Class=""$viewproject$.$viewns$.$viewclass$"">
    <Grid>
        $genxaml$
    </Grid>
</Page>
",
                    CodePlaceholder = @"Imports System
Imports Windows.UI.Xaml.Controls
Imports $viewmodelns$

Namespace $viewns$

    Public NotInheritable Partial Class $viewclass$
        Inherits Page

        Public Property ViewModel As $viewmodelclass$

        Public Sub New()
            Me.InitializeComponent()
            Me.ViewModel = New $viewmodelclass$()
        End Sub
    End Class
End Namespace
",
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
