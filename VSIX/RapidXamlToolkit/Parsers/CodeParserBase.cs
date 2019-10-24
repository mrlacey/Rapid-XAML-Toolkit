// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
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

        public CodeParserBase(ILogger logger, ProjectType projectType, int xamlIndent, Profile profileOverload = null)
        {
            Logger = logger;
            XamlIndentSize = xamlIndent;

            this.Profile = profileOverload ?? GetSettings().GetActiveProfile(projectType);

            xname = string.Empty;  // Reset this on parser creation as parsers are created for each new conversion and don't want old values.
        }

        public static IServiceProvider ServiceProvider { get; set; }

        public static ILogger Logger { get; set; }

        public static int XamlIndentSize { get; set; } = 4;

        public virtual string FileExtension { get; } = string.Empty;

        public Profile Profile { get; }

        protected static string[] TypesToSkipWhenCheckingForSubMembers { get; } = new[] { "String", "ValueType", "Object" };

        protected static string[] NamesOfPropertiesToExcludeFromOutput { get; } = new[] { "IsInDesignMode", "IsInDesignModeStatic", "DataStore" };

        [NotUnitTestable("Relies on ConfiguredSettings which relies on VS Package infrastructure.")]
        public static Settings GetSettings()
        {
            var configuredSettings = new ConfiguredSettings(ServiceProvider);

            return configuredSettings.ActualSettings;
        }

        public static string GetSelectionMemberName(List<string> names)
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

        public ParserOutput GetSingleItemOutput(SyntaxNode documentRoot, SemanticModel semModel, int caretPosition)
        {
            Logger?.RecordInfo(StringRes.Info_GetSingleItemOutput);
            var (propertyNode, classNode, methodNode) = this.GetNodeUnderCaret(documentRoot, caretPosition);

            if (propertyNode != null)
            {
                Logger?.RecordInfo(StringRes.Info_GetSinglePropertyOutput);

                var propDetails = this.GetPropertyDetails(propertyNode, semModel);

                if (propDetails != null)
                {
                    var (output, name, _) = this.GetOutputToAdd(semModel, propDetails);

                    return new ParserOutput
                    {
                        Name = name,
                        Output = output,
                        OutputType = ParserOutputType.Member,
                    };
                }
            }

            if (methodNode != null)
            {
                Logger?.RecordInfo(StringRes.Info_GetSingleMethodOutput);

                var methodDetails = this.GetMethodDetails(methodNode, semModel);

                if (methodDetails != null)
                {
                    var (output, name, _) = this.GetOutputToAdd(semModel, methodDetails);

                    return new ParserOutput
                    {
                        Name = name,
                        Output = output,
                        OutputType = ParserOutputType.Member,
                    };
                }
            }

            if (classNode != null)
            {
                Logger?.RecordInfo(StringRes.Info_GetSingleClassOutput);

                var className = this.GetIdentifier(classNode);

                var classTypeSymbol = (ITypeSymbol)semModel.GetDeclaredSymbol(classNode);
                var properties = this.GetAllPublicProperties(classTypeSymbol, semModel);

                var methods = this.GetAllPublicVoidMethods(classTypeSymbol, semModel);

                var output = new StringBuilder();

                var classGrouping = this.Profile.ClassGrouping;

                if (!string.IsNullOrWhiteSpace(classGrouping))
                {
                    output.AppendLine($"<{FormattedClassGroupingOpener(classGrouping)}>");
                }

                if (properties.Any() || methods.Any())
                {
                    Logger?.RecordInfo(StringRes.Info_ClassPropertyCount.WithParams(properties.Count));

                    var propertyOutput = new List<string>();

                    var numericCounter = 0;

                    foreach (var prop in properties)
                    {
                        Logger?.RecordInfo(StringRes.Info_AddingPropertyToOutput.WithParams(prop.Name));
                        var toAdd = this.GetOutputToAdd(semModel, prop, numericCounter);

                        numericCounter = toAdd.counter;
                        propertyOutput.Add(toAdd.output);
                    }

                    foreach (var method in methods)
                    {
                        Logger?.RecordInfo(StringRes.Info_AddingPropertyToOutput.WithParams(method.Name));
                        var toAdd = this.GetOutputToAdd(semModel, method, numericCounter);

                        numericCounter = toAdd.counter;
                        propertyOutput.Add(toAdd.output);
                    }

                    if (classGrouping != null
                     && (classGrouping.Equals(GridWithRowDefsIndicator, StringComparison.InvariantCultureIgnoreCase)
                      || classGrouping.Equals(GridWithRowDefs2ColsIndicator, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        Logger?.RecordInfo(StringRes.Info_AddingGridToOutput);

                        if (classGrouping.Equals(GridWithRowDefs2ColsIndicator, StringComparison.InvariantCultureIgnoreCase))
                        {
                            Logger?.RecordInfo(StringRes.Info_AddingColDefsToGrid);

                            output.AppendLine("<Grid.ColumnDefinitions>");
                            output.AppendLine("<ColumnDefinition Width=\"Auto\" />");
                            output.AppendLine("<ColumnDefinition Width=\"*\" />");
                            output.AppendLine("</Grid.ColumnDefinitions>");
                        }

                        output.AppendLine("<Grid.RowDefinitions>");

                        Logger?.RecordInfo(StringRes.Info_AddedRowDefsCount.WithParams(numericCounter));
                        for (var i = 1; i <= numericCounter + 1; i++)
                        {
                            output.AppendLine(i <= numericCounter
                                ? "<RowDefinition Height=\"Auto\" />"
                                : "<RowDefinition Height=\"*\" />");
                        }

                        output.AppendLine("</Grid.RowDefinitions>");
                    }

                    foreach (var po in propertyOutput)
                    {
                        if (!string.IsNullOrEmpty(po))
                        {
                            output.AppendLine(po);
                        }
                    }
                }
                else
                {
                    Logger?.RecordInfo(StringRes.Info_ClassNoPublicProperties);
                    output.AppendLine(StringRes.UI_NoPropertiesXaml);
                }

                if (!string.IsNullOrWhiteSpace(classGrouping))
                {
                    output.Append($"</{FormattedClassGroupingCloser(classGrouping)}>");
                }

                var finalOutput = output.ToString().FormatXaml(CodeParserBase.XamlIndentSize);

                return new ParserOutput
                {
                    Name = className,
                    Output = finalOutput,
                    OutputType = ParserOutputType.Class,
                };
            }

            Logger?.RecordInfo(StringRes.Info_NoPropertiesToOutput);
            return ParserOutput.Empty;
        }

        public virtual List<PropertyDetails> GetAllPublicProperties(ITypeSymbol typeSymbol, SemanticModel semModel)
        {
            return new List<PropertyDetails>();
        }

        public virtual List<MethodDetails> GetAllPublicVoidMethods(ITypeSymbol typeSymbol, SemanticModel semModel)
        {
            return new List<MethodDetails>();
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

        protected virtual (SyntaxNode propertyNode, SyntaxNode classNode, SyntaxNode methodNode) GetNodeUnderCaret(SyntaxNode documentRoot, int caretPosition)
        {
            throw new NotImplementedException();
        }

        protected virtual PropertyDetails GetPropertyDetails(SyntaxNode propertyDeclaration, SemanticModel semModel)
        {
            throw new NotImplementedException();
        }

        protected virtual MethodDetails GetMethodDetails(SyntaxNode methodDeclaration, SemanticModel semModel)
        {
            throw new NotImplementedException();
        }

        protected virtual (List<string> strings, int count) GetSubPropertyOutput(PropertyDetails property, SemanticModel semModel)
        {
            throw new NotImplementedException();
        }

        protected virtual string GetIdentifier(SyntaxNode syntaxNode)
        {
            throw new NotImplementedException();
        }

        protected (string output, string name, int counter) GetOutputToAdd(SemanticModel semModel, PropertyDetails prop, int numericCounter = 0)
        {
            var (output, counter) = this.GetPropertyOutputAndCounter(prop, numericCounter, semModel, () => this.GetSubPropertyOutput(prop, semModel));

            return (output, prop.Name, counter);
        }

        protected (string output, string name, int counter) GetOutputToAdd(SemanticModel semModel, MethodDetails method, int numericCounter = 0)
        {
            var (output, counter) = this.GetMethodOutputAndCounter(method, numericCounter, semModel);

            return (output, method.Name, counter);
        }

        protected (string output, int counter) GetSubPropertyOutputAndCounter(PropertyDetails property, int numericSubstitute)
        {
            // Type is blank as it's can't be used in a subproperty
            return this.FormatOutput(this.Profile.SubPropertyOutput, type: string.Empty, name: property.Name, numericSubstitute: numericSubstitute, symbol: property.Symbol, attributes: property.Attributes, getSubPropertyOutput: null);
        }

        // Mapping match order = Type > ReadOnly > Name
        // Mapping Type match priority = Att+TypeName > Att+T > TypeName > T
        protected (string output, int counter) GetPropertyOutputAndCounter(PropertyDetails property, int numericSubstitute, SemanticModel semModel, Func<(List<string> strings, int count)> getSubPropertyOutput = null, string namePrefix = "")
        {
            Mapping mappingOfInterest = null;
            string rawOutput = null;

            if (property.Attributes.Any())
            {
                // Get mappings with attributes that match the attributes or type we have
                mappingOfInterest = this.GetMappingOfInterest(property, includeAttributes: true);

                if (mappingOfInterest != null)
                {
                    rawOutput = mappingOfInterest.Output;
                }
                else
                {
                    mappingOfInterest = this.GetMappingOfInterest("T", property.Name, property.IsReadOnly, property.Attributes);

                    if (mappingOfInterest != null)
                    {
                        rawOutput = mappingOfInterest.Output;
                    }
                }
            }

            if (mappingOfInterest is null)
            {
                mappingOfInterest = this.GetMappingOfInterest(property, includeAttributes: false);

                if (mappingOfInterest != null)
                {
                    // Mapped a simple type
                    rawOutput = mappingOfInterest.Output;
                }
                else
                {
                    if (property.PropertyType.IsGenericTypeName())
                    {
                        // Handle mapping of generic type
                        var wildcardGenericType = property.PropertyType.Substring(0, property.PropertyType.ToCSharpFormat().IndexOf("<", StringComparison.Ordinal)) + "<T>";

                        Logger?.RecordInfo(StringRes.Info_SearchingForMappingWithGenericWildcard.WithParams(wildcardGenericType));

                        mappingOfInterest = this.GetMappingOfInterest(wildcardGenericType, property.Name, property.IsReadOnly, new List<AttributeDetails>());

                        if (mappingOfInterest != null)
                        {
                            rawOutput = mappingOfInterest.Output;
                        }
                    }

                    // See if there's a wildcard type mapping
                    var wildcardTypeMapping = this.GetMappingOfInterest("T", property.Name, property.IsReadOnly, new List<AttributeDetails>());

                    if (wildcardTypeMapping != null)
                    {
                        rawOutput = wildcardTypeMapping.Output;
                    }

                    if (rawOutput == null && semModel != null)
                    {
                        // Handle mapping of a complex type
                        var tempOutput = new StringBuilder();
                        var tempNumb = numericSubstitute;
                        foreach (var prop in this.GetAllPublicProperties(property.Symbol, semModel))
                        {
                            var prefix = string.IsNullOrWhiteSpace(namePrefix) ? property.Name : $"{namePrefix}.{property.Name}";

                            // Don't get into an infinite loop when a property has the same type as the containing class.
                            if (prefix.Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries).Contains(prop.Name))
                            {
                                break;
                            }

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

        protected (string output, int counter) GetMethodOutputAndCounter(MethodDetails method, int numericSubstitute, SemanticModel semModel)
        {
            var mappingOfInterest = this.GetMappingOfInterest(method);

            if (mappingOfInterest is null)
            {
                Logger?.RecordInfo(StringRes.Info_NoMappingFoundForMethod.WithParams(method.Name));
                return (null, numericSubstitute);
            }

            var rawOutput = mappingOfInterest.Output;

            return this.FormatMethodOutput(rawOutput, method.Name, numericSubstitute, method.Argument1Name, method.Argument2Name);
        }

        private (string output, int counter) FormatMethodOutput(string rawOutput, string name, int numericSubstitute, string arg1, string arg2)
        {
            Logger?.RecordInfo(StringRes.Info_FormattingOutputForMethod.WithParams(name));
            Logger?.RecordInfo(StringRes.Info_FormattingRawOutput.WithParams(rawOutput));

            if (rawOutput.Trim().Equals(Placeholder.NoOutput))
            {
                return (string.Empty, numericSubstitute);
            }
            else
            {
                var result = rawOutput.Replace(Placeholder.MethodName, name)
                                      .Replace(Placeholder.Argument1, arg1)
                                      .Replace(Placeholder.Argument2, arg2);

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

                var finalResult = result.FormatXaml(CodeParserBase.XamlIndentSize);

                return (finalResult, numericSubstitute);
            }
        }

        private (string output, int counter) FormatOutput(string rawOutput, string type, string name, int numericSubstitute, ITypeSymbol symbol, List<AttributeDetails> attributes, Func<(List<string> strings, int count)> getSubPropertyOutput)
        {
            Logger?.RecordInfo(StringRes.Info_FormattingOutputForProperty.WithParams(name));
            Logger?.RecordInfo(StringRes.Info_FormattingRawOutput.WithParams(rawOutput));

            var result = this.ReplaceAttributes(rawOutput, attributes);

            result = result.Replace(Placeholder.PropertyName, name)
                           .Replace(Placeholder.PropertyNameWithSpaces, name.AddSpacesToCamelCase())
                           .Replace(Placeholder.SafePropertyName, name.MakeNameSafe());

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

            var finalResult = result.FormatXaml(CodeParserBase.XamlIndentSize);

            return (finalResult, numericSubstitute);
        }

        private string ReplaceAttributes(string rawOutput, List<AttributeDetails> attributes)
        {
            var regex = new Regex(AllowedPlaceholderValidator.AttributePlaceholderPattern);

            var result = rawOutput;

            var match = regex.Match(result);

            while (match.Success)
            {
                var attrib = match.Groups["AttribName"].Value;

                Logger?.RecordInfo(StringRes.Info_OutputContainsAttributePlaceholder.WithParams(attrib));

                if (attributes?.Any(a => a.Name == attrib || $"{a.Name}Attribute" == attrib || a.Name == $"{attrib}Attribute") == true)
                {
                    var replace = match.Groups["Replace"].Value;

                    var substitute = replace.GetBetween("[", "]");

                    var attributeOfInterest = attributes.First(a => a.Name == attrib || $"{a.Name}Attribute" == attrib || a.Name == $"{attrib}Attribute");

                    if (int.TryParse(substitute, out int subIndex))
                    {
                        var replaceVal = attributeOfInterest.Arguments.FirstOrDefault(a => a.Index == subIndex);

                        if (replaceVal is null)
                        {
                            Logger?.RecordInfo(StringRes.Info_AttributeParameterWIthoutIndex.WithParams(subIndex));

                            // Output nothing for the attribute rather than something incomplete.
                            replace = string.Empty;
                        }
                        else
                        {
                            replace = replace.Replace($"[{substitute}]", replaceVal.Value);
                        }
                    }
                    else
                    {
                        var replaceVal = attributeOfInterest.Arguments.FirstOrDefault(a => a.Name == substitute);

                        if (replaceVal is null)
                        {
                            Logger?.RecordInfo(StringRes.Info_NamedAttributeParameterNotFound.WithParams(substitute));

                            // Output nothing for the attribute rather than something incomplete.
                            replace = string.Empty;
                        }
                        else
                        {
                            replace = replace.Replace($"[{substitute}]", replaceVal.Value);
                        }
                    }

                    result = result.Replace(match.Groups[0].Value, replace);
                }
                else
                {
                    Logger?.RecordInfo(StringRes.Info_AttributeNotFoundOnProperty);

                    var knownAttributes = string.Empty;

                    if (attributes.Any())
                    {
                        var attributeBuilder = new StringBuilder();

                        foreach (var att in attributes)
                        {
                            attributeBuilder.Append($"{att.Name}(");
                            foreach (var arg in att.Arguments)
                            {
                                attributeBuilder.Append($"{arg.Index}={arg.Name}:{arg.Value},");
                            }

                            attributeBuilder.Append(") ");
                        }

                        knownAttributes = attributeBuilder.ToString();
                    }
                    else
                    {
                        knownAttributes = "*NONE*";
                    }

                    Logger?.RecordInfo(StringRes.Info_KnownAttributes.WithParams(knownAttributes));

                    // If the attribute isn't specified on the property then chek for the fallback.
                    var fallback = match.Groups["Fallback"].Value;

                    if (!string.IsNullOrEmpty(fallback))
                    {
                        Logger?.RecordInfo(StringRes.Info_AddingAttributeFallback);

                        // Skip the first two chars as they just indicate the start of the fallback.
                        // Swap '@' for '$' to allow for the fallback containing replacements.
                        var fallbackToUse = fallback.Substring(2).Replace('@', '$');

                        result = result.Replace(match.Groups[0].Value, fallbackToUse);
                    }
                    else
                    {
                        // If no fallback is specified then output nothing.
                        result = result.Replace(match.Groups[0].Value, string.Empty);
                    }
                }

                match = match.NextMatch();
            }

            return result;
        }

        private Mapping GetMappingOfInterest(PropertyDetails property, bool includeAttributes)
        {
            // Enums can be mapped by name or that they're enums - check enum first
            if (property.Symbol?.BaseType?.Name == "Enum")
            {
                var enumMapping = this.GetMappingOfInterest("enum", property.Name, property.IsReadOnly, includeAttributes ? property.Attributes : new List<AttributeDetails>());

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

            return this.GetMappingOfInterest(property.PropertyType, property.Name, property.IsReadOnly, property.Attributes);
        }

        private Mapping GetMappingOfInterest(string type, string name, bool isReadOnly, List<AttributeDetails> attributes)
        {
            List<Mapping> typeMappings = null;

            if (attributes.Any())
            {
                typeMappings = this.Profile.Mappings.Where(
                    m => type.WithoutNamespaces().ToCSharpFormat().MatchesAnyOfInCSharpFormat(m.Type.RemoveAttributesFromTypes())
                      && m.Type.GetAttributes().Intersects(string.Join("|", attributes.Select(a => a.Name).ToList()))).ToList();

                if (!typeMappings.Any())
                {
                    typeMappings = this.Profile.Mappings.Where(
                        m => "T".MatchesAnyOfInCSharpFormat(m.Type.RemoveAttributesFromTypes())
                          && m.Type.GetAttributes().Intersects(string.Join("|", attributes.Select(a => a.Name).ToList()))).ToList();
                }

                if (!typeMappings.Any())
                {
                    typeMappings = this.Profile.Mappings.Where(
                        m => type.WithoutNamespaces().ToCSharpFormat().MatchesAnyOfInCSharpFormat(m.Type)).ToList();
                }
            }
            else
            {
                typeMappings = this.Profile.Mappings.Where(
                    m => type.WithoutNamespaces().ToCSharpFormat().MatchesAnyOfInCSharpFormat(m.Type)).ToList();
            }

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
                mappingOfInterest = typeMappings.FirstOrDefault(m => name.ToLowerInvariant().ContainsAnyOf(m?.NameContains?.ToLowerInvariant() ?? string.Empty) && !m.IfReadOnly)
                                 ?? typeMappings.FirstOrDefault(m => string.IsNullOrWhiteSpace(m?.NameContains) && !m.IfReadOnly);
            }

            return mappingOfInterest;
        }

        private Mapping FirstWithTwoTypes(List<Mapping> mappings, string methodName, string typeName1, string typeName2)
        {
            // Both types set explicitly
            // With or without space
            var mappingOfInterest = mappings.FirstOrDefault(
                    m => methodName.ToLowerInvariant().ContainsAnyOf(m?.NameContains?.ToLowerInvariant() ?? string.Empty)
                      && (m.Type.EndsWith($"({typeName1},{typeName2})")
                      || m.Type.EndsWith($"({typeName1}, {typeName2})")))
                ?? mappings.FirstOrDefault(
                    m => string.IsNullOrWhiteSpace(m?.NameContains?.ToLowerInvariant() ?? string.Empty)
                      && (m.Type.EndsWith($"({typeName1},{typeName2})")
                      || m.Type.EndsWith($"({typeName1}, {typeName2})")));

            if (mappingOfInterest != null)
            {
                return mappingOfInterest;
            }

            // first anything, second explicit
            mappingOfInterest = mappings.FirstOrDefault(
                    m => methodName.ToLowerInvariant().ContainsAnyOf(m?.NameContains?.ToLowerInvariant() ?? string.Empty)
                      && (m.Type.EndsWith($"(T,{typeName2})") || m.Type.EndsWith($"(T, {typeName2})")))
                ?? mappings.FirstOrDefault(
                    m => string.IsNullOrWhiteSpace(m?.NameContains?.ToLowerInvariant() ?? string.Empty)
                      && (m.Type.EndsWith($"(T,{typeName2})") || m.Type.EndsWith($"(T, {typeName2})")));

            if (mappingOfInterest != null)
            {
                return mappingOfInterest;
            }

            // Second anything, first explicit
            mappingOfInterest = mappings.FirstOrDefault(
                    m => methodName.ToLowerInvariant().ContainsAnyOf(m?.NameContains?.ToLowerInvariant() ?? string.Empty)
                      && (m.Type.EndsWith($"({typeName1},T)") || m.Type.EndsWith($"({typeName1}, T)")))
                ?? mappings.FirstOrDefault(
                    m => string.IsNullOrWhiteSpace(m?.NameContains?.ToLowerInvariant() ?? string.Empty)
                      && (m.Type.EndsWith($"({typeName1},T)") || m.Type.EndsWith($"({typeName1}, T)")));

            if (mappingOfInterest != null)
            {
                return mappingOfInterest;
            }

            // Both anything
            mappingOfInterest = mappings.FirstOrDefault(
                    m => methodName.ToLowerInvariant().ContainsAnyOf(m?.NameContains?.ToLowerInvariant() ?? string.Empty)
                      && (m.Type.EndsWith($"(T,T)") || m.Type.EndsWith($"(T, T)")))
                ?? mappings.FirstOrDefault(
                    m => string.IsNullOrWhiteSpace(m?.NameContains?.ToLowerInvariant() ?? string.Empty)
                      && (m.Type.EndsWith($"(T,T)") || m.Type.EndsWith($"(T, T)")));

            if (mappingOfInterest != null)
            {
                return mappingOfInterest;
            }

            return null;
        }

        private Mapping GetMappingOfInterest(MethodDetails method)
        {
            List<Mapping> typeMappings = new List<Mapping>();

            if (method.Attributes.Any())
            {
                foreach (var attribute in method.Attributes)
                {
                    typeMappings.AddRange(this.Profile.Mappings.Where(m => m.Type.StartsWith($"[{attribute.Name}]method(")).ToList());
                }
            }

            typeMappings.AddRange(this.Profile.Mappings.Where(m => m.Type.StartsWith("method(")).ToList());

            if (!string.IsNullOrWhiteSpace(method.Argument2Type?.Name))
            {
                var arg1TypeName = method.Argument1Type.Name.ToLowerInvariant();
                var arg2TypeName = method.Argument2Type.Name.ToLowerInvariant();

                return this.FirstWithTwoTypes(typeMappings, method.Name, arg1TypeName, arg2TypeName);
            }
            else if (!string.IsNullOrWhiteSpace(method.Argument1Type?.Name))
            {
                var arg1TypeName = method.Argument1Type.Name.ToLowerInvariant();

                var mappingOfInterest =
                     typeMappings.FirstOrDefault(
                         m => method.Name.ToLowerInvariant().ContainsAnyOf(m?.NameContains?.ToLowerInvariant() ?? string.Empty)
                           && (m.Type.Contains($"]method({arg1TypeName})") || m.Type.Equals($"method({arg1TypeName})")))
                     ?? typeMappings.FirstOrDefault(
                         m => string.IsNullOrWhiteSpace(m?.NameContains?.ToLowerInvariant() ?? string.Empty)
                           && (m.Type.Contains($"]method({arg1TypeName})") || m.Type.Equals($"method({arg1TypeName})")));

                if (mappingOfInterest != null)
                {
                    return mappingOfInterest;
                }

                mappingOfInterest =
                    typeMappings.FirstOrDefault(
                        m => method.Name.ToLowerInvariant().ContainsAnyOf(m?.NameContains?.ToLowerInvariant() ?? string.Empty)
                          && (m.Type.Contains($"]method(T)") || m.Type.Equals($"method(T)")))
                    ?? typeMappings.FirstOrDefault(
                        m => string.IsNullOrWhiteSpace(m?.NameContains?.ToLowerInvariant() ?? string.Empty)
                          && (m.Type.Contains($"]method(T)") || m.Type.Equals($"method(T)")));

                if (mappingOfInterest != null)
                {
                    return mappingOfInterest;
                }
            }
            else
            {
                return typeMappings.FirstOrDefault(
                        m => method.Name.ToLowerInvariant().ContainsAnyOf(m?.NameContains?.ToLowerInvariant() ?? string.Empty)
                          && m.Type.ToLowerInvariant().Contains("method()"))
                    ?? typeMappings.FirstOrDefault(
                        m => string.IsNullOrWhiteSpace(m?.NameContains?.ToLowerInvariant() ?? string.Empty)
                          && m.Type.ToLowerInvariant().Contains("method()"));
            }

            return null;
        }
    }
}
