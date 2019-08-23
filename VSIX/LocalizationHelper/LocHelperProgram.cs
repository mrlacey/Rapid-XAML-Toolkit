// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace LocalizationHelper
{
#pragma warning disable SA1649 // File name should match first type name
    internal class Program
#pragma warning restore SA1649 // File name should match first type name
    {
        private static readonly List<string> Cultures = new List<string> { "cs-CZ", "de-DE", "es-ES", "fr-FR", "it-IT", "ja-JP", "ko-KR", "pl-PL", "pt-BR", "ru-RU", "tr-TR", "zh-CN", "zh-TW" };

        private static void Main(string[] args)
        {
            Console.WriteLine("Rapid XAML Toolkit - Localization Helper");
            Console.WriteLine("----------------------------------------");
            Console.WriteLine();
            Console.WriteLine("Select an option");
            Console.WriteLine("1. Copy values from StringRes to other loc files");
            Console.WriteLine("2. Copy from StringRes to StringRes.en-US");
            Console.WriteLine("3. Extract new entries for localization");
            Console.WriteLine("4. Merge translated files");
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
                    ExtractNewEntriesForLocalization();
                    break;
                case ConsoleKey.D4:
                    MergeTranslatedFiles();
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

            foreach (var culture in Cultures)
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

        private static void ExtractNewEntriesForLocalization()
        {
            var basePath = "../../../RapidXamlToolkit/Resources/StringRes.{0}.resx";
            var enUsResxPath = string.Format(basePath, "en-US");

            var enusXdoc = new XmlDocument();
            enusXdoc.Load(enUsResxPath);

            var newFiles = new List<string>();

            foreach (var culture in Cultures)
            {
                Console.WriteLine();
                Console.WriteLine($"Procesing {culture}");

                var locResxPath = string.Format(basePath, culture);

                var locXdoc = new XmlDocument();
                locXdoc.Load(locResxPath);

                var locElements = locXdoc.GetElementsByTagName("data");

                // var locXdocNodes = locElements.Cast<XmlElement>().Select(n => n).ToList();
                var locXdocNodes = locElements.Cast<XmlElement>().Select(n => n).ToList();

                var newElements = new List<XmlElement>();

                foreach (XmlElement element in enusXdoc.GetElementsByTagName("data"))
                {
                    var elementName = element.GetAttribute("name");

                    if (!locXdocNodes.Any(n => n.GetAttribute("name") == elementName))
                    {
                        Console.WriteLine($"Need entry - {elementName}");
                        newElements.Add(element);
                    }
                }

                if (newElements.Any())
                {
                    var outputDoc = new XmlDocument();

                    XmlDeclaration xmlDeclaration = outputDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
                    XmlElement root = outputDoc.DocumentElement;
                    outputDoc.InsertBefore(xmlDeclaration, root);

                    XmlElement rootElement = outputDoc.CreateElement(string.Empty, "root", string.Empty);
                    outputDoc.AppendChild(rootElement);

                    foreach (var newElement in newElements)
                    {
                        var toAppend = outputDoc.CreateElement(string.Empty, newElement.Name, string.Empty);
                        toAppend.SetAttribute("name", newElement.GetAttribute("name"));
                        toAppend.SetAttribute("xml:space", newElement.GetAttribute("xml:space"));

                        var valueElement = outputDoc.CreateElement(string.Empty, "value", string.Empty);
                        valueElement.InnerText = newElement.SelectSingleNode("value").InnerText;
                        toAppend.AppendChild(valueElement);

                        var commentNode = newElement.SelectSingleNode("comment");

                        if (commentNode != null)
                        {
                            var cmntElement = outputDoc.CreateElement(string.Empty, "value", string.Empty);
                            cmntElement.InnerText = commentNode.InnerText;
                            toAppend.AppendChild(cmntElement);
                        }

                        rootElement.AppendChild(toAppend);
                    }

                    var newFileName = locResxPath.Replace(".resx", ".translation-needed.resx");

                    outputDoc.Save(newFileName);

                    newFiles.Add(newFileName);
                }
            }

            Console.WriteLine();

            if (newFiles.Any())
            {
                Console.WriteLine("The following files need translation:");

                foreach (var newFile in newFiles)
                {
                    Console.WriteLine($"- {newFile}");
                }

                Console.WriteLine("Once translated, rename to 'xxx.translation-done.resx' and run option 4.");
            }
            else
            {
                Console.WriteLine("No new files to translate");
            }
        }

        private static void MergeTranslatedFiles()
        {
            var basePath = "../../../RapidXamlToolkit/Resources/StringRes.{0}.resx";

            foreach (var culture in Cultures)
            {
                Console.WriteLine();
                Console.WriteLine($"Checking {culture}");

                var translatedPath = string.Format(basePath, $"{culture}.translation-done");

                if (File.Exists(translatedPath))
                {
                    var translatedXdoc = new XmlDocument();
                    translatedXdoc.Load(translatedPath);

                    var mainPath = string.Format(basePath, culture);

                    var mainXdoc = new XmlDocument();
                    mainXdoc.Load(mainPath);

                    foreach (XmlNode node in translatedXdoc.DocumentElement.ChildNodes)
                    {
                        XmlNode imported = mainXdoc.ImportNode(node, true);
                        mainXdoc.DocumentElement.AppendChild(imported);
                    }

                    mainXdoc.Save(mainPath);
                    File.Delete(translatedPath);

                    Console.WriteLine($"Merged {translatedPath}");
                }
                else
                {
                    Console.WriteLine("Nothing to merge");
                }
            }
        }
    }
}
