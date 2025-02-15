// Copyright (c) Matt Lacey Ltd. All rights reserved.
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

            var allMethods = documentRoot.DescendantNodes().OfType<MethodStatementSyntax>().ToList();

            Logger?.RecordInfo(StringRes.Info_DocumentMethodCount.WithParams(allMethods.Count));

            var methodsOfInterest = new List<MethodStatementSyntax>();

            foreach (var method in allMethods)
            {
                if (method.FullSpan.End >= selStart && method.FullSpan.Start < selEnd)
                {
                    methodsOfInterest.Add(method);
                }
            }

            Logger?.RecordInfo(StringRes.Info_MethodsInSelectedAreaCount.WithParams(methodsOfInterest.Count));

            var output = new StringBuilder();

            var numericCounter = 1;

            var memberNames = new List<string>();

            foreach (var prop in propertiesOfInterest)
            {
                var propDetails = this.GetPropertyDetails(prop, semModel);

                if (propDetails is null)
                {
                    Logger?.RecordInfo(StringRes.Info_UnexpectedPropertyType.WithParams(prop.GetType()));
                    continue;
                }

                if (propDetails.Name.IsOneOf(NamesOfPropertiesToExcludeFromOutput))
                {
                    Logger?.RecordInfo(StringRes.Info_NotIncludingExcludedProperty.WithParams(propDetails.Name));
                    continue;
                }

                Logger?.RecordInfo(StringRes.Info_AddingPropertyToOutput.WithParams(propDetails.Name));
                var toAdd = this.GetPropertyOutputAndCounter(propDetails, numericCounter, semModel, () => this.GetSubPropertyOutput(propDetails, semModel));

                if (!string.IsNullOrWhiteSpace(toAdd.Output))
                {
                    numericCounter = toAdd.Counter;
                    output.AppendLine(toAdd.Output);

                    memberNames.Add(propDetails.Name);
                }
            }

            foreach (var method in methodsOfInterest)
            {
                var memberDets = this.GetMethodDetails(method, semModel);

                Logger?.RecordInfo(StringRes.Info_AddingMemberToOutput.WithParams(memberDets.Name));
                var toAdd = this.GetMethodOutputAndCounter(memberDets, numericCounter, semModel);

                if (!string.IsNullOrWhiteSpace(toAdd.Output))
                {
                    numericCounter = toAdd.Counter;
                    output.AppendLine(toAdd.Output);

                    memberNames.Add(memberDets.Name);
                }
            }

            if (memberNames.Any())
            {
                var outputName = GetSelectionMemberName(memberNames);

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
                Logger?.RecordInfo(StringRes.Info_NoMembersToOutput);
                return ParserOutput.Empty;
            }
        }

        public override List<PropertyDetails> GetAllPublicProperties(ITypeSymbol typeSymbol, SemanticModel semModel)
        {
            var properties = new List<ISymbol>();

            foreach (var baseType in typeSymbol.GetSelfAndBaseTypes())
            {
                if (baseType.Name.IsOneOf(TypesToSkipWhenCheckingForSubMembers))
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

                    if (details != null)
                    {
                        Logger?.RecordInfo(StringRes.Info_FoundSubProperty.WithParams(details.Name));
                        result.Add(details);
                    }
                }
                else
                {
                    Logger?.RecordInfo(StringRes.Info_FoundSubPropertyOfUnknownType.WithParams(prop.Name));
                    result.Add(new PropertyDetails { Name = prop.Name, PropertyType = UnknownOrInvalidTypeName, IsReadOnly = false, Symbol = null });
                }
            }

            return result;
        }

        public override List<MethodDetails> GetAllPublicVoidMethods(ITypeSymbol typeSymbol, SemanticModel semModel)
        {
            var methods = new List<ISymbol>();

            foreach (var baseType in typeSymbol.GetSelfAndBaseTypes())
            {
                if (baseType.Name.IsOneOf(TypesToSkipWhenCheckingForSubMembers))
                {
                    continue;
                }

                switch (baseType.Kind)
                {
                    case SymbolKind.NamedType:
                        methods.AddRange(
                            baseType.GetMembers()
                                    .Where(
                                        m => m.Kind == SymbolKind.Method
                                          && m.DeclaredAccessibility == Accessibility.Public
                                          && ((IMethodSymbol)m).ReturnsVoid
                                          && !((IMethodSymbol)m).IsGenericMethod
                                          && !m.MetadataName.StartsWith("get_")
                                          && !m.MetadataName.StartsWith("set_")
                                          && !m.IsStatic));
                        break;
                    case SymbolKind.ErrorType:
                        Logger?.RecordInfo(StringRes.Info_CannotGetMethodsForKnownType.WithParams(baseType.Name));
                        break;
                }
            }

            var result = new List<MethodDetails>();

            foreach (var method in methods)
            {
                if (((IMethodSymbol)method).MethodKind == MethodKind.Constructor)
                {
                    continue;
                }

                var decRefs = method.OriginalDefinition.DeclaringSyntaxReferences;

                if (decRefs.Any())
                {
                    var decRef = decRefs.First();

                    SyntaxNode syntax = decRef.SyntaxTree.GetRoot().DescendantNodes(decRef.Span).OfType<MethodStatementSyntax>().FirstOrDefault();

                    if (syntax != null)
                    {
                        var details = this.GetMethodDetails(syntax, semModel);

                        Logger?.RecordInfo(StringRes.Info_FoundSubProperty.WithParams(details.Name));
                        result.Add(details);
                    }
                }
                else
                {
                    if (method.Name != ".ctor")
                    {
                        Logger?.RecordInfo(StringRes.Info_FoundSubMethodOfUnknownType.WithParams(method.Name));
                    }
                }
            }

            return result;
        }

        protected override (List<string> Strings, int Count) GetSubPropertyOutput(PropertyDetails property, SemanticModel semModel)
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

        protected override PropertyDetails GetPropertyDetails(SyntaxNode propertyNode, SemanticModel semModel)
        {
            var propertyName = this.GetIdentifier(propertyNode);
            var propertyType = Unknown;
            bool? propIsReadOnly = null;

            var descendantNodes = propertyNode.DescendantNodes().ToList();

            var paramListIndex = descendantNodes.IndexOf(descendantNodes.FirstOrDefault(n => n is ParameterListSyntax));

            if (paramListIndex > 0)
            {
                if (paramListIndex < descendantNodes.IndexOf(descendantNodes.FirstOrDefault(n => n is AccessorBlockSyntax)))
                {
                    return null;
                }
            }

            if (descendantNodes.Any(n => n is TupleTypeSyntax))
            {
                propertyType = "Tuple";
            }
            else if (descendantNodes.Any(n => n is SimpleAsClauseSyntax))
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

            pd.Attributes.AddRange(this.GetAttributeDetails(attributeLists));

            Logger?.RecordInfo(StringRes.Info_IdentifiedPropertySummary.WithParams(pd.Name, pd.PropertyType, pd.IsReadOnly));

            ITypeSymbol typeSymbol = this.GetTypeSymbol(semModel, propertyNode, pd);

            pd.Symbol = typeSymbol;

            return pd;
        }

        protected override MethodDetails GetMethodDetails(SyntaxNode methodDeclaration, SemanticModel semModel)
        {
            var md = new MethodDetails();

            if (methodDeclaration is MethodStatementSyntax methodDec)
            {
                md.Name = methodDec.Identifier.ValueText;

                if ((methodDec.ParameterList?.Parameters.Count ?? 0) > 0)
                {
                    var param1 = methodDec.ParameterList.Parameters.First();

                    md.Argument1Name = param1.Identifier.Identifier.Text;

                    md.Argument1Type = this.GetTypeSymbolWithFallback(param1.AsClause.Type, semModel, methodDec.SyntaxTree);

                    if (methodDec.ParameterList.Parameters.Count > 1)
                    {
                        var param2 = methodDec.ParameterList.Parameters[1];

                        md.Argument2Name = param2.Identifier.Identifier.Text;
                        md.Argument2Type = this.GetTypeSymbolWithFallback(param2.AsClause.Type, semModel, methodDec.SyntaxTree);
                    }
                }

                md.Attributes.AddRange(this.GetAttributeDetails(methodDec.AttributeLists));
            }
            else
            {
                Logger?.RecordInfo(StringRes.Info_UnexpectedMethodType.WithParams(methodDeclaration.GetType()));
            }

            return md;
        }

        protected IEnumerable<AttributeDetails> GetAttributeDetails(SyntaxList<AttributeListSyntax> attributeLists)
        {
            var result = new List<AttributeDetails>();

            foreach (var attribList in attributeLists)
            {
                foreach (var attrib in attribList?.Attributes)
                {
                    var att = new AttributeDetails();

                    if (attrib.Name is IdentifierNameSyntax ins)
                    {
                        att.Name = ins.Identifier.Text;
                    }
                    else
                    {
                        att.Name = attrib.Name.ToString();
                    }

                    var count = 1;

                    if (attrib?.ArgumentList?.Arguments.Any() ?? false)
                    {
                        foreach (var arg in attrib?.ArgumentList?.Arguments)
                        {
                            string name = null;
                            string value = null;

                            if (arg is SimpleArgumentSyntax sas)
                            {
                                name = sas.NameColonEquals?.Name.ToString();

                                if (sas.Expression is IdentifierNameSyntax expins)
                                {
                                    value = expins.Identifier.ValueText;
                                }
                                else if (sas.Expression is LiteralExpressionSyntax les)
                                {
                                    value = les.Token.ValueText;
                                }
                            }

                            value ??= arg.ToString();

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

        protected override (SyntaxNode PropertyNode, SyntaxNode ClassNode, SyntaxNode MethodNode) GetNodeUnderCaret(SyntaxNode documentRoot, int caretPosition)
        {
            SyntaxNode propertyNode = null;
            SyntaxNode classNode = null;
            SyntaxNode methodNode = null;

            var currentNode = documentRoot.FindToken(caretPosition).Parent;

            while (currentNode != null && propertyNode == null && classNode == null && methodNode == null)
            {
                if (currentNode is ClassBlockSyntax || currentNode is ModuleBlockSyntax || currentNode is TypeStatementSyntax)
                {
                    classNode = currentNode;
                }
                else if (currentNode is PropertyStatementSyntax || currentNode is PropertyBlockSyntax)
                {
                    propertyNode = currentNode;
                }
                else if (currentNode is MethodStatementSyntax)
                {
                    methodNode = currentNode;
                }

                currentNode = currentNode.Parent;
            }

            return (propertyNode, classNode, methodNode);
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
                // If the type is in another file, generate a new model to use to look up the typeinfo. Don't do this by default as it's expensive.
                var localSemModel = VisualBasicCompilation.Create(string.Empty).AddSyntaxTrees(tree).GetSemanticModel(tree, ignoreAccessibility: true);

                result = localSemModel.GetTypeInfo(ts).Type;
            }

            return result;
        }

        private ITypeSymbol GetTypeSymbol(SemanticModel semModel, SyntaxNode prop, PropertyDetails propDetails)
        {
            ITypeSymbol typeSymbol = null;

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
                    var innerType = ((SimpleAsClauseSyntax)pbs.PropertyStatement.AsClause).Type;

                    if (innerType is GenericNameSyntax igns)
                    {
                        typeSyntax = igns.TypeArgumentList.Arguments.First();
                    }
                    else if (innerType is QualifiedNameSyntax iqns)
                    {
                        typeSyntax = (iqns.Right as GenericNameSyntax).TypeArgumentList.Arguments.First();
                    }
                }

                if (typeSyntax == null)
                {
                    Logger?.RecordInfo(StringRes.Info_PropertyCannotBeAnalyzed.WithParams(prop.ToString()));
                }

                typeSymbol = this.GetTypeSymbolWithFallback(typeSyntax, semModel, prop.SyntaxTree);
            }
            else
            {
                if (prop is PropertyStatementSyntax pss)
                {
                    if (pss.AsClause != null)
                    {
                        TypeSyntax clauseType = null;

                        if (pss.AsClause is SimpleAsClauseSyntax sacs)
                        {
                            clauseType = sacs.Type;
                        }
                        else if (pss.AsClause is AsNewClauseSyntax ancs)
                        {
                            clauseType = ancs.Type();
                        }

                        typeSymbol = this.GetTypeSymbolWithFallback(clauseType, semModel, prop.SyntaxTree);
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
                    typeSymbol = this.GetTypeSymbolWithFallback(((SimpleAsClauseSyntax)pbs.PropertyStatement.AsClause).Type, semModel, prop.SyntaxTree);
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
