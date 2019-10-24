// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidXamlToolkit.Options;
using RapidXamlToolkit.Parsers;

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
            var cpb = new CodeParserBase(DefaultTestLogger.Create(), this.testProfile.ProjectType, 4, this.testProfile);
            var result = cpb.GetPropertyOutput("string", "MyProperty", isReadOnly: false);

            Assert.AreEqual(ReadWriteStringOutput.Replace("$name$", "MyProperty"), result);
        }

        [TestMethod]
        public void ReadWritePropertyIsIdentified()
        {
            var cpb = new CodeParserBase(DefaultTestLogger.Create(), this.testProfile.ProjectType, 4, this.testProfile);
            var result = cpb.GetPropertyOutput("string", "AnotherProperty", isReadOnly: true);

            Assert.AreEqual(ReadonlyStringOutput.Replace("$name$", "AnotherProperty"), result);
        }

        [TestMethod]
        public void SingleNameContainsIsIdentified()
        {
            var cpb = new CodeParserBase(DefaultTestLogger.Create(), this.testProfile.ProjectType, 4, this.testProfile);
            var result = cpb.GetPropertyOutput("int", "number1", isReadOnly: false);

            Assert.AreEqual(ReadWriteNumberIntOutput, result);
        }

        [TestMethod]
        public void MultipleNameContainsIsIdentified()
        {
            var cpb = new CodeParserBase(DefaultTestLogger.Create(), this.testProfile.ProjectType, 4, this.testProfile);
            var result = cpb.GetPropertyOutput("string", "EnteredPassword", isReadOnly: false);

            Assert.AreEqual(ReadWritePasswordStringOutput.Replace("$name$", "EnteredPassword"), result);
        }

        [TestMethod]
        public void ReadOnlyTakesPriorityOverContains()
        {
            var cpb = new CodeParserBase(DefaultTestLogger.Create(), this.testProfile.ProjectType, 4, this.testProfile);
            var result = cpb.GetPropertyOutput("string", "EnteredPwd", isReadOnly: true);

            Assert.AreEqual(ReadonlyStringOutput.Replace("$name$", "EnteredPwd"), result);
        }

        [TestMethod]
        public void TypeNameIsCaseInsensitive()
        {
            var cpb = new CodeParserBase(DefaultTestLogger.Create(), this.testProfile.ProjectType, 4, this.testProfile);
            var result = cpb.GetPropertyOutput("INT", "number1", isReadOnly: false);

            Assert.AreEqual(ReadWriteNumberIntOutput, result);
        }

        [TestMethod]
        public void NameContainsIsCaseInsensitive()
        {
            var cpb = new CodeParserBase(DefaultTestLogger.Create(), this.testProfile.ProjectType, 4, this.testProfile);
            var result = cpb.GetPropertyOutput("int", "Number1", isReadOnly: false);

            Assert.AreEqual(ReadWriteNumberIntOutput, result);
        }

        [TestMethod]
        public void NoNameContainsIsIdentified()
        {
            var cpb = new CodeParserBase(DefaultTestLogger.Create(), this.testProfile.ProjectType, 4, this.testProfile);
            var result = cpb.GetPropertyOutput("int", "numero2", isReadOnly: false);

            Assert.AreEqual(ReadWriteIntOutput, result);
        }

        [TestMethod]
        public void GetFallbackIfTypeNotMapped()
        {
            var cpb = new CodeParserBase(DefaultTestLogger.Create(), this.testProfile.ProjectType, 4, this.testProfile);
            var result = cpb.GetPropertyOutput("bool", "IsAdmin", isReadOnly: false);

            Assert.AreEqual(FallbackOutput, result);
        }

        [TestMethod]
        public void CanHandleMultipleNumberReplacements()
        {
            var gridProfile = new Profile
            {
                Name = "GridTestProfile",
                ProjectType = ProjectType.Uwp,
                ClassGrouping = "Grid",
                FallbackOutput = "<TextBlock Text=\"FALLBACK_$name$\" />",
                SubPropertyOutput = "<TextBlock Text=\"SUBPROP_$name$\" />",
                Mappings = new ObservableCollection<Mapping>
                {
                    new Mapping
                    {
                        Type = StringPropertyName,
                        NameContains = "",
                        Output = "<TextBlock Text=\"$name$\" Grid.Row=\"$incint$\" /><TextBlock Text=\"$name$\" Grid.Row=\"$incint$\" />",
                        IfReadOnly = false,
                    },
                },
            };

            var cpb = new CodeParserBase(DefaultTestLogger.Create(), gridProfile.ProjectType, 4, gridProfile);
            var result = cpb.GetPropertyOutput(StringPropertyName, "MyProperty", false);

            var expected = "<TextBlock Text=\"MyProperty\" Grid.Row=\"1\" />" + Environment.NewLine +
                           "<TextBlock Text=\"MyProperty\" Grid.Row=\"2\" />";

            StringAssert.AreEqual(expected, result);
        }

        [TestMethod]
        public void ReadOnlyPropertiesMatchNotReadOnlyRatherThanFallback()
        {
            var readonlyProfile = new Profile
            {
                Name = "readonlyProfile",
                ProjectType = ProjectType.Uwp,
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

            var cpb = new CodeParserBase(DefaultTestLogger.Create(), readonlyProfile.ProjectType, 4, readonlyProfile);
            var result = cpb.GetPropertyOutput(StringPropertyName, "MyProperty", isReadOnly: true);

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

            var cpb = new CodeParserBase(DefaultTestLogger.Create(), readonlyProfile.ProjectType, 4, readonlyProfile);
            var result = cpb.GetPropertyOutput(StringPropertyName, "MyProperty", isReadOnly: false);

            Assert.AreEqual("<Fallback />", result);
        }

        [TestMethod]
        public void GetSelectionPropertiesName_Null()
        {
            var result = CodeParserBase.GetSelectionMemberName(null);

            Assert.IsTrue(string.IsNullOrEmpty(result));
        }

        [TestMethod]
        public void GetSelectionPropertiesName_Empty()
        {
            var result = CodeParserBase.GetSelectionMemberName(new List<string>());

            Assert.IsTrue(string.IsNullOrEmpty(result));
        }

        [TestMethod]
        public void GetSelectionPropertiesName_One()
        {
            var result = CodeParserBase.GetSelectionMemberName(new List<string> { "one" });

            Assert.IsTrue(result.Equals("one"));
        }

        [TestMethod]
        public void GetSelectionPropertiesName_Two()
        {
            var result = CodeParserBase.GetSelectionMemberName(new List<string> { "one", "two" });

            Assert.IsTrue(result.Equals("one and two"));
        }

        [TestMethod]
        public void GetSelectionPropertiesName_Three()
        {
            var result = CodeParserBase.GetSelectionMemberName(new List<string> { "one", "two", "three" });

            Assert.IsTrue(result.Equals("one, two and 1 other member"));
        }

        [TestMethod]
        public void GetSelectionPropertiesName_Four()
        {
            var result = CodeParserBase.GetSelectionMemberName(new List<string> { "one", "two", "three", "four" });

            Assert.IsTrue(result.Equals("one, two and 2 other members"));
        }

        [TestMethod]
        public void GetSelectionPropertiesName_Five()
        {
            var result = CodeParserBase.GetSelectionMemberName(new List<string> { "one", "two", "three", "four", "five" });

            Assert.IsTrue(result.Equals("one, two and 3 other members"));
        }

        [TestMethod]
        public void CanMatchWildCardGenerics()
        {
            var wildcardGenericsProfile = new Profile
            {
                Name = "wildcardGenericsProfile",
                ProjectType = ProjectType.Uwp,
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

            var cpb = new CodeParserBase(DefaultTestLogger.Create(), wildcardGenericsProfile.ProjectType, 4, wildcardGenericsProfile);
            var result = cpb.GetPropertyOutput("List<string>", "MyProperty", isReadOnly: false);

            Assert.AreEqual("<Wildcard />", result);
        }

        [TestMethod]
        public void SpecificGenericsMatchBeforeWildCard()
        {
            var wildcardGenericsProfile = new Profile
            {
                Name = "wildcardGenericsProfile",
                ProjectType = ProjectType.Uwp,
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

            var cpb = new CodeParserBase(DefaultTestLogger.Create(), wildcardGenericsProfile.ProjectType, 4, wildcardGenericsProfile);
            var result = cpb.GetPropertyOutput("List<string>", "MyProperty", isReadOnly: false);

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

            var cpb = new CodeParserBase(DefaultTestLogger.Create(), profile.ProjectType, 4, profile);
            var result = cpb.GetPropertyOutput("string", "MyProperty", isReadOnly: false);

            Assert.AreEqual("My Property", result);
        }
    }
}
