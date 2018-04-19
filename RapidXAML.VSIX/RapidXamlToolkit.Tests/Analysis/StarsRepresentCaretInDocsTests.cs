// <copyright file="StarsRepresentCaretInDocsTests.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RapidXamlToolkit.Tests.Analysis
{
    public abstract class StarsRepresentCaretInDocsTests
    {
        public const string TestLibraryPath = "./Assets/TestLibrary.dll";

        public TestContext TestContext { get; set; }

        public Profile DefaultProfile => GetDefaultTestSettings().GetActiveProfile();

        public static Settings GetDefaultTestSettings()
        {
            return new Settings
            {
                ActiveProfileName = "UWP",
                Profiles = new List<Profile>
                {
                    new Profile
                    {
                        Name = "UWP",
                        ClassGrouping = "StackPanel",
                        FallbackOutput = "<TextBlock Text=\"FALLBACK_$name$\" />",
                        SubPropertyOutput = "<TextBlock Text=\"SUBPROP_$name$\" />",
                        Mappings = new List<Mapping>
                        {
                            new Mapping
                            {
                                Type = "String",
                                NameContains = "password|pwd",
                                Output = "<PasswordBox Password=\"{x:Bind $name$}\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "String",
                                NameContains = string.Empty,
                                Output = "<TextBox Text=\"{x:Bind $name$, Mode=TwoWay}\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "String",
                                NameContains = string.Empty,
                                Output = "<TextBlock Text=\"$name$\" />",
                                IfReadOnly = true,
                            },
                            new Mapping
                            {
                                Type = "int|Integer",
                                NameContains = string.Empty,
                                Output = "<Slider Minimum=\"0\" Maximum=\"100\" x:Name=\"$name$\" Value=\"{x:Bind $name$, Mode=TwoWay}\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "List<string>",
                                NameContains = string.Empty,
                                Output = "<ItemsControl ItemsSource=\"{x:Bind $name$}\"></ItemsControl>",
                                IfReadOnly = false,
                            },
                        },
                    },

                    new Profile
                    {
                        Name = "UWP (with labels)",
                        ClassGrouping = "Grid-plus-RowDefs",
                        FallbackOutput = "<TextBlock Text=\"$name$\" Grid.Row=\"$incint$\"><TextBlock Text=\"FALLBACK_$name$\" Grid.Row=\"$incint$\" />",
                        SubPropertyOutput = "<TextBlock Text=\"$name$\" Grid.Row=\"$incint$\"><TextBlock Text=\"SUBPROP_$name$\" Grid.Row=\"$incint$\" />",
                        Mappings = new List<Mapping>
                        {
                            new Mapping
                            {
                                Type = "String",
                                NameContains = "password|pwd",
                                Output = "<TextBlock Text=\"$name$\" Grid.Row=\"$incint$\"><PasswordBox Password=\"{x:Bind $name$}\" Grid.Row=\"$incint$\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "String",
                                NameContains = string.Empty,
                                Output = "<TextBlock Text=\"$name$\" Grid.Row=\"$incint$\"><TextBox Text=\"{x:Bind $name$, Mode=TwoWay}\" Grid.Row=\"$incint$\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "String",
                                NameContains = string.Empty,
                                Output = "<TextBlock Text=\"$name$\" Grid.Row=\"$incint$\"><TextBlock Text=\"$name$\" Grid.Row=\"$incint$\" />",
                                IfReadOnly = true,
                            },
                            new Mapping
                            {
                                Type = "int|Integer",
                                NameContains = string.Empty,
                                Output = "<TextBlock Text=\"$name$\" Grid.Row=\"$incint$\"><Slider Minimum=\"0\" Maximum=\"100\" x:Name=\"$name$\" Value=\"{x:Bind $name$, Mode=TwoWay}\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "List<string>",
                                NameContains = string.Empty,
                                Output = "<TextBlock Text=\"$name$\" Grid.Row=\"$incint$\"><ItemsControl ItemsSource=\"{x:Bind $name$}\"></ItemsControl>",
                                IfReadOnly = false,
                            },
                        },
                    },

                    new Profile
                    {
                        Name = "WPF",
                        ClassGrouping = "StackPanel",
                        FallbackOutput = "<TextBlock Text=\"$name$\" Grid.Row=\"$incint$\"><TextBlock Text=\"FALLBACK_$name$\" Grid.Row=\"$incint$\" />",
                        SubPropertyOutput = "<TextBlock Text=\"$name$\" Grid.Row=\"$incint$\"><TextBlock Text=\"SUBPROP_$name$\" Grid.Row=\"$incint$\" />",
                        Mappings = new List<Mapping>
                        {
                            new Mapping
                            {
                                Type = "String",
                                NameContains = string.Empty,
                                Output = "<TextBlock Text=\"$name$\" Grid.Row=\"$incint$\"><TextBox Text=\"{x:Bind $name$, Mode=TwoWay}\" Grid.Row=\"$incint$\" />",
                                IfReadOnly = false,
                            },
                            new Mapping
                            {
                                Type = "String",
                                NameContains = string.Empty,
                                Output = "<TextBlock Text=\"$name$\" Grid.Row=\"$incint$\"><TextBlock Text=\"$name$\" Grid.Row=\"$incint$\" />",
                                IfReadOnly = true,
                            },
                        },
                    },

                    new Profile
                    {
                        Name = "Xamarin.Forms",
                        ClassGrouping = "StackLayout",
                        FallbackOutput = "<TextBlock Text=\"$name$\" Grid.Row=\"$incint$\"><TextBlock Text=\"FALLBACK_$name$\" Grid.Row=\"$incint$\" />",
                        SubPropertyOutput = "<TextBlock Text=\"$name$\" Grid.Row=\"$incint$\"><TextBlock Text=\"SUBPROP_$name$\" Grid.Row=\"$incint$\" />",
                        Mappings = new List<Mapping>
                        {
                            new Mapping
                            {
                                Type = "String",
                                NameContains = "password|pwd",
                                Output = "<TextBlock Text=\"$name$\" Grid.Row=\"$incint$\"><PasswordBox Password=\"{x:Bind $name$}\" Grid.Row=\"$incint$\" />",
                                IfReadOnly = false,
                            },
                        },
                    },
                },
            };
        }

        protected void EachPositionBetweenStarsShouldProduceExpected(string code, AnalyzerOutput expected, bool isCSharp, Profile profileOverload)
        {
            var startPos = code.IndexOf("*", StringComparison.Ordinal);
            var endPos = code.LastIndexOf("*", StringComparison.Ordinal) - 1;

            var syntaxTree = isCSharp ? CSharpSyntaxTree.ParseText(code.Replace("*", string.Empty))
                                      : VisualBasicSyntaxTree.ParseText(code.Replace("*", string.Empty));

            Assert.IsNotNull(syntaxTree);

            var semModel = isCSharp ? CSharpCompilation.Create(string.Empty).AddSyntaxTrees(syntaxTree).GetSemanticModel(syntaxTree, ignoreAccessibility: true)
                                    : VisualBasicCompilation.Create(string.Empty).AddSyntaxTrees(syntaxTree).GetSemanticModel(syntaxTree, ignoreAccessibility: true);

            var positionsTested = 0;

            for (var pos = startPos; pos < endPos; pos++)
            {
                IDocumentAnalyzer analyzer = isCSharp ? new CSharpAnalyzer(DefaultTestLogger.Create()) as IDocumentAnalyzer : new VisualBasicAnalyzer(DefaultTestLogger.Create());

                var actual = analyzer.GetSingleItemOutput(syntaxTree.GetRoot(), semModel, pos, profileOverload);

                Assert.AreEqual(expected.OutputType, actual.OutputType, $"Failure at {pos} ({startPos}-{endPos})");
                Assert.AreEqual(expected.Name, actual.Name, $"Failure at {pos} ({startPos}-{endPos})");
                Assert.AreEqual(expected.Output, actual.Output, $"Failure at {pos} ({startPos}-{endPos})");

                positionsTested += 1;
            }

            this.TestContext.WriteLine($"{positionsTested} different positions tested.");
        }

        protected void PositionAtStarShouldProduceExpected(string code, AnalyzerOutput expected, bool isCSharp, Profile profileOverload)
        {
            var pos = code.IndexOf("*", StringComparison.Ordinal);

            var syntaxTree = isCSharp ? CSharpSyntaxTree.ParseText(code.Replace("*", string.Empty))
                                      : VisualBasicSyntaxTree.ParseText(code.Replace("*", string.Empty));

            Assert.IsNotNull(syntaxTree);

            var semModel = isCSharp ? CSharpCompilation.Create(string.Empty).AddSyntaxTrees(syntaxTree).GetSemanticModel(syntaxTree, true)
                                    : VisualBasicCompilation.Create(string.Empty).AddSyntaxTrees(syntaxTree).GetSemanticModel(syntaxTree, true);

            IDocumentAnalyzer analyzer = isCSharp ? new CSharpAnalyzer(DefaultTestLogger.Create()) as IDocumentAnalyzer : new VisualBasicAnalyzer(DefaultTestLogger.Create());

            var actual = analyzer.GetSingleItemOutput(syntaxTree.GetRoot(), semModel, pos, profileOverload);

            Assert.AreEqual(expected.OutputType, actual.OutputType);
            Assert.AreEqual(expected.Name, actual.Name);
            Assert.AreEqual(expected.Output, actual.Output);
        }

        protected void PositionAtStarShouldProduceExpectedUsingAdditonalFiles(string code, AnalyzerOutput expected, bool isCSharp, Profile profileOverload, params string[] additionalCode)
        {
            var pos = code.IndexOf("*", StringComparison.Ordinal);

            SyntaxTree syntaxTree = null;
            SemanticModel semModel = null;

            var projectId = ProjectId.CreateNewId();
            var documentId = DocumentId.CreateNewId(projectId);

            string language = string.Empty;
            string fileExt = string.Empty;

            if (isCSharp)
            {
                language = LanguageNames.CSharp;
                fileExt = "cs";
            }
            else
            {
                language = LanguageNames.VisualBasic;
                fileExt = "vb";
            }

            var solution = new AdhocWorkspace().CurrentSolution
                                               .AddProject(projectId, "MyProject", "MyProject", language)
                                               .AddDocument(documentId, $"MyFile.{fileExt}", code.Replace("*", string.Empty));

            foreach (var addCode in additionalCode)
            {
                solution = solution.AddDocument(DocumentId.CreateNewId(projectId), $"{System.IO.Path.GetRandomFileName()}.{fileExt}", addCode);
            }

            var document = solution.GetDocument(documentId);

            semModel = document.GetSemanticModelAsync().Result;
            syntaxTree = document.GetSyntaxTreeAsync().Result;

            IDocumentAnalyzer analyzer = isCSharp ? new CSharpAnalyzer(DefaultTestLogger.Create()) as IDocumentAnalyzer : new VisualBasicAnalyzer(DefaultTestLogger.Create());

            var actual = analyzer.GetSingleItemOutput(syntaxTree.GetRoot(), semModel, pos, profileOverload);

            Assert.AreEqual(expected.OutputType, actual.OutputType);
            Assert.AreEqual(expected.Name, actual.Name);
            Assert.AreEqual(expected.Output, actual.Output);
        }

        protected void PositionAtStarShouldProduceExpectedUsingAdditonalReferences(string code, AnalyzerOutput expected, bool isCSharp, Profile profileOverload, params string[] additionalReferences)
        {
            var pos = code.IndexOf("*", StringComparison.Ordinal);

            SyntaxTree syntaxTree = null;
            SemanticModel semModel = null;

            var projectId = ProjectId.CreateNewId();
            var documentId = DocumentId.CreateNewId(projectId);

            string language = string.Empty;
            string fileExt = string.Empty;

            if (isCSharp)
            {
                language = LanguageNames.CSharp;
                fileExt = "cs";
            }
            else
            {
                language = LanguageNames.VisualBasic;
                fileExt = "vb";
            }

            var solution = new AdhocWorkspace().CurrentSolution
                                               .AddProject(projectId, "MyProject", "MyProject", language)
                                               .AddDocument(documentId, $"MyFile.{fileExt}", code.Replace("*", string.Empty));

            foreach (var addRef in additionalReferences)
            {
                var lib = MetadataReference.CreateFromFile(Type.GetType(addRef).Assembly.Location);

                solution = solution.AddMetadataReference(projectId, lib);
            }

            var document = solution.GetDocument(documentId);

            semModel = document.GetSemanticModelAsync().Result;
            syntaxTree = document.GetSyntaxTreeAsync().Result;

            IDocumentAnalyzer analyzer = isCSharp ? new CSharpAnalyzer(DefaultTestLogger.Create()) as IDocumentAnalyzer : new VisualBasicAnalyzer(DefaultTestLogger.Create());

            var actual = analyzer.GetSingleItemOutput(syntaxTree.GetRoot(), semModel, pos, profileOverload);

            Assert.AreEqual(expected.OutputType, actual.OutputType);
            Assert.AreEqual(expected.Name, actual.Name);
            Assert.AreEqual(expected.Output, actual.Output);
        }

        protected void PositionAtStarShouldProduceExpectedUsingAdditonalLibraries(string code, AnalyzerOutput expected, bool isCSharp, Profile profileOverload, params string[] additionalLibraryPaths)
        {
            var pos = code.IndexOf("*", StringComparison.Ordinal);

            SyntaxTree syntaxTree = null;
            SemanticModel semModel = null;

            var projectId = ProjectId.CreateNewId();
            var documentId = DocumentId.CreateNewId(projectId);

            string language = string.Empty;
            string fileExt = string.Empty;

            if (isCSharp)
            {
                language = LanguageNames.CSharp;
                fileExt = "cs";
            }
            else
            {
                language = LanguageNames.VisualBasic;
                fileExt = "vb";
            }

            var solution = new AdhocWorkspace().CurrentSolution
                                               .AddProject(projectId, "MyProject", "MyProject", language)
                                               .AddDocument(documentId, $"MyFile.{fileExt}", code.Replace("*", string.Empty));

            foreach (var libPath in additionalLibraryPaths)
            {
                var lib = MetadataReference.CreateFromFile(libPath);

                solution = solution.AddMetadataReference(projectId, lib);
            }

            var document = solution.GetDocument(documentId);

            semModel = document.GetSemanticModelAsync().Result;
            syntaxTree = document.GetSyntaxTreeAsync().Result;

            IDocumentAnalyzer analyzer = isCSharp ? new CSharpAnalyzer(DefaultTestLogger.Create()) as IDocumentAnalyzer : new VisualBasicAnalyzer(DefaultTestLogger.Create());

            var actual = analyzer.GetSingleItemOutput(syntaxTree.GetRoot(), semModel, pos, profileOverload);

            Assert.AreEqual(expected.OutputType, actual.OutputType);
            Assert.AreEqual(expected.Name, actual.Name);
            Assert.AreEqual(expected.Output, actual.Output);
        }

        protected void SelectionBetweenStarsShouldProduceExpected(string code, AnalyzerOutput expected, bool isCSharp, Profile profileOverload)
        {
            var startPos = code.IndexOf("*", StringComparison.Ordinal);
            var endPos = code.LastIndexOf("*", StringComparison.Ordinal) - 1;

            var syntaxTree = isCSharp ? CSharpSyntaxTree.ParseText(code.Replace("*", string.Empty))
                : VisualBasicSyntaxTree.ParseText(code.Replace("*", string.Empty));

            Assert.IsNotNull(syntaxTree);

            var semModel = isCSharp ? CSharpCompilation.Create(string.Empty).AddSyntaxTrees(syntaxTree).GetSemanticModel(syntaxTree, true)
                : VisualBasicCompilation.Create(string.Empty).AddSyntaxTrees(syntaxTree).GetSemanticModel(syntaxTree, true);

            IDocumentAnalyzer analyzer = isCSharp ? new CSharpAnalyzer(DefaultTestLogger.Create()) as IDocumentAnalyzer : new VisualBasicAnalyzer(DefaultTestLogger.Create());

            var actual = analyzer.GetSelectionOutput(syntaxTree.GetRoot(), semModel, startPos, endPos, profileOverload);

            Assert.AreEqual(expected.OutputType, actual.OutputType);
            Assert.AreEqual(expected.Name, actual.Name);
            Assert.AreEqual(expected.Output, actual.Output);
        }
    }
}
