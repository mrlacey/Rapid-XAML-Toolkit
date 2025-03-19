using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace RapidXaml.CodeGen;

public class MauiGeneratorLogic
{
	private readonly string _name;
	private readonly string _version;

	public MauiGeneratorLogic(string name, string version)
	{
		_name = name;
		_version = version;
	}
	public byte[] GenerateCode(string inputFileName, string inputFileContent, string defaultNamespace, bool includeResourceLoading)
	{
		var output = new StringBuilder();

		var inputXml = new XmlDocument();

		try
		{
			inputXml.LoadXml(inputFileContent);

			var styles = new List<(string, string)>();
			var colors = new List<string>();
			var brushes = new List<string>();
			var integers = new Dictionary<string, int>();
			var doubles = new Dictionary<string, double>();
			var decimals = new Dictionary<string, decimal>();
			var thicknesses = new Dictionary<string, string>();

			var overrideNamespace = string.Empty;
			var ignoreTypes = new List<string>();
			var addUsing = new List<string>();

			var xmlns = new Dictionary<string, string>();
			var xmlnsToAdd = new Dictionary<string, string>();

			void ParseDocumentLevelAttributes(XmlElement element)
			{
				foreach (var att in element.Attributes)
				{
					if (att is XmlAttribute xa)
					{
						System.Diagnostics.Debug.WriteLine(xa.Name);

						if (xa.Name.StartsWith("xmlns:", StringComparison.InvariantCultureIgnoreCase)
							&& xa.Value.StartsWith("using:", StringComparison.InvariantCultureIgnoreCase))
						{
							xmlns.Add(xa.Name.Substring(6), xa.Value.Substring(6));
						}
						else
						if (xa.Name.EndsWith(":GenConfig.Namespace", StringComparison.InvariantCultureIgnoreCase))
						{
							overrideNamespace = xa.Value;
						}
						else if (xa.Name.EndsWith(":GenConfig.AddUsing", StringComparison.InvariantCultureIgnoreCase))
						{
							addUsing.Add(xa.Value);
						}
						else if (xa.Name.EndsWith(":GenConfig.IgnoreTypes", StringComparison.InvariantCultureIgnoreCase))
						{
							ignoreTypes = xa.Value.Split([",", " "], StringSplitOptions.RemoveEmptyEntries).ToList();
						}
					}
				}
			}

			ParseDocumentLevelAttributes(inputXml.DocumentElement);

			void ParseDocumentChildren(XmlElement docElement)
			{

				foreach (XmlNode element in docElement.ChildNodes)
				{
					if (element.Name == "#comment")
					{
						var val = element.Value.Trim();

						if (val.StartsWith("GenConfig:", StringComparison.InvariantCultureIgnoreCase))
						{
							var valParts = val.Split([":", "=", "\""], StringSplitOptions.RemoveEmptyEntries);

							if (valParts.Length == 3)
							{
								if (valParts[1].Equals("Namespace", StringComparison.InvariantCultureIgnoreCase))
								{
									overrideNamespace = valParts[2];
								}
								else if (valParts[1].Equals("IgnoreTypes", StringComparison.InvariantCultureIgnoreCase))
								{
									ignoreTypes = valParts[2].Split([",", " "], StringSplitOptions.RemoveEmptyEntries).ToList();
								}
								else if (valParts[1].Equals("AddUsing", StringComparison.InvariantCultureIgnoreCase))
								{
									addUsing.Add(valParts[2].Trim());
								}
							}
						}
					}
					else if (element.Name == "Color")
					{
						var attributes = element.Attributes;

						var xKey = attributes["x:Key"];

						if (xKey != null && !string.IsNullOrWhiteSpace(xKey.Value))
						{
							colors.Add(xKey.Value);
						}
					}
					else if (element.Name.EndsWith("Brush"))
					{
						var attributes = element.Attributes;

						var xKey = attributes["x:Key"];

						if (xKey != null && !string.IsNullOrWhiteSpace(xKey.Value))
						{
							brushes.Add(xKey.Value);
						}
					}
					else if (element.Name == "Double" || element.Name == "x:Double")
					{
						var attributes = element.Attributes;

						var xKey = attributes["x:Key"];

						if (xKey != null && !string.IsNullOrWhiteSpace(xKey.Value) && double.TryParse(element.InnerText, out var doubleValue))
						{
							doubles.Add(xKey.Value, doubleValue);
						}
					}
					else if (element.Name == "Decimal")
					{
						var attributes = element.Attributes;

						var xKey = attributes["x:Key"];

						if (xKey != null && !string.IsNullOrWhiteSpace(xKey.Value) && decimal.TryParse(element.InnerText, out var decimalValue))
						{
							decimals.Add(xKey.Value, decimalValue);
						}
					}
					else if (element.Name == "Integer" || element.Name == "x:Int16" || element.Name == "x:Int32" || element.Name == "x:Int64")
					{
						var attributes = element.Attributes;

						var xKey = attributes["x:Key"];

						if (xKey != null && !string.IsNullOrWhiteSpace(xKey.Value) && int.TryParse(element.InnerText, out int intValue))
						{
							integers.Add(xKey.Value, intValue);
						}
					}
					else if (element.Name == "Thickness")
					{
						// TODO: support this
						var attributes = element.Attributes;

						var xKey = attributes["x:Key"];

						if (xKey != null && !string.IsNullOrWhiteSpace(xKey.Value))
						{
							var parts = element.InnerText.Split([",", " "], StringSplitOptions.RemoveEmptyEntries);

							if (parts.Length == 1 || parts.Length == 2 || parts.Length == 4)
							{
								var allGood = true;
								foreach (var part in parts)
								{
									if (!double.TryParse(part, out double doubleValue))
									{
										allGood = false;
										break;
									}
								}

								if (allGood)
								{
									// Join the parts back together with commas so C# works even if XAML used spaces
									thicknesses.Add(xKey.Value, string.Join(",", parts));
								}
							}
						}
					}
					else if (element.Name == "Style")
					{
						var attributes = element.Attributes;

						var targetType = attributes["TargetType"];
						var xKey = attributes["x:Key"];

						if (targetType != null
							&& xKey != null
							&& !string.IsNullOrWhiteSpace(targetType.Value)
							&& !string.IsNullOrWhiteSpace(xKey.Value)
							&& !ignoreTypes.Contains(targetType.Value))
						{
							var ttValue = targetType.Value;

							var colonIndex = ttValue.IndexOf(':');

							if (colonIndex > 0)
							{
								var alias = ttValue.Substring(0, colonIndex);

								if (xmlns.ContainsKey(alias) && !xmlnsToAdd.ContainsKey(alias))
								{
									xmlnsToAdd.Add(alias, xmlns[alias]);
								}

								ttValue = ttValue.Substring(colonIndex + 1);
							}

							styles.Add((xKey.Value, ttValue));
						}
					}
				}
			}

			ParseDocumentChildren(inputXml.DocumentElement);

			output.AppendLine("// <auto-generated>");
			output.AppendLine($"// This file was generated by {GetName()}");
			output.AppendLine("// learn more at https://github.com/mrlacey/XamlStyleTypes");
			output.AppendLine("// </auto-generated>");
			output.AppendLine("// <auto-generated />");
			output.AppendLine();

			foreach (var item in GetDefaultNamespaces())
			{
				output.AppendLine($"using {item};");
			}

			foreach (var ns in addUsing)
			{
				output.AppendLine($"using {ns};");
			}

			foreach (var ns in xmlnsToAdd)
			{
				output.AppendLine($"using {ns.Value};");
			}

			var namespaceToUse = !string.IsNullOrWhiteSpace(overrideNamespace) ? overrideNamespace : defaultNamespace;

			output.AppendLine();

			output.AppendLine($"namespace {namespaceToUse}");
			output.AppendLine("{");

			void AddAnyColors()
			{
				if (colors.Any())
				{
					output.AppendLine($"    [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]");
					output.AppendLine($"    public static partial class AppColors");
					output.AppendLine($"    {{");

					foreach (var item in colors)
					{
						output.AppendLine($"        public const string {item} = \"{item}\";");
					}

					output.AppendLine("    }");
					output.AppendLine();
				}
			}

			AddAnyColors();

			void AddAnyBrushes()
			{
				if (brushes.Any())
				{
					output.AppendLine($"    [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]");
					output.AppendLine($"    public static partial class AppBrushes");
					output.AppendLine($"    {{");

					foreach (var item in brushes)
					{
						output.AppendLine($"        public const string {item} = \"{item}\";");
					}

					output.AppendLine("    }");
					output.AppendLine();
				}
			}

			AddAnyBrushes();

			void AddAnySizes()
			{
				if (doubles.Any() || decimals.Any() || integers.Any() || thicknesses.Any())
				{
					output.AppendLine($"    [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]");
					output.AppendLine($"    public static partial class AppSizes");
					output.AppendLine($"    {{");

					foreach (var item in doubles)
					{
						output.AppendLine($"        public const double {item.Key} = {item.Value};");
					}

					if (doubles.Any())
					{
						output.AppendLine();
					}

					foreach (var item in decimals)
					{
						output.AppendLine($"        public const decimal {item.Key} = {item.Value};");
					}

					if (decimals.Any())
					{
						output.AppendLine();
					}

					foreach (var item in integers)
					{
						output.AppendLine($"        public const int {item.Key} = {item.Value};");
					}

					if (integers.Any())
					{
						output.AppendLine();
					}

					foreach (var item in thicknesses)
					{
						output.AppendLine($"        public static Thickness {item.Key} = new Thickness({item.Value});");
					}

					output.AppendLine("    }");
					output.AppendLine();
				}
			}

			AddAnySizes();

			var fileIdentifier = inputFileName.GetHashCode().ToString().TrimStart('-');

			if (styles.Any())
			{
				AddAnyAdditionalStyleRelatedCode(output, fileIdentifier);

				foreach (var (xKey, targetType) in styles)
				{
					AddIndividualStyleClass(output, xKey, targetType, fileIdentifier, includeResourceLoading);
				}
			}

			if (!colors.Any() && !brushes.Any() && !styles.Any() && !doubles.Any() && !decimals.Any() && !integers.Any() && !thicknesses.Any())
			{
				output.AppendLine("    // No suitable content was found to generate classes for.");
			}

			output.AppendLine("}");
			output.AppendLine();
		}
		catch (Exception exc)
		{
			SetOutputToExceptionDetailsMessage(exc, output);
		}

		return Encoding.UTF8.GetBytes(output.ToString());
	}

	internal virtual void AddAnyAdditionalStyleRelatedCode(StringBuilder output, string fileIdentifier)
	{ }

	private void SetOutputToExceptionDetailsMessage(Exception exc, StringBuilder output)
	{
		output.Clear();
		output.AppendLine("#error XAML Style generation failed");
		output.AppendLine("/*");
		output.AppendLine("Please verify that the XAML file is valid.");
		output.AppendLine("Please report this as an issue at https://github.com/mrlacey/XamlStyleTypes/issues/new");
		output.AppendLine("If possible, include a copy of the XAML file and the below information.");
		output.AppendLine("Thanks.");
		output.AppendLine();
		output.AppendLine($"{exc.Message}");
		output.AppendLine($"{exc.Source}");
		output.AppendLine($"{exc.StackTrace}");
		output.AppendLine($"{exc.InnerException?.Message}");
		output.AppendLine($"{exc.InnerException?.Source}");
		output.AppendLine($"{exc.InnerException?.StackTrace}");
		output.AppendLine("*/");
		output.AppendLine();
	}

	internal string GetName() => _name;

	// Rely on implicit namespaces for MAUI
	internal List<string> GetDefaultNamespaces() => [];

	internal void AddIndividualStyleClass(StringBuilder output, string key, string targetType, string fileIdentifier, bool includeResourceLoading = false)
	{
		output.AppendLine();

		// See: https://learn.microsoft.com/en-gb/archive/blogs/codeanalysis/correct-usage-of-the-compilergeneratedattribute-and-the-generatedcodeattribute
		output.AppendLine($"    [global::System.CodeDom.Compiler.GeneratedCodeAttribute(\"{GetName()}\", \"{_version}\")]");
		output.AppendLine($"    [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]");
		output.AppendLine($"    public class {key} : {targetType}");
		output.AppendLine($"    {{");
		output.AppendLine($"        public {key}()");
		output.AppendLine($"        {{");
		output.AppendLine($"            if (App.Current.Resources.TryGetValue(\"{key}\", out object result))");
		output.AppendLine($"            {{");
		output.AppendLine($"                this.Style = result as Style;");
		output.AppendLine($"            }}");
		output.AppendLine($"        }}");

		if (includeResourceLoading)
		{
			output.AppendLine($"        public {key}(ResourceId resourceId) : this()");
			output.AppendLine($"        {{");
			output.AppendLine($"            this.LoadResources(resourceId);");
			output.AppendLine($"        }}");
		}

		output.AppendLine($"    }}");
	}
}
