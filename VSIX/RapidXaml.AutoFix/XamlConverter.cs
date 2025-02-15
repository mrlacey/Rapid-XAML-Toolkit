﻿// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using RapidXamlToolkit;
using RapidXamlToolkit.Tests;
using RapidXamlToolkit.Utils.IO;
using RapidXamlToolkit.XamlAnalysis;
using RapidXamlToolkit.XamlAnalysis.Processors;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXaml
{
    public class XamlConverter
    {
        public XamlConverter()
        {
            this.FileSystem = new NetStandardFileSystemAccess();
        }

#if DEBUG
        // This overload exists in debug for test use only
        public XamlConverter(IFileSystemAbstraction fsa)
        {
            this.FileSystem = fsa ?? new NetStandardFileSystemAccess();
        }
#endif

        private IFileSystemAbstraction FileSystem { get; set; }

        public (bool Success, IEnumerable<string> Details) ConvertFile(string xamlFilePath, IEnumerable<ICustomAnalyzer> analyzers)
        {
            if (!this.FileSystem.FileExists(xamlFilePath))
            {
                return (false, new[] { "File does not exist." });
            }

            if (this.FileSystem.GetFileExtension(xamlFilePath).ToLowerInvariant() != ".xaml")
            {
                return (false, new[] { "File must have a .xaml extension." });
            }

            if (!analyzers.Any())
            {
                return (false, new[] { "No analyzers provided. Without these no changes can be made." });
            }

            var output = new List<string>();

            try
            {
                this.AnalyzeXamlFile(xamlFilePath, analyzers, output);

                return (true, output);
            }
            catch (Exception exc)
            {
                output.Add(exc.Message);
                return (false, output);
            }
        }

        public (bool Success, IEnumerable<string> Details) ConvertAllFilesInProject(string projectFilePath, IEnumerable<ICustomAnalyzer> analyzers)
        {
            if (!this.FileSystem.FileExists(projectFilePath))
            {
                return (false, new[] { "File does not exist." });
            }

            var fileExt = this.FileSystem.GetFileExtension(projectFilePath);

            if (!fileExt.ToLowerInvariant().Equals(".csproj")
              & !fileExt.ToLowerInvariant().Equals(".vbproj"))
            {
                return (false, new[] { $"{fileExt} is not recognized as a project file." });
            }

            if (!analyzers.Any())
            {
                return (false, new[] { "No analyzers provided. Without these no changes can be made." });
            }

            var output = new List<string>
            {
                $"Analyzing files from '{projectFilePath}'",
            };

            try
            {
                var projFileLines = this.FileSystem.ReadAllLines(projectFilePath);
                var projDir = this.FileSystem.GetDirectoryName(projectFilePath);

                foreach (var line in projFileLines)
                {
                    var endPos = line.IndexOf(".xaml\"");
                    if (endPos > 1)
                    {
                        var startPos = line.IndexOf("Include");

                        if (startPos > 1)
                        {
                            var relativeFilePath = line.Substring(startPos + 9, endPos + 5 - startPos - 9);
                            var xamlFilePath = System.IO.Path.Combine(projDir, relativeFilePath);

                            this.AnalyzeXamlFile(xamlFilePath, analyzers, output, projectFilePath);
                        }
                    }
                }

                return (true, output);
            }
            catch (Exception exc)
            {
                output.Add(exc.Message);
                return (false, output);
            }
        }

        private void AnalyzeXamlFile(string xamlFilePath, IEnumerable<ICustomAnalyzer> analyzers, List<string> output, string projectFilePath = "")
        {
            output.Add($"Analyzing '{xamlFilePath}'");

            var text = this.FileSystem.GetAllFileText(xamlFilePath);

            var snapshot = new AutoFixTextSnapshot(text.Length);

            var logger = EmptyLogger.Create();
            var vspfp = new AutoFixProjectFilePath(projectFilePath);

            try
            {
                var processors = new List<(string, XamlElementProcessor)>();

                foreach (var analyzer in analyzers)
                {
                    output.Add($"Will analyze instances of '{analyzer.TargetType()}'.");

                    processors.Add(
                        (analyzer.TargetType(),
                         new CustomProcessorWrapper(analyzer, ProjectType.Any, projectFilePath, logger, vspfp)));
                }

                var tags = new TagList();

                XamlElementExtractor.Parse("Generic.xaml", snapshot, text, processors, tags, null, null, logger);

                var plural = tags.Count == 1 ? string.Empty : "s";
                output.Add($"Found {tags.Count} place{plural} to make changes.");

                tags.Reverse();  // Work back through the document to allow for modifications changing document length

                var finalActions = new List<CustomAnalysisTag>();

                foreach (var tag in tags)
                {
                    // This always should be a CustomAnalysisTag but doesn't hurt to check when casting.
                    if (tag is CustomAnalysisTag cat)
                    {
                        if (cat.Action == ActionType.AddXmlns)
                        {
                            finalActions.Add(cat);
                        }

                        var newElement = this.UpdateElementXaml(cat, output);

                        text = text.Substring(0, cat.AnalyzedElement.Location.Start) + newElement + text.Substring(cat.AnalyzedElement.Location.End());

                        foreach (var suppAction in cat.SupplementaryActions)
                        {
                            var sat = this.RepurposeTagForSupplementaryAction(cat, suppAction, newElement);

                            if (sat.Action == ActionType.AddXmlns)
                            {
                                finalActions.Add(sat);
                            }
                            else
                            {
                                newElement = this.UpdateElementXaml(sat, output);
                                text = text.Substring(0, sat.AnalyzedElement.Location.Start) + newElement + text.Substring(sat.AnalyzedElement.Location.End());
                            }
                        }
                    }
                }

                foreach (var faTag in finalActions)
                {
                    if (faTag.Action == ActionType.AddXmlns)
                    {
                        text = this.AddXmlns(faTag, text, output);
                    }
                }

                this.FileSystem.WriteAllFileText(xamlFilePath, text);
            }
            catch (Exception exc)
            {
                output.Add(exc.Message);
            }
        }

        private string AddXmlns(CustomAnalysisTag faTag, string xaml, List<string> output)
        {
            var element = RapidXamlElementExtractor.GetElement(xaml);

            bool exists = false;

            foreach (var attr in element.InlineAttributes)
            {
                if (attr.Name.StartsWith("xmlns:"))
                {
                    var alias = attr.Name.Substring(6);

                    if (alias == faTag.Name)
                    {
                        if (attr.StringValue == faTag.Value)
                        {
                            output.Add("XMLNS is already specified in the document.");
                        }
                        else
                        {
                            output.Add("XMLNS is already specified in the document but with a different value.");
                        }

                        exists = true;
                        break;
                    }
                }
            }

            if (!exists)
            {
                var insertPos = element.InlineAttributes?.LastOrDefault()?.Location?.End();

                output.Add($"Adding xmlns alias for '{faTag.Name}'");

                if (insertPos != null)
                {
                    xaml = xaml.Insert(insertPos.Value, $" xmlns:{faTag.Name}=\"{faTag.Value}\"");
                }
                else
                {
                    xaml = xaml.Insert(
                        element.Location.Start + element.Name.Length + 1,
                        $" xmlns:{faTag.Name}=\"{faTag.Value}\"");
                }
            }

            return xaml;
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
                    catd.Span = new RapidXamlSpan(tag.Span.Start, tag.Name.Length);
                }
                else
                {
                    catd.Span = new RapidXamlSpan(tag.Span.Start, tag.Span.Length);
                }
            }
            else
            {
                catd.Span = suppAction.Location.AddStartPos(tag.InsertPosition);
            }

            return new CustomAnalysisTag(catd);
        }

        private string UpdateElementXaml(CustomAnalysisTag cat, List<string> output)
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
                        newXaml = orig.Substring(0, cat.Span.Start + cat.Span.Length - cat.AnalyzedElement.Location.Start) + $" {cat.Name}=\"{cat.Value}\"" + orig.Substring(cat.Span.Start + cat.Span.Length - cat.AnalyzedElement.Location.Start);
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
                        newXaml = $"<{cat.Name}" + orig.Substring(cat.Span.Start + cat.Span.Length - cat.AnalyzedElement.Location.Start);
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
