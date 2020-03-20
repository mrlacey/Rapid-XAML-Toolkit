// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidXamlToolkit.XamlAnalysis;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.Tests.Manual.XamlAnalysis
{
    [TestClass]
    public class ParseRealDocumentsTests
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void TestXamlFilesIn_abstractionsxunit()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\abstractions.xunit");
        }

        [TestMethod]
        public void TestXamlFilesIn_alwaysuse()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\alwaysuse");
        }

        [TestMethod]
        public void TestXamlFilesIn_aniongithubCustomTool()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\aniongithub-CustomTool");
        }

        [TestMethod]
        public void TestXamlFilesIn_appmonkeys()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\app-monkeys");
        }

        [TestMethod]
        public void TestXamlFilesIn_bible()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\bible");
        }

        [TestMethod]
        public void TestXamlFilesIn_calculator()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\calculator");
        }

        [TestMethod]
        public void TestXamlFilesIn_ClearlyEditable()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\ClearlyEditable");
        }

        [TestMethod]
        public void TestXamlFilesIn_codestories()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\code-stories");
        }

        [TestMethod]
        public void TestXamlFilesIn_CollapseComments()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\CollapseComments");
        }

        [TestMethod]
        public void TestXamlFilesIn_CommandingUwpDocs()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\CommandingUwpDocs");
        }

        [TestMethod]
        public void TestXamlFilesIn_CoreTemplateStudio()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\CoreTemplateStudio");
        }

        [TestMethod]
        public void TestXamlFilesIn_DemoSnippets()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\DemoSnippets");
        }

        [TestMethod]
        public void TestXamlFilesIn_DetectWorkloads()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\DetectWorkloads");
        }

        [TestMethod]
        public void TestXamlFilesIn_dotmortenXamarinForms()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\dotmorten-Xamarin.Forms");
        }

        [TestMethod]
        public void TestXamlFilesIn_dotmortenXamarinFormsUWPShellSample()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\dotmorten-Xamarin.Forms.UWPShell.Sample");
        }

        [TestMethod]
        public void TestXamlFilesIn_ErrorHelper()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\ErrorHelper");
        }

        [TestMethod]
        public void TestXamlFilesIn_ErrorHighlighter()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\ErrorHighlighter");
        }

        [TestMethod]
        public void TestXamlFilesIn_Gastropods()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\Gastropods");
        }

        [TestMethod]
        public void TestXamlFilesIn_GetLiveXamlInfo()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\GetLiveXamlInfo");
        }

        [TestMethod]
        public void TestXamlFilesIn_githubdesktop()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\githubdesktop");
        }

        [TestMethod]
        public void TestXamlFilesIn_HotTips()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\HotTips");
        }

        [TestMethod]
        public void TestXamlFilesIn_Identity()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\Identity");
        }

        [TestMethod]
        public void TestXamlFilesIn_imagecomments()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\image-comments");
        }

        [TestMethod]
        public void TestXamlFilesIn_InvokeNavigationViewItems()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\InvokeNavigationViewItems");
        }

        [TestMethod]
        public void TestXamlFilesIn_IssuesHub()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\IssuesHub");
        }

        [TestMethod]
        public void TestXamlFilesIn_janczizikowsleek()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\janczizikow-sleek");
        }

        [TestMethod]
        public void TestXamlFilesIn_jcansdaleLocalizeVsix()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\jcansdale-LocalizeVsix");
        }

        [TestMethod]
        public void TestXamlFilesIn_LanguageRef()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\LanguageRef");
        }

        [TestMethod]
        public void TestXamlFilesIn_LinkedVsctIssue()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\LinkedVsctIssue");
        }

        [TestMethod]
        public void TestXamlFilesIn_LuisEntityHelpers()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\LuisEntityHelpers");
        }

        [TestMethod]
        public void TestXamlFilesIn_Marb2000XamlIslands()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\Marb2000XamlIslands");
        }

        [TestMethod]
        public void TestXamlFilesIn_MarkdownEditor()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\MarkdownEditor");
        }

        [TestMethod]
        public void TestXamlFilesIn_michaelhawkerXmlSyntaxVisualizerUWP()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\michael-hawker-XmlSyntaxVisualizerUWP");
        }

        [TestMethod]
        public void TestXamlFilesIn_microsoftuixaml()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\microsoft-ui-xaml");
        }

        [TestMethod]
        public void TestXamlFilesIn_microsoftuixamlnumberbox()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\microsoft-ui-xaml-numberbox");
        }

        [TestMethod]
        public void TestXamlFilesIn_microsoftuixamlspecs()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\microsoft-ui-xaml-specs");
        }

        [TestMethod]
        public void TestXamlFilesIn_microsoftwts()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\microsoft-wts");
        }

        [TestMethod]
        public void TestXamlFilesIn_mrlappmonkeys()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\mrl-app-monkeys");
        }

        [TestMethod]
        public void TestXamlFilesIn_mrlmicrosoftuixaml()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\mrl-microsoft-ui-xaml");
        }

        [TestMethod]
        public void TestXamlFilesIn_mrlaceyRXT()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\mrlacey-RXT");
        }

        [TestMethod]
        public void TestXamlFilesIn_MultiLineStringAnalyzer()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\MultiLineStringAnalyzer");
        }

        [TestMethod]
        public void TestXamlFilesIn_MVVMBasicSnippets()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\MVVMBasicSnippets");
        }

        [TestMethod]
        public void TestXamlFilesIn_MvvmCross()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\MvvmCross");
        }

        [TestMethod]
        public void TestXamlFilesIn_MyDotNetCoreWpfApp()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\MyDotNetCoreWpfApp");
        }

        [TestMethod]
        public void TestXamlFilesIn_natemcmastermsbuildtasks()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\natemcmaster-msbuild-tasks");
        }

        [TestMethod]
        public void TestXamlFilesIn_natemcmasterYarnMSBuild()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\natemcmaster-Yarn.MSBuild");
        }

        [TestMethod]
        public void TestXamlFilesIn_PagesTest()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\PagesTest");
        }

        [TestMethod]
        public void TestXamlFilesIn_PowerToys()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\PowerToys");
        }

        [TestMethod]
        public void TestXamlFilesIn_RandomXAMLFiles()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\Random XAML Files");
        }

        [TestMethod]
        public void TestXamlFilesIn_RapidXamlDemos()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\RapidXaml-Demos");
        }

        [TestMethod]
        public void TestXamlFilesIn_RapidXamlDev()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\RapidXamlDev");
        }

        [TestMethod]
        public void TestXamlFilesIn_ReproTreeView()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\ReproTreeView");
        }

        [TestMethod]
        public void TestXamlFilesIn_ResPsuedoLoc()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\ResPsuedoLoc");
        }

        [TestMethod]
        public void TestXamlFilesIn_rxtdemos()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\rxt-demos");
        }

        [TestMethod]
        public void TestXamlFilesIn_ShowKeys()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\ShowKeys");
        }

        [TestMethod]
        public void TestXamlFilesIn_ShowSelection()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\ShowSelection");
        }

        [TestMethod]
        public void TestXamlFilesIn_ShowTheShortcut()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\ShowTheShortcut");
        }

        [TestMethod]
        public void TestXamlFilesIn_SignVsix()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\SignVsix");
        }

        [TestMethod]
        public void TestXamlFilesIn_SimpleJsonAnalyzer()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\SimpleJsonAnalyzer");
        }

        [TestMethod]
        public void TestXamlFilesIn_SimpleTesting()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\SimpleTesting");
        }

        [TestMethod]
        public void TestXamlFilesIn_SmartHotel360()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\SmartHotel360");
        }

        [TestMethod]
        public void TestXamlFilesIn_StringResourceVisualizer()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\StringResourceVisualizer");
        }

        [TestMethod]
        public void TestXamlFilesIn_tempwts3206()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\temp-wts3206");
        }

        [TestMethod]
        public void TestXamlFilesIn_tempwts3206e()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\temp-wts3206e");
        }

        [TestMethod]
        public void TestXamlFilesIn_terminal()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\terminal");
        }

        [TestMethod]
        public void TestXamlFilesIn_testtemplates()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\test-templates");
        }

        [TestMethod]
        public void TestXamlFilesIn_Uno()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\Uno");
        }

        [TestMethod]
        public void TestXamlFilesIn_UnoPlayground()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\Uno.Playground");
        }

        [TestMethod]
        public void TestXamlFilesIn_unoworkshops()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\unoworkshops");
        }

        [TestMethod]
        public void TestXamlFilesIn_UWPCommunityToolkit()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\UWPCommunityToolkit");
        }

        [TestMethod]
        public void TestXamlFilesIn_UWPCommunityToolkitDocs()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\UWPCommunityToolkitDocs");
        }

        [TestMethod]
        public void TestXamlFilesIn_UwpDesignTimeData()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\UwpDesignTimeData");
        }

        [TestMethod]
        public void TestXamlFilesIn_UwpDevTidy()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\UwpDevTidy");
        }

        [TestMethod]
        public void TestXamlFilesIn_UwpEssentials()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\UwpEssentials");
        }

        [TestMethod]
        public void TestXamlFilesIn_VBUWPTemplates()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\VB-UWP-Templates");
        }

        [TestMethod]
        public void TestXamlFilesIn_VBStyleAnalyzer()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\VBStyleAnalyzer");
        }

        [TestMethod]
        public void TestXamlFilesIn_visor()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\visor");
        }

        [TestMethod]
        public void TestXamlFilesIn_VsixTools()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\VsixTools");
        }

        [TestMethod]
        public void TestXamlFilesIn_VSSDKExtensibilitySamples()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\VSSDK-Extensibility-Samples");
        }

        [TestMethod]
        public void TestXamlFilesIn_VSWaterMark()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\VSWaterMark");
        }

        [TestMethod]
        public void TestXamlFilesIn_WarnAboutTodos()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\WarnAboutTodos");
        }

        [TestMethod]
        public void TestXamlFilesIn_WindowsappsampleXamlHosting()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\Windows-appsample-Xaml-Hosting");
        }

        [TestMethod]
        public void TestXamlFilesIn_WindowsTemplateStudio()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\WindowsTemplateStudio");
        }

        [TestMethod]
        public void TestXamlFilesIn_WindowsTestHelpers()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\WindowsTestHelpers");
        }

        [TestMethod]
        public void TestXamlFilesIn_winforms()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\winforms");
        }

        [TestMethod]
        public void TestXamlFilesIn_wpf()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\wpf");
        }

        [TestMethod]
        public void TestXamlFilesIn_wts3206d()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\wts3206d");
        }

        [TestMethod]
        public void TestXamlFilesIn_wts3206e()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\wts3206e");
        }

        [TestMethod]
        public void TestXamlFilesIn_xamarinformssamples()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\xamarin-forms-samples");
        }

        [TestMethod]
        public void TestXamlFilesIn_XamarinEssentials()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\Xamarin.Essentials");
        }

        [TestMethod]
        public void TestXamlFilesIn_XamarinForms()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\Xamarin.Forms");
        }

        [TestMethod]
        public void TestXamlFilesIn_XamarinFormsUWPShellSample()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\Xamarin.Forms.UWPShell.Sample");
        }

        [TestMethod]
        public void TestXamlFilesIn_XamlControlsGallery()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\Xaml-Controls-Gallery");
        }

        [TestMethod]
        public void TestXamlFilesIn_xamldesignerextensibilitysamples()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\xaml-designer-extensibility-samples");
        }

        [TestMethod]
        public void TestXamlFilesIn_XamlIslandBlogPostSample()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\XamlIslandBlogPostSample");
        }

        [TestMethod]
        public void TestXamlFilesIn_xunit()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\xunit");
        }

        [TestMethod]
        public void TestXamlFilesIn_miscFiles()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\_miscFiles");
        }

        private static IEnumerable<string> GetXamlFiles(string folder)
        {
            foreach (var file in Directory.GetFiles(folder, "*.xaml", SearchOption.AllDirectories))
            {
                yield return file;
            }
        }

        private void CanParseWithoutErrors(string folderPath)
        {
            foreach (var filePath in GetXamlFiles(folderPath))
            {
                var text = File.ReadAllText(filePath);

                if (text.IsValidXml())
                {
                    var result = new RapidXamlDocument();

                    var snapshot = new FakeTextSnapshot();

                    var logger = DefaultTestLogger.Create();

                    var processors = RapidXamlDocument.GetAllProcessors(ProjectType.Any, string.Empty, logger);

                    var customProcessor = new CustomProcessorWrapper(new StubCustomAnalysisProcessor(), ProjectType.Any, logger);

                    processors.Add(("Application", customProcessor));
                    processors.Add(("Page", customProcessor));
                    processors.Add(("DrawingGroup", customProcessor));
                    processors.Add(("ResourceDictionary", customProcessor));
                    processors.Add(("UserControl", customProcessor));
                    processors.Add(("Canvas", customProcessor));
                    processors.Add(("Viewbox", customProcessor));
                    processors.Add(("PhoneApplicationPage", customProcessor));
                    processors.Add(("Window", customProcessor));
                    processors.Add(("ContentPage", customProcessor));
                    processors.Add(("MasterDetailPage", customProcessor));
                    processors.Add(("NavigationPage", customProcessor));
                    processors.Add(("TabbedPage", customProcessor));
                    processors.Add(("CarouselPage", customProcessor));
                    processors.Add(("TemplatedPage", customProcessor));
                    processors.Add(("Shell", customProcessor));

                    XamlElementExtractor.Parse(ProjectType.Any, filePath, snapshot, text, processors, result.Tags);

                    Debug.WriteLine($"Found {result.Tags.Count} taggable issues in '{filePath}'.");

                    if (result.Tags.Count > 0)
                    {
                        // if (result.Tags.Count > 10)
                        if (result.Tags.OfType<RapidXamlDisplayedTag>().Any())
                        {
                            // This can be useful to examine what is being tagged.
                            Debugger.Break();
                        }

                        this.TestContext.WriteLine($"Found {result.Tags.Count} taggable issues in '{filePath}'.");
                        this.TestContext.AddResultFile(filePath);
                    }
                }
                else
                {
                    Debug.WriteLine($"Invalid XAML found in '{filePath}'.");

                    this.TestContext.WriteLine($"Invalid XAML found in '{filePath}'.");
                    this.TestContext.AddResultFile(filePath);
                }
            }
        }
    }
}
