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
        public void TextXamlFilesIn_abstractionsxunit()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\abstractions.xunit");
        }

        [TestMethod]
        public void TextXamlFilesIn_calculator()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\calculator");
        }

        [TestMethod]
        public void TextXamlFilesIn_CollapseComments()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\CollapseComments");
        }

        [TestMethod]
        public void TextXamlFilesIn_CommandingUwpDocs()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\CommandingUwpDocs");
        }

        [TestMethod]
        public void TextXamlFilesIn_CoreTemplateStudio()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\CoreTemplateStudio");
        }

        [TestMethod]
        public void TextXamlFilesIn_DemoSnippets()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\DemoSnippets");
        }

        [TestMethod]
        public void TextXamlFilesIn_DetectWorkloads()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\DetectWorkloads");
        }

        [TestMethod]
        public void TextXamlFilesIn_dotmortenXamarinForms()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\dotmorten-Xamarin.Forms");
        }

        [TestMethod]
        public void TextXamlFilesIn_dotmortenXamarinFormsUWPShellSample()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\dotmorten-Xamarin.Forms.UWPShell.Sample");
        }

        [TestMethod]
        public void TextXamlFilesIn_Gastropods()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\Gastropods");
        }

        [TestMethod]
        public void TextXamlFilesIn_GetLiveXamlInfo()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\GetLiveXamlInfo");
        }

        [TestMethod]
        public void TextXamlFilesIn_githubdesktop()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\githubdesktop");
        }

        [TestMethod]
        public void TextXamlFilesIn_Identity()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\Identity");
        }

        [TestMethod]
        public void TextXamlFilesIn_InvokeNavigationViewItems()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\InvokeNavigationViewItems");
        }

        [TestMethod]
        public void TextXamlFilesIn_IssuesHub()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\IssuesHub");
        }

        [TestMethod]
        public void TextXamlFilesIn_jcansdaleLocalizeVsix()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\jcansdale-LocalizeVsix");
        }

        [TestMethod]
        public void TextXamlFilesIn_LuisEntityHelpers()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\LuisEntityHelpers");
        }

        [TestMethod]
        public void TextXamlFilesIn_MarkdownEditor()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\MarkdownEditor");
        }

        [TestMethod]
        public void TextXamlFilesIn_microsoftuixaml()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\microsoft-ui-xaml");
        }

        [TestMethod]
        public void TextXamlFilesIn_microsoftuixamlnumberbox()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\microsoft-ui-xaml-numberbox");
        }

        [TestMethod]
        public void TextXamlFilesIn_microsoftuixamlspecs()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\microsoft-ui-xaml-specs");
        }

        [TestMethod]
        public void TextXamlFilesIn_microsoftwts()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\microsoft-wts");
        }

        [TestMethod]
        public void TextXamlFilesIn_mrlaceyRXT()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\mrlacey-RXT");
        }

        [TestMethod]
        public void TextXamlFilesIn_MultiLineStringAnalyzer()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\MultiLineStringAnalyzer");
        }

        [TestMethod]
        public void TextXamlFilesIn_MVVMBasicSnippets()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\MVVMBasicSnippets");
        }

        [TestMethod]
        public void TextXamlFilesIn_RapidXAMLToolkit()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\Rapid-XAML-Toolkit");
        }

        [TestMethod]
        public void TextXamlFilesIn_ReproTreeView()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\ReproTreeView");
        }

        [TestMethod]
        public void TextXamlFilesIn_ResPsuedoLoc()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\ResPsuedoLoc");
        }

        [TestMethod]
        public void TextXamlFilesIn_rxtdemos()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\rxt-demos");
        }

        [TestMethod]
        public void TextXamlFilesIn_ShowSelection()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\ShowSelection");
        }

        [TestMethod]
        public void TextXamlFilesIn_SignVsix()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\SignVsix");
        }

        [TestMethod]
        public void TextXamlFilesIn_SimpleJsonAnalyzer()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\SimpleJsonAnalyzer");
        }

        [TestMethod]
        public void TextXamlFilesIn_StringResourceVisualizer()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\StringResourceVisualizer");
        }

        [TestMethod]
        public void TextXamlFilesIn_tempwts3206()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\temp-wts3206");
        }

        [TestMethod]
        public void TextXamlFilesIn_tempwts3206e()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\temp-wts3206e");
        }

        [TestMethod]
        public void TextXamlFilesIn_terminal()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\terminal");
        }

        [TestMethod]
        public void TextXamlFilesIn_testtemplates()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\test-templates");
        }

        [TestMethod]
        public void TextXamlFilesIn_Uno()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\Uno");
        }

        [TestMethod]
        public void TextXamlFilesIn_UWPCommunityToolkit()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\UWPCommunityToolkit");
        }

        [TestMethod]
        public void TextXamlFilesIn_UWPCommunityToolkitDocs()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\UWPCommunityToolkitDocs");
        }

        [TestMethod]
        public void TextXamlFilesIn_UwpDesignTimeData()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\UwpDesignTimeData");
        }

        [TestMethod]
        public void TextXamlFilesIn_UwpDevTidy()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\UwpDevTidy");
        }

        [TestMethod]
        public void TextXamlFilesIn_UwpEssentials()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\UwpEssentials");
        }

        [TestMethod]
        public void TextXamlFilesIn_VBUWPTemplates()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\VB-UWP-Templates");
        }

        [TestMethod]
        public void TextXamlFilesIn_VBStyleAnalyzer()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\VBStyleAnalyzer");
        }

        [TestMethod]
        public void TextXamlFilesIn_visor()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\visor");
        }

        [TestMethod]
        public void TextXamlFilesIn_VsixTools()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\VsixTools");
        }

        [TestMethod]
        public void TextXamlFilesIn_VSWaterMark()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\VSWaterMark");
        }

        [TestMethod]
        public void TextXamlFilesIn_WarnAboutTodos()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\WarnAboutTodos");
        }

        [TestMethod]
        public void TextXamlFilesIn_WindowsTemplateStudio()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\WindowsTemplateStudio");
        }

        [TestMethod]
        public void TextXamlFilesIn_WindowsTestHelpers()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\WindowsTestHelpers");
        }

        [TestMethod]
        public void TextXamlFilesIn_winforms()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\winforms");
        }

        [TestMethod]
        public void TextXamlFilesIn_wpf()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\wpf");
        }

        [TestMethod]
        public void TextXamlFilesIn_wts3206d()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\wts3206d");
        }

        [TestMethod]
        public void TextXamlFilesIn_wts3206e()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\wts3206e");
        }

        [TestMethod]
        public void TextXamlFilesIn_xamarinformssamples()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\xamarin-forms-samples");
        }

        [TestMethod]
        public void TextXamlFilesIn_XamarinForms()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\Xamarin.Forms");
        }

        [TestMethod]
        public void TextXamlFilesIn_XamarinFormsUWPShellSample()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\Xamarin.Forms.UWPShell.Sample");
        }

        [TestMethod]
        public void TextXamlFilesIn_xunit()
        {
            this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\xunit");
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

                    try
                    {
                    XamlElementExtractor.Parse(ProjectType.Any, filePath, snapshot, text, RapidXamlDocument.GetAllProcessors(ProjectType.Any), result.Tags);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }

                    Debug.WriteLine($"Found {result.Tags.Count} taggable issues in '{filePath}'.");

                    if (result.Tags.Count > 0)
                    {
                        // if (result.Tags.Count > 10)
                        if (result.Tags.OfType<RapidXamlDisplayedTag>().Any())
                        {
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
