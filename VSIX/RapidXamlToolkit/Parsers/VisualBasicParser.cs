// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using RapidXamlToolkit.Logging;
using RapidXamlToolkit.Options;
using RapidXamlToolkit.Resources;
using RapidXamlToolkit.VisualStudioIntegration;

namespace RapidXamlToolkit.Parsers
{
    public class VisualBasicParser : CodeParserBase, IDocumentParser
    {
        public VisualBasicParser(ILogger logger, ProjectType projectType, int xamlIndent = 4, Profile profileOverload = null)
            : base(logger, projectType, xamlIndent, profileOverload)
        {
            Logger?.RecordInfo(StringRes.Info_AnalyzingVisualBasicCode.WithParams(Telemetry.CoreDetails.GetVersion()));
        }

        public override string FileExtension { get; } = "vb";

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

            var allProperties = documentRoot.DescendantNodes().OfType<PropertyStatementSyntax>().ToList();

            Logger?.RecordInfo(StringRes.Info_DocumentPropertyCount.WithParams(allProperties.Count));

            var propertiesOfInterest = new List<PropertyStatementSyntax>();

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
                        // By default we don't output static/shared properties.
                        // However, if working with a module (for which all properties are automatically shared)
                        // we don't exclude shared properties
                        if (baseType is ITypeSymbol ts && ts.TypeKind == TypeKind.Module)
                        {
                            properties.AddRange(baseType.GetMembers().Where(m => m.Kind == SymbolKind.Property
                                                                              && m.DeclaredAccessibility == Accessibility.Public));
                        }
                        else
                        {
                            properties.AddRange(baseType.GetMembers().Where(m => m.Kind == SymbolKind.Property
                                                                              && m.DeclaredAccessibility == Accessibility.Public
                                                                              && !m.IsShared()));
                        }

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

                    var pbs = decRef.SyntaxTree.GetRoot().DescendantNodes(decRef.Span).OfType<PropertyBlockSyntax>().FirstOrDefault();

                    var syntax = pbs ?? (SyntaxNode)decRef.SyntaxTree.GetRoot().DescendantNodes(decRef.Span).OfType<PropertyStatementSyntax>().FirstOrDefault();

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

        private PropertyDetails GetPropertyDetails(SyntaxNode propertyNode, SemanticModel semModel)
        {
            var propertyName = this.GetIdentifier(propertyNode);
            var propertyType = Unknown;
            bool? propIsReadOnly = null;

            var descendantNodes = propertyNode.DescendantNodes().ToList();

            if (descendantNodes.Any(n => n is SimpleAsClauseSyntax))
            {
                propertyType = descendantNodes.OfType<SimpleAsClauseSyntax>().FirstOrDefault()?.Type.ToString();
            }
            else if (descendantNodes.Any(n => n is ObjectCreationExpressionSyntax))
            {
                propertyType = descendantNodes.OfType<ObjectCreationExpressionSyntax>().FirstOrDefault()?.Type.ToString();
            }
            else if (descendantNodes.Any(n => n is GenericNameSyntax))
            {
                propertyType = descendantNodes.OfType<GenericNameSyntax>().FirstOrDefault()?.ToString();
            }
            else if (descendantNodes.Any(n => n is PredefinedTypeSyntax))
            {
                propertyType = descendantNodes.OfType<PredefinedTypeSyntax>().FirstOrDefault()?.Keyword.ValueText;
            }

            if (descendantNodes.FirstOrDefault() is ParameterListSyntax)
            {
                propertyType += "()";
            }

            // Handle nullable types where the '?' is on the property name
            if (propertyType == Unknown && propertyNode is PropertyStatementSyntax propss)
            {
                if (propss.HasTrailingTrivia)
                {
                    var triviaList = propss.GetTrailingTrivia().ToList();
                    triviaList.Reverse();

                    foreach (var trivia in triviaList)
                    {
                        var triviaStr = trivia.ToString();

                        if (triviaStr.Contains("As "))
                        {
                            propertyType = triviaStr.Substring(triviaStr.IndexOf("As ") + 3);
                        }

                        if (triviaStr == "?")
                        {
                            propertyType += triviaStr;
                        }
                    }
                }
            }

            // Remove any namespace qualifications as we match class names as strings
            if (propertyType?.Contains(".") == true)
            {
                if (propertyType.Contains("Of "))
                {
                    var ofPos = propertyType.IndexOf("Of ");
                    var dotBeforeOfPos = propertyType.Substring(0, ofPos).LastIndexOf(".");

                    if (dotBeforeOfPos > -1)
                    {
                        propertyType = propertyType.Substring(dotBeforeOfPos + 1);
                    }

                    if (propertyType.Contains("."))
                    {
                        propertyType = propertyType.Substring(0, propertyType.IndexOf("Of ") + 3) +
                                       propertyType.Substring(propertyType.LastIndexOf(".") + 1);
                    }
                }
                else
                {
                    propertyType = propertyType.Substring(propertyType.LastIndexOf(".") + 1);
                }
            }

            SyntaxList<AttributeListSyntax> attributeLists;

            if (propertyNode is PropertyStatementSyntax pss)
            {
                if (pss.Parent is PropertyBlockSyntax)
                {
                    propertyNode = pss.Parent;
                }
                else
                {
                    propIsReadOnly = pss.Modifiers.Any(m => m.RawKind == (int)SyntaxKind.ReadOnlyKeyword);
                }

                attributeLists = pss.AttributeLists;
            }

            if (propertyNode is PropertyBlockSyntax pbs)
            {
                var setter = pbs.ChildNodes().FirstOrDefault(n => n.RawKind == (int)SyntaxKind.SetAccessorBlock);

                propIsReadOnly = setter != null
                    ? (setter as AccessorBlockSyntax)?.AccessorStatement.Modifiers.Any(m => m.RawKind == (int)SyntaxKind.PrivateKeyword)
                    : pbs.PropertyStatement.Modifiers.Any(m => m.RawKind == (int)SyntaxKind.ReadOnlyKeyword);
            }

            var pd = new PropertyDetails
            {
                Name = propertyName,
                PropertyType = propertyType,
                IsReadOnly = propIsReadOnly ?? false,
            };

            foreach (var attribList in attributeLists)
            {
                foreach (var attrib in attribList.Attributes)
                {
                    var att = new AttributeDetails
                    {
                        Name = attrib.Name.ToString(),
                    };

                    var count = 1;

                    if (attrib?.ArgumentList?.Arguments.Any() ?? false)
                    {
                        foreach (var arg in attrib.ArgumentList.Arguments)
                        {
                            string name = null;
                            string value = null;

                            if (arg is SimpleArgumentSyntax sas)
                            {
                                name = sas.NameColonEquals?.Name.ToString();
                                value = (sas.Expression as IdentifierNameSyntax)?.Identifier.ValueText;
                            }

                            if (value == null)
                            {
                                value = arg.ToString();
                            }

                            Logger?.RecordInfo(StringRes.Info_FoundAttributeArgument.WithParams(name, value));

                            att.Arguments.Add(new AttributeArgumentDetails
                            {
                                Index = count++,
                                Name = name,
                                Value = value,
                            });
                        }
                    }
                    else
                    {
                        Logger?.RecordInfo(StringRes.Info_NoArgumentsForAttribute);
                    }

                    pd.Attributes.Add(att);
                }
            }

            Logger?.RecordInfo(StringRes.Info_IdentifiedPropertySummary.WithParams(pd.Name, pd.PropertyType, pd.IsReadOnly));

            ITypeSymbol typeSymbol = this.GetTypeSymbol(semModel, propertyNode, pd);

            pd.Symbol = typeSymbol;

            return pd;
        }

        private (string output, string name, int counter) GetOutputToAdd(SemanticModel semModel, PropertyDetails prop, int numericCounter = 0)
        {
            var (output, counter) = this.GetPropertyOutputAndCounter(prop, numericCounter, semModel, () => this.GetSubPropertyOutput(prop, semModel));

            return (output, prop.Name, counter);
        }

        private string GetIdentifier(SyntaxNode syntaxNode)
        {
            if (syntaxNode is ModuleBlockSyntax)
            {
                return this.GetIdentifier(syntaxNode.ChildNodes().FirstOrDefault(n => n is ModuleStatementSyntax));
            }

            if (syntaxNode is ClassBlockSyntax)
            {
                return this.GetIdentifier(syntaxNode.ChildNodes().FirstOrDefault(n => n is ClassStatementSyntax));
            }

            if (syntaxNode is PropertyBlockSyntax pbs)
            {
                return this.GetIdentifier(pbs.PropertyStatement);
            }

            return syntaxNode?.ChildTokens().FirstOrDefault(t => t.RawKind == (int)SyntaxKind.IdentifierToken).ValueText;
        }

        private (SyntaxNode propertyNode, SyntaxNode classNode) GetNodeUnderCaret(SyntaxNode documentRoot, int caretPosition)
        {
            SyntaxNode propertyNode = null;
            SyntaxNode classNode = null;

            var currentNode = documentRoot.FindToken(caretPosition).Parent;

            while (currentNode != null && propertyNode == null && classNode == null)
            {
                if (currentNode is ClassBlockSyntax || currentNode is ModuleBlockSyntax || currentNode is TypeStatementSyntax)
                {
                    classNode = currentNode;
                }

                if (currentNode is PropertyStatementSyntax || currentNode is PropertyBlockSyntax)
                {
                    propertyNode = currentNode;
                }

                currentNode = currentNode.Parent;
            }

            return (propertyNode, classNode);
        }

        private ITypeSymbol GetTypeSymbol(SemanticModel semModel, SyntaxNode prop, PropertyDetails propDetails)
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
                    var localSemModel = VisualBasicCompilation.Create(string.Empty).AddSyntaxTrees(tree).GetSemanticModel(tree, ignoreAccessibility: true);

                    result = localSemModel.GetTypeInfo(ts).Type;
                }

                return result;
            }

            if (propDetails.PropertyType.IsGenericTypeName())
            {
                Logger?.RecordInfo(StringRes.Info_GettingGenericType);
                TypeSyntax typeSyntax = null;

                if (prop is PropertyStatementSyntax pss)
                {
                    if (pss.AsClause is SimpleAsClauseSyntax sacs)
                    {
                        if (sacs.Type is GenericNameSyntax gns)
                        {
                            typeSyntax = gns.TypeArgumentList.Arguments.First();
                        }
                        else if (sacs.Type is QualifiedNameSyntax qns)
                        {
                            typeSyntax = ((GenericNameSyntax)qns.Right).TypeArgumentList.Arguments.First();
                        }
                        else
                        {
                            Logger?.RecordInfo(StringRes.Info_PropertyTypeNotRecognizedAsGeneric.WithParams(propDetails.PropertyType));
                        }
                    }
                }
                else if (prop is PropertyBlockSyntax pbs)
                {
                    typeSyntax = ((GenericNameSyntax)((SimpleAsClauseSyntax)pbs.PropertyStatement.AsClause).Type).TypeArgumentList.Arguments.First();
                }

                if (typeSyntax == null)
                {
                    Logger?.RecordInfo(StringRes.Info_PropertyCannotBeAnalyzed.WithParams(prop.ToString()));
                }

                typeSymbol = GetWithFallback(typeSyntax, semModel, prop.SyntaxTree);
            }
            else
            {
                if (prop is PropertyStatementSyntax pss)
                {
                    if (pss.AsClause != null)
                    {
                        typeSymbol = GetWithFallback(((SimpleAsClauseSyntax)pss.AsClause).Type, semModel, prop.SyntaxTree);
                    }
                    else
                    {
                        try
                        {
                            if (pss.Identifier.TrailingTrivia.ToString().StartsWith("?"))
                            {
                                var propSyn = VisualBasicSyntaxTree.ParseText(prop.ToFullString().Replace("?", string.Empty).Trim() + "?");

                                var propType = ((SimpleAsClauseSyntax)((CompilationUnitSyntax)propSyn.GetRoot()).Members.OfType<PropertyStatementSyntax>().FirstOrDefault()?.AsClause)?.Type;
                                var propSemModel = VisualBasicCompilation.Create(string.Empty).AddSyntaxTrees(propSyn).GetSemanticModel(propSyn, true);

                                typeSymbol = propSemModel.GetTypeInfo(propType).Type;
                            }
                            else
                            {
                                Logger?.RecordInfo(StringRes.Info_PropertyCannotBeAnalyzed.WithParams(prop.ToString()));
                            }
                        }
                        catch (Exception)
                        {
                            Logger?.RecordInfo(StringRes.Info_FailedToGetNullableType.WithParams(propDetails.Name));
                        }
                    }
                }
                else if (prop is PropertyBlockSyntax pbs)
                {
                    typeSymbol = GetWithFallback(((SimpleAsClauseSyntax)pbs.PropertyStatement.AsClause).Type, semModel, prop.SyntaxTree);
                }
                else
                {
                    Logger?.RecordInfo(StringRes.Info_PropertyCannotBeAnalyzed.WithParams(prop.ToString()));
                }
            }

            return typeSymbol;
        }
    }
}
