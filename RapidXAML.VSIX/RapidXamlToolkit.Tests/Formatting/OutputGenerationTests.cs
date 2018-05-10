// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidXamlToolkit.Analyzers;
using RapidXamlToolkit.Options;

namespace RapidXamlToolkit.Tests.Formatting
{
    [TestClass]
    public class OutputGenerationTests
    {
        private const string FallbackOutput = "<TextBlock Fallback=\"True\" />";
        private const string SubPropertyOutput = "<TextBlock Subproperty=\"True\" />";
        private const string ReadonlyStringOutput = "<TextBlock Text=\"$name$\" />";
        private const string ReadWriteStringOutput = "<TextBox Text=\"{x:Bind $name$, Mode=TwoWay}\" />";
        private const string ReadWritePasswordStringOutput = "<PasswordBox Password=\"{x:Bind $name$}\" />";
        private const string ReadWriteNumberIntOutput = "<ReadWriteNumberIntOutput />";
        private const string ReadWriteIntOutput = "<ReadWriteIntOutput />";

        private const string StringPropertyName = "String";

        private readonly Profile testProfile;

        public OutputGenerationTests()
        {
            this.testProfile = new Profile
            {
                Name = "TestProfile",
                ClassGrouping = "StackPanel",
                FallbackOutput = FallbackOutput,
                SubPropertyOutput = SubPropertyOutput,
                Mappings = new ObservableCollection<Mapping>
                {
                    new Mapping
                    {
                        Type = StringPropertyName,
                        NameContains = "password|pwd",
                        Output = ReadWritePasswordStringOutput,
                        IfReadOnly = false,
                    },
                    new Mapping
                    {
                        Type = StringPropertyName,
                        NameContains = "",
                        Output = ReadWriteStringOutput,
                        IfReadOnly = false,
                    },
                    new Mapping
                    {
                        Type = StringPropertyName,
                        NameContains = "",
                        Output = ReadonlyStringOutput,
                        IfReadOnly = true,
                    },
                    new Mapping
                    {
                        Type = "int",
                        NameContains = "number",
                        Output = ReadWriteNumberIntOutput,
                        IfReadOnly = false,
                    },
                    new Mapping
                    {
                        Type = "int",
                        NameContains = "",
                        Output = ReadWriteIntOutput,
                        IfReadOnly = false,
                    },
                },
            };
        }

        [TestMethod]
        public void GetsNonFilteredOutputIfNameDoesntMatch()
        {
            var result = AnalyzerBase.GetPropertyOutput(this.testProfile, "string", "MyProperty", false);

            Assert.AreEqual(ReadWriteStringOutput.Replace("$name$", "MyProperty"), result);
        }

        [TestMethod]
        public void ReadWritePropertyIsIdentified()
        {
            var result = AnalyzerBase.GetPropertyOutput(this.testProfile, "string", "AnotherProperty", true);

            Assert.AreEqual(ReadonlyStringOutput.Replace("$name$", "AnotherProperty"), result);
        }

        [TestMethod]
        public void SingleNameContainsIsIdentified()
        {
            var result = AnalyzerBase.GetPropertyOutput(this.testProfile, "int", "number1", false);

            Assert.AreEqual(ReadWriteNumberIntOutput, result);
        }

        [TestMethod]
        public void MultipleNameContainsIsIdentified()
        {
            var result = AnalyzerBase.GetPropertyOutput(this.testProfile, "string", "EnteredPassword", false);

            Assert.AreEqual(ReadWritePasswordStringOutput.Replace("$name$", "EnteredPassword"), result);
        }

        [TestMethod]
        public void ReadOnlyTakesPriorityOverContains()
        {
            var result = AnalyzerBase.GetPropertyOutput(this.testProfile, "string", "EnteredPwd", true);

            Assert.AreEqual(ReadonlyStringOutput.Replace("$name$", "EnteredPwd"), result);
        }

        [TestMethod]
        public void TypeNameIsCaseInsensitive()
        {
            var result = AnalyzerBase.GetPropertyOutput(this.testProfile, "INT", "number1", false);

            Assert.AreEqual(ReadWriteNumberIntOutput, result);
        }

        [TestMethod]
        public void NameContainsIsCaseInsensitive()
        {
            var result = AnalyzerBase.GetPropertyOutput(this.testProfile, "int", "Number1", false);

            Assert.AreEqual(ReadWriteNumberIntOutput, result);
        }

        [TestMethod]
        public void NoNameContainsIsIdentified()
        {
            var result = AnalyzerBase.GetPropertyOutput(this.testProfile, "int", "numero2", false);

            Assert.AreEqual(ReadWriteIntOutput, result);
        }

        [TestMethod]
        public void GetFallbackIfTypeNotMapped()
        {
            var result = AnalyzerBase.GetPropertyOutput(this.testProfile, "bool", "IsAdmin", false);

            Assert.AreEqual(FallbackOutput, result);
        }

        [TestMethod]
        public void CanHandleMultipleNumberReplacements()
        {
            var gridProfile = new Profile
            {
                Name = "GridTestProfile",
                ClassGrouping = "Grid",
                FallbackOutput = "<TextBlock Text=\"FALLBACK_$name$\" />",
                SubPropertyOutput = "<TextBlock Text=\"SUBPROP_$name$\" />",
                Mappings = new ObservableCollection<Mapping>
                {
                    new Mapping
                    {
                        Type = StringPropertyName,
                        NameContains = "",
                        Output = "<TextBlock Text=\"$name$\" Grid.Row=\"$incint$\"><TextBlock Text=\"$name$\" Grid.Row=\"$incint$\" />",
                        IfReadOnly = false,
                    },
                },
            };

            var result = AnalyzerBase.GetPropertyOutput(gridProfile, StringPropertyName, "MyProperty", false);

            Assert.AreEqual("<TextBlock Text=\"MyProperty\" Grid.Row=\"1\"><TextBlock Text=\"MyProperty\" Grid.Row=\"2\" />", result);
        }

        [TestMethod]
        public void ReadOnlyPropertiesMatchNotReadOnlyRatherThanFallback()
        {
            var readonlyProfile = new Profile
            {
                Name = "readonlyProfile",
                ClassGrouping = "Grid",
                FallbackOutput = "<Fallback />",
                Mappings = new ObservableCollection<Mapping>
                {
                    new Mapping
                    {
                        Type = StringPropertyName,
                        NameContains = "",
                        Output = "<ReadAndWrite />",
                        IfReadOnly = false,
                    },
                },
            };

            var result = AnalyzerBase.GetPropertyOutput(readonlyProfile, StringPropertyName, "MyProperty", isReadOnly: true);

            Assert.AreEqual("<ReadAndWrite />", result);
        }

        [TestMethod]
        public void WriteablePropertiesMatchFallbackIfOnlySpecificReadOnlyDefined()
        {
            var readonlyProfile = new Profile
            {
                Name = "readonlyProfile",
                ClassGrouping = "Grid",
                FallbackOutput = "<Fallback />",
                Mappings = new ObservableCollection<Mapping>
                {
                    new Mapping
                    {
                        Type = StringPropertyName,
                        NameContains = "",
                        Output = "<ReadOnly />",
                        IfReadOnly = true,
                    },
                },
            };

            var result = AnalyzerBase.GetPropertyOutput(readonlyProfile, StringPropertyName, "MyProperty", isReadOnly: false);

            Assert.AreEqual("<Fallback />", result);
        }

        [TestMethod]
        public void GetSelectionPropertiesName_Null()
        {
            var result = AnalyzerBase.GetSelectionPropertiesName(null);

            Assert.IsTrue(string.IsNullOrEmpty(result));
        }

        [TestMethod]
        public void GetSelectionPropertiesName_Empty()
        {
            var result = AnalyzerBase.GetSelectionPropertiesName(new List<string>());

            Assert.IsTrue(string.IsNullOrEmpty(result));
        }

        [TestMethod]
        public void GetSelectionPropertiesName_One()
        {
            var result = AnalyzerBase.GetSelectionPropertiesName(new List<string> { "one" });

            Assert.IsTrue(result.Equals("one"));
        }

        [TestMethod]
        public void GetSelectionPropertiesName_Two()
        {
            var result = AnalyzerBase.GetSelectionPropertiesName(new List<string> { "one", "two" });

            Assert.IsTrue(result.Equals("one and two"));
        }

        [TestMethod]
        public void GetSelectionPropertiesName_Three()
        {
            var result = AnalyzerBase.GetSelectionPropertiesName(new List<string> { "one", "two", "three" });

            Assert.IsTrue(result.Equals("one, two and 1 other property"));
        }

        [TestMethod]
        public void GetSelectionPropertiesName_Four()
        {
            var result = AnalyzerBase.GetSelectionPropertiesName(new List<string> { "one", "two", "three", "four" });

            Assert.IsTrue(result.Equals("one, two and 2 other properties"));
        }

        [TestMethod]
        public void CanMatchWildCardGenerics()
        {
            var wildcardGenericsProfile = new Profile
            {
                Name = "wildcardGenericsProfile",
                ClassGrouping = "Grid",
                FallbackOutput = "<Fallback />",
                Mappings = new ObservableCollection<Mapping>
                {
                    new Mapping
                    {
                        Type = "List<T>",
                        NameContains = "",
                        Output = "<Wildcard />",
                        IfReadOnly = false,
                    },
                },
            };

            var result = AnalyzerBase.GetPropertyOutput(wildcardGenericsProfile, "List<string>", "MyProperty", isReadOnly: false);

            Assert.AreEqual("<Wildcard />", result);
        }

        [TestMethod]
        public void SpecificGenericsMatchBeforeWildCard()
        {
            var wildcardGenericsProfile = new Profile
            {
                Name = "wildcardGenericsProfile",
                ClassGrouping = "Grid",
                FallbackOutput = "<Fallback />",
                Mappings = new ObservableCollection<Mapping>
                {
                    new Mapping
                    {
                        Type = "List<T>",
                        NameContains = "",
                        Output = "<Wildcard />",
                        IfReadOnly = false,
                    },
                    new Mapping
                    {
                        Type = "List<string>",
                        NameContains = "",
                        Output = "<ListOfStrings />",
                        IfReadOnly = false,
                    },
                },
            };

            var result = AnalyzerBase.GetPropertyOutput(wildcardGenericsProfile, "List<string>", "MyProperty", isReadOnly: false);

            Assert.AreEqual("<ListOfStrings />", result);
        }

        [TestMethod]
        public void CorrectlySplitCamelCasePropertyNames()
        {
            var profile = TestProfile.CreateEmpty();
            profile.Mappings.Add(new Mapping
            {
                Type = "string",
                IfReadOnly = false,
                NameContains = string.Empty,
                Output = "$namewithspaces$",
            });

            var result = AnalyzerBase.GetPropertyOutput(profile, "string", "MyProperty", isReadOnly: false);

            Assert.AreEqual("My Property", result);
        }
    }
}
