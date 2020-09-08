// Copyright (c) Matt Lacey Ltd. All rights reserved.
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
                    var vsa = new TestVisualStudioAbstraction();
                    var logger = DefaultTestLogger.Create();

                    var processors = RapidXamlDocument.GetAllProcessors(ProjectType.Any, string.Empty, vsa, logger);

                    var customProcessor = new CustomProcessorWrapper(new StubCustomAnalysisProcessor(), ProjectType.Any, string.Empty, logger, vsa);

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

                    XamlElementExtractor.Parse(filePath, snapshot, text, processors, result.Tags, null, null, logger);

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
