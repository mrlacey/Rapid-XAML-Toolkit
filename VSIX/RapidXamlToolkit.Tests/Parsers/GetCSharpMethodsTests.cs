// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidXamlToolkit.Options;
using RapidXamlToolkit.Parsers;

namespace RapidXamlToolkit.Tests.Parsers
{
    [TestClass]
    public class GetCSharpMethodsTests : CSharpTestsBase
    {
        [TestMethod]
        public void GetAllMethodOptions()
        {
            var code = @"
namespace tests
{
    class Class1☆
    {
        public void OnPhotoTaken(CameraControlEventArgs args) { }

        public void ZoomIn() => _zoomService?.ZoomIn();

        public void Undo() {  }

        public async void SwitchTheme(ElementTheme theme) { }

        public async void Redo() {  }

        public void MethodName(string name, int amount) { }
    }
}";

            var expectedOutput = "<StackPanel>"
         + Environment.NewLine + "    <TextBlock Text=\"ONEPARAM_OnPhotoTaken_args\" />"
         + Environment.NewLine + "    <TextBlock Text=\"NOPARAMS_ZoomIn\" />"
         + Environment.NewLine + "    <TextBlock Text=\"NOPARAMS_Undo\" />"
         + Environment.NewLine + "    <TextBlock Text=\"ONEPARAM_SwitchTheme_theme\" />"
         + Environment.NewLine + "    <TextBlock Text=\"NOPARAMS_Redo\" />"
         + Environment.NewLine + "    <TextBlock Text=\"TWOPARAMS_MethodName_name_amount\" />"
         + Environment.NewLine + "</StackPanel>";

            var expected = new ParserOutput
            {
                Name = "Class1",
                Output = expectedOutput,
                OutputType = ParserOutputType.Class,
            };

            this.PositionAtStarShouldProduceExpected(code, expected, this.MethodTestProfile());
        }

        [TestMethod]
        public void GetAllMethodOptions_Selection()
        {
            var code = @"
namespace tests
{
    class Class1
    {☆
        public void OnPhotoTaken(CameraControlEventArgs args) { }

        public void ZoomIn() => _zoomService?.ZoomIn();

        public void Undo() {  }

        public async void SwitchTheme(ElementTheme theme) { }

        public async void Redo() {  }

        public void MethodName(string name, int amount) { }
    }☆
}";

            var expectedOutput = "<TextBlock Text=\"ONEPARAM_OnPhotoTaken_args\" />"
         + Environment.NewLine + "<TextBlock Text=\"NOPARAMS_ZoomIn\" />"
         + Environment.NewLine + "<TextBlock Text=\"NOPARAMS_Undo\" />"
         + Environment.NewLine + "<TextBlock Text=\"ONEPARAM_SwitchTheme_theme\" />"
         + Environment.NewLine + "<TextBlock Text=\"NOPARAMS_Redo\" />"
         + Environment.NewLine + "<TextBlock Text=\"TWOPARAMS_MethodName_name_amount\" />";

            var expected = new ParserOutput
            {
                Name = "OnPhotoTaken, ZoomIn and 4 other members",
                Output = expectedOutput,
                OutputType = ParserOutputType.Selection,
            };

            this.SelectionBetweenStarsShouldProduceExpected(code, expected, this.MethodTestProfile());
        }

        [TestMethod]
        public void GetSingleMethod_NoParams()
        {
            var code = @"
namespace tests
{
    class Class1
    {
        public void Un☆do() {  }
    }
}";

            var expectedOutput = "<TextBlock Text=\"NOPARAMS_Undo\" />";

            var expected = new ParserOutput
            {
                Name = "Undo",
                Output = expectedOutput,
                OutputType = ParserOutputType.Member,
            };

            this.PositionAtStarShouldProduceExpected(code, expected, this.MethodTestProfile());
        }

        [TestMethod]
        public void GetSingleMethod_NoParamsLambda()
        {
            var code = @"
namespace tests
{
    class Class1
    {
        public void Zoo☆mIn() => _zoomService?.ZoomIn();
    }
}";

            var expectedOutput = "<TextBlock Text=\"NOPARAMS_ZoomIn\" />";

            var expected = new ParserOutput
            {
                Name = "ZoomIn",
                Output = expectedOutput,
                OutputType = ParserOutputType.Member,
            };

            this.PositionAtStarShouldProduceExpected(code, expected, this.MethodTestProfile());
        }

        [TestMethod]
        public void GetSingleMethod_OneParam()
        {
            var code = @"
namespace tests
{
    class Class1
    {
        public void OnPhotoT☆aken(CameraControlEventArgs args) { }
    }
}";

            var expectedOutput = "<TextBlock Text=\"ONEPARAM_OnPhotoTaken_args\" />";

            var expected = new ParserOutput
            {
                Name = "OnPhotoTaken",
                Output = expectedOutput,
                OutputType = ParserOutputType.Member,
            };

            this.PositionAtStarShouldProduceExpected(code, expected, this.MethodTestProfile());
        }

        [TestMethod]
        public void GetSingleMethod_TwoParams()
        {
            var code = @"
namespace tests
{
    class Class1
    {
        public void Metho☆dName(string name, int amount) { }
    }
}";

            var expectedOutput = "<TextBlock Text=\"TWOPARAMS_MethodName_name_amount\" />";

            var expected = new ParserOutput
            {
                Name = "MethodName",
                Output = expectedOutput,
                OutputType = ParserOutputType.Member,
            };

            this.PositionAtStarShouldProduceExpected(code, expected, this.MethodTestProfile());
        }

        [TestMethod]
        public void CanMapMethodsBasedOnAttributes()
        {
            var code = @"
namespace tests
{
    class Class1☆
    {
        public void MethodName(string name, int amount) { }

        [DoNotGenerate]
        public void IgnoredMethodName() { }

        [DoNotGenerate]
        public void IgnoredMethodName2(string name) { }

        [DoNotGenerate]
        public void IgnoredMethodName3(string name, int amount) { }
    }
}";

            var expectedOutput = "<StackPanel>"
         + Environment.NewLine + "    <TextBlock Text=\"TWOPARAMS_MethodName_name_amount\" />"
         + Environment.NewLine + "    <One />"
         + Environment.NewLine + "    <Two />"
         + Environment.NewLine + "</StackPanel>";

            var expected = new ParserOutput
            {
                Name = "Class1",
                Output = expectedOutput,
                OutputType = ParserOutputType.Class,
            };

            var profile = this.MethodTestProfile();
            profile.Mappings.Add(new Mapping
            {
                Type = "[DoNotGenerate]method()",
                NameContains = "",
                Output = "$nooutput$",
                IfReadOnly = false,
            });
            profile.Mappings.Add(new Mapping
            {
                Type = "[DoNotGenerate]method(T)",
                NameContains = "",
                Output = "<One />",
                IfReadOnly = false,
            });
            profile.Mappings.Add(new Mapping
            {
                Type = "[DoNotGenerate]method(T,T)",
                NameContains = "",
                Output = "<Two />",
                IfReadOnly = false,
            });

            this.PositionAtStarShouldProduceExpected(code, expected, profile);
        }

        [TestMethod]
        public void AttributeBasedMapping_NoParams()
        {
            var code = @"
namespace tests
{
    class Class1☆
    {
        [Filter0]
        [Filter1]
        public void IgnoredMethodName1() { }

        [Filter1]
        public void IgnoredMethodName2() { }
    }
}";

            var expectedOutput = "<StackPanel>"
         + Environment.NewLine + "    <FilterOne />"
         + Environment.NewLine + "    <FilterOne />"
         + Environment.NewLine + "</StackPanel>";

            var expected = new ParserOutput
            {
                Name = "Class1",
                Output = expectedOutput,
                OutputType = ParserOutputType.Class,
            };

            var profile = this.MethodTestProfile();
            profile.Mappings.Add(new Mapping
            {
                Type = "[Filter1]method()",
                NameContains = "",
                Output = "<FilterOne />",
                IfReadOnly = false,
            });

            this.PositionAtStarShouldProduceExpected(code, expected, profile);
        }

        [TestMethod]
        public void AttributeBasedMapping_IsTypeSensitive_OneParam()
        {
            var code = @"
namespace tests
{
    class Class1☆
    {
        [Filter1]
        public void IgnoredMethodName1(int id) { }

        [Filter1]
        public void IgnoredMethodName2(string name) { }

        [Filter1]
        public void IgnoredMethodName3(string name) { }
    }
}";

            var expectedOutput = "<StackPanel>"
         + Environment.NewLine + "    <FilterOne />"
         + Environment.NewLine + "    <FilterTwo />"
         + Environment.NewLine + "    <FilterTwo />"
         + Environment.NewLine + "</StackPanel>";

            var expected = new ParserOutput
            {
                Name = "Class1",
                Output = expectedOutput,
                OutputType = ParserOutputType.Class,
            };

            var profile = this.MethodTestProfile();
            profile.Mappings.Add(new Mapping
            {
                Type = "[Filter1]method(T)",
                NameContains = "",
                Output = "<FilterOne />",
                IfReadOnly = false,
            });
            profile.Mappings.Add(new Mapping
            {
                Type = "[Filter1]method(string)",
                NameContains = "",
                Output = "<FilterTwo />",
                IfReadOnly = false,
            });

            this.PositionAtStarShouldProduceExpected(code, expected, profile);
        }

        [TestMethod]
        public void AttributeBasedMapping_IsTypeSensitive_TwoParams()
        {
            var code = @"
namespace tests
{
    class Class1☆
    {
        [Filter1]
        public void IgnoredMethodName1(int id, int refId) { }

        [Filter1]
        public void IgnoredMethodName2(string name, int id) { }

        [Filter1]
        public void IgnoredMethodName3(int id, string name) { }

        [Filter1]
        public void IgnoredMethodName4(string name, string description) { }
    }
}";

            var expectedOutput = "<StackPanel>"
         + Environment.NewLine + "    <FilterOne />"
         + Environment.NewLine + "    <FilterTwo />"
         + Environment.NewLine + "    <FilterThree />"
         + Environment.NewLine + "    <FilterFour />"
         + Environment.NewLine + "</StackPanel>";

            var expected = new ParserOutput
            {
                Name = "Class1",
                Output = expectedOutput,
                OutputType = ParserOutputType.Class,
            };

            var profile = this.MethodTestProfile();
            profile.Mappings.Add(new Mapping
            {
                Type = "[Filter1]method(string,string)",
                NameContains = "",
                Output = "<FilterFour />",
                IfReadOnly = false,
            });
            profile.Mappings.Add(new Mapping
            {
                Type = "[Filter1]method(string,T)",
                NameContains = "",
                Output = "<FilterTwo />",
                IfReadOnly = false,
            });
            profile.Mappings.Add(new Mapping
            {
                Type = "[Filter1]method(T,T)",
                NameContains = "",
                Output = "<FilterOne />",
                IfReadOnly = false,
            });
            profile.Mappings.Add(new Mapping
            {
                Type = "[Filter1]method(T,string)",
                NameContains = "",
                Output = "<FilterThree />",
                IfReadOnly = false,
            });

            this.PositionAtStarShouldProduceExpected(code, expected, profile);
        }

        private Profile MethodTestProfile()
        {
            var profile = TestProfile.CreateEmpty();
            profile.ClassGrouping = "StackPanel";
            profile.FallbackOutput = "<TextBlock Text=\"FALLBACK_$name$\" />";
            profile.Mappings.Add(new Mapping
            {
                Type = "method()",
                NameContains = "",
                Output = "<TextBlock Text=\"NOPARAMS_$method$\" />",
                IfReadOnly = false,
            });
            profile.Mappings.Add(new Mapping
            {
                Type = "method(T)",
                NameContains = "",
                Output = "<TextBlock Text=\"ONEPARAM_$method$_$arg1$\" />",
                IfReadOnly = false,
            });
            profile.Mappings.Add(new Mapping
            {
                Type = "method(T,T)",
                NameContains = "",
                Output = "<TextBlock Text=\"TWOPARAMS_$method$_$arg1$_$arg2$\" />",
                IfReadOnly = false,
            });

            return profile;
        }
    }
}
