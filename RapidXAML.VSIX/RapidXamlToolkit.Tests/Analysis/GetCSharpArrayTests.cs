// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidXamlToolkit.Analyzers;
using RapidXamlToolkit.Options;

namespace RapidXamlToolkit.Tests.Analysis
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

            var expected = new AnalyzerOutput
            {
                Name = "Class1",
                Output = @"<Bool />
<BoolBrackets />
<ArrayBool />",
                OutputType = AnalyzerOutputType.Class,
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

            var expected = new AnalyzerOutput
            {
                Name = "MyBoolBrackets",
                Output = @"<BoolBrackets />",
                OutputType = AnalyzerOutputType.Property,
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

            var expected = new AnalyzerOutput
            {
                Name = "MyArrayBool",
                Output = @"<ArrayBool />",
                OutputType = AnalyzerOutputType.Property,
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

            var expected = new AnalyzerOutput
            {
                Name = "MyBool, MyBoolBrackets and 1 other property",
                Output = @"<Bool />
<BoolBrackets />
<ArrayBool />",
                OutputType = AnalyzerOutputType.Selection,
            };

            this.SelectionBetweenStarsShouldProduceExpected(code, expected, this.ArrayTestsProfile);
        }
    }
}
