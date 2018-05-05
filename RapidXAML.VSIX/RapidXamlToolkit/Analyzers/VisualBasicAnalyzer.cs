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

namespace RapidXamlToolkit.Analyzers
{
    public class VisualBasicAnalyzer : AnalyzerBase, IDocumentAnalyzer
    {
        public VisualBasicAnalyzer(ILogger logger)
            : base(logger)
        {
            Logger?.RecordInfo($"Analyzing VB code.  (v{Telemetry.CoreDetails.GetVersion()})");
        }

        public override string FileExtension { get; } = "vb";

        public static (List<string> strings, int count) GetSubPropertyOutput(PropertyDetails property, Profile profile, SemanticModel semModel)
        {
            var result = new List<string>();

            var subProperties = GetAllPublicProperties(property.Symbol, semModel);

            var numericSubstitute = 0;

            if (subProperties.Any())
            {
                Logger?.RecordInfo($"Property {property.Name} has {subProperties.Count} sub-properties.");

                foreach (var subprop in subProperties)
                {
                    Logger?.RecordInfo($"Getting sub-property output for {subprop.Name}.");

                    var (output, counter) = GetSubPropertyOutputAndCounter(profile, subprop.Name, numericSubstitute: numericSubstitute);

                    numericSubstitute = counter;
                    result.Add(output);
                }
            }
            else
            {
                Logger?.RecordInfo($"{property.Name} is of type '{property.PropertyType}' which has has no sub-properties.");

                // There are no subproperties so leave blank
                var (output, counter) = GetSubPropertyOutputAndCounter(profile, string.Empty, numericSubstitute: numericSubstitute);

                numericSubstitute = counter;
                result.Add(output);
            }

            return (result, numericSubstitute);
        }

        public static PropertyDetails GetPropertyDetails(SyntaxNode propertyNode, SemanticModel semModel)
        {
            var propertyName = GetIdentifier(propertyNode);
            var propertyType = "**unknown**";
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

            // Remove any namespace qualifications as we match class names as strings
            if (propertyType?.Contains(".") == true)
            {
                if (propertyType.Contains("Of "))
                {
                    propertyType = propertyType.Substring(0, propertyType.IndexOf("Of ") + 3) + propertyType.Substring(propertyType.LastIndexOf(".") + 1);
                }
                else
                {
                    propertyType = propertyType.Substring(propertyType.LastIndexOf(".") + 1);
                }
            }

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

            Logger?.RecordInfo($"Identified property as Name='{pd.Name}' Type='{pd.PropertyType}' IsReadOnly='{pd.IsReadOnly}' ");

            ITypeSymbol typeSymbol = GetTypeSymbol(semModel, propertyNode, pd);

            pd.Symbol = typeSymbol;

            return pd;
        }

        public static string GetIdentifier(SyntaxNode syntaxNode)
        {
            if (syntaxNode is ModuleBlockSyntax)
            {
                return GetIdentifier(syntaxNode.ChildNodes().FirstOrDefault(n => n is ModuleStatementSyntax));
            }

            if (syntaxNode is ClassBlockSyntax)
            {
                return GetIdentifier(syntaxNode.ChildNodes().FirstOrDefault(n => n is ClassStatementSyntax));
            }

            if (syntaxNode is PropertyBlockSyntax pbs)
            {
                return GetIdentifier(pbs.PropertyStatement);
            }

            return syntaxNode?.ChildTokens().FirstOrDefault(t => t.RawKind == (int)SyntaxKind.IdentifierToken).ValueText;
        }

        public static (string output, string name, int counter) GetOutputToAdd(SemanticModel semModel, Profile profileOverload, PropertyDetails prop, int numericCounter = 0)
        {
            var (output, counter) = profileOverload == null
                ? GetPropertyOutputAndCounterForActiveProfile(prop, numericCounter, () => GetSubPropertyOutput(prop, GetSettings().GetActiveProfile(), semModel))
                : GetPropertyOutputAndCounter(profileOverload, prop, numericCounter, () => GetSubPropertyOutput(prop, profileOverload, semModel));

            return (output, prop.Name, counter);
        }

        public static (SyntaxNode propertyNode, SyntaxNode classNode) GetNodeUnderCaret(SyntaxNode documentRoot, int caretPosition)
        {
            SyntaxNode propertyNode = null;
            SyntaxNode classNode = null;

            var currentNode = documentRoot.FindToken(caretPosition).Parent;

            while (currentNode != null && propertyNode == null && classNode == null)
            {
                if (currentNode is ClassBlockSyntax || currentNode is ModuleBlockSyntax)
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

        public AnalyzerOutput GetSingleItemOutput(SyntaxNode documentRoot, SemanticModel semModel, int caretPosition, Profile profileOverload = null)
        {
            Logger?.RecordInfo("Getting oputput for a single item.");
            var (propertyNode, classNode) = GetNodeUnderCaret(documentRoot, caretPosition);

            if (propertyNode != null)
            {
                Logger?.RecordInfo("Getting oputput for a single property.");

                var propDetails = GetPropertyDetails(propertyNode, semModel);

                var (output, name, _) = GetOutputToAdd(semModel, profileOverload, propDetails);

                return new AnalyzerOutput
                {
                    Name = name,
                    Output = output,
                    OutputType = AnalyzerOutputType.Property,
                };
            }

            if (classNode != null)
            {
                Logger?.RecordInfo("Getting output for the class");

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
                    Logger?.RecordInfo($"Class contains {properties.Count} properties.");

                    var propertyOutput = new List<string>();

                    var numericCounter = 0;

                    foreach (var prop in properties)
                    {
                        Logger?.RecordInfo($"Adding {prop.Name} to the output.");
                        var toAdd = GetOutputToAdd(semModel, profileOverload, prop, numericCounter);

                        numericCounter = toAdd.counter;
                        propertyOutput.Add(toAdd.output);
                    }

                    if (classGrouping != null
                     && (classGrouping.Equals(GridWithRowDefsIndicator, StringComparison.InvariantCultureIgnoreCase)
                      || classGrouping.Equals(GridWithRowDefs2ColsIndicator, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        Logger?.RecordInfo("Adding Grid to output.");

                        if (classGrouping.Equals(GridWithRowDefs2ColsIndicator, StringComparison.InvariantCultureIgnoreCase))
                        {
                            Logger?.RecordInfo("Adding ColumnDefinitions to Grid.");

                            output.AppendLine("<Grid.ColumnDefinitions>");
                            output.AppendLine("<ColumnDefinition Width=\"Auto\" />");
                            output.AppendLine("<ColumnDefinition Width=\"*\" />");
                            output.AppendLine("</Grid.ColumnDefinitions>");
                        }

                        output.AppendLine("<Grid.RowDefinitions>");

                        Logger?.RecordInfo($"Adding {numericCounter} RowDefinitions to Grid.");
                        for (var i = 1; i <= numericCounter; i++)
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
                    Logger?.RecordInfo("Class contains no public properties.");
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

            Logger?.RecordInfo("No properties to provide output for.");
            return AnalyzerOutput.Empty;
        }

        public AnalyzerOutput GetSelectionOutput(SyntaxNode documentRoot, SemanticModel semModel, int selStart, int selEnd, Profile profileOverload = null)
        {
            Logger?.RecordInfo("Getting output for the selection.");

            var allProperties = documentRoot.DescendantNodes().OfType<PropertyStatementSyntax>().ToList();

            Logger?.RecordInfo($"Document contains {allProperties.Count} properties.");

            var propertiesOfInterest = new List<PropertyStatementSyntax>();

            foreach (var prop in allProperties)
            {
                if (prop.FullSpan.End >= selStart && prop.FullSpan.Start < selEnd)
                {
                    propertiesOfInterest.Add(prop);
                }
            }

            Logger?.RecordInfo($"{propertiesOfInterest.Count} properties within the selection area.");

            var output = new StringBuilder();

            var numericCounter = 1;

            var propertyNames = new List<string>();

            foreach (var prop in propertiesOfInterest)
            {
                var propDetails = GetPropertyDetails(prop, semModel);

                if (propDetails.Name.IsOneOf(NamesOfPropertiesToExcludeFromOutput))
                {
                    Logger?.RecordInfo($"Not including property '{propDetails.Name}' as it's on the exclusion list.");
                    continue;
                }

                Logger?.RecordInfo($"Adding property '{propDetails.Name}' to the output.");
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

                Logger?.RecordInfo($"Returning {outputName}");

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
                Logger?.RecordInfo("No properties to provide output for.");
                return AnalyzerOutput.Empty;
            }
        }

        private static ITypeSymbol GetTypeSymbol(SemanticModel semModel, SyntaxNode prop, PropertyDetails propDetails)
        {
            ITypeSymbol typeSymbol = null;

            if (propDetails.PropertyType.IsGenericTypeName())
            {
                Logger?.RecordInfo("Getting a generic type.");
                TypeSyntax typeSyntax = null;

                if (prop is PropertyStatementSyntax pss)
                {
                    typeSyntax = ((GenericNameSyntax)((SimpleAsClauseSyntax)pss.AsClause).Type).TypeArgumentList.Arguments.First();
                }

                if (prop is PropertyBlockSyntax pbs)
                {
                    typeSyntax = ((GenericNameSyntax)((SimpleAsClauseSyntax)pbs.PropertyStatement.AsClause).Type).TypeArgumentList.Arguments.First();
                }

                try
                {
                    typeSymbol = semModel.GetTypeInfo(typeSyntax).Type;
                }
                catch (Exception)
                {
                    // The semanticmodel passed into this method is the one for the active document.
                    // If the type is in another file, generate a new model to use to look up the typeinfo. Don't do this by default as it's expensive.
                    var localSemModel = VisualBasicCompilation.Create(string.Empty).AddSyntaxTrees(prop.SyntaxTree).GetSemanticModel(prop.SyntaxTree, ignoreAccessibility: true);

                    typeSymbol = localSemModel.GetTypeInfo(typeSyntax).Type;
                }
            }
            else
            {
                if (prop is PropertyStatementSyntax pss)
                {
                    try
                    {
                        typeSymbol = semModel.GetTypeInfo(((SimpleAsClauseSyntax)pss.AsClause).Type).Type;
                    }
                    catch (Exception)
                    {
                        // The semanticmodel passed into this method is the one for the active document.
                        // If the type is in another file, generate a new model to use to look up the typeinfo. Don't do this by default as it's expensive.
                        var localSemModel = VisualBasicCompilation.Create(string.Empty).AddSyntaxTrees(prop.SyntaxTree).GetSemanticModel(prop.SyntaxTree, ignoreAccessibility: true);

                        typeSymbol = localSemModel.GetTypeInfo(((SimpleAsClauseSyntax)pss.AsClause).Type).Type;
                    }
                }
                else if (prop is PropertyBlockSyntax pbs)
                {
                    try
                    {
                        typeSymbol = semModel.GetTypeInfo(((SimpleAsClauseSyntax)pbs.PropertyStatement.AsClause).Type).Type;
                    }
                    catch (Exception)
                    {
                        // The semanticmodel passed into this method is the one for the active document.
                        // If the type is in another file, generate a new model to use to look up the typeinfo. Don't do this by default as it's expensive.
                        var localSemModel = VisualBasicCompilation.Create(string.Empty).AddSyntaxTrees(prop.SyntaxTree).GetSemanticModel(prop.SyntaxTree, ignoreAccessibility: true);

                        typeSymbol = localSemModel.GetTypeInfo(((SimpleAsClauseSyntax)pbs.PropertyStatement.AsClause).Type).Type;
                    }
                }
            }

            return typeSymbol;
        }

        private static List<PropertyDetails> GetAllPublicProperties(ITypeSymbol typeSymbol, SemanticModel semModel)
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
                        properties.AddRange(baseType.GetMembers().Where(m => m.Kind == SymbolKind.Property && m.DeclaredAccessibility == Accessibility.Public));
                        break;
                    case SymbolKind.ErrorType:
                        Logger?.RecordInfo($"Cannot get sub-properties for known type '{baseType.Name}'.");
                        break;
                }
            }

            var result = new List<PropertyDetails>();

            foreach (var prop in properties)
            {
                if (prop.Name.IsOneOf(NamesOfPropertiesToExcludeFromOutput))
                {
                    Logger?.RecordInfo($"Not including property '{prop.Name}' as it's on the exclusion list.");
                    continue;
                }

                var decRefs = prop.OriginalDefinition.DeclaringSyntaxReferences;

                if (decRefs.Any())
                {
                    var decRef = decRefs.First();

                    var pbs = decRef.SyntaxTree.GetRoot().DescendantNodes(decRef.Span).OfType<PropertyBlockSyntax>().FirstOrDefault();

                    var syntax = pbs ?? (SyntaxNode)decRef.SyntaxTree.GetRoot().DescendantNodes(decRef.Span).OfType<PropertyStatementSyntax>().FirstOrDefault();

                    var details = GetPropertyDetails(syntax, semModel);

                    Logger?.RecordInfo($"Found sub-property '{details.Name}'.");
                    result.Add(details);
                }
                else
                {
                    Logger?.RecordInfo($"Found sub-property of unknown type: {prop.Name}.");
                    result.Add(new PropertyDetails { Name = prop.Name, PropertyType = UnknownOrInvalidTypeName, IsReadOnly = false, Symbol = null });
                }
            }

            return result;
        }
    }
}
