// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidXamlToolkit.Options;
using RapidXamlToolkit.Parsers;

namespace RapidXamlToolkit.Tests.Parsers
{
    [TestClass]
    public class GetVisualBasicNullableTests : VisualBasicTestsBase
    {
        private Profile NullableTestsProfile
        {
            get
            {
                var profile = TestProfile.CreateEmpty();
                profile.Mappings.Add(new Mapping
                {
                    Type = "Boolean",
                    IfReadOnly = false,
                    NameContains = string.Empty,
                    Output = "<Bool />",
                });
                profile.Mappings.Add(new Mapping
                {
                    Type = "Boolean?",
                    IfReadOnly = false,
                    NameContains = string.Empty,
                    Output = "<BoolQ />",
                });
                profile.Mappings.Add(new Mapping
                {
                    Type = "Nullable(Of Boolean)",
                    IfReadOnly = false,
                    NameContains = string.Empty,
                    Output = "<NullBool />",
                });

                // This should never match as the one above will be found first
                profile.Mappings.Add(new Mapping
                {
                    Type = "System.Nullable(Of Boolean)",
                    IfReadOnly = false,
                    NameContains = string.Empty,
                    Output = "<SysNullBool />",
                });

                return profile;
            }
        }

        [TestMethod]
        public void GetNullablePropertiesInClass()
        {
            var code = @"
Namespace tests
    Class Cla☆ss1
        Public Property MyBool As Boolean
        Public Property MyBoolQ As Boolean? 
        Public Property MyNullableBool As Nullable(Of Boolean)
        Public Property MyFqNullableBool As System.Nullable(Of Boolean)
    End Class
End Namespace";

            var expectedXaml = "<Bool />"
       + Environment.NewLine + "<BoolQ />"
       + Environment.NewLine + "<NullBool />"
       + Environment.NewLine + "<NullBool />";

            var expected = new ParserOutput
            {
                Name = "Class1",
                Output = expectedXaml,
                OutputType = ParserOutputType.Class,
            };

            this.PositionAtStarShouldProduceExpected(code, expected, this.NullableTestsProfile);
        }

        [TestMethod]
        public void GetNullablePropertyShorthand()
        {
            var code = @"
Namespace tests
    Class Class1
        Public Property MyBo☆olQ As Boolean? 
    End Class
End Namespace";

            var expected = new ParserOutput
            {
                Name = "MyBoolQ",
                Output = "<BoolQ />",
                OutputType = ParserOutputType.Member,
            };

            this.PositionAtStarShouldProduceExpected(code, expected, this.NullableTestsProfile);
        }

        [TestMethod]
        public void GetNullablePropertyIndicatedOnPropertyName()
        {
            var code = @"
Namespace tests
    Class Class1
        Public Property MyBo☆olQ? As Boolean
    End Class
End Namespace";

            var expected = new ParserOutput
            {
                Name = "MyBoolQ",
                Output = "<BoolQ />",
                OutputType = ParserOutputType.Member,
            };

            this.PositionAtStarShouldProduceExpected(code, expected, this.NullableTestsProfile);
        }

        [TestMethod]
        public void GetNullablePropertyLonghand()
        {
            var code = @"
Namespace tests
    Class Class1
        Public Property MyNullable☆Bool As Nullable(Of Boolean)
    End Class
End Namespace";

            var expected = new ParserOutput
            {
                Name = "MyNullableBool",
                Output = "<NullBool />",
                OutputType = ParserOutputType.Member,
            };

            this.PositionAtStarShouldProduceExpected(code, expected, this.NullableTestsProfile);
        }

        [TestMethod]
        public void GetNullablePropertyFullyQualified()
        {
            var code = @"
Namespace tests
    Class Class1
        Public Property MyFqNull☆ableBool As System.Nullable(Of Boolean)
    End Class
End Namespace";

            var expected = new ParserOutput
            {
                Name = "MyFqNullableBool",
                Output = "<NullBool />",
                OutputType = ParserOutputType.Member,
            };

            this.PositionAtStarShouldProduceExpected(code, expected, this.NullableTestsProfile);
        }

        [TestMethod]
        public void GetListOfNullableProperty()
        {
            var code = @"
Namespace tests
    Class Class1
        Public Property MyListOf☆Nullables As List(Of Boolean?)
    End Class
End Namespace";

            var profile = this.NullableTestsProfile;
            profile.Mappings.Add(new Mapping
            {
                Type = "List(Of Boolean?)",
                NameContains = string.Empty,
                IfReadOnly = false,
                Output = "<LBnull />",
            });

            var expected = new ParserOutput
            {
                Name = "MyListOfNullables",
                Output = "<LBnull />",
                OutputType = ParserOutputType.Member,
            };

            this.PositionAtStarShouldProduceExpected(code, expected, profile);
        }

        [TestMethod]
        public void GetNullablePropertiesInSelection()
        {
            var code = @"
Namespace tests
    Class Class1
        ☆Public Property MyBool As Boolean
        Public Property MyBoolQ As Boolean? 
        Public Property MyNullableBool As Nullable(Of Boolean)
        Public Property MyFqNullableBool As System.Nullable(Of Boolean)☆
    End Class
End Namespace";

            var expectedXaml = "<Bool />"
       + Environment.NewLine + "<BoolQ />"
       + Environment.NewLine + "<NullBool />"
       + Environment.NewLine + "<NullBool />";

            var expected = new ParserOutput
            {
                Name = "MyBool, MyBoolQ and 2 other properties",
                Output = expectedXaml,
                OutputType = ParserOutputType.Selection,
            };

            this.SelectionBetweenStarsShouldProduceExpected(code, expected, this.NullableTestsProfile);
        }
    }
}
