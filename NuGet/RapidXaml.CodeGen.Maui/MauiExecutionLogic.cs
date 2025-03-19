using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace RapidXaml.CodeGen;

public partial class MauiExecutionLogic
{
	private readonly IFileSystem _file;
	private readonly ITaskLogWrapper _log;

	public MauiExecutionLogic(IFileSystem file, ITaskLogWrapper logWrapper)
	{
		_file = file;
		_log = logWrapper;
	}

	public bool Execute(string[] inputFiles, string generationNamespace , bool supportResxGeneration, string[] resxInputFiles)
	{
		try
		{
			List<string> GetFilesFromTaskItem(string taskItem, string filePattern)
			{
				List<string> result = [];

				if (taskItem.Contains("*"))
				{
					string fullFilePath = taskItem;
					string fileNamePattern = _file.PathGetFileName(fullFilePath);
					string sourceDirectory = fullFilePath.Replace(fileNamePattern, string.Empty);

					var searchOptions = SearchOption.TopDirectoryOnly;

					if (sourceDirectory.EndsWith("**\\"))
					{
						sourceDirectory = sourceDirectory.Substring(0, sourceDirectory.Length - 3);
						searchOptions = SearchOption.AllDirectories;
					}

					if (_file.DirectoryExists(sourceDirectory))
					{
						if (string.IsNullOrEmpty(fileNamePattern))
						{
							fileNamePattern = filePattern;
						}

						var foundFiles = _file.DirectoryEnumerateFiles(sourceDirectory, fileNamePattern, searchOptions);

						result.AddRange(foundFiles);
					}
					else
					{
						_log.LogError($"{nameof(MauiStyleGenerator)}: Could not find directory: '{sourceDirectory}' ");
					}
				}
				else
				{
					result.Add(taskItem);
				}

				return result;
			}

			List<string> resxFilesOfInterest = [];

			if (supportResxGeneration)
			{
				foreach (var inputResxItem in resxInputFiles)
				{
					List<string> resxFiles = GetFilesFromTaskItem(inputResxItem, "*.resx");

					foreach (var resxFile in resxFiles)
					{
						if (!_file.PathGetFileNameWithoutExtension(resxFile).Contains("."))
						{
							// Neutral language resources, not locale specific ones
							resxFilesOfInterest.Add(resxFile);
						}
					}
				}
			}

			foreach (var inputFileItem in inputFiles)
			{
				//_log.LogImportantMessage($"InputFile: {inputFileItem}::{inputFileItem.ItemSpec} ");

				List<string> inputFilePaths = GetFilesFromTaskItem(inputFileItem, "*.xaml");

				if (inputFilePaths.Count > 0)
				{
					foreach (var inputPath in inputFilePaths)
					{
						var inputFileFullName = _file.GetFilePath(inputPath);
						var inputFileName = _file.GetFileName(inputPath);

						if (inputPath.EndsWith(".xaml", StringComparison.InvariantCultureIgnoreCase))
						{
							_log.LogNormalMessage($"Generating types for {inputFileFullName}");

							var outputFileName = inputFileFullName.Substring(0, inputFileFullName.Length - 5) + ".cs";

							var inputFileContents = _file.FileReadAllText(inputFileFullName);

							// TODO: get the version from the assembly
							var generator = new MauiGeneratorLogic("RapidXaml.CodeGen.Maui.MauiStyleGenerator", version: "0.3.0");

							var generated = generator.GenerateCode(inputFileName, inputFileContents, generationNamespace, includeResourceLoading: (supportResxGeneration && resxFilesOfInterest.Any()));

							_file.FileWriteAllBytes(outputFileName, generated);
						}
						else
						{
							_log.LogWarning($"{nameof(MauiStyleGenerator)}: Skipping generation of {inputFileFullName}");
						}
					}
				}
				else
				{
					_log.LogError($"{nameof(MauiStyleGenerator)}: No files found in '{inputFileItem}' ");
				}
			}

			if (supportResxGeneration)
			{
				Dictionary<string, Dictionary<string, List<SupportedProperty>>> resourcesOfInterest = [];

				const string separator = "_._";

				var resxOutputFileName = string.Empty;

				foreach (var resourceFile in resxFilesOfInterest)
				{
					// May not be where expected if multiple resx files are used - but fine for initial POC.
					resxOutputFileName = _file.PathChangeExtension(resourceFile, "resx.ResourceLoader.cs");

					var doc = new XmlDocument();
					doc.LoadXml(_file.FileReadAllText(resourceFile));

					foreach (XmlElement element in doc.GetElementsByTagName("data"))
					{
						var name = element.GetAttribute("name");

						if (name.Contains(separator))
						{
							if (!resourcesOfInterest.ContainsKey(resourceFile))
							{
								resourcesOfInterest[resourceFile] = [];
							}
							var parts = name.Split([separator], StringSplitOptions.RemoveEmptyEntries);

							if (parts.Length == 2)
							{
								var resourceId = parts[0];

								if (!resourcesOfInterest[resourceFile].ContainsKey(resourceId))
								{
									resourcesOfInterest[resourceFile].Add(resourceId, []);
								}

								var propertyType = GetSupportedPropertyType(parts[1]);

								if (propertyType != SupportedProperty.Unknown)
								{
									resourcesOfInterest[resourceFile][resourceId].Add(propertyType);
								}
								else
								{
									_log.LogWarning($"{nameof(MauiStyleGenerator)}: Unsupported property type: '{parts[1]}'");
								}
							}
							else
							{
								_log.LogWarning($"{nameof(MauiStyleGenerator)}: Unexpected value for resource with name: '{name}'");
							}
						}
					}
				}

				if (!string.IsNullOrEmpty(resxOutputFileName))
				{
					// Always create the enum, even if no resources found of the generated StyleTypes won't compile
					var resIdEnum = GenerateResourceIdsEnum(generationNamespace, resourcesOfInterest);

					var resLoaderClass = GenerateResourceLoader(resourcesOfInterest);
					_file.FileWriteAllBytes(resxOutputFileName, Encoding.UTF8.GetBytes(resIdEnum += resLoaderClass));
				}
			}

			return true;
		}
		catch (Exception ex)
		{
			_log.LogErrorFromException(ex, showStackTrace: true);
			return false;
		}
	}


	private SupportedProperty GetSupportedPropertyType(string formattedPropertyName)
		=> formattedPropertyName switch
		{
			"Content" => SupportedProperty.Content,
			"Placeholder" => SupportedProperty.Placeholder,
			"Text" => SupportedProperty.Text,
			"Title" => SupportedProperty.Title,
			"ToolTipProperties.Text" => SupportedProperty.ToolTipText,
			"SemanticProperties.Description" => SupportedProperty.SemanticDescription,
			"SemanticProperties.Hint" => SupportedProperty.SemanticHint,
			"AutomationProperties.Name" => SupportedProperty.AutomationName,
			"AutomationProperties.HelpText" => SupportedProperty.AutomationHelpText,
			_ => SupportedProperty.Unknown,
		};

	public string GenerateResourceIdsEnum(string generationNamespace, Dictionary<string, Dictionary<string, List<SupportedProperty>>> allResourcesOfInterest)
	{
		List<string> uniqueResources = [];

		foreach (var item in allResourcesOfInterest)
		{
			uniqueResources.AddRange(item.Value.Keys);
		}

		uniqueResources = uniqueResources.Distinct().ToList();

		StringBuilder sb = new();

		sb.AppendLine($"namespace {generationNamespace};");
		sb.AppendLine();
		sb.AppendLine("public enum ResourceId");
		sb.AppendLine("{");

		foreach (var res in uniqueResources)
		{
			sb.AppendLine($"    {res},");
		}

		sb.AppendLine("}");

		return sb.ToString();
	}

	public string GenerateResourceLoader(Dictionary<string, Dictionary<string, List<SupportedProperty>>> allResourcesOfInterest)
	{
		StringBuilder sb = new();

		sb.AppendLine();
		sb.AppendLine("public static class GeneratedResourceLoader");
		sb.AppendLine("{");
		sb.AppendLine("    public static void LoadResources(this StyleableElement element, ResourceId resId)");
		sb.AppendLine("    {");
		sb.AppendLine("        switch (resId)");
		sb.AppendLine("        {");

		// TODO: Review how this handles multiple resx files containing duplicate resource names
		foreach (var fileItem in allResourcesOfInterest)
		{
			// TODO: Need to also add a using directive for the namespace of the file (based on directory structure)
			var fileName = _file.PathGetFileNameWithoutExtension(fileItem.Key);

			foreach (var res in fileItem.Value)
			{
				sb.AppendLine($"            case ResourceId.{res.Key}:");

				foreach (var resProp in res.Value)
				{
					switch (resProp)
					{
						case SupportedProperty.Content:
							sb.AppendLine($"                if (element is RadioButton {res.Key}AsRadioButton)");
							sb.AppendLine($"                {{");
							sb.AppendLine($"                    {res.Key}AsRadioButton.Content = {fileName}.{res.Key}___Content;");
							sb.AppendLine($"                }}");
							break;
						case SupportedProperty.Placeholder:
							sb.AppendLine($"                if (element is InputView {res.Key}AsInputViewPh)");
							sb.AppendLine($"                {{");
							sb.AppendLine($"                    {res.Key}AsInputViewPh.Placeholder = {fileName}.{res.Key}___Placeholder;");
							sb.AppendLine($"                }}");
							break;
						case SupportedProperty.Text:
							sb.AppendLine($"                if (element is Label {res.Key}AsLabelTxt)");
							sb.AppendLine($"                {{");
							sb.AppendLine($"                    {res.Key}AsLabelTxt.Text = {fileName}.{res.Key}___Text;");
							sb.AppendLine($"                }}");
							sb.AppendLine($"                if (element is InputView {res.Key}AsInputViewTxt)");
							sb.AppendLine($"                {{");
							sb.AppendLine($"                    {res.Key}AsInputViewTxt.Text = {fileName}.{res.Key}___Text;");
							sb.AppendLine($"                }}");
							sb.AppendLine($"                if (element is MenuItem {res.Key}AsMenuItem)");
							sb.AppendLine($"                {{");
							sb.AppendLine($"                    {res.Key}AsMenuItem.Text = {fileName}.{res.Key}___Text;");
							sb.AppendLine($"                }}");
							sb.AppendLine($"                if (element is MenuBarItem {res.Key}AsMenuBarItem)");
							sb.AppendLine($"                {{");
							sb.AppendLine($"                    {res.Key}AsMenuBarItem.Text = {fileName}.{res.Key}___Text;");
							sb.AppendLine($"                }}");
							break;
						case SupportedProperty.Title:
							sb.AppendLine($"                if (element is Picker {res.Key}AsPicker)");
							sb.AppendLine($"                {{");
							sb.AppendLine($"                    {res.Key}AsPicker.Title = {fileName}.{res.Key}___Title;");
							sb.AppendLine($"                }}");
							break;
						case SupportedProperty.ToolTipText:
							sb.AppendLine($"                ToolTipProperties.SetText(element, {fileName}.{res.Key}___ToolTipProperties_Text);");
							break;
						case SupportedProperty.SemanticDescription:
							sb.AppendLine($"                SemanticProperties.SetDescription(element, {fileName}.{res.Key}___SemanticProperties_Description);");
							break;
						case SupportedProperty.SemanticHint:
							sb.AppendLine($"                SemanticProperties.SetHint(element, {fileName}.{res.Key}___SemanticProperties_Hint);");
							break;
						case SupportedProperty.AutomationName:
							sb.AppendLine($"                AutomationProperties.SetName(element, {fileName}.{res.Key}___AutomationProperties_Name);");
							break;
						case SupportedProperty.AutomationHelpText:
							sb.AppendLine($"                AutomationProperties.SetHelpText(element, {fileName}.{res.Key}___AutomationProperties_HelpText);");
							break;
						case SupportedProperty.Unknown:
						default:
							break;
					}
				}

				sb.AppendLine($"            break;");
			}
		}

		sb.AppendLine("            default:");
		sb.AppendLine("                break;");
		sb.AppendLine("        }");
		sb.AppendLine("    }");
		sb.AppendLine("}");

		return sb.ToString();
	}
}

