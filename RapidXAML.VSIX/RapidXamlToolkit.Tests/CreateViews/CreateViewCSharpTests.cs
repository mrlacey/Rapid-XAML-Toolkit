// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidXamlToolkit.Commands;
using RapidXamlToolkit.Options;

namespace RapidXamlToolkit.Tests.CreateViews
{
    [TestClass]
    public class CreateViewCSharpTests
    {
        [TestMethod]
        public async Task CorrectOutputInSameFolder()
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
                FileText = " public class TestViewModel { public string OnlyProperty { get; set;} }",
            };

            var synTree = CSharpSyntaxTree.ParseText(fs.FileText);
            var semModel = CSharpCompilation.Create(string.Empty).AddSyntaxTrees(synTree).GetSemanticModel(synTree, ignoreAccessibility: true);

            var vsa = new TestVisualStudioAbstraction
            {
                SyntaxTree = synTree,
                SemanticModel = semModel,
                ActiveProject = new ProjectWrapper() { Name = "App", FileName = @"C:\Test\App\App.csproj" },
            };

            var sut = new CreateViewCommandLogic(profile, DefaultTestLogger.Create(), vsa, fs);

            await sut.ExecuteAsync(@"C:\Test\App\Files\TestViewModel.cs");

            var expectedXaml = "<Page"
       + Environment.NewLine + "    x:Class=\"App.Files.TestPage\">"
       + Environment.NewLine + "    <Grid>"
       + Environment.NewLine + "        <StackPanel>"
       + Environment.NewLine + "            <TextBlock FB=\"True\" Text=\"OnlyProperty\" />"
       + Environment.NewLine + "        </StackPanel>"
       + Environment.NewLine + "    </Grid>"
       + Environment.NewLine + "</Page>"
       + Environment.NewLine + "";

            var expectedCodeBehind = @"using System;
using Windows.UI.Xaml.Controls;
using App.Files;

namespace App.Files
{
    public sealed partial class TestPage : Page
    {
        public TestViewModel ViewModel { get; set; }

        public TestPage()
        {
            this.InitializeComponent();
            this.ViewModel = new TestViewModel();
        }
    }
}
";

            Assert.IsTrue(sut.CreateView);
            Assert.AreEqual(@"C:\Test\App\Files\TestPage.xaml", sut.XamlFileName);
            Assert.AreEqual(@"C:\Test\App\Files\TestPage.xaml.cs", sut.CodeFileName);
            Assert.AreEqual(expectedXaml, sut.XamlFileContents);
            Assert.AreEqual(expectedCodeBehind, sut.CodeFileContents);
        }

        [TestMethod]
        public async Task CorrectOutputInSameProject()
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
                FileText = " public class TestViewModel { public string OnlyProperty { get; set;} }",
            };

            var synTree = CSharpSyntaxTree.ParseText(fs.FileText);
            var semModel = CSharpCompilation.Create(string.Empty).AddSyntaxTrees(synTree).GetSemanticModel(synTree, ignoreAccessibility: true);

            var vsa = new TestVisualStudioAbstraction
            {
                SyntaxTree = synTree,
                SemanticModel = semModel,
                ActiveProject = new ProjectWrapper() { Name = "App", FileName = @"C:\Test\App\App.csproj" },
            };

            var sut = new CreateViewCommandLogic(profile, DefaultTestLogger.Create(), vsa, fs);

            await sut.ExecuteAsync(@"C:\Test\App\ViewModels\TestViewModel.cs");

            var expectedXaml = "<Page"
       + Environment.NewLine + "    x:Class=\"App.Views.TestPage\">"
       + Environment.NewLine + "    <Grid>"
       + Environment.NewLine + "        <StackPanel>"
       + Environment.NewLine + "            <TextBlock FB=\"True\" Text=\"OnlyProperty\" />"
       + Environment.NewLine + "        </StackPanel>"
       + Environment.NewLine + "    </Grid>"
       + Environment.NewLine + "</Page>"
       + Environment.NewLine + "";

            var expectedCodeBehind = @"using System;
using Windows.UI.Xaml.Controls;
using App.ViewModels;

namespace App.Views
{
    public sealed partial class TestPage : Page
    {
        public TestViewModel ViewModel { get; set; }

        public TestPage()
        {
            this.InitializeComponent();
            this.ViewModel = new TestViewModel();
        }
    }
}
";

            Assert.IsTrue(sut.CreateView);
            Assert.AreEqual(@"C:\Test\App\Views\TestPage.xaml", sut.XamlFileName);
            Assert.AreEqual(@"C:\Test\App\Views\TestPage.xaml.cs", sut.CodeFileName);
            Assert.AreEqual(expectedXaml, sut.XamlFileContents);
            Assert.AreEqual(expectedCodeBehind, sut.CodeFileContents);
        }

        [TestMethod]
        public async Task CorrectOutputInOtherProject()
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
                FileText = " public class TestViewModel { public string OnlyProperty { get; set;} }",
            };

            var synTree = CSharpSyntaxTree.ParseText(fs.FileText);
            var semModel = CSharpCompilation.Create(string.Empty).AddSyntaxTrees(synTree).GetSemanticModel(synTree, ignoreAccessibility: true);

            var vsa = new TestVisualStudioAbstraction
            {
                SyntaxTree = synTree,
                SemanticModel = semModel,
                ActiveProject = new ProjectWrapper { Name = "App.ViewModels", FileName = @"C:\Test\App.ViewModels\App.ViewModels.csproj" },
                NamedProject = new ProjectWrapper { Name = "App", FileName = @"C:\Test\App\App.csproj" },
            };

            var sut = new CreateViewCommandLogic(profile, DefaultTestLogger.Create(), vsa, fs);

            await sut.ExecuteAsync(@"C:\Test\App.ViewModels\TestViewModel.cs");

            var expectedXaml = "<Page"
       + Environment.NewLine + "    x:Class=\"App.Views.TestPage\">"
       + Environment.NewLine + "    <Grid>"
       + Environment.NewLine + "        <StackPanel>"
       + Environment.NewLine + "            <TextBlock FB=\"True\" Text=\"OnlyProperty\" />"
       + Environment.NewLine + "        </StackPanel>"
       + Environment.NewLine + "    </Grid>"
       + Environment.NewLine + "</Page>"
       + Environment.NewLine + "";

            var expectedCodeBehind = @"using System;
using Windows.UI.Xaml.Controls;
using App.ViewModels;

namespace App.Views
{
    public sealed partial class TestPage : Page
    {
        public TestViewModel ViewModel { get; set; }

        public TestPage()
        {
            this.InitializeComponent();
            this.ViewModel = new TestViewModel();
        }
    }
}
";

            Assert.IsTrue(sut.CreateView);
            Assert.AreEqual(@"C:\Test\App\Views\TestPage.xaml", sut.XamlFileName);
            Assert.AreEqual(@"C:\Test\App\Views\TestPage.xaml.cs", sut.CodeFileName);
            Assert.AreEqual(expectedXaml, sut.XamlFileContents);
            Assert.AreEqual(expectedCodeBehind, sut.CodeFileContents);
        }

        [TestMethod]
        public async Task FileExistsAndDoNotOverwriteMeansNoNewFileCreatedAsync()
        {
            var profile = this.GetDefaultTestProfile();

            profile.ViewGeneration.AllInSameProject = true;
            profile.ViewGeneration.ViewModelDirectoryName = "ViewModels";
            profile.ViewGeneration.ViewModelFileSuffix = "ViewModel";
            profile.ViewGeneration.XamlFileDirectoryName = "Views";
            profile.ViewGeneration.XamlFileSuffix = "Page";

            var fs = new TestFileSystem
            {
                FileExistsResponse = true,
                FileText = " public class TestViewModel { public string OnlyProperty { get; set;} }",
            };

            var synTree = CSharpSyntaxTree.ParseText(fs.FileText);
            var semModel = CSharpCompilation.Create(string.Empty).AddSyntaxTrees(synTree).GetSemanticModel(synTree, ignoreAccessibility: true);

            var vsa = new TestVisualStudioAbstraction
            {
                UserConfirmsResult = false,
                SyntaxTree = synTree,
                SemanticModel = semModel,
                ActiveProject = new ProjectWrapper() { Name = "App", FileName = @"C:\Test\App\App.csproj" },
            };
            var sut = new CreateViewCommandLogic(profile, DefaultTestLogger.Create(), vsa, fs);

            await sut.ExecuteAsync(@"C:\Test\App\ViewModels\TestViewModel.cs");

            Assert.IsFalse(sut.CreateView);
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
    x:Class=""$viewns$.$viewclass$"">
    <Grid>
        $genxaml$
    </Grid>
</Page>
",
                    CodePlaceholder = @"using System;
using Windows.UI.Xaml.Controls;
using $viewmodelns$;

namespace $viewns$
{
    public sealed partial class $viewclass$ : Page
    {
        public $viewmodelclass$ ViewModel { get; set; }

        public $viewclass$()
        {
            this.InitializeComponent();
            this.ViewModel = new $viewmodelclass$();
        }
    }
}
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
