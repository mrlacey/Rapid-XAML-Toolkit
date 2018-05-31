// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using RapidXamlToolkit.Logging;
using RapidXamlToolkit.Options;
using CSSyntax = Microsoft.CodeAnalysis.CSharp.Syntax;
using VBSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace RapidXamlToolkit.Commands
{
    public class SetDataContextCommandLogic
    {
        private readonly Profile profile;
        private readonly ILogger logger;
        private readonly IFileSystemAbstraction fileSystem;
        private readonly IVisualStudioAbstraction vs;

        public SetDataContextCommandLogic(Profile profile, ILogger logger, IVisualStudioAbstraction vs, IFileSystemAbstraction fileSystem = null)
        {
            this.profile = profile ?? throw new ArgumentNullException(nameof(profile));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.vs = vs ?? throw new ArgumentNullException(nameof(vs));
            this.fileSystem = fileSystem ?? new WindowsFileSystem();
        }

        public static int GetLineEndPos(string text, int line)
        {
            var result = 0;

            for (int i = 0; i < line; i++)
            {
                result = text.IndexOf('\n', result) + 1;
            }

            return result;
        }

        public (string viewName, string viewModelName, string vmNamespace) InferViewModelNameFromFileName(string fileName)
        {
            // Incoming file may be XAML or code-behind
            // to allow for double extensions
            var viewClass = this.fileSystem.GetFileNameWithoutExtension(fileName).RemoveFromEndIfExists(".xaml");

            var vm = viewClass.RemoveFromEndIfExists(this.profile.ViewGeneration.XamlFileSuffix)
                              .Append(this.profile.ViewGeneration.ViewModelFileSuffix);

            var proj = this.vs.GetActiveProject();

            var vmNs = string.Empty;

            if (proj != null)
            {
                vmNs = $"{proj.Name}.{this.profile.ViewGeneration.ViewModelDirectoryName}";
            }

            return (viewClass, vm, vmNs);
        }

        public bool ShouldEnableCommand()
        {
            var result = false;
            var activeDocName = this.vs.GetActiveDocumentFileName();

            // Is a XAML *or* code-behind file
            if (activeDocName.ToLowerInvariant().Contains(".xaml"))
            {
                var inXamlDoc = activeDocName.EndsWith(".xaml", StringComparison.InvariantCultureIgnoreCase);

                var (_, viewModelName, vmNamespace) = this.InferViewModelNameFromFileName(activeDocName);

                // Only show based on current doc - will need to switch to other doc if not set there
                if (inXamlDoc)
                {
                    if (this.profile.Datacontext.SetsXamlPageAttribute)
                    {
                        var docText = this.vs.GetActiveDocumentText();

                        var contentToInsert = this.profile.Datacontext.XamlPageAttribute.Replace(Placeholder.ViewModelClass, viewModelName)
                                                                                        .Replace(Placeholder.ViewModelNamespace, vmNamespace);

                        if (!docText.Contains(contentToInsert))
                        {
                            result = true;
                        }
                    }
                }
                else
                {
                    if (this.profile.Datacontext.SetsAnyCodeBehindContent)
                    {
                        var docText = this.vs.GetActiveDocumentText();

                        // Compare without whitespace to allow for VS reformatting the code we add
                        var docTextWithoutWhitespace = docText.RemoveAllWhitespace();

                        if (this.profile.Datacontext.SetsCodeBehindPageContent)
                        {
                            var contentToInsert = this.profile.Datacontext.CodeBehindPageContent.Replace(Placeholder.ViewModelClass, viewModelName).Replace(Placeholder.ViewModelNamespace, vmNamespace);

                            if (!docTextWithoutWhitespace.Contains(contentToInsert.RemoveAllWhitespace()))
                            {
                                result = true;
                            }
                        }

                        if (!result && this.profile.Datacontext.SetsCodeBehindConstructorContent)
                        {
                            var ctorCodeToInsert = this.profile.Datacontext.CodeBehindConstructorContent.Replace(Placeholder.ViewModelClass, viewModelName);

                            if (!docTextWithoutWhitespace.Contains(ctorCodeToInsert.RemoveAllWhitespace()))
                            {
                                result = true;
                            }
                        }
                    }
                }
            }

            return result;
        }

        public (bool anythingToAdd, int lineNoToAddAfter, string contentToAdd) GetPageAttributeToAdd(string viewModelName, string vmNamespace)
        {
            var add = false;
            var lineNo = -1;
            var content = string.Empty;

            var docText = this.vs.GetActiveDocumentText();
            var docLines = docText.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

            var contentToInsert = this.profile.Datacontext.XamlPageAttribute.Replace(Placeholder.ViewModelClass, viewModelName).Replace(Placeholder.ViewModelNamespace, vmNamespace);

            if (!docText.Contains(contentToInsert))
            {
                add = true;
                var pageOpeningTagEnd = docText.IndexOf(">");

                // Assume attributes are on different lines
                var openingPageTag = docText.Substring(0, pageOpeningTagEnd);

                var lineNumberToInsertAfter = openingPageTag.Select((c, i) => openingPageTag.Substring(i)).Count(sub => sub.StartsWith(Environment.NewLine));

                var lineToAddAfter = docLines[lineNumberToInsertAfter];

                var whitespaceLength = lineToAddAfter.Length - lineToAddAfter.TrimStart().Length;

                lineNo = lineNumberToInsertAfter;
                content = $"{Environment.NewLine}{new string(' ', whitespaceLength)}{contentToInsert}";
            }

            return (add, lineNo, content);
        }

        public (bool anythingToAdd, int lineNoToAddAfter, string contentToAdd, bool constructorAdded) GetCodeBehindConstructorContentToAdd(string activeDocText, SyntaxNode documentRoot, string viewName, string viewModelName)
        {
            var add = false;
            var lineNo = 0;
            var content = string.Empty;
            var ctorAdded = false;

            // Compare without whitespace to allow for VS reformatting the code we add
            var docTextWithoutWhitespace = activeDocText.RemoveAllWhitespace();

            var ctorCodeToInsert = this.profile.Datacontext.CodeBehindConstructorContent.Replace(Placeholder.ViewModelClass, viewModelName);

            if (!docTextWithoutWhitespace.Contains(ctorCodeToInsert.RemoveAllWhitespace()))
            {
                int ctorEndPos = 0;

                if (this.vs.ActiveDocumentIsCSharp())
                {
                    var allConstructors = documentRoot.DescendantNodes().OfType<CSSyntax.ConstructorDeclarationSyntax>().ToList();

                    if (allConstructors.Any())
                    {
                        ctorEndPos = allConstructors.First().Span.End;

                        // Count based on count of linefeed as may not use CrLf
                        lineNo = activeDocText.Take(ctorEndPos).Count(c => c == '\n');

                        content = $"{Environment.NewLine}{Environment.NewLine}{ctorCodeToInsert}";
                    }
                    else
                    {
                        var classDeclaration = documentRoot.DescendantNodes().OfType<CSSyntax.ClassDeclarationSyntax>().FirstOrDefault();

                        if (classDeclaration != null)
                        {
                            ctorEndPos = classDeclaration.OpenBraceToken.Span.End;
                        }

                        var defaultConstructor = this.profile.Datacontext.DefaultCodeBehindConstructor.Replace(Placeholder.ViewClass, viewName);

                        content = Environment.NewLine
                                + defaultConstructor.Insert(
                                        defaultConstructor.LastIndexOf("}"),
                                        $"{Environment.NewLine}{ctorCodeToInsert}{Environment.NewLine}")
                                + Environment.NewLine;

                        ctorAdded = true;

                        // Count based on count of linefeed as may not use CrLf
                        // Add one because don't get the one where the end pos is
                        lineNo = activeDocText.Take(ctorEndPos).Count(c => c == '\n') + 1;
                    }
                }
                else
                {
                    var allConstructors = documentRoot.DescendantNodes().OfType<VBSyntax.ConstructorBlockSyntax>().ToList();

                    if (allConstructors.Any())
                    {
                        ctorEndPos = allConstructors.First().Span.End;
                        lineNo = activeDocText.Take(ctorEndPos).Count(c => c == '\n');

                        content = $"{Environment.NewLine}{Environment.NewLine}{ctorCodeToInsert}";
                    }
                    else
                    {
                        var classBlock = documentRoot.DescendantNodes().OfType<VBSyntax.ClassBlockSyntax>().FirstOrDefault();

                        var classStatement = classBlock?.DescendantNodes().OfType<VBSyntax.ClassStatementSyntax>().FirstOrDefault();

                        if (classStatement != null)
                        {
                            ctorEndPos = classStatement.Span.End;

                            var implementsStatement = classBlock.DescendantNodes().OfType<VBSyntax.ImplementsStatementSyntax>().LastOrDefault();

                            if (implementsStatement != null)
                            {
                                ctorEndPos = implementsStatement.Span.End;
                            }
                            else
                            {
                                var inheritsStatement = classBlock.DescendantNodes().OfType<VBSyntax.InheritsStatementSyntax>().FirstOrDefault();

                                if (inheritsStatement != null)
                                {
                                    ctorEndPos = inheritsStatement.Span.End;
                                }
                            }
                        }
                        else
                        {
                            var moduleStatement = documentRoot.DescendantNodes().OfType<VBSyntax.ModuleStatementSyntax>().FirstOrDefault();

                            if (moduleStatement != null)
                            {
                                ctorEndPos = moduleStatement.Span.End;
                            }
                        }

                        if (ctorEndPos == 0)
                        {
                            // TODO: ISSUE#22 handle not finding anywhere to add the content - no output?
                        }

                        var defaultConstructor = this.profile.Datacontext.DefaultCodeBehindConstructor.Replace(Placeholder.ViewClass, viewName);

                        content = Environment.NewLine
                                + defaultConstructor.Insert(
                                        defaultConstructor.LastIndexOf("End "),
                                        $"{Environment.NewLine}{ctorCodeToInsert}{Environment.NewLine}")
                                + Environment.NewLine;

                        lineNo = activeDocText.Take(ctorEndPos).Count(c => c == '\n') + 1;

                        ctorAdded = true;
                    }
                }

                add = true;
            }

            return (add, lineNo, content, ctorAdded);
        }

        public (bool anythingToAdd, int lineNoToAddAfter, string contentToAdd) GetCodeBehindPageContentToAdd(string activeDocText, SyntaxNode documentRoot, string viewModelName, string vmNamespace, int lineToInsertAt = -1)
        {
            var add = false;
            var lineNo = 0;
            var content = string.Empty;

            // Compare without whitespace to allow for VS reformatting the code we add
            var docTextWithoutWhitespace = activeDocText.RemoveAllWhitespace();

            var contentToInsert = this.profile.Datacontext.CodeBehindPageContent
                                                          .Replace(Placeholder.ViewModelClass, viewModelName)
                                                          .Replace(Placeholder.ViewModelNamespace, vmNamespace);

            if (!docTextWithoutWhitespace.Contains(contentToInsert.RemoveAllWhitespace()))
            {
                int ctorEndPos = 0;

                // Get end of constructor
                if (this.vs.ActiveDocumentIsCSharp())
                {
                    var allConstructors = documentRoot.DescendantNodes().OfType<CSSyntax.ConstructorDeclarationSyntax>().ToList();

                    if (allConstructors.Any())
                    {
                        ctorEndPos = allConstructors.First().Span.End;
                    }
                    else
                    {
                        var classDeclaration = documentRoot.DescendantNodes().OfType<CSSyntax.ClassDeclarationSyntax>().FirstOrDefault();

                        if (classDeclaration != null)
                        {
                            ctorEndPos = classDeclaration.OpenBraceToken.Span.End;
                        }
                    }
                }
                else
                {
                    var allConstructors = documentRoot.DescendantNodes().OfType<VBSyntax.ConstructorBlockSyntax>().ToList();

                    if (allConstructors.Any())
                    {
                        ctorEndPos = allConstructors.First().Span.End;
                    }
                    else
                    {
                        // If the constructor was added above it won't be in the SyntaxNode (documentRoot)
                        if (lineToInsertAt == -1)
                        {
                            var classBlock = documentRoot.DescendantNodes().OfType<VBSyntax.ClassBlockSyntax>().FirstOrDefault();

                            var classStatement = classBlock?.DescendantNodes().OfType<VBSyntax.ClassStatementSyntax>().FirstOrDefault();

                            if (classStatement != null)
                            {
                                ctorEndPos = classStatement.Span.End;

                                var implementsStatement = classBlock.DescendantNodes().OfType<VBSyntax.ImplementsStatementSyntax>().LastOrDefault();

                                if (implementsStatement != null)
                                {
                                    ctorEndPos = implementsStatement.Span.End;
                                }
                                else
                                {
                                    var inheritsStatement = classBlock.DescendantNodes().OfType<VBSyntax.InheritsStatementSyntax>().FirstOrDefault();

                                    if (inheritsStatement != null)
                                    {
                                        ctorEndPos = inheritsStatement.Span.End;
                                    }
                                }
                            }
                            else
                            {
                                var moduleStatement = documentRoot.DescendantNodes().OfType<VBSyntax.ModuleBlockSyntax>().FirstOrDefault();

                                if (moduleStatement != null)
                                {
                                    ctorEndPos = moduleStatement.Span.End;
                                }
                            }
                        }
                    }
                }

                if (ctorEndPos == 0 && lineToInsertAt == -1)
                {
                    // TODO: Issue#22 handle not finding anywhere to add the content - no output?
                }

                if (lineToInsertAt > -1)
                {
                    lineNo = lineToInsertAt;
                }
                else
                {
                    lineNo = activeDocText.Take(ctorEndPos).Count(c => c == '\n') + 1;
                }

                add = true;
                content = $"{Environment.NewLine}{Environment.NewLine}{contentToInsert}";
            }

            return (add, lineNo, content);
        }

        public (bool anythingToAdd, int lineNoToAddAfter, string contentToAdd)[] GetCodeBehindContentToAdd(string viewName, string viewModelName, string vmNamespace, SyntaxNode documentRoot)
        {
            var result = new (bool add, int line, string content)[2];

            // The Active Document Text is the underlying model of the document and what we directly manipulate
            // The documentRoot is the Roslyn model of the document
            // We combine the raw text and tree models to produce the output we need
            var activeDocText = this.vs.GetActiveDocumentText();

            bool constructorAdded = false;

            // Add to the constructor first as will need to create it if it doesn't exist
            // If added other content first we couldn't position it relative to the new constructor
            if (this.profile.Datacontext.SetsCodeBehindConstructorContent)
            {
                var ctorAddition = this.GetCodeBehindConstructorContentToAdd(activeDocText, documentRoot, viewName, viewModelName);
                result[0] = (ctorAddition.anythingToAdd, ctorAddition.lineNoToAddAfter, ctorAddition.contentToAdd);
                constructorAdded = ctorAddition.constructorAdded;
            }
            else
            {
                result[0] = (false, 0, string.Empty);
            }

            if (this.profile.Datacontext.SetsCodeBehindPageContent)
            {
                if (result[0].add)
                {
                    var insertPos = GetLineEndPos(activeDocText, result[0].line);

                    activeDocText = activeDocText.Insert(insertPos, result[0].content);
                }

                result[1] = this.GetCodeBehindPageContentToAdd(activeDocText, documentRoot, viewModelName, vmNamespace, constructorAdded ? result[0].line + result[0].content.Count(c => c == '\n') - 1 : -1);
            }
            else
            {
                result[1] = (false, -1, string.Empty);
            }

            return result;
        }
    }
}
