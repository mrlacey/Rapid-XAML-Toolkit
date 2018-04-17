// <copyright file="CSharpAnalyzer.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RapidXamlToolkit
{
    public class CSharpAnalyzer : AnalyzerBase, IDocumentAnalyzer
    {
        public override string FileExtension { get; } = "cs";

        public static (List<string> strings, int count) GetSubPropertyOutput(PropertyDetails property, Profile profile, SemanticModel semModel)
        {
            var result = new List<string>();

            var subProperties = GetAllPublicProperties(property.Symbol, semModel);

            var numericSubstitute = 0;

            if (subProperties.Any())
            {
                foreach (var subprop in subProperties)
                {
                    var (output, counter) = GetSubPropertyOutputAndCounter(profile, subprop.Name, numericSubstitute: numericSubstitute);

                    numericSubstitute = counter;
                    result.Add(output);
                }
            }
            else
            {
                // There are no subproperties so leave blank
                var (output, counter) = GetSubPropertyOutputAndCounter(profile, string.Empty, numericSubstitute: numericSubstitute);

                numericSubstitute = counter;
                result.Add(output);
            }

            return (result, numericSubstitute);
        }

        public static PropertyDetails GetPropertyDetails(PropertyDeclarationSyntax propertyDeclaration, SemanticModel semModel)
        {
            var propertyType = "**unknown**";

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
                    break;
            }

            var propertyName = GetIdentifier(propertyDeclaration);

            bool? propIsReadOnly = null;
            var setter = propertyDeclaration.AccessorList.Accessors
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

            ITypeSymbol typeSymbol = GetTypeSymbol(semModel, propertyDeclaration, pd);

            pd.Symbol = typeSymbol;

            return pd;
        }

        public static string GetIdentifier(SyntaxNode syntaxNode)
        {
            return syntaxNode?.ChildTokens().FirstOrDefault(t => t.Kind() is SyntaxKind.IdentifierToken).ValueText;
        }

        public static (PropertyDeclarationSyntax propertyNode, ClassDeclarationSyntax classNode) GetNodeUnderCaret(SyntaxNode documentRoot, int caretPosition)
        {
            PropertyDeclarationSyntax propertyNode = null;
            ClassDeclarationSyntax classNode = null;
            var currentNode = documentRoot.FindToken(caretPosition).Parent;

            while (currentNode != null && propertyNode == null && classNode == null)
            {
                if (currentNode is ClassDeclarationSyntax)
                {
                    classNode = currentNode as ClassDeclarationSyntax;
                }

                if (currentNode is PropertyDeclarationSyntax)
                {
                    propertyNode = currentNode as PropertyDeclarationSyntax;
                }

                currentNode = currentNode.Parent;
            }

            return (propertyNode, classNode);
        }

        public AnalyzerOutput GetSingleItemOutput(SyntaxNode documentRoot, SemanticModel semModel, int caretPosition, Profile profileOverload = null)
        {
            var (propertyNode, classNode) = GetNodeUnderCaret(documentRoot, caretPosition);

            if (propertyNode != null)
            {
                var propDetails = GetPropertyDetails(propertyNode, semModel);

                var (output, name, _) = GetOutputToAdd(semModel, profileOverload, propDetails);

                return new AnalyzerOutput
                {
                    Name = name,
                    Output = output,
                    OutputType = AnalyzerOutputType.Property,
                };
            }
            else if (classNode != null)
            {
                var className = GetIdentifier(classNode);

                var classTypeSymbol = (ITypeSymbol)semModel.GetDeclaredSymbol(classNode);
                var properties = GetAllPublicProperties(classTypeSymbol, semModel);

                var output = new StringBuilder();

                var classGrouping = profileOverload == null ? GetClassGroupingForActiveProfile() : profileOverload.ClassGrouping;

                if (!string.IsNullOrWhiteSpace(classGrouping))
                {
                    output.AppendLine($"<{FormattedClassGroupingOpener(classGrouping)}>");
                }

                if (properties.Any())
                {
                    var propertyOutput = new List<string>();

                    var numericCounter = 0;

                    foreach (var prop in properties)
                    {
                        var toAdd = GetOutputToAdd(semModel, profileOverload, prop, numericCounter);

                        numericCounter = toAdd.counter;
                        propertyOutput.Add(toAdd.output);
                    }

                    if (classGrouping.Equals(GridWithRowDefsIndicator, StringComparison.InvariantCultureIgnoreCase)
                     || classGrouping.Equals(GridWithRowDefs2ColsIndicator, StringComparison.InvariantCultureIgnoreCase))
                    {
                        if (classGrouping.Equals(GridWithRowDefs2ColsIndicator, StringComparison.InvariantCultureIgnoreCase))
                        {
                            output.AppendLine("<Grid.ColumnDefinitions>");
                            output.AppendLine("<ColumnDefinition Width=\"Auto\" />");
                            output.AppendLine("<ColumnDefinition Width=\"*\" />");
                            output.AppendLine("</Grid.ColumnDefinitions>");
                        }

                        output.AppendLine("<Grid.RowDefinitions>");

                        for (int i = 1; i <= numericCounter; i++)
                        {
                            output.AppendLine(i < numericCounter
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
                    output.AppendLine(NoPropertiesXaml);
                }

                if (!string.IsNullOrWhiteSpace(classGrouping))
                {
                    output.Append($"</{FormattedClassGroupingCloser(classGrouping)}>");
                }

                return new AnalyzerOutput
                {
                    Name = className,
                    Output = output.ToString(),
                    OutputType = AnalyzerOutputType.Class,
                };
            }
            else
            {
                return AnalyzerOutput.Empty;
            }
        }

        public AnalyzerOutput GetSelectionOutput(SyntaxNode documentRoot, SemanticModel semModel, int selStart, int selEnd, Profile profileOverload = null)
        {
            var allProperties = documentRoot.DescendantNodes().OfType<PropertyDeclarationSyntax>().ToList();

            var propertiesOfInterest = new List<PropertyDeclarationSyntax>();

            foreach (var prop in allProperties)
            {
                if (prop.FullSpan.End >= selStart && prop.FullSpan.Start < selEnd)
                {
                    propertiesOfInterest.Add(prop);
                }
            }

            var output = new StringBuilder();

            var numericCounter = 1;

            var propertyNames = new List<string>();

            foreach (var prop in propertiesOfInterest)
            {
                var propDetails = GetPropertyDetails(prop, semModel);

                var toAdd = profileOverload == null
                        ? GetPropertyOutputAndCounterForActiveProfile(propDetails, numericCounter, () => GetSubPropertyOutput(propDetails, GetSettings().GetActiveProfile(), semModel))
                        : GetPropertyOutputAndCounter(profileOverload, propDetails, numericCounter, () => GetSubPropertyOutput(propDetails, profileOverload, semModel));

                numericCounter = toAdd.counter;
                output.AppendLine(toAdd.output);

                propertyNames.Add(propDetails.Name);
            }

            if (propertyNames.Any())
            {
                var outputName = GetSelectionPropertiesName(propertyNames);

                // Trim end of output to remove trailing newline
                return new AnalyzerOutput
                {
                    Name = outputName,
                    Output = output.ToString().TrimEnd(),
                    OutputType = AnalyzerOutputType.Selection,
                };
            }
            else
            {
                return AnalyzerOutput.Empty;
            }
        }

        private static (string output, string name, int counter) GetOutputToAdd(SemanticModel semModel, Profile profileOverload, PropertyDetails prop, int numericCounter = 0)
        {
            var (output, counter) = profileOverload == null
                ? GetPropertyOutputAndCounterForActiveProfile(prop, numericCounter, () => GetSubPropertyOutput(prop, GetSettings().GetActiveProfile(), semModel))
                : GetPropertyOutputAndCounter(profileOverload, prop, numericCounter, () => GetSubPropertyOutput(prop, profileOverload, semModel));

            return (output, prop.Name, counter);
        }

        private static ITypeSymbol GetTypeSymbol(SemanticModel semModel, PropertyDeclarationSyntax prop, PropertyDetails propDetails)
        {
            ITypeSymbol typeSymbol;
            if (propDetails.PropertyType.IsGenericTypeName())
            {
                var t = ((GenericNameSyntax)prop.Type).TypeArgumentList.Arguments.First();

                typeSymbol = semModel.GetTypeInfo(t).Type;
            }
            else
            {
                try
                {
                    typeSymbol = semModel.GetTypeInfo(prop.Type).Type;
                }
                catch (Exception)
                {
                    // The semanticmodel passed into this method is the one for the active document.
                    // If the type is in another file, generate a new model to use to look up the typeinfo. Don't do this by default as it's expensive.
                    var localSemModel = CSharpCompilation.Create(string.Empty).AddSyntaxTrees(prop.SyntaxTree).GetSemanticModel(prop.SyntaxTree, ignoreAccessibility: true);

                    typeSymbol = localSemModel.GetTypeInfo(prop.Type).Type;
                }
            }

            return typeSymbol;
        }

        private static List<PropertyDetails> GetAllPublicProperties(ITypeSymbol typeSymbol, SemanticModel semModel)
        {
            var properties = new List<ISymbol>();

            foreach (var baseType in typeSymbol.GetSelfAndBaseTypes())
            {
                var skipTypes = new[] { "String", "ValueType", "Object" };

                if (baseType.Name.IsOneOf(TypesToSkipWhenCheckingForSubProperties))
                {
                    continue;
                }

                switch (baseType.Kind)
                {
                    case SymbolKind.NamedType:
                        properties.AddRange(baseType.GetMembers().Where(m => m.Kind == SymbolKind.Property && m.DeclaredAccessibility == Accessibility.Public));
                        break;
                    case SymbolKind.ErrorType:

                        //// TODO: Log that type is unknown and so may not be able to provide all output expected

                        break;
                }
            }

            var result = new List<PropertyDetails>();

            foreach (var prop in properties)
            {
                var decRefs = prop.OriginalDefinition.DeclaringSyntaxReferences;

                if (decRefs.Any())
                {
                    var decRef = decRefs.First();

                    var syntax = decRef.SyntaxTree.GetRoot().DescendantNodes(decRef.Span).OfType<PropertyDeclarationSyntax>().First();

                    var details = GetPropertyDetails(syntax, semModel);

                    result.Add(details);
                }
                else
                {
                    result.Add(new PropertyDetails { Name = prop.Name, PropertyType = UnknownOrInvalidTypeName, IsReadOnly = false, Symbol = null });
                }
            }

            return result;
        }
    }
}
