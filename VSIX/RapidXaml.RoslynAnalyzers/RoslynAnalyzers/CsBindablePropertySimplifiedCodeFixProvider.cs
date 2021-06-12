// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RapidXamlToolkit.Resources;

namespace RapidXamlToolkit.RoslynAnalyzers
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(CsBindablePropertySimplifiedCodeFixProvider))]
    [Shared]
    public class CsBindablePropertySimplifiedCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(CsExpandAutoPropertiesAnalyzer.BindablePropertySimplifiedDiagnosticId); }
        }

        public sealed override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var pds = root.FindToken(diagnosticSpan.Start).Parent as PropertyDeclarationSyntax;

            var title = StringRes.UI_AnalyzerFixBindablePropertySimplifiedTitle;

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: title,
                    createChangedDocument: c => this.ToBindablePropertyAsync(context.Document, pds, c),
                    equivalenceKey: title),
                diagnostic);
        }

        private async Task<Document> ToBindablePropertyAsync(Document document, SyntaxNode equalsSyntax, CancellationToken cancellationToken)
        {
            var pds = equalsSyntax as PropertyDeclarationSyntax;

            var propName = pds.Identifier.ToString();
            var propType = pds.GetTypeName();

            var className = ((ClassDeclarationSyntax)pds.Parent).Identifier.Text;

            var bindPropName = $"{propName}Property";

            PropertyDeclarationSyntax backingProperty =
                SyntaxFactory.PropertyDeclaration(SyntaxFactory.ParseTypeName(propType), propName)
                             .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));

            var getCall =
                SyntaxFactory.InvocationExpression(SyntaxFactory.IdentifierName("GetValue"))
                             .AddArgumentListArguments(
                                SyntaxFactory.Argument(SyntaxFactory.IdentifierName(bindPropName)));

            var getCast = SyntaxFactory.CastExpression(
                SyntaxFactory.ParseTypeName(propType),
                getCall);

            backingProperty = backingProperty.AddAccessorListAccessors(
                SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                    .WithExpressionBody(SyntaxFactory.ArrowExpressionClause(getCast))
                    .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)));

            var setCall =
                SyntaxFactory.InvocationExpression(SyntaxFactory.IdentifierName("SetValue"))
                             .AddArgumentListArguments(
                                SyntaxFactory.Argument(SyntaxFactory.IdentifierName(bindPropName)),
                                SyntaxFactory.Argument(SyntaxFactory.IdentifierName("value")));

            backingProperty = backingProperty.AddAccessorListAccessors(
                SyntaxFactory.AccessorDeclaration(
                    SyntaxKind.SetAccessorDeclaration).WithExpressionBody(
                    SyntaxFactory.ArrowExpressionClause(setCall))
                    .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)));

            var bindProperty = SyntaxFactory.FieldDeclaration(
                SyntaxFactory.VariableDeclaration(
                    SyntaxFactory.ParseTypeName("BindableProperty"),
                    SyntaxFactory.SeparatedList(
                        new[]
                        {
                            SyntaxFactory.VariableDeclarator(SyntaxFactory.Identifier(bindPropName))
                                         .WithInitializer(SyntaxFactory.EqualsValueClause(
                                        SyntaxFactory.InvocationExpression(
                                            SyntaxFactory.MemberAccessExpression(
                                                SyntaxKind.SimpleMemberAccessExpression,
                                                SyntaxFactory.IdentifierName("BindableProperty"),
                                                SyntaxFactory.IdentifierName("Create")),
                                            SyntaxFactory.ArgumentList(
                                                default(SeparatedSyntaxList<ArgumentSyntax>)
                                                .Add(
                                                    SyntaxFactory.Argument(
                                                        SyntaxFactory.InvocationExpression(
                                                            SyntaxFactory.IdentifierName("nameof"))
                                                        .AddArgumentListArguments(
                                                            SyntaxFactory.Argument(SyntaxFactory.IdentifierName(propName)))))
                                                .Add(
                                                    SyntaxFactory.Argument(
                                                        SyntaxFactory.InvocationExpression(
                                                            SyntaxFactory.IdentifierName("typeof"))
                                                         .AddArgumentListArguments(
                                                            SyntaxFactory.Argument(SyntaxFactory.IdentifierName(propType)))))
                                                .Add(
                                                    SyntaxFactory.Argument(
                                                        SyntaxFactory.InvocationExpression(
                                                            SyntaxFactory.IdentifierName("typeof"))
                                                         .AddArgumentListArguments(
                                                            SyntaxFactory.Argument(SyntaxFactory.IdentifierName(className)))))
                                                .Add(
                                                    SyntaxFactory.Argument(
                                                        SyntaxFactory.DefaultExpression(SyntaxFactory.ParseTypeName(propType)))
                                                        .WithNameColon(SyntaxFactory.NameColon(SyntaxFactory.IdentifierName("defaultValue"))))
                                                .Add(
                                                    SyntaxFactory.Argument(
                                                        SyntaxFactory.MemberAccessExpression(
                                                            SyntaxKind.SimpleMemberAccessExpression,
                                                            SyntaxFactory.IdentifierName("BindingMode"),
                                                            SyntaxFactory.IdentifierName("Default")))
                                                        .WithNameColon(SyntaxFactory.NameColon(SyntaxFactory.IdentifierName("defaultBindingMode")))))))),
                        })))
                .AddModifiers(
                    SyntaxFactory.Token(SyntaxKind.PublicKeyword),
                    SyntaxFactory.Token(SyntaxKind.StaticKeyword),
                    SyntaxFactory.Token(SyntaxKind.ReadOnlyKeyword));

            var newNodes = new List<SyntaxNode>
            {
                backingProperty,
                bindProperty,
            };

            var oldRoot = await document.GetSyntaxRootAsync(cancellationToken);
            var newRoot = oldRoot.ReplaceNode(pds, newNodes);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}
