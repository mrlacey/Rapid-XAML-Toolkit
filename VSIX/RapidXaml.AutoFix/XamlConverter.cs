// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RapidXamlToolkit;
using RapidXamlToolkit.Commands;
using RapidXamlToolkit.Tests;
using RapidXamlToolkit.XamlAnalysis;
using RapidXamlToolkit.XamlAnalysis.Processors;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXaml
{
    public class XamlConverter
    {
        public XamlConverter()
        {
            this.FileSystem = new WindowsFileSystem();
        }

#if DEBUG
        // This overload exists in debug for test use only
        public XamlConverter(IFileSystemAbstraction fsa)
        {
            this.FileSystem = fsa ?? new WindowsFileSystem();
        }
#endif

        private IFileSystemAbstraction FileSystem { get; set; }

        public (bool success, IEnumerable<string> details) ConvertFile(string xamlFilePath, IEnumerable<ICustomAnalyzer> analyzers)
        {
            if (!this.FileSystem.FileExists(xamlFilePath))
            {
                return (false, new[] { "File does not exist." });
            }

            if (Path.GetExtension(xamlFilePath).ToLowerInvariant() != ".xaml")
            {
                return (false, new[] { "File must have a .xaml extension." });
            }

            if (!analyzers.Any())
            {
                return (false, new[] { "No analyzers provided. Without these no changes can be made." });
            }

            var text = this.FileSystem.GetAllFileText(xamlFilePath);

            var output = new List<string>
            {
                $"Analyzing '{xamlFilePath}'",
            };

            var snapshot = new FakeTextSnapshot();

            var logger = EmptyLogger.Create();
            var vsAbstraction = new AutoFixVisualStudioAbstraction();

            try
            {
                var processors = new List<(string, XamlElementProcessor)>();

                foreach (var analyzer in analyzers)
                {
                    output.Add($"Preparing to analyze '{analyzer.TargetType()}'.");

                    processors.Add(
                        (analyzer.TargetType(),
                         new CustomProcessorWrapper(analyzer, ProjectType.Any, string.Empty, logger, vsAbstraction)));
                }

                var tags = new TagList();

                XamlElementExtractor.Parse(ProjectType.Any, "Generic.xaml", snapshot, text, processors, tags, vsAbstraction, skipEveryElementProcessor: true);

                output.Add($"Found {tags.Count} places to make changes.");

                foreach (var tag in tags)
                {
                    // base this on CustomAnalysisAction.InnerExecute
                    // but need a new VsAbstraction (that doesn't need to worry about line number offset)
                    // This always should be a CustomAnalysisTag but doesn't hurt to check when casting.
                    if (tag is CustomAnalysisTag cat)
                    {
                        switch (cat.Action)
                        {
                            case ActionType.AddAttribute:
                                break;
                            case ActionType.AddChild:
                                break;
                            case ActionType.HighlightWithoutAction:
                                // NOOP - Nothing to fix here
                                break;
                            case ActionType.RemoveAttribute:
                                break;
                            case ActionType.RemoveChild:
                                break;
                            case ActionType.RenameElement:

                                var orig = cat.AnalyzedElement.OriginalString;

                                // Note that cat.Span is for the name of the element in the opening tag
                                if (orig.EndsWith("/>"))
                                {
                                    text = text.Substring(0, cat.Span.Start) + cat.Name + text.Substring(cat.Span.End);
                                }
                                else
                                {
                                    var startNameEnd = cat.ElementName.Length + 1;
                                    var endNameStart = orig.LastIndexOf("</");

                                    var updated = $"<{cat.Name}" + orig.Substring(startNameEnd, endNameStart - startNameEnd) + $"</{cat.Name}>";
                                    text = text.Substring(0, cat.AnalyzedElement.Location.Start) + updated + text.Substring(cat.AnalyzedElement.Location.Start + cat.AnalyzedElement.Location.Length);
                                }

                                break;
                            case ActionType.ReplaceElement:
                                break;
                            default:
                                break;
                        }
                    }
                }

                this.FileSystem.WriteAllFileText(xamlFilePath, text);
                return (true, output);
            }
            catch (Exception exc)
            {
                output.Add(exc.Message);
                return (false, output);
            }
        }

        public (bool success, List<string> details) ConvertAllFilesInProject(string projectFilePath, IEnumerable<ICustomAnalyzer> analyzers)
        {
            throw new NotImplementedException("TODO");
        }
    }
}
