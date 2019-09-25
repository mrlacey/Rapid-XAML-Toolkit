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

                    SyntaxNode syntax = decRef.SyntaxTree.GetRoot().DescendantNodes(decRef.Span).OfType<PropertyDeclarationSyntax>().FirstOrDefault();

                    if (syntax is null)
                    {
                        syntax = decRef.SyntaxTree.GetRoot().DescendantNodes(decRef.Span).OfType<IndexerDeclarationSyntax>().FirstOrDefault();

                        if (syntax != null)
                        {
                            // Don't output anything for indexers.
                            continue;
                        }
                    }

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

        protected override (List<string> strings, int count) GetSubPropertyOutput(PropertyDetails property, SemanticModel semModel)
        {
            var result = new List<string>();

            var symbol = property.Symbol;

            if (symbol is INamedTypeSymbol nts)
            {
                if (nts.Arity > 0)
                {
                    System.Diagnostics.Debug.WriteLine(symbol);
                    symbol = nts.TypeArguments.First();
                }
            }

            var subProperties = this.GetAllPublicProperties(symbol, semModel);

            var numericSubstitute = 0;

            if (subProperties.Any())
            {
                Logger?.RecordInfo(StringRes.Info_SubpropertyCount.WithParams(property.Name, subProperties.Count));

                foreach (var subprop in subProperties)
                {
                    Logger?.RecordInfo(StringRes.Info_GettingSubPropertyOutput.WithParams(subprop.Name));

                    var (output, counter) = this.GetSubPropertyOutputAndCounter(subprop, numericSubstitute: numericSubstitute);

                    numericSubstitute = counter;
                    result.Add(output);
                }
            }
            else
            {
                Logger?.RecordInfo(StringRes.Info_PropertyTypeHasNoSubProperties.WithParams(property.Name, property.PropertyType));

                // There are no subproperties so leave blank
                var (output, counter) = this.GetSubPropertyOutputAndCounter(PropertyDetails.Empty, numericSubstitute: numericSubstitute);

                numericSubstitute = counter;
                result.Add(output);
            }

            return (result, numericSubstitute);
        }

        protected override PropertyDetails GetPropertyDetails(SyntaxNode propertyDeclaration, SemanticModel semModel)
        {
            var propertyType = Unknown;
            string propertyName = null;
            AccessorDeclarationSyntax setter = null;
            SyntaxList<AttributeListSyntax> attributeList;

            if (propertyDeclaration is PropertyDeclarationSyntax propDecSyntax)
            {
                switch (propDecSyntax.Type)
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
                        propertyType = qns.ToString();
                        break;
                    case NullableTypeSyntax nts:

                        switch (nts.ElementType)
                        {
                            case PredefinedTypeSyntax npts:
                                propertyType = npts.Keyword.Text;
                                break;
                            case IdentifierNameSyntax nins:
                                propertyType = nins.Identifier.ValueText;
                                break;
                            case QualifiedNameSyntax nqns:
                                propertyType = nqns.ToString();

                                if (nqns.Right is GenericNameSyntax qngns)
                                {
                                    propertyType += qngns.TypeArgumentList.ToString();
                                }

                                break;
                        }

                        if (!propertyType.ToLowerInvariant().Contains("nullable"))
                        {
                            propertyType += nts.QuestionToken.Text;
                        }

                        break;
                    case ArrayTypeSyntax ats:
                        propertyType = ats.ToString();
                        break;
                }

                propertyName = this.GetIdentifier(propertyDeclaration);

                setter = propDecSyntax?.AccessorList?.Accessors.FirstOrDefault(a => a.RawKind == (ushort)SyntaxKind.SetAccessorDeclaration);

                attributeList = propDecSyntax.AttributeLists;
            }
            else
            {
                Logger?.RecordInfo(StringRes.Info_UnexpectedPropertyType.WithParams(propertyDeclaration.GetType()));
            }

            bool? propIsReadOnly;

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

            pd.Attributes.AddRange(this.GetAttributes(attributeList));

            Logger?.RecordInfo(StringRes.Info_IdentifiedPropertySummary.WithParams(pd.Name, pd.PropertyType, pd.IsReadOnly));

            ITypeSymbol typeSymbol = this.GetTypeSymbol(semModel, propertyDeclaration as BasePropertyDeclarationSyntax, pd);

            pd.Symbol = typeSymbol;

            return pd;
        }

        protected IEnumerable<AttributeDetails> GetAttributes(SyntaxList<AttributeListSyntax> attributeList)
        {
            var result = new List<AttributeDetails>();

            foreach (var attribList in attributeList)
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
                        foreach (var arg in attrib?.ArgumentList.Arguments)
                        {
                            string name;
                            if (arg?.NameColon != null)
                            {
                                name = arg.NameColon.Name.Identifier.Text;
                            }
                            else if (arg?.NameEquals != null)
                            {
                                name = arg.NameEquals.Name.ToString();
                            }
                            else
                            {
                                name = string.Empty;
                            }

                            var expression = arg.Expression;

                            string value;
                            if (expression is IdentifierNameSyntax ins)
                            {
                                value = ins.Identifier.Value.ToString();
                            }
                            else if (expression is LiteralExpressionSyntax les)
                            {
                                value = les.ToString().Replace("\"", string.Empty);
                            }
                            else
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

                    result.Add(att);
                }
            }

            return result;
        }

        protected override string GetIdentifier(SyntaxNode syntaxNode)
        {
            return syntaxNode?.ChildTokens().FirstOrDefault(t => t.Kind() is SyntaxKind.IdentifierToken).ValueText;
        }

        protected override (SyntaxNode propertyNode, SyntaxNode classNode) GetNodeUnderCaret(SyntaxNode documentRoot, int caretPosition)
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

        private ITypeSymbol GetTypeSymbolWithFallback(TypeSyntax ts, SemanticModel sm, SyntaxTree tree)
        {
            ITypeSymbol result;

            try
            {
                result = sm.GetTypeInfo(ts).Type;
            }
            catch (Exception)
            {
                // By default, the semanticmodel passed into this method is the one for the active document.
                // If the type is in another file, generate a new model to use to look up the typeinfo.
                // Don't do this by default as it's expensive.
                var localSemModel = CSharpCompilation.Create(string.Empty).AddSyntaxTrees(tree).GetSemanticModel(tree, ignoreAccessibility: true);

                result = localSemModel.GetTypeInfo(ts).Type;
            }

            return result;
        }

        private ITypeSymbol GetTypeSymbol(SemanticModel semModel, BasePropertyDeclarationSyntax prop, PropertyDetails propDetails)
        {
            ITypeSymbol typeSymbol = null;

            if (propDetails.PropertyType.IsGenericTypeName())
            {
                Logger?.RecordInfo(StringRes.Info_GettingGenericType);

                if (prop.Type is GenericNameSyntax gns)
                {
                    var t = gns.TypeArgumentList.Arguments.First();

                    if (t is QualifiedNameSyntax tqns)
                    {
                        var right = tqns.Right;

                        if (right is IdentifierNameSyntax rins)
                        {
                            System.Diagnostics.Debug.WriteLine(rins);
                            t = right;
                        }
                        else if (right is GenericNameSyntax rgns)
                        {
                            t = rgns.TypeArgumentList.Arguments.First();
                        }
                    }

                    typeSymbol = this.GetTypeSymbolWithFallback(gns, semModel, prop.SyntaxTree);
                }
                else if (prop.Type is QualifiedNameSyntax qns)
                {
                    var t = ((GenericNameSyntax)qns.Right).TypeArgumentList.Arguments.First();

                    typeSymbol = this.GetTypeSymbolWithFallback(t, semModel, prop.SyntaxTree);
                }
                else
                {
                    Logger?.RecordInfo(StringRes.Info_PropertyTypeNotRecognizedAsGeneric.WithParams(propDetails.PropertyType));
                }
            }

            if (typeSymbol == null)
            {
                typeSymbol = this.GetTypeSymbolWithFallback(prop.Type, semModel, prop.SyntaxTree);
            }

            if (typeSymbol == null)
            {
                Logger?.RecordInfo(StringRes.Info_PropertyCannotBeAnalyzed.WithParams(prop.ToString()));
            }

            return typeSymbol;
        }
    }
}
