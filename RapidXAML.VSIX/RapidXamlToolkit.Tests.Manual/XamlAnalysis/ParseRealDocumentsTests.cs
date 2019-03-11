// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidXamlToolkit.XamlAnalysis;

namespace RapidXamlToolkit.Tests.Manual.XamlAnalysis
{
    [TestClass]
    public class ParseRealDocumentsTests
    {
        public TestContext TestContext { get; set; }

        public static IEnumerable<object[]> GetXamlFiles()
        {
            // Change this to something appropriate on teh machine you're running this test on.
            const string rootFolder = @"C:\Users\matt\Documents\GitHub\calculator";

            foreach (var file in Directory.GetFiles(rootFolder, "*.xaml", SearchOption.AllDirectories))
            {
                yield return new object[] { file };
            }
        }

        [DataTestMethod]
        [DynamicData(nameof(GetXamlFiles), DynamicDataSourceType.Method)]
        public void CanParseWithoutErrors(string filePath)
        {
            var result = new RapidXamlDocument();

            var text = File.ReadAllText(filePath);

            if (text.IsValidXml())
            {
                result.RawText = text;

                var snapshot = new RapidXamlToolkit.Tests.FakeTextSnapshot();

                XamlElementExtractor.Parse(snapshot, text, RapidXamlDocument.GetAllProcessors(), result.Tags);

                Debug.WriteLine($"Found {result.Tags.Count} taggable issues in '{filePath}'.");

                if (result.Tags.Count > 0)
                {
                    if (result.Tags.Count > 10)
                    {
                      // Debugger.Break();
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
