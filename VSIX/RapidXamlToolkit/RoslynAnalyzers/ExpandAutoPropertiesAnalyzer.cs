// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using RapidXamlToolkit.Parsers;
using RapidXamlToolkit.Resources;

namespace RapidXamlToolkit.RoslynAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ExpandAutoPropertiesAnalyzer : DiagnosticAnalyzer
    {
        // Ids need to be defined but shouldn't ever be visible as descriptor is hidden
        public const string OnPropertyChangedDiagnosticId = "RXD001";
        public const string SetDiagnosticId = "RXD002";
        public const string SetPropertyDiagnosticId = "RXD003";
        public const string DependencyPropertyDiagnosticId = "RXD004";
        private const string Category = "Rapid XAML";

        // You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
        // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Localizing%20Analyzers.md for more on localization
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(StringRes.AnalyzerTitle), StringRes.ResourceManager, typeof(StringRes));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(StringRes.AnalyzerMessageFormat), StringRes.ResourceManager, typeof(StringRes));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(StringRes.AnalyzerDescription), StringRes.ResourceManager, typeof(StringRes));

        private static DiagnosticDescriptor onPropertyRule = new DiagnosticDescriptor(OnPropertyChangedDiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Hidden, isEnabledByDefault: true, description: Description);

        private static DiagnosticDescriptor setRule = new DiagnosticDescriptor(SetDiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Hidden, isEnabledByDefault: true, description: Description);

        private static DiagnosticDescriptor setPropertyRule = new DiagnosticDescriptor(SetPropertyDiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Hidden, isEnabledByDefault: true, description: Description);

        private static DiagnosticDescriptor dependencyPropertyRule = new DiagnosticDescriptor(DependencyPropertyDiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Hidden, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(onPropertyRule, setRule, setPropertyRule, dependencyPropertyRule);
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(this.AnalyzeSyntaxNode, SyntaxKind.PropertyDeclaration);
        }

        private void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
        {
            var pds = (PropertyDeclarationSyntax)context.Node;

            if (pds.AccessorList.ToString().Replace(" ", string.Empty) == "{get;set;}")
            {
                var classNode = pds.Parent;
                var classTypeSymbol = (ITypeSymbol)context.SemanticModel.GetDeclaredSymbol(classNode);

                var propertyNames = new List<string>();

                foreach (var baseType in classTypeSymbol.GetSelfAndBaseTypes())
                {
                    propertyNames.AddRange(baseType.GetMembers().Where(m => m.Kind == SymbolKind.Method && !m.IsStatic).Select(m => m.Name));

                    if (baseType.Name == "DependencyObject")
                    {
                        var depPropertyDiagnostic = Diagnostic.Create(dependencyPropertyRule, context.ContainingSymbol.Locations[0], pds.Identifier);

                        context.ReportDiagnostic(depPropertyDiagnostic);
                    }
                }

                if (propertyNames.Contains("Set"))
                {
                    var setDiagnostic = Diagnostic.Create(setRule, context.ContainingSymbol.Locations[0], pds.Identifier);

                    context.ReportDiagnostic(setDiagnostic);
                }

                if (propertyNames.Contains("SetProperty"))
                {
                    var setPropertyDiagnostic = Diagnostic.Create(setPropertyRule, context.ContainingSymbol.Locations[0], pds.Identifier);

                    context.ReportDiagnostic(setPropertyDiagnostic);
                }

                if (propertyNames.Contains("OnPropertyChanged"))
                {
                    var onPropertyDiagnostic = Diagnostic.Create(onPropertyRule, context.ContainingSymbol.Locations[0], pds.Identifier);

                    context.ReportDiagnostic(onPropertyDiagnostic);
                }
            }
        }
    }
}
