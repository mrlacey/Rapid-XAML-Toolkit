// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidXaml;

namespace RapidXamlToolkit.Tests.AutoFix
{
    [TestClass]
    public class ProjectTests
    {
        [TestMethod]
        public void AllFilesAreAnalyzed()
        {
            var xamlFile1 = @"<Page>
    <WebView />
</Page>";
            var xamlFile2 = @"<Page>
    <Grid />
</Page>";
            var xamlFile3 = @"<Page>
    <WebView></WebView>
</Page>";

            var projFile = @"<?xml version=""1.0"" encoding=""utf-8""?>
<Project ToolsVersion=""15.0"" DefaultTargets=""Build"" xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"">
  <Import Project=""$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props"" Condition=""Exists('$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props')"" />
  <PropertyGroup>
    <Configuration Condition="" '$(Configuration)' == '' "">Debug</Configuration>
    <Platform Condition="" '$(Platform)' == '' "">x86</Platform>
    <ProjectGuid>{5B9F660F-7966-4AF6-8B83-9A6CB2E81FF4}</ProjectGuid>
    <OutputType>AppContainerExe</OutputType>
    <AppDesignerFolder>Properties</AppDesignerFolder>
    <RootNamespace>BlankWinuiUwpApp</RootNamespace>
    <AssemblyName>BlankWinuiUwpApp</AssemblyName>
    <DefaultLanguage>en-US</DefaultLanguage>
    <TargetPlatformIdentifier>UAP</TargetPlatformIdentifier>
    <TargetPlatformVersion Condition="" '$(TargetPlatformVersion)' == '' "">10.0.19041.0</TargetPlatformVersion>
    <TargetPlatformMinVersion>10.0.17763.0</TargetPlatformMinVersion>
    <MinimumVisualStudioVersion>16</MinimumVisualStudioVersion>
    <FileAlignment>512</FileAlignment>
    <ProjectTypeGuids>{A5A43C5B-DE2A-4C0C-9213-0A381AF9435A};{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}</ProjectTypeGuids>
    <WindowsXamlEnableOverview>true</WindowsXamlEnableOverview>
    <IsWinUIAlpha Condition=""'$(IsWinUIAlpha)' == ''"">true</IsWinUIAlpha>
    <WindowsKitsPath Condition=""'$(IsWinUIAlpha)' == 'true'"">WinUI-Alpha-Projects-Don-t-Use-SDK-Xaml-Tools</WindowsKitsPath>
    <PackageCertificateKeyFile>BlankWinuiUwpApp_TemporaryKey.pfx</PackageCertificateKeyFile>
  </PropertyGroup>
  <PropertyGroup Condition=""'$(Configuration)|$(Platform)' == 'Debug|x86'"">
    <DebugSymbols>true</DebugSymbols>
    <OutputPath>bin\x86\Debug\</OutputPath>
    <DefineConstants>DEBUG;TRACE;NETFX_CORE;WINDOWS_UWP</DefineConstants>
    <DebugType>full</DebugType>
    <PlatformTarget>x86</PlatformTarget>
    <UseVSHostingProcess>false</UseVSHostingProcess>
    <ErrorReport>prompt</ErrorReport>
    <Prefer32Bit>true</Prefer32Bit>
  </PropertyGroup>
  <PropertyGroup Condition=""'$(Configuration)|$(Platform)' == 'Release|x86'"">
    <OutputPath>bin\x86\Release\</OutputPath>
    <DefineConstants>TRACE;NETFX_CORE;WINDOWS_UWP</DefineConstants>
    <Optimize>true</Optimize>
    <DebugType>pdbonly</DebugType>
    <PlatformTarget>x86</PlatformTarget>
    <UseVSHostingProcess>false</UseVSHostingProcess>
    <ErrorReport>prompt</ErrorReport>
    <Prefer32Bit>true</Prefer32Bit>
    <UseDotNetNativeToolchain>true</UseDotNetNativeToolchain>
  </PropertyGroup>
  <PropertyGroup Condition=""'$(Configuration)|$(Platform)' == 'Debug|x64'"">
    <DebugSymbols>true</DebugSymbols>
    <OutputPath>bin\x64\Debug\</OutputPath>
    <DefineConstants>DEBUG;TRACE;NETFX_CORE;WINDOWS_UWP</DefineConstants>
    <DebugType>full</DebugType>
    <PlatformTarget>x64</PlatformTarget>
    <UseVSHostingProcess>false</UseVSHostingProcess>
    <ErrorReport>prompt</ErrorReport>
    <Prefer32Bit>true</Prefer32Bit>
  </PropertyGroup>
  <PropertyGroup Condition=""'$(Configuration)|$(Platform)' == 'Release|x64'"">
    <OutputPath>bin\x64\Release\</OutputPath>
    <DefineConstants>TRACE;NETFX_CORE;WINDOWS_UWP</DefineConstants>
    <Optimize>true</Optimize>
    <DebugType>pdbonly</DebugType>
    <PlatformTarget>x64</PlatformTarget>
    <UseVSHostingProcess>false</UseVSHostingProcess>
    <ErrorReport>prompt</ErrorReport>
    <Prefer32Bit>true</Prefer32Bit>
    <UseDotNetNativeToolchain>true</UseDotNetNativeToolchain>
  </PropertyGroup>
  <PropertyGroup>
    <RestoreProjectStyle>PackageReference</RestoreProjectStyle>
  </PropertyGroup>
  <ItemGroup>
    <Compile Include=""App.xaml.cs"">
      <DependentUpon>App.xaml</DependentUpon>
    </Compile>
    <Compile Include=""MainPage.xaml.cs"">
      <DependentUpon>MainPage.xaml</DependentUpon>
    </Compile>
    <Compile Include=""Page1.xaml.cs"">
      <DependentUpon>Page1.xaml</DependentUpon>
    </Compile>
    <Compile Include=""Page2.xaml.cs"">
      <DependentUpon>Page2.xaml</DependentUpon>
    </Compile>
    <Compile Include=""Page3.xaml.cs"">
      <DependentUpon>Page3.xaml</DependentUpon>
    </Compile>
    <Compile Include=""Properties\AssemblyInfo.cs"" />
  </ItemGroup>
  <ItemGroup>
    <AppxManifest Include=""Package.appxmanifest"">
      <SubType>Designer</SubType>
    </AppxManifest>
    <None Include=""BlankWinuiUwpApp_TemporaryKey.pfx"" />
  </ItemGroup>
  <ItemGroup>
    <Content Include=""Properties\Default.rd.xml"" />
    <Content Include=""Assets\LockScreenLogo.scale-200.png"" />
    <Content Include=""Assets\SplashScreen.scale-200.png"" />
    <Content Include=""Assets\Square150x150Logo.scale-200.png"" />
    <Content Include=""Assets\Square44x44Logo.scale-200.png"" />
    <Content Include=""Assets\Square44x44Logo.targetsize-24_altform-unplated.png"" />
    <Content Include=""Assets\StoreLogo.png"" />
    <Content Include=""Assets\Wide310x150Logo.scale-200.png"" />
  </ItemGroup>
  <ItemGroup>
    <ApplicationDefinition Include=""App.xaml"">
      <Generator>MSBuild:Compile</Generator>
    </ApplicationDefinition>
    <Page Include=""MainPage.xaml"">
      <Generator>MSBuild:Compile</Generator>
    </Page>
    <Page Include=""Page1.xaml"">
      <SubType>Designer</SubType>
      <Generator>MSBuild:Compile</Generator>
    </Page>
    <Page Include=""Page2.xaml"">
      <SubType>Designer</SubType>
      <Generator>MSBuild:Compile</Generator>
    </Page>
    <Page Include=""Page3.xaml"">
      <SubType>Designer</SubType>
      <Generator>MSBuild:Compile</Generator>
    </Page>
  </ItemGroup>
  <ItemGroup>
    <PackageReference Include=""Microsoft.NETCore.UniversalWindowsPlatform"">
      <Version>6.2.10</Version>
    </PackageReference>
    <PackageReference Include=""Microsoft.WinUI"">
      <Version>3.0.0-preview2.200713.0</Version>
    </PackageReference>
  </ItemGroup>
  <PropertyGroup Condition="" '$(VisualStudioVersion)' == '' or '$(VisualStudioVersion)' &lt; '$(MinimumVisualStudioVersion)' "">
    <VisualStudioVersion>$(MinimumVisualStudioVersion)</VisualStudioVersion>
  </PropertyGroup>
  <Import Project=""$(MSBuildExtensionsPath)\Microsoft\WindowsXaml\v$(VisualStudioVersion)\Microsoft.Windows.UI.Xaml.CSharp.targets"" />
</Project>";

            var expectedXaml1 = @"<Page>
    <WebView2 Source=""https://rapidxaml.dev/"" />
</Page>";

            var expectedXaml2 = @"<Page>
    <Grid />
</Page>";

            var expectedXaml3 = @"<Page>
    <WebView2 Source=""https://rapidxaml.dev/""></WebView2>
</Page>";

            var fs = new BespokeTestFileSystem
            {
                FileExistsResponse = true,
                FileLines = projFile.Split(new[] { Environment.NewLine }, StringSplitOptions.None),
            };

            fs.FilesAndContents.Add("MyApp.csproj", projFile);
            fs.FilesAndContents.Add("Page1.xaml", xamlFile1);
            fs.FilesAndContents.Add("Page2.xaml", xamlFile2);
            fs.FilesAndContents.Add("Page3.xaml", xamlFile3);
#if DEBUG
            var sut = new XamlConverter(fs);

            var (success, details) = sut.ConvertAllFilesInProject("MyApp.csproj", new[] { new WebViewMultipleActionsAnalyzer() });

            Assert.AreEqual(true, success);
            Assert.IsTrue(details.Count() > 3);
            Assert.AreEqual(expectedXaml1, fs.WrittenFiles["Page1.xaml"]);
            Assert.AreEqual(expectedXaml2, fs.WrittenFiles["Page2.xaml"]);
            Assert.AreEqual(expectedXaml3, fs.WrittenFiles["Page3.xaml"]);
#endif
        }

        public class BespokeTestFileSystem : TestFileSystem
        {
            public Dictionary<string, string> FilesAndContents { get; set; } = new Dictionary<string, string>();

            public Dictionary<string, string> WrittenFiles { get; set; } = new Dictionary<string, string>();

            public string[] FileLines { get; set; }

            public override string GetAllFileText(string fileName)
            {
                if (this.FilesAndContents.ContainsKey(fileName))
                {
                    return this.FilesAndContents[fileName];
                }
                else
                {
                    return string.Empty;
                }
            }

            public override void WriteAllFileText(string fileName, string fileContents)
            {
                this.WrittenFiles.Add(fileName, fileContents);
            }

            public override string[] ReadAllLines(string path)
            {
                return this.FileLines;
            }
        }

        public class WebViewMultipleActionsAnalyzer : ICustomAnalyzer
        {
            public string TargetType() => "WebView";

            public AnalysisActions Analyze(RapidXamlElement element, ExtraAnalysisDetails extraDetails)
            {
                var result = AutoFixAnalysisActions.RenameElement("WebView2");

                result.AndAddAttribute("Source", "https://rapidxaml.dev/");

                return result;
            }
        }
    }
}
