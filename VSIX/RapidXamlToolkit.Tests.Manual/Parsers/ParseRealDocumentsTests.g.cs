// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidXamlToolkit.Commands;
using RapidXamlToolkit.DragDrop;

namespace RapidXamlToolkit.Tests.Manual.Parsers
{
    [TestClass]
    public class ParseRealDocumentsTests
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public async Task TestCodeFilesIn_abstractionsxunit()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\abstractions.xunit");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_alwaysuse()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\alwaysuse");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_aniongithubCustomTool()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\aniongithub-CustomTool");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_appmonkeys()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\app-monkeys");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_AppInstallerEditor()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\AppInstallerEditor");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_AspNetCoreDocs()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\AspNetCore.Docs");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_bible()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\bible");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_botbuildersamples()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\botbuilder-samples");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_Bravo()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\Bravo");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_brewjournal()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\brewjournal");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_cakevs()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\cake-vs");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_calculator()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\calculator");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_CharacterMapUWP()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\Character-Map-UWP");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_ClearlyEditable()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\ClearlyEditable");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_codestories()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\code-stories");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_CollapseComments()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\CollapseComments");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_CommandingUwpDocs()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\CommandingUwpDocs");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_CommentLinks()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\CommentLinks");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_ConstVisualizer()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\ConstVisualizer");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_ContentGridMockup()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\ContentGridMockup");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_CoreTemplateStudio()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\CoreTemplateStudio");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_CreatingcrossplatformapplicationswithUno()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\Creating-cross-platform-applications-with-Uno");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_CrossPlatformTemplateStudio()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\CrossPlatformTemplateStudio");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_DemoSnippets()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\DemoSnippets");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_DetectWorkloads()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\DetectWorkloads");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_DontCopyAlways()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\DontCopyAlways");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_dotmortenXamarinForms()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\dotmorten-Xamarin.Forms");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_dotmortenXamarinFormsUWPShellSample()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\dotmorten-Xamarin.Forms.UWPShell.Sample");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_dotnetroslyn()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\dotnet-roslyn");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_ErrorHelper()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\ErrorHelper");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_ErrorHighlighter()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\ErrorHighlighter");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_essentialuikitforxamarinforms()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\essential-ui-kit-for-xamarin.forms");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_FestiveEditor()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\FestiveEditor");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_Gastropods()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\Gastropods");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_GetLiveXamlInfo()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\GetLiveXamlInfo");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_ghpkgtest()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\ghpkgtest");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_githubappswithgraphql()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\github-apps-with-graphql");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_githubwebhookswithrest()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\github-webhooks-with-rest");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_githubdesktop()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\githubdesktop");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_GitStatusBg()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\GitStatusBg");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_hicknhacksemanticcolorizer()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\hicknhack-semantic-colorizer");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_HotTips()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\HotTips");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_Identity()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\Identity");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_imagecomments()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\image-comments");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_ImageOptimizer()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\ImageOptimizer");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_Infragisticsunosamples()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\Infragistics-uno-samples");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_InvokeNavigationViewItems()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\InvokeNavigationViewItems");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_IssuesHub()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\IssuesHub");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_janczizikowsleek()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\janczizikow-sleek");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_jcansdaleLocalizeVsix()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\jcansdale-LocalizeVsix");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_kaxaml()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\kaxaml");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_LanguageRef()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\LanguageRef");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_LinkedVsctIssue()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\LinkedVsctIssue");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_LuisEntityHelpers()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\LuisEntityHelpers");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_madskristensenFileIcons()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\madskristensen-FileIcons");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_MahAppsMetro()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\MahApps.Metro");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_MahAppsMetroSimpleChildWindow()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\MahApps.Metro.SimpleChildWindow");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_Marb2000XamlIslands()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\Marb2000XamlIslands");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_MarkdownEditor()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\MarkdownEditor");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_michaelhawkerXmlSyntaxVisualizerUWP()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\michael-hawker-XmlSyntaxVisualizerUWP");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_microsoftuixaml()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\microsoft-ui-xaml");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_microsoftuixamlnumberbox()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\microsoft-ui-xaml-numberbox");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_microsoftuixamlspecs()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\microsoft-ui-xaml-specs");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_microsoftwts()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\microsoft-wts");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_mlltdUnoReference()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\mlltd-UnoReference");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_mlltdXamarinReference()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\mlltd-XamarinReference");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_mrlappmonkeys()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\mrl-app-monkeys");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_mrlmicrosoftuixaml()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\mrl-microsoft-ui-xaml");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_mrlVertiPaqAnalyzer()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\mrl-VertiPaq-Analyzer");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_mrlaceyRXT()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\mrlacey-RXT");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_MrlaceySnippets()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\MrlaceySnippets");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_MultiLineStringAnalyzer()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\MultiLineStringAnalyzer");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_mvegacaWindowsTemplateStudio()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\mvegaca-WindowsTemplateStudio");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_mvegacaWinUISamples()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\mvegaca-WinUISamples");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_MVVMSamples()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\MVVM-Samples");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_MVVMBasicSnippets()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\MVVMBasicSnippets");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_MvvmCross()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\MvvmCross");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_MyDotNetCoreWpfApp()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\MyDotNetCoreWpfApp");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_natemcmastermsbuildtasks()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\natemcmaster-msbuild-tasks");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_natemcmasterYarnMSBuild()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\natemcmaster-Yarn.MSBuild");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_PagesTest()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\PagesTest");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_PowerToys()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\PowerToys");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_Prism()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\Prism");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_PrivateWindowsTemplateStudio()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\Private-WindowsTemplateStudio");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_ProjectReunionSamples()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\Project-Reunion-Samples");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_ProjectReunion()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\ProjectReunion");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_ProWinuiTemplates()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\ProWinuiTemplates");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_radeonasmtools()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\radeon-asm-tools");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_RandomXAMLFiles()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\Random XAML Files");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_RapidXamlDemos()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\RapidXaml-Demos");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_RapidXamlDev()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\RapidXamlDev");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_RealTimeGraphX()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\RealTimeGraphX");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_ReproTreeView()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\ReproTreeView");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_ResPsuedoLoc()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\ResPsuedoLoc");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_rxtdemos()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\rxt-demos");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_ShowKeys()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\ShowKeys");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_ShowMeTheXAML()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\ShowMeTheXAML");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_ShowSelection()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\ShowSelection");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_ShowTheShortcut()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\ShowTheShortcut");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_SignVsix()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\SignVsix");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_SimpleJsonAnalyzer()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\SimpleJsonAnalyzer");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_SimpleTesting()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\SimpleTesting");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_SlnFileDiff()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\SlnFileDiff");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_SmartHotel360()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\SmartHotel360");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_sqlbiBravo()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\sqlbi-Bravo");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_StringResourceVisualizer()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\StringResourceVisualizer");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_tempwts3206()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\temp-wts3206");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_tempwts3206e()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\temp-wts3206e");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_terminal()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\terminal");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_testtemplates()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\test-templates");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_tryconvert()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\try-convert");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_TryConvertXaml()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\TryConvertXaml");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_Uno()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\Uno");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_unobook()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\uno-book");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_UnoPlayground()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\Uno.Playground");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_UnoSkiaSharpExtended()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\Uno.SkiaSharp.Extended");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_unobookcodepoc()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\unobookcodepoc");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_UnoDeployTest()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\UnoDeployTest");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_unoworkshops()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\unoworkshops");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_UWPMVVMToolkitSample()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\UWP-MVVM-Toolkit-Sample");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_UWPCommunityToolkit()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\UWPCommunityToolkit");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_UWPCommunityToolkitDocs()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\UWPCommunityToolkitDocs");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_UwpDesignTimeData()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\UwpDesignTimeData");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_UwpDevTidy()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\UwpDevTidy");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_UwpEssentials()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\UwpEssentials");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_VBUWPTemplates()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\VB-UWP-Templates");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_VBStyleAnalyzer()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\VBStyleAnalyzer");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_VertiPaqAnalyzer()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\VertiPaq-Analyzer");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_VincentHNetCSharpForMarkup()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\VincentH-Net-CSharpForMarkup");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_visor()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\visor");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_VisualStudioOutputFilterExtension()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\VisualStudio-Output-Filter-Extension");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_vscodegitlens()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\vscode-gitlens");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_VSConsole()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\VSConsole");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_VsctIntellisense()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\VsctIntellisense");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_VsEditorExtensions()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\VsEditorExtensions");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_VsixCommon()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\VsixCommon");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_VsixTools()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\VsixTools");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_VSSDKExtensibilitySamples()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\VSSDK-Extensibility-Samples");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_VSWaterMark()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\VSWaterMark");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_WarnAboutTodos()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\WarnAboutTodos");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_WCTMVVMSamples()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\WCT-MVVM-Samples");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_Win2DSamples()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\Win2D-Samples");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_Windowsappsamplecustomersordersdatabase()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\Windows-appsample-customers-orders-database");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_Windowsappsamplelunchscheduler()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\Windows-appsample-lunch-scheduler");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_WindowsappsampleXamlHosting()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\Windows-appsample-Xaml-Hosting");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_windowsuwp()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\windows-uwp");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_WindowsCommunityToolkitwiki()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\WindowsCommunityToolkit-wiki");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_WindowsTemplateStudio()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\WindowsTemplateStudio");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_WindowsTestHelpers()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\WindowsTestHelpers");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_winforms()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\winforms");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_wingetpkgs()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\winget-pkgs");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_WinUI3Demos()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\WinUI-3-Demos");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_WinUIEssentials()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\WinUI-Essentials");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_wpf()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\wpf");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_WTSXamarinFormsPreview()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\WTS-Xamarin-Forms-Preview");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_WTSXamarinFormsPreviewfork()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\WTS-Xamarin-Forms-Preview-fork");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_wts3206d()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\wts3206d");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_wts3206e()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\wts3206e");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_xamarinformssamples()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\xamarin-forms-samples");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_XamarinEssentials()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\Xamarin.Essentials");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_XamarinForms()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\Xamarin.Forms");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_XamarinFormsUWPShellSample()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\Xamarin.Forms.UWPShell.Sample");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_XamarinCommunityToolkit()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\XamarinCommunityToolkit");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_xamlbindingtool()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\xaml-binding-tool");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_XamlControlsGallery()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\Xaml-Controls-Gallery");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_xamldesignerextensibilitysamples()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\xaml-designer-extensibility-samples");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_XamlBenchmark()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\XamlBenchmark");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_XamlIntellisenseLimitationExample()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\XamlIntellisenseLimitationExample");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_XamlIslandBlogPostSample()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\XamlIslandBlogPostSample");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_xunit()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\xunit");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_ZenCodingVS()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\ZenCodingVS");
        }

        [TestMethod]
        public async Task TestCodeFilesIn_miscFiles()
        {
            await this.CanParseWithoutErrors(@"C:\Users\matt\Documents\GitHub\_miscFiles");
        }

        private static IEnumerable<string> GetCodeFiles(string folder)
        {
            foreach (var file in Directory.GetFiles(folder, "*.cs", SearchOption.AllDirectories))
            {
                yield return file;
            }

            foreach (var file in Directory.GetFiles(folder, "*.vb", SearchOption.AllDirectories))
            {
                yield return file;
            }
        }

        private async Task CanParseWithoutErrors(string folderPath)
        {
            var profile = RapidXamlToolkit.Tests.TestProfile.CreateEmpty();
            profile.ClassGrouping = "Grid";
            profile.FallbackOutput = "<TextBlock Text=\"$name$\" />";

            var logger = new RecordingTestLogger();

            // DropHandlerLogic already has everything needed to parse a file so reuse that for testing.
            var dhl = new DropHandlerLogic(
                logger,
                new HybridTestVisualStudioAbstraction(),
                new WindowsFileSystem(),
                profile);

            bool anyFailures = false;

            foreach (var filePath in GetCodeFiles(folderPath))
            {
                string output;

                if (filePath.Contains("\\obj\\") ||
                    filePath.Contains("\\AssemblyInfo.") ||
                    filePath.Contains("GlobalSuppressions.") ||
                    filePath.Contains("_postaction.") ||
                    filePath.Contains("_gpostaction."))
                {
                    continue;
                }

                try
                {
                    Debug.WriteLine($"Attempting to parse '{filePath}'.");

                    output = await dhl.ExecuteAsync(filePath, 0, ProjectType.Any);

                    if (output is null)
                    {
                        this.TestContext.WriteLine($"No output after parsing '{filePath}'");
                        this.TestContext.AddResultFile(filePath);

                        var lastLogMsg = logger.Info.Last();

                        if (!lastLogMsg.StartsWith("Unable to find class definition in file")
                         && !lastLogMsg.Equals("No properties to provide output for."))
                        {
                            anyFailures = true;
                        }
                    }
                }
                catch (Exception exc)
                {
                    this.TestContext.WriteLine($"Found error while parsing '{filePath}'{Environment.NewLine}{exc.Message}");
                    this.TestContext.AddResultFile(filePath);
                    anyFailures = true;
                }
            }

            Assert.IsFalse(anyFailures);
        }
    }
}
