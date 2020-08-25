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
                    output.Add($"Will analyze instances of '{analyzer.TargetType()}'.");

                    processors.Add(
                        (analyzer.TargetType(),
                         new CustomProcessorWrapper(analyzer, ProjectType.Any, string.Empty, logger, vsAbstraction)));
                }

                var tags = new TagList();

                XamlElementExtractor.Parse(ProjectType.Any, "Generic.xaml", snapshot, text, processors, tags, vsAbstraction, skipEveryElementProcessor: true);

                var plural = tags.Count == 1 ? string.Empty : "s";
                output.Add($"Found {tags.Count} place{plural} to make changes.");

                tags.Reverse();  // Work back through the document to allow for modifications changing document length

                // TODO: consider adding an actiontype to include xmlns at top level of document

                // TODO: handle SuplementaryActions
                foreach (var tag in tags)
                {
                    // base this on CustomAnalysisAction.InnerExecute
                    // but need a new VsAbstraction (that doesn't need to worry about line number offset)
                    // This always should be a CustomAnalysisTag but doesn't hurt to check when casting.
                    if (tag is CustomAnalysisTag cat)
                    {
                        var orig = cat.AnalyzedElement.OriginalString;

                        switch (cat.Action)
                        {
                            case ActionType.AddAttribute:
                                output.Add($"Adding attribute '{cat.Name}' to {cat.ElementName}");

                                if (orig.EndsWith("/>"))
                                {
                                    text = text.Substring(0, cat.Span.Start - 1) + orig.Substring(0, orig.Length - 2) + $"{cat.Name}=\"{cat.Value}\" />" + text.Substring(cat.AnalyzedElement.Location.Start + cat.AnalyzedElement.Location.Length);
                                }
                                else
                                {
                                    text = text.Substring(0, cat.Span.End) + $" {cat.Name}=\"{cat.Value}\"" + text.Substring(cat.Span.End);
                                }

                                break;
                            case ActionType.AddChild:
                                // TODO: implement AddChild
                                break;
                            case ActionType.HighlightWithoutAction:
                                // NOOP - Nothing to fix here
                                break;
                            case ActionType.RemoveAttribute:

                                var attrs = cat.AnalyzedElement.GetAttributes(cat.Name).ToList();

                                if (attrs.Count() == 1)
                                {
                                    output.Add($"Remove attribute '{cat.Name}' from {cat.ElementName}");

                                    var attr = attrs.First();
                                    var attrString = orig.Substring(attr.Location.Start - cat.AnalyzedElement.Location.Start, attr.Location.Length);

                                    text = text.Substring(0, cat.AnalyzedElement.Location.Start) + orig.Substring(0, attr.Location.Start - cat.AnalyzedElement.Location.Start) + orig.Substring(attr.Location.Start - cat.AnalyzedElement.Location.Start + attr.Location.Length) + text.Substring(cat.AnalyzedElement.Location.Start + cat.AnalyzedElement.Location.Length);
                                }
                                else
                                {
                                    output.Add($"Not removing attribute '{cat.Name}' from {cat.ElementName} as it doesn't exist");
                                }

                                break;
                            case ActionType.RemoveChild:
                                // TODO: implement RemoveChild
                                break;
                            case ActionType.RenameElement:
                                output.Add($"Renaming an instance of {cat.ElementName} to {cat.Name}");

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
                                output.Add($"Replacing {cat.ElementName}");

                                text = text.Substring(0, cat.AnalyzedElement.Location.Start) + cat.Content + text.Substring(cat.AnalyzedElement.Location.Start + cat.AnalyzedElement.Location.Length);

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
            // TODO: ISSUE#380 Implement XamlConverter support for project files
            throw new NotImplementedException("Coming Soon");
        }
    }
}
