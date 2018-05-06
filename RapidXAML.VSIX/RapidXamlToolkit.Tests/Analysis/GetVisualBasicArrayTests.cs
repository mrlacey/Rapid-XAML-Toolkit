// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidXamlToolkit.Analyzers;
using RapidXamlToolkit.Options;

namespace RapidXamlToolkit.Tests.Analysis
{
    [TestClass]
    public class GetVisualBasicArrayTests : VisualBasicTestsBase
    {
        private Profile ArrayTestsProfile
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
                    Type = "Array",
                    IfReadOnly = false,
                    NameContains = string.Empty,
                    Output = "<Array />",
                });
                profile.Mappings.Add(new Mapping
                {
                    Type = "Boolean()",
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
Namespace tests
    Class Cla*ss1
        Public Property MyBool As Boolean 
        Public Property MyArray As Array
        Public Property MyArrayBool() As Boolean
    End Class
End Namespace";

            var expected = new AnalyzerOutput
            {
                Name = "Class1",
                Output = @"<Bool />
<Array />
<ArrayBool />
",
                OutputType = AnalyzerOutputType.Class,
            };

            this.PositionAtStarShouldProduceExpected(code, expected, this.ArrayTestsProfile);
        }

        [TestMethod]
        public void GetArrayProperty()
        {
            var code = @"
Namespace tests
    Class Class1
        Public Property MyAr*ray As Array
    End Class
End Namespace";

            var expected = new AnalyzerOutput
            {
                Name = "MyArray",
                Output = @"<Array />",
                OutputType = AnalyzerOutputType.Property,
            };

            this.PositionAtStarShouldProduceExpected(code, expected, this.ArrayTestsProfile);
        }

        [TestMethod]
        public void GetArrayPropertyBrackets()
        {
            var code = @"
Namespace tests
    Class Class1
        Public Property MyAr*rayBool() As Boolean
    End Class
End Namespace";

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
Namespace tests
    Class Class1
       *Public Property MyBool As Boolean 
        Public Property MyArray As Array
        Public Property MyArrayBool() As Boolean*
    End Class
End Namespace";

            var expected = new AnalyzerOutput
            {
                Name = "MyBool, MyArray and 1 other property",
                Output = @"<Bool />
<Array />
<ArrayBool />",
                OutputType = AnalyzerOutputType.Selection,
            };

            this.SelectionBetweenStarsShouldProduceExpected(code, expected, this.ArrayTestsProfile);
        }
    }
}
