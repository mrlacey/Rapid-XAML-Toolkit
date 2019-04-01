// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RapidXamlToolkit.Logging;
using RapidXamlToolkit.Options;
using RapidXamlToolkit.Resources;
using IServiceProvider = System.IServiceProvider;

namespace RapidXamlToolkit.Parsers
{
    public class CodeParserBase
    {
        public const string UnknownOrInvalidTypeName = "*UNKNOWN-INVALID-TYPE*";  // Asterisk as first character ensures it is invalid

        public const string GridWithRowDefsIndicator = "GRID-PLUS-ROWDEFS";

        public const string GridWithRowDefs2ColsIndicator = "GRID-PLUS-ROWDEFS-2COLS";

        internal const string Unknown = "**unknown**";

        // Used to store the generated xname for reuse when formatting subsequent properties
        private static string xname = string.Empty;

        public CodeParserBase(ILogger logger, int xamlIndent, Profile profileOverload = null)
        {
            Logger = logger;
            XamlIndentSize = xamlIndent;

            this.Profile = profileOverload ?? GetSettings().GetActiveProfile();

            xname = string.Empty;  // Reset this on parser creation as parsers created for each new conversion and don't want old values.
        }

        public static IServiceProvider ServiceProvider { get; set; }

        public static ILogger Logger { get; set; }

        public static int XamlIndentSize { get; set; } = 4;

        public virtual string FileExtension { get; } = string.Empty;

        public Profile Profile { get; }

        protected static string[] TypesToSkipWhenCheckingForSubProperties { get; } = new[] { "String", "ValueType", "Object" };

        protected static string[] NamesOfPropertiesToExcludeFromOutput { get; } = new[] { "IsInDesignMode", "IsInDesignModeStatic", "DataStore" };

        [NotUnitTestable("Relies on ConfiguredSettings which relies on VS Package infrastructure.")]
        public static Settings GetSettings()
        {
            var configuredSettings = new ConfiguredSettings(ServiceProvider);

            return configuredSettings.ActualSettings;
        }

        public static string GetSelectionPropertiesName(List<string> names)
        {
            if (names == null || !names.Any())
            {
                return string.Empty;
            }

            var result = names.First();

            if (names.Count == 2)
            {
                result = StringRes.UI_SelectionTwoNames.WithParams(names[0], names[1]);
            }
            else if (names.Count == 3)
            {
                result = StringRes.UI_SelectionThreeNames.WithParams(names[0], names[1]);
            }
            else if (names.Count > 3)
            {
                result = StringRes.UI_SelectionMoreThanThreeNames.WithParams(names[0], names[1], names.Count - 2);
            }

            return result;
        }

        public string GetPropertyOutput(string type, string name, bool isReadOnly, SemanticModel semModel = null, Func<(List<string> strings, int count)> getSubProperties = null)
        {
            return this.GetPropertyOutputAndCounter(new PropertyDetails { PropertyType = type, Name = name, IsReadOnly = isReadOnly }, 1, semModel, getSubProperties).output;
        }

        public virtual List<PropertyDetails> GetAllPublicProperties(ITypeSymbol typeSymbol, SemanticModel semModel)
        {
            return new List<PropertyDetails>();
        }

        protected static string FormattedClassGroupingOpener(string classGrouping)
        {
            switch (classGrouping.ToUpperInvariant())
            {
                case GridWithRowDefsIndicator:
                case GridWithRowDefs2ColsIndicator:
                    return "Grid";
                default:
                    return classGrouping;
            }
        }

        protected static string FormattedClassGroupingCloser(string classGrouping)
        {
            if (classGrouping.Contains(" "))
            {
                return classGrouping.Substring(0, classGrouping.IndexOf(" ", StringComparison.Ordinal));
            }

            switch (classGrouping.ToUpperInvariant())
            {
                case GridWithRowDefsIndicator:
                case GridWithRowDefs2ColsIndicator:
                    return "Grid";
                default:
                    return classGrouping;
            }
        }

        protected (string output, int counter) GetSubPropertyOutputAndCounter(string name, int numericSubstitute)
        {
            // Type is blank as it's can't be used in a subproperty
            return this.FormatOutput(this.Profile.SubPropertyOutput, type: string.Empty, name: name, numericSubstitute: numericSubstitute, symbol: null, attributes: null, getSubPropertyOutput: null);
        }

        protected (string output, int counter) GetPropertyOutputAndCounter(PropertyDetails property, int numericSubstitute, SemanticModel semModel, Func<(List<string> strings, int count)> getSubPropertyOutput = null, string namePrefix = "")
        {
            var mappingOfInterest = this.GetMappingOfInterest(property);

            string rawOutput = null;

            if (mappingOfInterest != null)
            {
                rawOutput = mappingOfInterest.Output;
            }
            else
            {
                if (property.PropertyType.IsGenericTypeName())
                {
                    var wildcardGenericType = property.PropertyType.Substring(0, property.PropertyType.ToCSharpFormat().IndexOf("<", StringComparison.Ordinal)) + "<T>";

                    Logger?.RecordInfo(StringRes.Info_SearchingForMappingWithGenericWildcard.WithParams(wildcardGenericType));

                    mappingOfInterest = this.GetMappingOfInterest(wildcardGenericType, property.Name, property.IsReadOnly);

                    if (mappingOfInterest != null)
                    {
                        rawOutput = mappingOfInterest.Output;
                    }
                }

                if (rawOutput == null && semModel != null)
                {
                    var tempOutput = new StringBuilder();
                    var tempNumb = numericSubstitute;
                    foreach (var prop in this.GetAllPublicProperties(property.Symbol, semModel))
                    {
                        var prefix = string.IsNullOrWhiteSpace(namePrefix) ? property.Name : $"{namePrefix}.{property.Name}";
                        var (output, counter) = this.GetPropertyOutputAndCounter(prop, tempNumb, semModel, getSubPropertyOutput, namePrefix: prefix);
                        tempOutput.AppendLine(output);
                        tempNumb = counter;
                    }

                    if (tempOutput.Length > 0)
                    {
                        rawOutput = tempOutput.ToString();
                    }
                }

                if (rawOutput == null)
                {
                    Logger?.RecordInfo(StringRes.Info_NoMappingFoundUsingFallback);
                    rawOutput = this.Profile?.FallbackOutput;
                }
            }

            if (rawOutput == null)
            {
                // Should only reach here if profile?.FallbackOutput is null but that shouldn't be possible as profile validation checks for this.
                return (null, numericSubstitute);
            }

            if (!string.IsNullOrWhiteSpace(namePrefix))
            {
                rawOutput = rawOutput.Replace(Placeholder.PropertyName, $"{namePrefix}.{Placeholder.PropertyName}");
            }

            return this.FormatOutput(rawOutput, property.PropertyType, property.Name, numericSubstitute, property.Symbol, property.Attributes, getSubPropertyOutput);
        }

        private (string output, int counter) FormatOutput(string rawOutput, string type, string name, int numericSubstitute, ITypeSymbol symbol, List<AttributeDetails> attributes, Func<(List<string> strings, int count)> getSubPropertyOutput)
        {
            Logger?.RecordInfo(StringRes.Info_FormattingOutputForProperty.WithParams(name));

            var result = rawOutput.Replace(Placeholder.PropertyName, name)
                                  .Replace(Placeholder.PropertyNameWithSpaces, name.AddSpacesToCamelCase());

            if (!string.IsNullOrWhiteSpace(xname))
            {
                result = result.Replace(Placeholder.RepeatingXName, xname);
            }
            else
            {
                if (result.Contains(Placeholder.RepeatingXName))
                {
                    var placeholderPos = result.IndexOf(Placeholder.RepeatingXName);

                    // If placeholder is in an attribute (expected) then remove the attribute too
                    if (result.Substring(placeholderPos - 2, 2) == "=\""
                     && result.Substring(placeholderPos + Placeholder.RepeatingXName.Length, 1) == "\"")
                    {
                        var attStartPos = result.Substring(0, placeholderPos).LastIndexOf(' ');

                        result = result.Remove(attStartPos, placeholderPos - attStartPos + Placeholder.RepeatingXName.Length + 1);
                    }
                    else
                    {
                        result = result.Replace(Placeholder.RepeatingXName, string.Empty);
                    }
                }
            }

            if (result.Contains(Placeholder.XName))
            {
                // xname is a combination of propertyname and the type of the UIElement. e.g. IdTextBlock, FirstNameTextBox
                // Save this for possible future reuse in subsequent properties
                xname = $"{name}{rawOutput.GetXamlElement()}";

                result = result.Replace(Placeholder.XName, xname);
            }

            if (type.IsGenericTypeName())
            {
                Logger?.RecordInfo(StringRes.Info_FormattingOutputForGenericType.WithParams(type));

                type = type.ToCSharpFormat();
                var typeArgument = type.Substring(type.IndexOf("<", StringComparison.Ordinal) + 1, type.Length - type.IndexOf("<", StringComparison.Ordinal) - 2);
                result = result.Replace(Placeholder.PropertyType, typeArgument.AsXamlFriendlyTypeArgument());
            }
            else
            {
                Logger?.RecordInfo(StringRes.Info_FormattingOutputForNonGenericType.WithParams(type));

                result = result.Replace(Placeholder.PropertyType, type.AsXamlFriendlyTypeArgument());
            }

            if (rawOutput.Contains(Placeholder.SubProperties))
            {
                bool subPropertyInsideGridPlusRowDefs =
                    (rawOutput.IndexOf(Placeholder.SubProperties, StringComparison.OrdinalIgnoreCase)
                    > rawOutput.IndexOf(GridWithRowDefsIndicator, StringComparison.OrdinalIgnoreCase)
                 && rawOutput.IndexOf("/" + GridWithRowDefsIndicator, StringComparison.OrdinalIgnoreCase)
                    > rawOutput.IndexOf(Placeholder.SubProperties, StringComparison.OrdinalIgnoreCase))
                    ||
                    (rawOutput.IndexOf(Placeholder.SubProperties, StringComparison.OrdinalIgnoreCase)
                    > rawOutput.IndexOf(GridWithRowDefs2ColsIndicator, StringComparison.OrdinalIgnoreCase)
                    && rawOutput.IndexOf("/" + GridWithRowDefs2ColsIndicator, StringComparison.OrdinalIgnoreCase)
                    > rawOutput.IndexOf(Placeholder.SubProperties, StringComparison.OrdinalIgnoreCase));

                var subProps = getSubPropertyOutput?.Invoke();

                var replacement = new StringBuilder();

                if (!result.StartsWith(Placeholder.SubProperties))
                {
                    // put the sub-property on a new line if there's other output first
                    replacement.AppendLine();
                }

                if (subProps.HasValue)
                {
                    Logger?.RecordInfo(StringRes.Info_FoundPropertyCount.WithParams(subProps.Value.strings.Count));

                    foreach (var subProp in subProps.Value.strings)
                    {
                        replacement.AppendLine(subProp);
                    }
                }

                result = result.Replace(Placeholder.SubProperties, replacement.ToString());

                if (subPropertyInsideGridPlusRowDefs)
                {
                    Logger?.RecordInfo(StringRes.Info_FormattingSubpropertiesInsideGrid);
                    var opener = new StringBuilder();

                    opener.AppendLine("<Grid>");

                    if (rawOutput.Contains(GridWithRowDefs2ColsIndicator))
                    {
                        Logger?.RecordInfo(StringRes.Info_AddingColDefsToGrid);

                        opener.AppendLine("<Grid.ColumnDefinitions>");
                        opener.AppendLine("<ColumnDefinition Width=\"Auto\" />");
                        opener.AppendLine("<ColumnDefinition Width=\"*\" />");
                        opener.AppendLine("</Grid.ColumnDefinitions>");
                    }

                    if (subProps.HasValue)
                    {
                        opener.AppendLine("<Grid.RowDefinitions>");

                        Logger?.RecordInfo(StringRes.Info_AddedRowDefsCount.WithParams(subProps.Value.count));

                        for (int i = 1; i <= subProps.Value.count; i++)
                        {
                            opener.AppendLine(i < subProps.Value.count
                                ? "<RowDefinition Height=\"Auto\" />"
                                : "<RowDefinition Height=\"*\" />");
                        }

                        opener.Append("</Grid.RowDefinitions>");
                    }

                    var placeHolderPos = result.IndexOf(GridWithRowDefs2ColsIndicator, StringComparison.InvariantCultureIgnoreCase);
                    if (placeHolderPos > -1)
                    {
                        var placeHolderEndPos = result.IndexOf(">", placeHolderPos, StringComparison.InvariantCultureIgnoreCase);

                        result = result.Substring(0, placeHolderPos - 1) + opener + result.Substring(placeHolderEndPos + 1);

                        placeHolderPos = result.IndexOf("</" + GridWithRowDefs2ColsIndicator + ">", StringComparison.InvariantCultureIgnoreCase);

                        result = result.Substring(0, placeHolderPos) + "</Grid>" + result.Substring(placeHolderPos + GridWithRowDefs2ColsIndicator.Length + 3);
                    }

                    placeHolderPos = result.IndexOf(GridWithRowDefsIndicator, StringComparison.InvariantCultureIgnoreCase);

                    if (placeHolderPos > -1)
                    {
                        var placeHolderEndPos = result.IndexOf(">", placeHolderPos, StringComparison.InvariantCultureIgnoreCase);

                        result = result.Substring(0, placeHolderPos - 1) + opener + result.Substring(placeHolderEndPos + 1);

                        placeHolderPos = result.IndexOf("</" + GridWithRowDefsIndicator + ">", StringComparison.InvariantCultureIgnoreCase);

                        result = result.Substring(0, placeHolderPos) + "</Grid>" + result.Substring(placeHolderPos + GridWithRowDefsIndicator.Length + 3);
                    }
                }
            }

            if (result.Contains(Placeholder.EnumMembers))
            {
                var enumMembers = symbol.GetMembers().Where(m => m.Kind == SymbolKind.Field && !m.IsImplicitlyDeclared).ToList();

                var replacement = new StringBuilder();

                if (!result.StartsWith(Placeholder.EnumMembers))
                {
                    // put the elements on a new line if there's other output first
                    replacement.AppendLine();
                }

                Logger?.RecordInfo(StringRes.Info_EnumElementCount.WithParams(enumMembers.Count));

                if (enumMembers.Any())
                {
                    foreach (var member in enumMembers)
                    {
                        var line = this.Profile.EnumMemberOutput.Replace(Placeholder.EnumElement, member.Name)
                                                                .Replace(Placeholder.EnumElementWithSpaces, member.Name.AddSpacesToCamelCase())
                                                                .Replace(Placeholder.EnumPropName, name);

                        replacement.AppendLine(line);
                    }
                }

                result = result.Replace(Placeholder.EnumMembers, replacement.ToString());
            }

            var currentNumber = numericSubstitute;

            while (result.Contains(Placeholder.IncrementingInteger))
            {
                Logger?.RecordInfo(StringRes.Info_ReplacingIncIntPlaceholder);

                var subPosition = result.IndexOf(Placeholder.IncrementingInteger, StringComparison.OrdinalIgnoreCase);

                result = result.Remove(subPosition, Placeholder.IncrementingInteger.Length);
                result = result.Insert(subPosition, numericSubstitute.ToString());

                numericSubstitute += 1;

                currentNumber = numericSubstitute - 1;
            }

            while (result.Contains(Placeholder.RepeatingInteger))
            {
                Logger?.RecordInfo(StringRes.Info_ReplacingRepIntPlaceholder);

                var subPosition = result.IndexOf(Placeholder.RepeatingInteger, StringComparison.OrdinalIgnoreCase);

                result = result.Remove(subPosition, Placeholder.RepeatingInteger.Length);
                result = result.Insert(subPosition, currentNumber.ToString());
            }

            if (rawOutput == Placeholder.NoOutput)
            {
                result = string.Empty;
            }

            var attributePattern = "\\$att:(?'AttribName'[a-zA-Z]{2,}):(?'Replace'[ a-zA-Z0-9=\\\"\\\"\\{\\[\\]\\}]{1,})\\$";
            var regex = new Regex(attributePattern);

            var match = regex.Match(result);

            while (match.Success)
            {
                var attrib = match.Groups["AttribName"].Value;

                if (attributes?.Any(a => a.Name == attrib || $"{a.Name}Attribute" == attrib || a.Name == $"{attrib}Attribute") == true)
                {
                    var replace = match.Groups["Replace"].Value;

                    var substitute = replace.GetBetween("[", "]");

                    var attributeOfInterest = attributes.First(a => a.Name == attrib || $"{a.Name}Attribute" == attrib || a.Name == $"{attrib}Attribute");

                    if (int.TryParse(substitute, out int subIndex))
                    {
                        var replaceVal = attributeOfInterest.Arguments.FirstOrDefault(a => a.Index == subIndex);

                        replace = replace.Replace($"[{substitute}]", replaceVal.Value);
                    }
                    else
                    {
                        var replaceVal = attributeOfInterest.Arguments.FirstOrDefault(a => a.Name == substitute);

                        replace = replace.Replace($"[{substitute}]", replaceVal.Value);
                    }

                    result = result.Replace(match.Groups[0].Value, replace);
                }
                else
                {
                    // If the attribute isn't specified on the property then output nothing
                    result = result.Replace(match.Groups[0].Value, string.Empty);
                }

                match = match.NextMatch();
            }

            var finalResult = result.FormatXaml(CodeParserBase.XamlIndentSize);

            return (finalResult, numericSubstitute);
        }

        private Mapping GetMappingOfInterest(PropertyDetails property)
        {
            // Enums can be mapped by name or that they're enums - check enum first
            if (property.Symbol?.BaseType?.Name == "Enum")
            {
                var enumMapping = this.GetMappingOfInterest("enum", property.Name, property.IsReadOnly);

                if (enumMapping != null)
                {
                    Logger?.RecordInfo(StringRes.Info_FoundEnumMapping.WithParams(property.Name));
                    return enumMapping;
                }
                else
                {
                    Logger?.RecordInfo(StringRes.Info_NoEnumMappingFound.WithParams(property.Name));
                }
            }

            return this.GetMappingOfInterest(property.PropertyType, property.Name, property.IsReadOnly);
        }

        private Mapping GetMappingOfInterest(string type, string name, bool isReadOnly)
        {
            var typeMappings = this.Profile.Mappings.Where(m => type.ToCSharpFormat().MatchesAnyOfInCSharpFormat(m.Type)).ToList();

            if (!isReadOnly)
            {
                Logger?.RecordInfo(StringRes.Info_ProperyIsNotReadOnly);
                typeMappings = typeMappings.Where(m => m.IfReadOnly == false).ToList();
            }

            Mapping mappingOfInterest = null;

            // Readonly types match readonly mappings first
            if (isReadOnly)
            {
                Logger?.RecordInfo(StringRes.Info_PropertyIsReadOnly);
                mappingOfInterest = typeMappings.FirstOrDefault(m => name.ToLowerInvariant().ContainsAnyOf(m.NameContains.ToLowerInvariant()) && m.IfReadOnly)
                                 ?? typeMappings.FirstOrDefault(m => string.IsNullOrWhiteSpace(m.NameContains) && m.IfReadOnly);
            }

            // writeable types don't match readonly mappings
            // readonly types match writeable mappings if no readonly mappings
            if (mappingOfInterest == null)
            {
                Logger?.RecordInfo(StringRes.Info_LookingForReadWriteMappings);
                mappingOfInterest = typeMappings.FirstOrDefault(m => name.ToLowerInvariant().ContainsAnyOf(m.NameContains.ToLowerInvariant()) && !m.IfReadOnly)
                                 ?? typeMappings.FirstOrDefault(m => string.IsNullOrWhiteSpace(m.NameContains) && !m.IfReadOnly);
            }

            return mappingOfInterest;
        }
    }
}
