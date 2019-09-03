// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RapidXamlToolkit.Logging;
using RapidXamlToolkit.Options;
using RapidXamlToolkit.Resources;

namespace RapidXamlToolkit.Parsers
{
    public class CSharpParser : CodeParserBase, IDocumentParser
    {
        public CSharpParser(ILogger logger, ProjectType projectType, int xamlIndent = 4, Profile profileOverload = null)
            : base(logger, projectType, xamlIndent, profileOverload)
        {
            Logger?.RecordInfo(StringRes.Info_AnalyzingCSharpCode.WithParams(Telemetry.CoreDetails.GetVersion()));
        }

        public override string FileExtension { get; } = "cs";

        public ParserOutput GetSingleItemOutput(SyntaxNode documentRoot, SemanticModel semModel, int caretPosition)
        {
            Logger?.RecordInfo(StringRes.Info_GetSingleItemOutput);
            var (propertyNode, classNode) = this.GetNodeUnderCaret(documentRoot, caretPosition);

            if (propertyNode != null)
            {
                Logger?.RecordInfo(StringRes.Info_GetSinglePropertyOutput);

                var propDetails = this.GetPropertyDetails(propertyNode, semModel);

                var (output, name, _) = this.GetOutputToAdd(semModel, propDetails);

                return new ParserOutput
                {
                    Name = name,
                    Output = output,
                    OutputType = ParserOutputType.Property,
                };
            }

            if (classNode != null)
            {
                Logger?.RecordInfo(StringRes.Info_GetSingleClassOutput);

                var className = this.GetIdentifier(classNode);

                var classTypeSymbol = (ITypeSymbol)semModel.GetDeclaredSymbol(classNode);
                var properties = this.GetAllPublicProperties(classTypeSymbol, semModel);

                var output = new StringBuilder();

                var classGrouping = this.Profile.ClassGrouping;

                if (!string.IsNullOrWhiteSpace(classGrouping))
                {
                    output.AppendLine($"<{FormattedClassGroupingOpener(classGrouping)}>");
                }

                if (properties.Any())
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
                        for (int i = 1; i <= numericCounter + 1; i++)
                        {
                            output.AppendLine(i <= numericCounter
                                ? "<RowDefinition Height=\"Auto\" />"
                                : "<RowDefinition Height=\"*\" />");
                        }

                        output.AppendLine("</Grid.RowDefinitions>");
                    }

                    foreach (var po in propertyOutput)
                    {
                        output.AppendLine(po);
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

        public ParserOutput GetSelectionOutput(SyntaxNode documentRoot, SemanticModel semModel, int selStart, int selEnd)
        {
            Logger?.RecordInfo(StringRes.Info_GetSelectionOutput);

            var allProperties = documentRoot.DescendantNodes().OfType<PropertyDeclarationSyntax>().ToList();

            Logger?.RecordInfo(StringRes.Info_DocumentPropertyCount.WithParams(allProperties.Count));

            var propertiesOfInterest = new List<PropertyDeclarationSyntax>();

            foreach (var prop in allProperties)
            {
                if (prop.FullSpan.End >= selStart && prop.FullSpan.Start < selEnd)
                {
                    propertiesOfInterest.Add(prop);
                }
            }

            Logger?.RecordInfo(StringRes.Info_PropertiesInSelectedAreaCount.WithParams(propertiesOfInterest.Count));

            var output = new StringBuilder();

            var numericCounter = 1;

            var propertyNames = new List<string>();

            foreach (var prop in propertiesOfInterest)
            {
                var propDetails = this.GetPropertyDetails(prop, semModel);

                if (propDetails.Name.IsOneOf(NamesOfPropertiesToExcludeFromOutput))
                {
                    Logger?.RecordInfo(StringRes.Info_NotIncludingExcludedProperty.WithParams(propDetails.Name));
                    continue;
                }

                Logger?.RecordInfo(StringRes.Info_AddingPropertyToOutput.WithParams(propDetails.Name));
                var toAdd = this.GetPropertyOutputAndCounter(propDetails, numericCounter, semModel, () => this.GetSubPropertyOutput(propDetails, semModel));

                if (!string.IsNullOrWhiteSpace(toAdd.output))
                {
                    numericCounter = toAdd.counter;
                    output.AppendLine(toAdd.output);

                    propertyNames.Add(propDetails.Name);
                }
            }

            if (propertyNames.Any())
            {
                var outputName = GetSelectionPropertiesName(propertyNames);

                Logger?.RecordInfo(StringRes.Info_ReturningOutput.WithParams(outputName));

                // Trim end of output to remove trailing newline
                return new ParserOutput
                {
                    Name = outputName,
                    Output = output.ToString().TrimEnd(),
                    OutputType = ParserOutputType.Selection,
                };
            }
            else
            {
                Logger?.RecordInfo(StringRes.Info_NoPropertiesToOutput);
                return ParserOutput.Empty;
            }
        }

        public override List<PropertyDetails> GetAllPublicProperties(ITypeSymbol typeSymbol, SemanticModel semModel)
        {
            var properties = new List<ISymbol>();

            foreach (var baseType in typeSymbol.GetSelfAndBaseTypes())
            {
                if (baseType.Name.IsOneOf(TypesToSkipWhenCheckingForSubProperties))
                {
                    continue;
                }

                switch (baseType.Kind)
                {
                    case SymbolKind.NamedType:
                        properties.AddRange(baseType.GetMembers().Where(m => m.Kind == SymbolKind.Property && m.DeclaredAccessibility == Accessibility.Public && !m.IsStatic));
                        break;
                    case SymbolKind.ErrorType:
                        Logger?.RecordInfo(StringRes.Info_CannotGetPropertiesForKnownType.WithParams(baseType.Name));
                        break;
                }
            }

            var result = new List<PropertyDetails>();

            foreach (var prop in properties)
            {
                if (prop.Name.IsOneOf(NamesOfPropertiesToExcludeFromOutput))
                {
                    Logger?.RecordInfo(StringRes.Info_NotIncludingExcludedProperty.WithParams(prop.Name));
                    continue;
                }

                var decRefs = prop.OriginalDefinition.DeclaringSyntaxReferences;

                if (decRefs.Any())
                {
                    var decRef = decRefs.First();

                    var syntax = decRef.SyntaxTree.GetRoot().DescendantNodes(decRef.Span).OfType<PropertyDeclarationSyntax>().First();

                    var details = this.GetPropertyDetails(syntax, semModel);

                    Logger?.RecordInfo(StringRes.Info_FoundSubProperty.WithParams(details.Name));
                    result.Add(details);
                }
                else
                {
                    Logger?.RecordInfo(StringRes.Info_FoundSubPropertyOfUnknownType.WithParams(prop.Name));
                    result.Add(new PropertyDetails { Name = prop.Name, PropertyType = UnknownOrInvalidTypeName, IsReadOnly = false, Symbol = null });
                }
            }

            return result;
        }

        private (List<string> strings, int count) GetSubPropertyOutput(PropertyDetails property, SemanticModel semModel)
        {
            var result = new List<string>();

            var subProperties = this.GetAllPublicProperties(property.Symbol, semModel);

            var numericSubstitute = 0;

            if (subProperties.Any())
            {
                Logger?.RecordInfo(StringRes.Info_SubpropertyCount.WithParams(property.Name, subProperties.Count));

                foreach (var subprop in subProperties)
                {
                    Logger?.RecordInfo(StringRes.Info_GettingSubPropertyOutput.WithParams(subprop.Name));

                    var (output, counter) = this.GetSubPropertyOutputAndCounter(subprop.Name, numericSubstitute: numericSubstitute);

                    numericSubstitute = counter;
                    result.Add(output);
                }
            }
            else
            {
                Logger?.RecordInfo(StringRes.Info_PropertyTypeHasNoSubProperties.WithParams(property.Name, property.PropertyType));

                // There are no subproperties so leave blank
                var (output, counter) = this.GetSubPropertyOutputAndCounter(string.Empty, numericSubstitute: numericSubstitute);

                numericSubstitute = counter;
                result.Add(output);
            }

            return (result, numericSubstitute);
        }

        private PropertyDetails GetPropertyDetails(PropertyDeclarationSyntax propertyDeclaration, SemanticModel semModel)
        {
            var propertyType = Unknown;

            switch (propertyDeclaration.Type)
            {
                case GenericNameSyntax gns:
                    propertyType = gns.ToString(); // Lazy way to get generic types
                    break;
                case PredefinedTypeSyntax pds:
                    propertyType = pds.Keyword.ValueText;
                    break;
                case IdentifierNameSyntax ins:
                    propertyType = ins.Identifier.ValueText;
                    break;
                case QualifiedNameSyntax qns:
                    propertyType = qns.Right.Identifier.ValueText;

                    if (qns.Right is GenericNameSyntax qgns)
                    {
                        propertyType += qgns.TypeArgumentList.ToString();
                    }

                    break;
                case NullableTypeSyntax nts:
                    propertyType = ((PredefinedTypeSyntax)nts.ElementType).Keyword.Text;

                    if (!propertyType.ToLowerInvariant().Contains("nullable"))
                    {
                        propertyType += nts.QuestionToken.Text;
                    }

                    break;
                case ArrayTypeSyntax ats:
                    propertyType = ats.ToString();
                    break;
            }

            var propertyName = this.GetIdentifier(propertyDeclaration);

            bool? propIsReadOnly;
            var setter = propertyDeclaration?.AccessorList?.Accessors
                .FirstOrDefault(a => a.RawKind == (ushort)SyntaxKind.SetAccessorDeclaration);

            if (setter == null)
            {
                propIsReadOnly = true;
            }
            else
            {
                var setterModifiers = setter.Modifiers;
                propIsReadOnly = setterModifiers.Any(m => m.Kind() == SyntaxKind.PrivateKeyword);
            }

            var pd = new PropertyDetails
            {
                Name = propertyName,
                PropertyType = propertyType,
                IsReadOnly = propIsReadOnly ?? false,
            };

            foreach (var attribList in propertyDeclaration.AttributeLists)
            {
                foreach (var attrib in attribList.Attributes)
                {
                    var att = new AttributeDetails
                    {
                        Name = attrib.Name.ToString(),
                    };

                    var count = 1;
                    foreach (var arg in attrib.ArgumentList.Arguments)
                    {
                        string name = null;
                        string value = null;

                        if (arg?.NameColon != null)
                        {
                            name = arg.NameColon.Name.ToString();
                            value = ((arg.NameColon.Parent as AttributeArgumentSyntax).Expression as IdentifierNameSyntax).Identifier.Value.ToString();
                        }
                        else if (arg?.NameEquals != null)
                        {
                            name = arg.NameEquals.Name.ToString();
                            value = ((arg.NameEquals.Parent as AttributeArgumentSyntax).Expression as IdentifierNameSyntax).Identifier.Value.ToString();
                        }
                        else
                        {
                            value = arg.ToString();
                        }

                        att.Arguments.Add(new AttributeArgumentDetails
                        {
                            Index = count++,
                            Name = name,
                            Value = value,
                        });
                    }

                    pd.Attributes.Add(att);
                }
            }

            Logger?.RecordInfo(StringRes.Info_IdentifiedPropertySummary.WithParams(pd.Name, pd.PropertyType, pd.IsReadOnly));

            ITypeSymbol typeSymbol = this.GetTypeSymbol(semModel, propertyDeclaration, pd);

            pd.Symbol = typeSymbol;

            return pd;
        }

        private string GetIdentifier(SyntaxNode syntaxNode)
        {
            return syntaxNode?.ChildTokens().FirstOrDefault(t => t.Kind() is SyntaxKind.IdentifierToken).ValueText;
        }

        private (PropertyDeclarationSyntax propertyNode, TypeDeclarationSyntax classNode) GetNodeUnderCaret(SyntaxNode documentRoot, int caretPosition)
        {
            PropertyDeclarationSyntax propertyNode = null;
            TypeDeclarationSyntax classNode = null;
            var currentNode = documentRoot.FindToken(caretPosition).Parent;

            while (currentNode != null && propertyNode == null && classNode == null)
            {
                if (currentNode is ClassDeclarationSyntax)
                {
                    classNode = currentNode as ClassDeclarationSyntax;
                }

                if (currentNode is StructDeclarationSyntax)
                {
                    classNode = currentNode as StructDeclarationSyntax;
                }

                if (currentNode is PropertyDeclarationSyntax)
                {
                    propertyNode = currentNode as PropertyDeclarationSyntax;
                }

                currentNode = currentNode.Parent;
            }

            return (propertyNode, classNode);
        }

        private (string output, string name, int counter) GetOutputToAdd(SemanticModel semModel, PropertyDetails prop, int numericCounter = 0)
        {
            var (output, counter) = this.GetPropertyOutputAndCounter(prop, numericCounter, semModel, () => this.GetSubPropertyOutput(prop, semModel));

            return (output, prop.Name, counter);
        }

        private ITypeSymbol GetTypeSymbol(SemanticModel semModel, PropertyDeclarationSyntax prop, PropertyDetails propDetails)
        {
            ITypeSymbol typeSymbol = null;

            ITypeSymbol GetWithFallback(TypeSyntax ts, SemanticModel sm, SyntaxTree tree)
            {
                ITypeSymbol result;

                try
                {
                    result = sm.GetTypeInfo(ts).Type;
                }
                catch (Exception)
                {
                    // By default, the semanticmodel passed into this method is the one for the active document.
                    // If the type is in another file, generate a new model to use to look up the typeinfo. Don't do this by default as it's expensive.
                    var localSemModel = CSharpCompilation.Create(string.Empty).AddSyntaxTrees(tree).GetSemanticModel(tree, ignoreAccessibility: true);

                    result = localSemModel.GetTypeInfo(ts).Type;
                }

                return result;
            }

            if (propDetails.PropertyType.IsGenericTypeName())
            {
                Logger?.RecordInfo(StringRes.Info_GettingGenericType);

                if (prop.Type is GenericNameSyntax gns)
                {
                    var t = gns.TypeArgumentList.Arguments.First();

                    typeSymbol = GetWithFallback(t, semModel, prop.SyntaxTree);
                }
                else if (prop.Type is QualifiedNameSyntax qns)
                {
                    var t = ((GenericNameSyntax)qns.Right).TypeArgumentList.Arguments.First();

                    typeSymbol = GetWithFallback(t, semModel, prop.SyntaxTree);
                }
                else
                {
                    Logger?.RecordInfo(StringRes.Info_PropertyTypeNotRecognizedAsGeneric.WithParams(propDetails.PropertyType));
                }
            }

            if (typeSymbol == null)
            {
                typeSymbol = GetWithFallback(prop.Type, semModel, prop.SyntaxTree);
            }

            if (typeSymbol == null)
            {
                Logger?.RecordInfo(StringRes.Info_PropertyCannotBeAnalyzed.WithParams(prop.ToString()));
            }

            return typeSymbol;
        }
    }
}
