// Copyright (c) Microsoft Corporation. All rights reserved.
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
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(DependencyPropertyCodeFixProvider))]
    [Shared]
    public class DependencyPropertyCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(ExpandAutoPropertiesAnalyzer.DependencyPropertyDiagnosticId); }
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

            var title = StringRes.UI_AnalyzerFixDependencyPropertyTitle;

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: title,
                    createChangedDocument: c => this.ToDependencyPropertyAsync(context.Document, pds, c),
                    equivalenceKey: title),
                diagnostic);
        }

        private async Task<Document> ToDependencyPropertyAsync(Document document, SyntaxNode equalsSyntax, CancellationToken cancellationToken)
        {
            var pds = equalsSyntax as PropertyDeclarationSyntax;

            var propName = pds.Identifier.ToString();
            var propType = (pds.Type as PredefinedTypeSyntax).Keyword.Text;

            var className = ((ClassDeclarationSyntax)pds.Parent).Identifier.Text;

            var depPropName = $"{propName}Property";
            var callbackName = $"On{propName}Changed";

            PropertyDeclarationSyntax backingProperty =
                SyntaxFactory.PropertyDeclaration(SyntaxFactory.ParseTypeName(propType), propName)
                             .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));

            var getCall =
                SyntaxFactory.InvocationExpression(SyntaxFactory.IdentifierName("GetValue"))
                             .AddArgumentListArguments(
                                SyntaxFactory.Argument(SyntaxFactory.IdentifierName(depPropName)));

            var getCast = SyntaxFactory.CastExpression(
                SyntaxFactory.ParseTypeName(propType),
                SyntaxFactory.ParenthesizedExpression(getCall));

            var returnStatement = SyntaxFactory.ReturnStatement(getCast);

            backingProperty = backingProperty.AddAccessorListAccessors(
                SyntaxFactory.AccessorDeclaration(
                    SyntaxKind.GetAccessorDeclaration,
                    SyntaxFactory.Block(SyntaxFactory.List(new[] { returnStatement }))));

            var setCall = SyntaxFactory.ExpressionStatement(
                SyntaxFactory.InvocationExpression(SyntaxFactory.IdentifierName("SetValue"))
                             .AddArgumentListArguments(
                                SyntaxFactory.Argument(SyntaxFactory.IdentifierName(depPropName)),
                                SyntaxFactory.Argument(SyntaxFactory.IdentifierName("value"))));

            backingProperty = backingProperty.AddAccessorListAccessors(
                SyntaxFactory.AccessorDeclaration(
                    SyntaxKind.SetAccessorDeclaration,
                    SyntaxFactory.Block(SyntaxFactory.List(new[] { setCall }))));

            var depProperty = SyntaxFactory.FieldDeclaration(
                SyntaxFactory.VariableDeclaration(
                    SyntaxFactory.ParseTypeName("DependencyProperty"),
                    SyntaxFactory.SeparatedList(
                        new[]
                        {
                            SyntaxFactory.VariableDeclarator(SyntaxFactory.Identifier(depPropName))
                                         .WithInitializer(SyntaxFactory.EqualsValueClause(
                                        SyntaxFactory.InvocationExpression(
                                            SyntaxFactory.MemberAccessExpression(
                                                SyntaxKind.SimpleMemberAccessExpression,
                                                SyntaxFactory.IdentifierName("DependencyProperty"),
                                                SyntaxFactory.IdentifierName("Register")),
                                            SyntaxFactory.ArgumentList(
                                                default(SeparatedSyntaxList<ArgumentSyntax>)
                                                .Add(
                                                    SyntaxFactory.Argument(
                                                        SyntaxFactory.LiteralExpression(
                                                            SyntaxKind.StringLiteralExpression,
                                                            SyntaxFactory.Literal(propName))))
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
                                                        SyntaxFactory.ObjectCreationExpression(
                                                            SyntaxFactory.IdentifierName("PropertyMetadata"))
                                                         .AddArgumentListArguments(
                                                            SyntaxFactory.Argument(SyntaxFactory.DefaultExpression(SyntaxFactory.ParseTypeName(propType))),
                                                            SyntaxFactory.Argument(
                                                                SyntaxFactory.ObjectCreationExpression(
                                                                    SyntaxFactory.IdentifierName("PropertyChangedCallback"))
                                                                     .AddArgumentListArguments(
                                                                        SyntaxFactory.Argument(SyntaxFactory.IdentifierName(callbackName))))))))))),
                        })))
                .AddModifiers(
                    SyntaxFactory.Token(SyntaxKind.PublicKeyword),
                    SyntaxFactory.Token(SyntaxKind.StaticKeyword),
                    SyntaxFactory.Token(SyntaxKind.ReadOnlyKeyword));

            var callbackMethod = SyntaxFactory.MethodDeclaration(
                SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.VoidKeyword)),
                SyntaxFactory.Identifier(callbackName))
                .AddParameterListParameters(
                    SyntaxFactory.Parameter(
                        SyntaxFactory.Identifier("d"))
                    .WithType(SyntaxFactory.IdentifierName("DependencyObject")),
                    SyntaxFactory.Parameter(
                        SyntaxFactory.Identifier("e"))
                    .WithType(SyntaxFactory.IdentifierName("DependencyPropertyChangedEventArgs")))
                .AddBodyStatements(
                    SyntaxFactory.ThrowStatement(
                        SyntaxFactory.ObjectCreationExpression(
                            SyntaxFactory.IdentifierName("NotImplementedException"))
                        .AddArgumentListArguments()))
                .AddModifiers(
                    SyntaxFactory.Token(SyntaxKind.PrivateKeyword),
                    SyntaxFactory.Token(SyntaxKind.StaticKeyword));

            var newNodes = new List<SyntaxNode>();
            newNodes.Add(backingProperty);
            newNodes.Add(depProperty);
            newNodes.Add(callbackMethod);

            var oldRoot = await document.GetSyntaxRootAsync(cancellationToken);
            var newRoot = oldRoot.ReplaceNode(pds, newNodes);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}
