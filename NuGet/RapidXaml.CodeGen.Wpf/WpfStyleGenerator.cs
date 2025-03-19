using System;
using System.Collections.Generic;
using System.IO;
using GeneratorCore;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace RapidXaml.CodeGen
{
    public class WpfStyleGenerator : Task
    {
        [Required]
        public ITaskItem[] InputFiles { get; set; } = Array.Empty<ITaskItem>();

        [Required]
        public string GenerationNamespace { get; set; }

        public override bool Execute()
        {
            try
            {
                foreach (var inputFileItem in InputFiles)
                {
                    //Log.LogMessage(MessageImportance.High, $"InputFile: {inputFileItem}::{inputFileItem.ItemSpec} ");

                    List<string> inputFilePaths = new();

                    if (inputFileItem.ItemSpec.Contains("*"))
                    {
                        string fullFilePath = inputFileItem.ItemSpec;
                        string fileNamePattern = Path.GetFileName(fullFilePath);
                        string sourceDirectory = fullFilePath.Replace(fileNamePattern, string.Empty);

                        var searchOptions = SearchOption.TopDirectoryOnly;

                        if (sourceDirectory.EndsWith("**\\"))
                        {
                            sourceDirectory = sourceDirectory.Substring(0, sourceDirectory.Length - 3);
                            searchOptions = SearchOption.AllDirectories;
                        }

                        if (Directory.Exists(sourceDirectory))
                        {
                            if (string.IsNullOrEmpty(fileNamePattern))
                            {
                                fileNamePattern = "*.xaml";
                            }

                            var foundFiles = Directory.EnumerateFiles(sourceDirectory, fileNamePattern, searchOptions);

                            inputFilePaths.AddRange(foundFiles);
                        }
                        else
                        {
                            Log.LogError($"{nameof(WpfStyleGenerator)}: Could not find directory: '{sourceDirectory}' ");
                        }
                    }
                    else
                    {
                        inputFilePaths.Add(inputFileItem.ItemSpec);
                    }

                    if (inputFilePaths.Count > 0)
                    {
                        foreach (var inputPath in inputFilePaths)
                        {
                            var inputFile = new FileInfo(inputPath);

                            if (inputFile.FullName.EndsWith(".xaml", StringComparison.InvariantCultureIgnoreCase))
                            {
                                Log.LogMessage(MessageImportance.Normal, $"Generating types for {inputFile.FullName}");

                                var outputFileName = inputFile.FullName.Substring(0, inputFile.FullName.Length - 5) + ".cs";

                                var inputFileContents = File.ReadAllText(inputFile.FullName);

                                var generator = new WpfGeneratorLogic($"{nameof(WpfStyleGenerator)} from RapidXaml.CodeGen.Wpf");

                                var generated = generator.GenerateCode(inputFile.Name, inputFileContents, GenerationNamespace, includeResourceLoading: false);

                                File.WriteAllBytes(outputFileName, generated);
                            }
                            else
                            {
                                Log.LogWarning($"{nameof(WpfStyleGenerator)}: Skipping generation of {inputFile.FullName}");
                            }
                        }
                    }
                    else
                    {
                        Log.LogError($"{nameof(WpfStyleGenerator)}: No files found in '{inputFileItem.ItemSpec}' ");
                    }
                }

                return true;

            }
            catch (Exception ex)
            {
                Log.LogErrorFromException(ex, showStackTrace: true);
                return false;
            }
        }
    }
}
