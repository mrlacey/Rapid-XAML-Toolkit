// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using RapidXamlToolkit.Resources;

namespace RapidXamlToolkit.RoslynAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class CsExpandAutoPropertiesAnalyzer : DiagnosticAnalyzer
    {
        // Ids need to be defined but shouldn't ever be visible as descriptor is hidden
        public const string OnPropertyChangedDiagnosticId = "RXRA001";
        public const string SetDiagnosticId = "RXRA002";
        public const string SetPropertyDiagnosticId = "RXRA003";
        public const string DependencyPropertyDiagnosticId = "RXRA004";

        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(StringRes.Info_ExpandAutoPropertyAnalyzerTitle), StringRes.ResourceManager, typeof(StringRes));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(StringRes.Info_ExpandAutoPropertyAnalyzerMessage), StringRes.ResourceManager, typeof(StringRes));

        private static readonly DiagnosticDescriptor OnPropertyRule = new DiagnosticDescriptor(OnPropertyChangedDiagnosticId, Title, MessageFormat, StringRes.RapidXamlToolkit, DiagnosticSeverity.Hidden, isEnabledByDefault: true);

        private static readonly DiagnosticDescriptor SetRule = new DiagnosticDescriptor(SetDiagnosticId, Title, MessageFormat, StringRes.RapidXamlToolkit, DiagnosticSeverity.Hidden, isEnabledByDefault: true);

        private static readonly DiagnosticDescriptor SetPropertyRule = new DiagnosticDescriptor(SetPropertyDiagnosticId, Title, MessageFormat, StringRes.RapidXamlToolkit, DiagnosticSeverity.Hidden, isEnabledByDefault: true);

        private static readonly DiagnosticDescriptor DependencyPropertyRule = new DiagnosticDescriptor(DependencyPropertyDiagnosticId, Title, MessageFormat, StringRes.RapidXamlToolkit, DiagnosticSeverity.Hidden, isEnabledByDefault: true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(OnPropertyRule, SetRule, SetPropertyRule, DependencyPropertyRule);
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
            try
            {
                var pds = (PropertyDeclarationSyntax)context.Node;

                if ((pds?.AccessorList?.ToString().Replace(" ", string.Empty) ?? string.Empty) == "{get;set;}")
                {
                    var classNode = pds.Parent;
                    var classTypeSymbol = (ITypeSymbol)context.SemanticModel.GetDeclaredSymbol(classNode);

                    var propertyNames = new List<string>();

                    foreach (var baseType in classTypeSymbol.GetSelfAndBaseTypes())
                    {
                        propertyNames.AddRange(baseType.GetMembers().Where(m => m.Kind == SymbolKind.Method && !m.IsStatic).Select(m => m.Name));

                        if (baseType.Name == "DependencyObject")
                        {
                            var depPropertyDiagnostic = Diagnostic.Create(DependencyPropertyRule, context.ContainingSymbol.Locations[0], pds.Identifier);

                            context.ReportDiagnostic(depPropertyDiagnostic);
                            break;  // Don't bother looking any deeper in the inheritance hierarchy if found what looking for.
                        }
                    }

                    if (propertyNames.Contains("Set"))
                    {
                        var setDiagnostic = Diagnostic.Create(SetRule, context.ContainingSymbol.Locations[0], pds.Identifier);

                        context.ReportDiagnostic(setDiagnostic);
                    }

                    if (propertyNames.Contains("SetProperty"))
                    {
                        var setPropertyDiagnostic = Diagnostic.Create(SetPropertyRule, context.ContainingSymbol.Locations[0], pds.Identifier);

                        context.ReportDiagnostic(setPropertyDiagnostic);
                    }

                    if (propertyNames.Contains("OnPropertyChanged"))
                    {
                        var onPropertyDiagnostic = Diagnostic.Create(OnPropertyRule, context.ContainingSymbol.Locations[0], pds.Identifier);

                        context.ReportDiagnostic(onPropertyDiagnostic);
                    }
                }
            }
            catch (System.Exception exc)
            {
                System.Diagnostics.Debug.WriteLine(exc);
#if DEBUG
                // Throw in debug to fail tests.
                throw;
#endif
            }
        }
    }
}
