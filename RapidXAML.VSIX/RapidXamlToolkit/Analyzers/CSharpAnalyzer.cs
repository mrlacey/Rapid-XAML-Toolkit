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
        public new string FileExtension { get; } = "cs";

        public static (List<string> strings, int count) GetSubPropertyOutput(ITypeSymbol typeSymbol, string propertyName, Profile profile)
        {
            var result = new List<string>();

            // CSharp Symbols are internal (so inaccessible) and won't be there for all types
            var x = typeSymbol.GetMembers(); // .OfType<Microsoft.CodeAnalysis.CSharp.Symbols.SourcePropertySymbol>();

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

        public static (string propertyType, string propertyname, bool propertyIsReadOnly) GetPropertyInfo(PropertyDeclarationSyntax propertyDeclaration)
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

            return (propertyType, propertyName, propIsReadOnly ?? false);
        }

        public static string GetIdentifier(SyntaxNode syntaxNode)
        {
            return syntaxNode?.ChildTokens().FirstOrDefault(t => t.Kind() is SyntaxKind.IdentifierToken).ValueText;
        }

        public AnalyzerOutput GetSingleItemOutput(SyntaxNode documentRoot, SemanticModel semModel, int caretPosition, Profile profileOverload = null)
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

                var inheritedProperties = GetInheritedPropertiesFromClassNode(semModel, classNode);

                properties.AddRange(inheritedProperties);

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
                var (pType, pName, pIsReadOnly) = GetPropertyInfo(prop);

                var typeInfo = semModel.GetTypeInfo(prop.Type).Type;

                var toAdd = profileOverload == null ? GetPropertyOutputForActiveProfile(pType, pName, pIsReadOnly, numericCounter, () => GetSubPropertyOutput(typeInfo, pName, GetSettings().GetActiveProfile()))
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

        private static (string output, string name, int counter) GetOutputToAdd(SemanticModel semModel, Profile profileOverload, PropertyDeclarationSyntax prop, int numericCounter = 0)
        {
            var (pType, pName, pIsReadOnly) = GetPropertyInfo(prop);

            ITypeSymbol typeSymbol;

            if (pType.IsGenericTypeName())
            {
                var t = ((GenericNameSyntax)prop.Type).TypeArgumentList.Arguments.First();

                typeSymbol = semModel.GetTypeInfo(t).Type;
            }
            else
            {
                // SymbolInfo *may* be able to provide more about the properties of the underlying type
                // semModel.GetSymbolInfo(propertyNode.Type).Symbol

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

            var (output, counter) = profileOverload == null
                ? GetPropertyOutputForActiveProfile(pType, pName, pIsReadOnly, numericCounter, () => GetSubPropertyOutput(typeSymbol, pName, GetSettings().GetActiveProfile()))
                : GetPropertyOutput(profileOverload, pType, pName, pIsReadOnly, numericCounter, () => GetSubPropertyOutput(typeSymbol, pName, profileOverload));

            return (output, pName, counter);
        }

        private static List<PropertyDeclarationSyntax> GetAllPublicPropertiesFromClassNode(ClassDeclarationSyntax classNode)
        {
            return classNode.DescendantNodes().OfType<PropertyDeclarationSyntax>().Where(n => n.Modifiers.Any(SyntaxKind.PublicKeyword)).ToList();
        }

        private static List<PropertyDeclarationSyntax> GetInheritedPropertiesFromClassNode(SemanticModel semModel, ClassDeclarationSyntax classNode)
        {
            var typeC = (ITypeSymbol)semModel.GetDeclaredSymbol(classNode);
            var types = typeC.GetBaseTypes();
            var members = types.SelectMany(n => n.GetMembers()).Where(m => m.Kind == SymbolKind.Property && m.DeclaredAccessibility == Accessibility.Public).ToList();

            var result = new List<PropertyDeclarationSyntax>();

            foreach (var member in members)
            {
                var decRef = member.OriginalDefinition.DeclaringSyntaxReferences[0];

                result.Add(decRef.SyntaxTree.GetRoot().DescendantNodes(decRef.Span).OfType<PropertyDeclarationSyntax>().First());
            }

            return result;
        }
    }
}
