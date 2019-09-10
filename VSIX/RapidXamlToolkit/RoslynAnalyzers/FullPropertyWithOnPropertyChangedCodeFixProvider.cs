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

namespace RapidXamlToolkit.RoslynAnalyzers
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(FullPropertyWithOnPropertyChangedCodeFixProvider))]
    [Shared]
    public class FullPropertyWithOnPropertyChangedCodeFixProvider : CodeFixProvider
    {
        private const string Title = "To full property that calls `OnPropertyChanged`";

        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(ExpandAutoPropertiesAnalyzer.OnPropertyChangedDiagnosticId); }
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

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: Title,
                    createChangedDocument: c => this.AddBackingFieldAsync(context.Document, pds, c),
                    equivalenceKey: Title),
                diagnostic);
        }

        private async Task<Document> AddBackingFieldAsync(Document document, SyntaxNode equalsSyntax, CancellationToken cancellationToken)
        {
            var pds = equalsSyntax as PropertyDeclarationSyntax;

            var propName = pds.Identifier.ToString();

            var fieldName = "_" + char.ToLowerInvariant(propName[0]) + propName.Substring(1);

            var fieldType = (pds.Type as PredefinedTypeSyntax).Keyword.Text;

            var backingField = SyntaxFactory.FieldDeclaration(
                SyntaxFactory.VariableDeclaration(
                    SyntaxFactory.ParseTypeName(fieldType),
                    SyntaxFactory.SeparatedList(
                        new[]
                        {
                            SyntaxFactory.VariableDeclarator(SyntaxFactory.Identifier(fieldName)),
                        })))
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PrivateKeyword));

            PropertyDeclarationSyntax newProperty =
                SyntaxFactory.PropertyDeclaration(SyntaxFactory.ParseTypeName(fieldType), propName)
                             .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));

            newProperty = newProperty.AddAccessorListAccessors(
                SyntaxFactory.AccessorDeclaration(
                    SyntaxKind.GetAccessorDeclaration,
                    SyntaxFactory.Block(
                        SyntaxFactory.List(new[]
                        {
                            SyntaxFactory.ReturnStatement(SyntaxFactory.IdentifierName(fieldName)),
                        })
                    )));

            var setProperty = SyntaxFactory.ExpressionStatement(
                            SyntaxFactory.AssignmentExpression(
                                SyntaxKind.SimpleAssignmentExpression,
                                SyntaxFactory.IdentifierName(fieldName),
                                SyntaxFactory.IdentifierName("value")),
                            SyntaxFactory.Token(SyntaxKind.SemicolonToken));

            var onPropertyChangedCall = SyntaxFactory.ExpressionStatement(
                SyntaxFactory.InvocationExpression(
                    SyntaxFactory.IdentifierName("OnPropertyChanged"))
                .AddArgumentListArguments(
                        SyntaxFactory.Argument(
                            SyntaxFactory.InvocationExpression(
                                SyntaxFactory.IdentifierName("nameof"))
                            .AddArgumentListArguments(
                                        SyntaxFactory.Argument(
                                            SyntaxFactory.IdentifierName(propName))))));

            newProperty = newProperty.AddAccessorListAccessors(
                SyntaxFactory.AccessorDeclaration(
                    SyntaxKind.SetAccessorDeclaration,
                    SyntaxFactory.Block(SyntaxFactory.List(new[] { setProperty, onPropertyChangedCall })))
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PrivateKeyword)));

            var oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            var newNodes = new List<SyntaxNode>();
            newNodes.Add(backingField);
            newNodes.Add(newProperty);

            var newRoot = oldRoot.ReplaceNode(pds, newNodes);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}
