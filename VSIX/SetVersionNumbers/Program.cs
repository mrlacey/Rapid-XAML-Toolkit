// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace SetVersionNumbers
{
    class Program
    {
        private static List<string> AssemblyInfoFiles = new List<string>
        {
            "../../../../RapidXaml.Analysis/Properties/AssemblyInfo.cs",
            "../../../../RapidXaml.Common/Properties/AssemblyInfo.cs",
            "../../../../RapidXaml.CustomAnalysis/Properties/AssemblyInfo.cs",
            "../../../../RapidXaml.Generation/Properties/AssemblyInfo.cs",
            "../../../../RapidXaml.RoslynAnalyzers/Properties/AssemblyInfo.cs",
            "../../../../RapidXaml.Shared/Properties/AssemblyInfo.cs",
            "../../../../RapidXamlToolkit/Properties/AssemblyInfo.cs",
        };

        private static List<string> PackageFiles = new List<string>
        {
            "../../../../RapidXaml.Analysis/RapidXamlAnalysisPackage.cs",
            "../../../../RapidXaml.Generation/RapidXamlGenerationPackage.cs",
            "../../../../RapidXaml.RoslynAnalyzers/RapidXamlRoslynAnalyzersPackage.cs",
            "../../../../RapidXamlToolkit/RapidXamlPackage.cs",
        };

        private static List<string> VsixManifestFiles = new List<string>
        {
            "../../../../RapidXaml.Analysis/source.extension.vsixmanifest",
            "../../../../RapidXaml.Common/source.extension.vsixmanifest",
            "../../../../RapidXaml.Generation/source.extension.vsixmanifest",
            "../../../../RapidXaml.RoslynAnalyzers/source.extension.vsixmanifest",
            "../../../../RapidXamlToolkit/source.extension.vsixmanifest",
        };

        private static List<string> NuSpecFiles = new List<string>
        {
            "../../../../RapidXaml.CustomAnalysis/RapidXamlCustomAnalysis.nuspec",
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
            SetNuSpecVersions(versionNo);
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

                xmlDoc.Save(manifestFile);
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

                xmlDoc.Save(nuspecFile);
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

            File.WriteAllLines(inFile, newLineContents);
        }
    }
}
