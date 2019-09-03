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

                    XamlElementExtractor.Parse(ProjectType.Any, filePath, snapshot, text, RapidXamlDocument.GetAllProcessors(ProjectType.Any), result.Tags);

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
