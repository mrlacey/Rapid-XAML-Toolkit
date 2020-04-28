// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.Text;
using RapidXamlToolkit;
using RapidXamlToolkit.Commands;
using RapidXamlToolkit.VisualStudioIntegration;
using RapidXamlToolkit.XamlAnalysis;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXaml.AnalysisExe
{
    public class Program
    {
#pragma warning disable SA1310 // Field names should not contain underscore
        private const int ERRORCODE_FILE_DOES_NOT_EXIST = 2;
        private const int ERRORCODE_NOT_A_PROJECT_FILE = 3;
#pragma warning restore SA1310 // Field names should not contain underscore

        public static void Main(string[] args)
        {
            Console.WriteLine("Hello World!  25");

            if (args.Length < 1)
            {
                Console.WriteLine("expecting a project file as first command argument.");
            }
            else
            {
                var projectPath = args[0];

                if (!File.Exists(projectPath))
                {
                    // TODO: log file not exists
                    Environment.ExitCode = ERRORCODE_FILE_DOES_NOT_EXIST;
                    return;
                }

                var fileExt = Path.GetExtension(projectPath);

                if (!fileExt.ToLowerInvariant().Equals(".csproj")
                  & !fileExt.ToLowerInvariant().Equals(".vbproj"))
                {
                    // TODO: log this
                    Environment.ExitCode = ERRORCODE_NOT_A_PROJECT_FILE;
                    return;
                }

                var projFileLines = File.ReadAllLines(projectPath);

                var projDir = Path.GetDirectoryName(projectPath);

                //// Console.WriteLine($"Warning: {projDir}");

                var bavsa = new BuildAnalysisVisualStudioAbstraction();

                foreach (var line in projFileLines)
                {
                    var endPos = line.IndexOf(".xaml\"");
                    if (endPos > 1)
                    {
                        var startPos = line.IndexOf("Include");

                        if (startPos > 1)
                        {
                            var relativeFilePath = line.Substring(startPos + 9, endPos + 5 - startPos - 9);

                            var xamlFilePath = Path.Combine(projDir, relativeFilePath);

                            // Log.LogMessage(MessageImportance.High, $"- {relativeFilePath}");
                            Console.WriteLine($"Warning: {xamlFilePath}");

                            var snapshot = new BuildAnalysisTextSnapshot(xamlFilePath);

                            var rxdoc = RapidXamlDocument.Create(snapshot, xamlFilePath, bavsa, projectPath);

                          //// XamlElementExtractor.Parse(ProjectType.Any, xamlFilePath, snapshot, text, RapidXamlDocument.GetAllProcessors(ProjectType.Any), result.Tags);

                            if (rxdoc.Tags.Count > 0)
                            {
                                Console.WriteLine($"Found {rxdoc.Tags.Count} taggable issues in '{xamlFilePath}'.");

                                var tagsOfInterest = rxdoc.Tags
                                                          .Where(t => t is RapidXamlDisplayedTag)
                                                          .Select(t => t as RapidXamlDisplayedTag)
                                                          .ToList();

                                Console.WriteLine($"Found {tagsOfInterest.Count} issues to report '{xamlFilePath}'.");

                                foreach (var issue in tagsOfInterest)
                                {
                                    string messageType = null;
                                    switch (issue.ConfiguredErrorType)
                                    {
                                        case TagErrorType.Error:
                                            messageType = "error";
                                            break;
                                        case TagErrorType.Warning:
                                            messageType = "warning";
                                            break;
                                        case TagErrorType.Suggestion:
                                            // TODO: work out what should set messageType to here
                                            messageType = "message";
                                            break;
                                        case TagErrorType.Hidden:
                                            break;
                                    }

                                    if (!string.IsNullOrEmpty(messageType))
                                    {
                                        Console.WriteLine($"{relativeFilePath}({issue.Line},{issue.Column},{issue.Line},{issue.Column}) {messageType} {issue.ErrorCode}: {issue.Description}");
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
