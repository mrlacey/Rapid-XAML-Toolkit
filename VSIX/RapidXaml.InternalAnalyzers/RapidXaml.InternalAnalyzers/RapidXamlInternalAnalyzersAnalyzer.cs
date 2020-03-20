// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace RapidXaml.InternalAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class RapidXamlInternalAnalyzersAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "RXILOC";
        private const string Category = "Naming";
        private static readonly string Title = "Incorrect use of hard-coded string.";
        private static readonly string MessageFormat = "Hard-coded string passed to '{0}'";
        private static readonly string Description = "All strings passed to ILogger methods must be localized.";

        private static DiagnosticDescriptor rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(rule);
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(this.AnalyzeSyntaxNode, SyntaxKind.InvocationExpression);
        }

        private void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
        {
            var invocationExpression = (InvocationExpressionSyntax)context.Node;

            var expr = invocationExpression.Expression;

            var checkArgs = string.Empty;

            if (expr is MemberBindingExpressionSyntax mbes)
            {
                var mbesName = mbes.Name.ToString();

                if (mbesName == "RecordGeneralError")
                {
                    checkArgs = mbesName;
                }
                else if (mbesName == "RecordNotice")
                {
                    checkArgs = mbesName;
                }
                else if (mbesName == "RecordInfo")
                {
                    checkArgs = mbesName;
                }
                else if (mbesName == "RecordFeatureUsage")
                {
                    checkArgs = mbesName;
                }
                else if (mbesName == "RecordError")
                {
                    checkArgs = mbesName;
                }
            }
            else if (expr is MemberAccessExpressionSyntax maes)
            {
                var maesName = maes.Name.ToString();

                if (maesName == "RecordGeneralError")
                {
                    checkArgs = maesName;
                }
                else if (maesName == "RecordNotice")
                {
                    checkArgs = maesName;
                }
                else if (maesName == "RecordInfo")
                {
                    checkArgs = maesName;
                }
                else if (maesName == "RecordFeatureUsage")
                {
                    checkArgs = maesName;
                }
                else if (maesName == "RecordError")
                {
                    checkArgs = maesName;
                }
            }

            if (!string.IsNullOrWhiteSpace(checkArgs))
            {
                var arg = invocationExpression.ArgumentList.Arguments.FirstOrDefault();

                if (arg != null)
                {
                    if (arg.Expression is LiteralExpressionSyntax
                     || arg.Expression.ToString().StartsWith("\""))
                    {
                        var diagnostic = Diagnostic.Create(rule, arg.GetLocation(), checkArgs);

                        context.ReportDiagnostic(diagnostic);
                    }
                }
            }
        }
    }
}
