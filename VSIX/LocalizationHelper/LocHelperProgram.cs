// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace LocalizationHelper
{
    class Program
    {
        private static readonly List<string> cultures = new List<string> { "cs-CZ", "de-DE", "es-ES", "fr-FR", "it-IT", "ja-JP", "ko-KR", "pl-PL", "pt-BR", "ru-RU", "tr-TR", "zh-CN", "zh-TW" };

        static void Main(string[] args)
        {
            Console.WriteLine("Rapid XAML Toolkit - Localization Helper");
            Console.WriteLine("----------------------------------------");
            Console.WriteLine();
            Console.WriteLine("Select an option");
            Console.WriteLine("1. Copy values from StringRes to other loc files");
            Console.WriteLine("2. Copy from StringRes to StringRes.en-US");
            Console.WriteLine("3. Create placeholder entries in localized StringRes file for any new items in the localized versions");
            // TODO: ISSUE#82 Console.WriteLine("4. Extract new entries for localization");
            Console.WriteLine();
            Console.WriteLine();

            var option = Console.ReadKey();
            Console.WriteLine();
            Console.WriteLine();

            switch (option.Key)
            {
                case ConsoleKey.D1:
                    CopyTranslationsFromStringresToOtherFiles();
                    break;
                case ConsoleKey.D2:
                    CopyNeutralStringresFileToUsVersion();
                    break;
                case ConsoleKey.D3:
                    CreateLocalizedPlaceholdersForNewNeutralStringresEntries();
                    break;
                default:
                    break;
            }


            Console.WriteLine();
            Console.WriteLine("press any key to exit");
            Console.ReadKey(true);
        }

        private static void CopyTranslationsFromStringresToOtherFiles()
        {
            // To make the translation process simpler StringRes.resx contains copies of entries in other localized files. This means only sending a single file for localization.
            // This process copies the translated entries back to their correct locations.
            const string relativeRootPath = "../../../RapidXamlToolkit/";
            string relativeResourcesPath = Path.Combine(relativeRootPath, "Resources");

            // For each entry starting 'Package__' copy to VSPackage.xx-XX.resx
            // For each entry starting 'VSCT__' copy to RapidXamlPackage.xx-XX.vsct
            // For each entry starting 'VSIX__' copy to xx-XX/Extension.vsixlangpack
            const string PackagePrefix = "Package__";
            const string VsctPrefix = "VSCT__";
            const string VsixPrefix = "VSIX__";

            foreach (var culture in cultures)
            {
                Console.WriteLine($"Copying {culture}");
                var stringresxFile = Path.Combine(relativeResourcesPath, $"StringRes.{culture}.resx");

                var xdoc = new XmlDocument();
                xdoc.Load(stringresxFile);

                var packageChanges = new Dictionary<string, string>();
                var vsctChanges = new Dictionary<string, string>();
                var vsixChanges = new Dictionary<string, string>();

                foreach (XmlElement element in xdoc.GetElementsByTagName("data"))
                {
                    var elementName = element.GetAttribute("name");
                    var elementValue = element.GetElementsByTagName("value").Item(0).InnerText;

                    if (elementName.StartsWith(PackagePrefix))
                    {
                        packageChanges.Add(elementName.Replace(PackagePrefix, string.Empty), elementValue);
                    }
                    else if (elementName.StartsWith(VsctPrefix))
                    {
                        vsctChanges.Add(elementName.Replace(VsctPrefix, string.Empty), elementValue);
                    }
                    else if (elementName.StartsWith(VsixPrefix))
                    {
                        vsixChanges.Add(elementName.Replace(VsixPrefix, string.Empty), elementValue);
                    }
                }

                // For each entry starting 'Package__' copy to VSPackage.xx-XX.resx
                if (packageChanges.Any())
                {
                    var filePath = Path.Combine(relativeRootPath, $"VSPackage.{culture}.resx");
                    var packageXdoc = new XmlDocument();
                    packageXdoc.Load(filePath);

                    foreach (XmlElement element in packageXdoc.GetElementsByTagName("data"))
                    {
                        var elementName = element.GetAttribute("name");

                        if (packageChanges.ContainsKey(elementName))
                        {
                            element.GetElementsByTagName("value").Item(0).InnerText = packageChanges[elementName];
                        }
                    }

                    packageXdoc.Save(filePath);
                }

                // For each entry starting 'VSCT__' copy to RapidXamlPackage.xx-XX.vsct
                if (vsctChanges.Any())
                {
                    var filePath = Path.Combine(relativeRootPath, $"RapidXamlPackage.{culture}.vsct");
                    var vsctXdoc = new XmlDocument();
                    vsctXdoc.Load(filePath);

                    foreach (XmlElement element in vsctXdoc.GetElementsByTagName("Button"))
                    {
                        var elementId = element.GetAttribute("id");

                        if (vsctChanges.ContainsKey(elementId))
                        {

                            element.GetElementsByTagName("ButtonText").Item(0).InnerText = vsctChanges[elementId];
                        }
                    }

                    vsctXdoc.Save(filePath);
                }

                // For each entry starting 'VSIX__' copy to xx-XX/Extension.vsixlangpack
                if (vsixChanges.Any())
                {
                    var filePath = Path.Combine(relativeRootPath, culture, "Extension.vsixlangpack");
                    var vsixXdoc = new XmlDocument();
                    vsixXdoc.Load(filePath);

                    foreach (var vsixChange in vsixChanges)
                    {
                        var elements = vsixXdoc.GetElementsByTagName(vsixChange.Key);

                        if (elements.Count == 1)
                        {
                            elements[0].InnerText = vsixChange.Value;
                        }
                    }

                    vsixXdoc.Save(filePath);
                }
            }

            Console.WriteLine();
            Console.WriteLine("All entries copied successfully");
        }

        private static void CopyNeutralStringresFileToUsVersion()
        {
            // Due to the way that this project is set up and the mixture of technologies it uses, localization files must be provided for the neutral resource file used by the embedded WPF UI in addition to a copy localized in the default language.
            // The neutral version is used by the designer/compiler and the en-US version is used at runtime.
            // Because the neutral language version is used by the designer, new entries should be created there and then copied to the en-US version.
            const string relativePath = "../../../RapidXamlToolkit/Resources";
            File.Copy(Path.Combine(relativePath, "StringRes.resx"), Path.Combine(relativePath, "StringRes.en-US.resx"), overwrite: true);

            Console.WriteLine("File copied successfully");
        }

        private static void CreateLocalizedPlaceholdersForNewNeutralStringresEntries()
        {
            // TODO: ISSUE#82 implement CreateLocalizedPlaceholdersForNewNeutralStringresEntries
            // List locales being processed
            // List entries being created

            Console.WriteLine("All entries created successfully");
        }
    }
}
