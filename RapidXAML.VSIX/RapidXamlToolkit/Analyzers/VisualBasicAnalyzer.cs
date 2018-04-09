// <copyright file="VisualBasicAnalyzer.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace RapidXamlToolkit
{
    public class VisualBasicAnalyzer : AnalyzerBase, IDocumentAnalyzer
    {
        public new string FileExtension { get; } = "vb";

        public static (List<string> strings, int count) GetSubPropertyOutput(ITypeSymbol typeSymbol, string propertyName, Profile profile)
        {
            var result = new List<string>();

            var x = typeSymbol.GetMembers();

            var numericSubstitute = 0;

            foreach (var symbol in x)
            {
                if (symbol.Kind == SymbolKind.Property)
                {
                    // As can't (yet) get details about properties, forcibly get the fallback
                    var (output, counter) = GetPropertyOutput(profile, "UNKNOWNTYPE", symbol.Name, isReadOnly: false, numericSubstitute: numericSubstitute);

                    numericSubstitute = counter;
                    result.Add(output);
                }
            }

            if (!result.Any())
            {
                var (output, counter) = GetPropertyOutput(profile, "UNKNOWNTYPE", propertyName, isReadOnly: false, numericSubstitute: numericSubstitute);

                numericSubstitute = counter;
                result.Add(output);
            }

            return (result, numericSubstitute);
        }

        public static List<DeclarationStatementSyntax> GetAllPublicPropertiesFromClassNode(SyntaxNode classNode)
        {
            var result = new List<DeclarationStatementSyntax>();

            if (classNode is ClassBlockSyntax cbs)
            {
                var pssList = cbs.DescendantNodes().OfType<PropertyStatementSyntax>()
                    .Where(n => n.Modifiers.Any(SyntaxKind.PublicKeyword)).ToList();

                result.AddRange(pssList);

                var pbsList = cbs.DescendantNodes().OfType<PropertyBlockSyntax>()
                    .Where(n => n.Accessors.Any(SyntaxKind.PublicKeyword)).ToList();

                result.AddRange(pbsList);
            }

            if (classNode is ModuleBlockSyntax mbs)
            {
                var pssList = mbs.DescendantNodes().OfType<PropertyStatementSyntax>()
                    .Where(n => n.Modifiers.Any(SyntaxKind.PublicKeyword)).ToList();

                result.AddRange(pssList);

                var pbsList = mbs.DescendantNodes().OfType<PropertyBlockSyntax>()
                    .Where(n => n.Accessors.Any(SyntaxKind.PublicKeyword)).ToList();

                result.AddRange(pbsList);
            }

            return result;
        }

        public static (string propertyType, string propertyname, bool propertyIsReadOnly) GetPropertyInfo(SyntaxNode propertyNode)
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

                if (setter != null)
                {
                    propIsReadOnly = (setter as AccessorBlockSyntax)?.AccessorStatement.Modifiers.Any(m => m.RawKind == (int)SyntaxKind.PrivateKeyword);
                }
                else
                {
                    propIsReadOnly = pbs.PropertyStatement.Modifiers.Any(m => m.RawKind == (int)SyntaxKind.ReadOnlyKeyword);
                }
            }

            return (propertyType, propertyName, propIsReadOnly ?? false);
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

        public static (string output, string name, int counter) GetOutputToAdd(SemanticModel semModel, Profile profileOverload, SyntaxNode prop, int numericCounter = 0)
        {
            var (pType, pName, pIsReadOnly) = GetPropertyInfo(prop);

            ITypeSymbol typeSymbol = null;

            if (pType.IsGenericTypeName())
            {
                TypeSyntax typeSyntax = null;

                if (prop is PropertyStatementSyntax pss)
                {
                    typeSyntax = ((GenericNameSyntax)((SimpleAsClauseSyntax)pss.AsClause).Type).TypeArgumentList.Arguments.First();
                }

                if (prop is PropertyBlockSyntax pbs)
                {
                    typeSyntax = ((GenericNameSyntax)((SimpleAsClauseSyntax)pbs.PropertyStatement.AsClause).Type).TypeArgumentList.Arguments.First();
                }

                typeSymbol = semModel.GetTypeInfo(typeSyntax).Type;
            }
            else
            {
                if (prop is PropertyStatementSyntax pss)
                {
                    typeSymbol = semModel.GetTypeInfo(((SimpleAsClauseSyntax)pss.AsClause).Type).Type;
                }
                else if (prop is PropertyBlockSyntax pbs)
                {
                    typeSymbol = semModel.GetTypeInfo(((SimpleAsClauseSyntax)pbs.PropertyStatement.AsClause).Type).Type;
                }
            }

            var (output, counter) = profileOverload == null
                ? GetPropertyOutputForActiveProfile(pType, pName, pIsReadOnly, numericCounter, () => GetSubPropertyOutput(typeSymbol, pName, GetSettings().GetActiveProfile()))
                : GetPropertyOutput(profileOverload, pType, pName, pIsReadOnly, numericCounter, () => GetSubPropertyOutput(typeSymbol, pName, profileOverload));

            return (output, pName, counter);
        }

        public AnalyzerOutput GetSingleItemOutput(SyntaxNode documentRoot, SemanticModel semModel, int caretPosition, Profile profileOverload = null)
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

            if (propertyNode != null)
            {
                var (output, name, _) = GetOutputToAdd(semModel, profileOverload, propertyNode);

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

                var properties = GetAllPublicPropertiesFromClassNode(classNode);

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
            var allProperties = documentRoot.DescendantNodes().OfType<PropertyStatementSyntax>().ToList();

            var propertiesOfInterest = new List<PropertyStatementSyntax>();

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
                var (pType, pName, pIsReadOnly) = GetPropertyInfo(prop);

                var typeInfo = semModel.GetTypeInfo(((SimpleAsClauseSyntax)prop.AsClause).Type).Type;

                var toAdd = profileOverload == null
                        ? GetPropertyOutputForActiveProfile(pType, pName, pIsReadOnly, numericCounter, () => GetSubPropertyOutput(typeInfo, pName, GetSettings().GetActiveProfile()))
                        : GetPropertyOutput(profileOverload, pType, pName, pIsReadOnly, numericCounter, () => GetSubPropertyOutput(typeInfo, pName, profileOverload));

                numericCounter = toAdd.counter;
                output.AppendLine(toAdd.output);

                propertyNames.Add(pName);
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
    }
}
