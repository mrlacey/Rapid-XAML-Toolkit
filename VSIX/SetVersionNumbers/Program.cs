// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace SetVersionNumbers
{
    class Program
    {
        private static readonly List<string> AssemblyInfoFiles = new List<string>
        {
            "../../../../RapidXaml.Analysis/Properties/AssemblyInfo.cs",
            "../../../../RapidXaml.AutoFix/Properties/AssemblyInfo.cs",
            "../../../../RapidXaml.Common/Properties/AssemblyInfo.cs",
            "../../../../RapidXaml.CustomAnalysis/Properties/AssemblyInfo.cs",
            "../../../../RapidXaml.Generation/Properties/AssemblyInfo.cs",
            "../../../../RapidXaml.RoslynAnalyzers/Properties/AssemblyInfo.cs",
            "../../../../RapidXaml.Shared/Properties/AssemblyInfo.cs",
            "../../../../RapidXamlToolkit/Properties/AssemblyInfo.cs",
            "../../../../../Templates/RapidXaml.Templates/Properties/AssemblyInfo.cs",
        };

        private static readonly List<string> PackageFiles = new List<string>
        {
            "../../../../RapidXaml.Analysis/RapidXamlAnalysisPackage.cs",
            "../../../../RapidXaml.Generation/RapidXamlGenerationPackage.cs",
            "../../../../RapidXaml.RoslynAnalyzers/RapidXamlRoslynAnalyzersPackage.cs",
            "../../../../RapidXamlToolkit/RapidXamlPackage.cs",
            "../../../../../Templates/RapidXaml.Templates/RapidXamlTemplatesPackage.cs",
        };

        private static readonly List<string> VsixManifestFiles = new List<string>
        {
            "../../../../RapidXaml.Analysis/source.extension.vsixmanifest",
            "../../../../RapidXaml.Common/source.extension.vsixmanifest",
            "../../../../RapidXaml.Generation/source.extension.vsixmanifest",
            "../../../../RapidXaml.RoslynAnalyzers/source.extension.vsixmanifest",
            "../../../../RapidXamlToolkit/source.extension.vsixmanifest",
            "../../../../../Templates/RapidXaml.Templates/source.extension.vsixmanifest",
        };

        private static readonly List<string> NuSpecFiles = new List<string>
        {
            "../../../../RapidXaml.AutoFix/RapidXaml.AutoFix.nuspec",
            "../../../../RapidXaml.CustomAnalysis/RapidXaml.CustomAnalysis.nuspec",
            "../../../../RapidXaml.BuildAnalysis/RapidXaml.BuildAnalysis.nuspec",
        };

        private static readonly List<string> NugetProjectFiles = new List<string>
        {
            "../../../../RapidXaml.AutoFix/RapidXaml.AutoFix.csproj",
            "../../../../RapidXaml.CustomAnalysis/RapidXaml.CustomAnalysis.csproj",
            "../../../../RapidXaml.BuildAnalysis/RapidXaml.BuildAnalysis.csproj",
        };

        private static readonly List<string> DependencyFiles = new List<string>
        {
            "../../../../../Templates/CustomAnalysisItemTemplate/CustomAnalysisItemTemplate.vstemplate",
            "../../../../../Templates/CustomAnalysisProjectTemplate/Analyzer/CustomAnalyzerProject.csproj",
            "../../../../../Templates/CustomAnalysisProjectTemplate/Analyzer/CustomAnalyzerProject.nuspec",
            "../../../../../Templates/CustomAnalysisProjectTemplate/AnalyzerTests/CustomAnalyzerProjectTests.csproj",
        };

        private static readonly List<string> ProjectFilesContainingVersionNumbers = new List<string>
        {
            "../../../../RapidXaml.AnalysisExe/RapidXaml.AnalysisExe.csproj",
        };

        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Pass the desired version number as the first and only argument.");
            }
            else
            {
                var verNo = args[0];
                SetVersionNumbers(verNo);
            }
        }

        private static void SetVersionNumbers(string versionNo)
        {
            Console.WriteLine("Settings version numbers to {0}", versionNo);

            SetAssemblyInfoNumbers(versionNo);
            SetPackageProductIds(versionNo);
            SetVsixManifestVersions(versionNo);
            SetNugetProjectVersions(versionNo);
            SetNuSpecVersions(versionNo);
            SetDependencyVersions(versionNo);
            SetVersionNumbersInProjectFiles(versionNo);
        }

        private static void SetVersionNumbersInProjectFiles(string versionNo)
        {
            foreach (var file in ProjectFilesContainingVersionNumbers)
            {
                var lines = File.ReadAllLines(file);
                var output = new List<string>();
                foreach (var line in lines)
                {
                    if (line.StartsWith("    <Version>"))
                    {
                        output.Add($"    <Version>{versionNo}</Version>");
                    }
                    else
                    {
                        output.Add(line);
                    }
                }

                File.WriteAllLines(file, output.ToArray());
            }
        }

        private static void SetDependencyVersions(string versionNo)
        {
            foreach (var file in DependencyFiles)
            {
                var lines = File.ReadAllLines(file);
                var output = new List<string>();
                foreach (var line in lines)
                {
                    if (line.Trim().StartsWith("<package id=\"RapidXaml.CustomAnalysis\""))
                    {
                        output.Add($"            <package id=\"RapidXaml.CustomAnalysis\" version=\"{versionNo}\" />");
                    }
                    else if (line.Trim().StartsWith("<PackageReference Include=\"RapidXaml.CustomAnalysis\" "))
                    {
                        output.Add($"        <PackageReference Include=\"RapidXaml.CustomAnalysis\" Version=\"{versionNo}\" />");
                    }
                    else if (line.Trim().StartsWith("<dependency id=\"RapidXaml.CustomAnalysis\" "))
                    {
                        output.Add($"                <dependency id=\"RapidXaml.CustomAnalysis\" version=\"{versionNo}\" exclude=\"Build,Analyzers\" />");
                    }
                    else
                    {
                        output.Add(line);
                    }
                }

                File.WriteAllLines(file, output.ToArray());
            }
        }

        private static void SetAssemblyInfoNumbers(string versionNo)
        {
            var parts = $"{versionNo}.0.0.0.0".Split('.', StringSplitOptions.RemoveEmptyEntries);

            var fourPartVersionNumber = $"{parts[0]}.{parts[1]}.{parts[2]}.{parts[3]}";

            foreach (var assInfoFile in AssemblyInfoFiles)
            {
                ReplaceLineStarting(
                            "[assembly: AssemblyVersion",
                            inFile: assInfoFile,
                            with: $"[assembly: AssemblyVersion(\"{fourPartVersionNumber}\")]");

                ReplaceLineStarting(
                            "[assembly: AssemblyFileVersion",
                            inFile: assInfoFile,
                            with: $"[assembly: AssemblyFileVersion(\"{fourPartVersionNumber}\")]");
            }
        }

        private static void SetPackageProductIds(string versionNo)
        {
            foreach (var packFile in PackageFiles)
            {
                ReplaceLineStarting(
                            "    [InstalledProductRegistration(",
                            inFile: packFile,
                            with: $"    [InstalledProductRegistration(\"#110\", \"#112\", \"{versionNo}\")] // Info on this package for Help/About");
            }
        }

        private static void SetVsixManifestVersions(string versionNo)
        {
            foreach (var manifestFile in VsixManifestFiles)
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.Load(manifestFile);
                var identity = xmlDoc.GetElementsByTagName("Identity");

                identity[0].Attributes["Version"].Value = versionNo;

                using var sw = new StreamWriter(manifestFile, false, Encoding.UTF8);
                xmlDoc.Save(sw);
            }
        }

        private static void SetNuSpecVersions(string versionNo)
        {
            foreach (var nuspecFile in NuSpecFiles)
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.Load(nuspecFile);
                var version = xmlDoc.GetElementsByTagName("version");

                version[0].InnerText = versionNo;

                using var sw = new StreamWriter(nuspecFile, false, Encoding.UTF8);
                xmlDoc.Save(sw);
            }
        }

        private static void SetNugetProjectVersions(string versionNo)
        {
            foreach (var nuspecFile in NugetProjectFiles)
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.Load(nuspecFile);
                var version = xmlDoc.GetElementsByTagName("Version");

                version[0].InnerText = versionNo;

                using var sw = new StreamWriter(nuspecFile, false, Encoding.UTF8);
                xmlDoc.Save(sw);
            }
        }

        private static void ReplaceLineStarting(string starting, string inFile, string with)
        {
            var lines = File.ReadAllLines(inFile);

            var newLineContents = new List<string>();

            foreach (var line in lines)
            {
                if (line.StartsWith(starting))
                {
                    newLineContents.Add(with);
                }
                else
                {
                    newLineContents.Add(line);
                }
            }

            File.WriteAllLines(inFile, newLineContents, Encoding.UTF8);
        }
    }
}
