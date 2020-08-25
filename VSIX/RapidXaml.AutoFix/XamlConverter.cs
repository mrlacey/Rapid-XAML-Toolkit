// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.Text;
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

                foreach (var tag in tags)
                {
                    // This always should be a CustomAnalysisTag but doesn't hurt to check when casting.
                    if (tag is CustomAnalysisTag cat)
                    {
                        var newElement = this.UpdateElementXaml(text, cat, output);

                        text = text.Substring(0, cat.AnalyzedElement.Location.Start) + newElement + text.Substring(cat.AnalyzedElement.Location.End());

                        foreach (var suppAction in cat.SupplementaryActions)
                        {
                            var sat = this.RepurposeTagForSupplementaryAction(cat, suppAction, newElement);

                            newElement = this.UpdateElementXaml(text, sat, output);
                            text = text.Substring(0, sat.AnalyzedElement.Location.Start) + newElement + text.Substring(sat.AnalyzedElement.Location.End());
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

        private CustomAnalysisTag RepurposeTagForSupplementaryAction(CustomAnalysisTag tag, AnalysisAction suppAction, string elementXaml)
        {
            var ae = RapidXamlElementExtractor.GetElement(elementXaml, tag.AnalyzedElement.Location.Start);

            var catd = new CustomAnalysisTagDependencies
            {
                AnalyzedElement = ae,
                Action = suppAction,
                ElementName = ae.Name,
                FileName = tag.FileName,
                InsertPos = tag.InsertPosition,
                Logger = tag.Logger,
                ProjectFilePath = tag.ProjectFilePath,
                Snapshot = tag.Snapshot,
                //// Don't need to set VsAbstraction as tags only need it for referencing settings but supplementary actions don't need to know about settings.
            };

            if (suppAction.Location == null)
            {
                if (tag.Action == ActionType.RenameElement)
                {
                    catd.Span = new Span(tag.Span.Start, tag.Name.Length);
                }
                else
                {
                    catd.Span = tag.Span;
                }
            }
            else
            {
                catd.Span = suppAction.Location.ToSpanPlusStartPos(tag.InsertPosition);
            }

            return new CustomAnalysisTag(catd);
        }

        private string UpdateElementXaml(string text, CustomAnalysisTag cat, List<string> output)
        {
            var orig = cat.AnalyzedElement.OriginalString;
            var newXaml = orig;

            switch (cat.Action)
            {
                case ActionType.AddAttribute:
                    output.Add($"Adding attribute '{cat.Name}' to {cat.ElementName}");

                    if (orig.EndsWith("/>"))
                    {
                        newXaml = orig.Substring(0, orig.Length - 2) + $"{cat.Name}=\"{cat.Value}\" />";
                    }
                    else
                    {
                        newXaml = orig.Substring(0, cat.Span.End - cat.AnalyzedElement.Location.Start) + $" {cat.Name}=\"{cat.Value}\"" + orig.Substring(cat.Span.End - cat.AnalyzedElement.Location.Start);
                    }

                    break;
                case ActionType.AddChild:
                    output.Add($"Adding child element to {cat.ElementName}");

                    if (orig.EndsWith("/>"))
                    {
                        var replacementXaml = $">{Environment.NewLine}{cat.Content}{Environment.NewLine}</{cat.ElementName}>";

                        newXaml = orig.Substring(0, orig.Length - 2) + replacementXaml;
                    }
                    else
                    {
                        var insertPos = orig.IndexOf('>');

                        newXaml = orig.Substring(0, insertPos + 1) + $"{Environment.NewLine}{cat.Content}" + orig.Substring(insertPos + 1);
                    }

                    break;
                case ActionType.HighlightWithoutAction:
                    // NOOP - Nothing to fix here
                    break;
                case ActionType.RemoveAttribute:

                    var attrs = cat.AnalyzedElement.GetAttributes(cat.Name).ToList();

                    if (attrs.Count() == 1)
                    {
                        output.Add($"Removing attribute '{cat.Name}' from {cat.ElementName}");

                        var attr = attrs.First();
                        newXaml = orig.Substring(0, attr.Location.Start - cat.AnalyzedElement.Location.Start) + orig.Substring(attr.Location.End() - cat.AnalyzedElement.Location.Start);
                    }
                    else
                    {
                        output.Add($"Not removing attribute '{cat.Name}' from {cat.ElementName} as it doesn't exist");
                    }

                    break;
                case ActionType.RemoveChild:
                    var children = cat.AnalyzedElement.GetChildren(cat.Element.Name).ToList();

                    if (children.Count() >= 1)
                    {
                        children.Reverse();

                        foreach (var child in children)
                        {
                            output.Add($"Removing child '{cat.Element.Name}' from {cat.ElementName}");

                            newXaml = newXaml.Substring(0, child.Location.Start - cat.AnalyzedElement.Location.Start) + newXaml.Substring(child.Location.End() - cat.AnalyzedElement.Location.Start);
                        }
                    }
                    else
                    {
                        output.Add($"Not removing child '{cat.Name}' from {cat.ElementName} as it doesn't exist");
                    }

                    break;
                case ActionType.RenameElement:
                    output.Add($"Renaming an instance of {cat.ElementName} to {cat.Name}");

                    if (orig.EndsWith("/>"))
                    {
                        newXaml = $"<{cat.Name}" + orig.Substring(cat.Span.End - cat.AnalyzedElement.Location.Start);
                    }
                    else
                    {
                        var startNameEnd = cat.ElementName.Length + 1;
                        var endNameStart = orig.LastIndexOf("</");

                        var updated = $"<{cat.Name}" + orig.Substring(startNameEnd, endNameStart - startNameEnd) + $"</{cat.Name}>";
                        newXaml = updated;
                    }

                    break;
                case ActionType.ReplaceElement:
                    output.Add($"Replacing {cat.ElementName}");

                    newXaml = cat.Content;

                    break;
                default:
                    break;
            }

            return newXaml;
        }
    }
}
