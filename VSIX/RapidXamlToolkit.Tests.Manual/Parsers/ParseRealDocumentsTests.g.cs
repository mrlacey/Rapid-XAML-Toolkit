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
