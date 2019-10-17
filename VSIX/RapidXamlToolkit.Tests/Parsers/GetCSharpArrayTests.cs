// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidXamlToolkit.Options;
using RapidXamlToolkit.Parsers;

namespace RapidXamlToolkit.Tests.Parsers
{
    [TestClass]
    public class GetCSharpArrayTests : CSharpTestsBase
    {
        private Profile ArrayTestsProfile
        {
            get
            {
                var profile = TestProfile.CreateEmpty();
                profile.Mappings.Add(new Mapping
                {
                    Type = "bool",
                    IfReadOnly = false,
                    NameContains = string.Empty,
                    Output = "<Bool />",
                });
                profile.Mappings.Add(new Mapping
                {
                    Type = "bool[]",
                    IfReadOnly = false,
                    NameContains = string.Empty,
                    Output = "<BoolBrackets />",
                });
                profile.Mappings.Add(new Mapping
                {
                    Type = "Array<bool>",
                    IfReadOnly = false,
                    NameContains = string.Empty,
                    Output = "<ArrayBool />",
                });

                return profile;
            }
        }

        [TestMethod]
        public void GetArrayPropertiesInClass()
        {
            var code = @"
using System;

namespace tests
{
    class Cla☆ss1
    {
        public bool MyBool { get; set; }
        public bool[] MyBoolBrackets { get; set; }
        public Array<bool> MyArrayBool { get; set; }
    }
}";

            var expectedXaml = "<Bool />"
       + Environment.NewLine + "<BoolBrackets />"
       + Environment.NewLine + "<ArrayBool />";

            var expected = new ParserOutput
            {
                Name = "Class1",
                Output = expectedXaml,
                OutputType = ParserOutputType.Class,
            };

            this.PositionAtStarShouldProduceExpected(code, expected, this.ArrayTestsProfile);
        }

        [TestMethod]
        public void GetArrayPropertyBrackets()
        {
            var code = @"
namespace tests
{
    class Class1
    {
        public bool[] MyBoo☆lBrackets { get; set; }
    }
}";

            var expected = new ParserOutput
            {
                Name = "MyBoolBrackets",
                Output = "<BoolBrackets />",
                OutputType = ParserOutputType.Member,
            };

            this.PositionAtStarShouldProduceExpected(code, expected, this.ArrayTestsProfile);
        }

        [TestMethod]
        public void GetArrayPropertyLonghand()
        {
            var code = @"
using System;

namespace tests
{
    class Class1
    {
        public Array<bool> MyAr☆rayBool { get; set; }
    }
}";

            var expected = new ParserOutput
            {
                Name = "MyArrayBool",
                Output = "<ArrayBool />",
                OutputType = ParserOutputType.Member,
            };

            this.PositionAtStarShouldProduceExpected(code, expected, this.ArrayTestsProfile);
        }

        [TestMethod]
        public void GetArrayPropertiesInSelection()
        {
            var code = @"
using System;

namespace tests
{
    class Class1
    {
       ☆ public bool MyBool { get; set; }
        public bool[] MyBoolBrackets { get; set; }
        public Array<bool> MyArrayBool { get; set; }☆
    }
}";

            var expectedXaml = "<Bool />"
       + Environment.NewLine + "<BoolBrackets />"
       + Environment.NewLine + "<ArrayBool />";

            var expected = new ParserOutput
            {
                Name = "MyBool, MyBoolBrackets and 1 other member",
                Output = expectedXaml,
                OutputType = ParserOutputType.Selection,
            };

            this.SelectionBetweenStarsShouldProduceExpected(code, expected, this.ArrayTestsProfile);
        }
    }
}
