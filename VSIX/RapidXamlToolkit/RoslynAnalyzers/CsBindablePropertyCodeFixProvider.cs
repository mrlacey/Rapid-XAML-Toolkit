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
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(CsBindablePropertyCodeFixProvider))]
    [Shared]
    public class CsBindablePropertyCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(CsExpandAutoPropertiesAnalyzer.BindablePropertyDiagnosticId); }
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

            var title = StringRes.UI_AnalyzerFixBindablePropertyTitle;

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
            var callbackName = $"On{propName}Changed";
            var validationName = $"IsValid{propName}Value";

            PropertyDeclarationSyntax backingProperty =
                SyntaxFactory.PropertyDeclaration(SyntaxFactory.ParseTypeName(propType), propName)
                             .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));

            var getCall =
                SyntaxFactory.InvocationExpression(SyntaxFactory.IdentifierName("GetValue"))
                             .AddArgumentListArguments(
                                SyntaxFactory.Argument(SyntaxFactory.IdentifierName(bindPropName)));

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
                                SyntaxFactory.Argument(SyntaxFactory.IdentifierName(bindPropName)),
                                SyntaxFactory.Argument(SyntaxFactory.IdentifierName("value"))));

            backingProperty = backingProperty.AddAccessorListAccessors(
                SyntaxFactory.AccessorDeclaration(
                    SyntaxKind.SetAccessorDeclaration,
                    SyntaxFactory.Block(SyntaxFactory.List(new[] { setCall }))));

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
                                                        .WithNameColon(SyntaxFactory.NameColon(SyntaxFactory.IdentifierName("defaultBindingMode"))))
                                                .Add(
                                                    SyntaxFactory.Argument(
                                                        SyntaxFactory.IdentifierName(validationName))
                                                        .WithNameColon(SyntaxFactory.NameColon(SyntaxFactory.IdentifierName("validateValue"))))
                                                .Add(
                                                    SyntaxFactory.Argument(
                                                        SyntaxFactory.IdentifierName(callbackName))
                                                        .WithNameColon(SyntaxFactory.NameColon(SyntaxFactory.IdentifierName("propertyChanged")))))))),
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
                        SyntaxFactory.Identifier("bindable"))
                    .WithType(SyntaxFactory.IdentifierName("BindableObject")),
                    SyntaxFactory.Parameter(
                        SyntaxFactory.Identifier("oldValue"))
                    .WithType(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.ObjectKeyword))),
                    SyntaxFactory.Parameter(
                        SyntaxFactory.Identifier("newValue"))
                    .WithType(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.ObjectKeyword))))
                .WithBody(
                    SyntaxFactory.Block().WithCloseBraceToken(
                        SyntaxFactory.Token(
                            SyntaxFactory.TriviaList(
                                SyntaxFactory.Comment($"// TODO: Property changed implementation goes here{System.Environment.NewLine}")),
                            SyntaxKind.CloseBraceToken,
                            SyntaxFactory.TriviaList())))
                .AddModifiers(
                    SyntaxFactory.Token(SyntaxKind.PrivateKeyword),
                    SyntaxFactory.Token(SyntaxKind.StaticKeyword));

            var validationMethod = SyntaxFactory.MethodDeclaration(
                SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.BoolKeyword)),
                SyntaxFactory.Identifier(validationName))
                .AddParameterListParameters(
                    SyntaxFactory.Parameter(
                        SyntaxFactory.Identifier("view"))
                    .WithType(SyntaxFactory.IdentifierName("BindableObject")),
                    SyntaxFactory.Parameter(
                        SyntaxFactory.Identifier("value"))
                    .WithType(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.ObjectKeyword))))
                .AddBodyStatements(
                    SyntaxFactory.ReturnStatement(SyntaxFactory.LiteralExpression(SyntaxKind.TrueLiteralExpression))
                    .WithLeadingTrivia(SyntaxFactory.Comment($"// Add logic for validating value as necessary{System.Environment.NewLine}")))
                .AddModifiers(
                    SyntaxFactory.Token(SyntaxKind.PrivateKeyword),
                    SyntaxFactory.Token(SyntaxKind.StaticKeyword));

            var newNodes = new List<SyntaxNode>
            {
                backingProperty,
                bindProperty,
                callbackMethod,
                validationMethod,
            };

            var oldRoot = await document.GetSyntaxRootAsync(cancellationToken);
            var newRoot = oldRoot.ReplaceNode(pds, newNodes);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}
