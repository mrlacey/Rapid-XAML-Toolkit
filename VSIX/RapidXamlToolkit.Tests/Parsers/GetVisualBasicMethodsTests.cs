// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidXamlToolkit.Options;
using RapidXamlToolkit.Parsers;

namespace RapidXamlToolkit.Tests.Parsers
{
    [TestClass]
    public class GetVisualBasicMethodsTests : VisualBasicTestsBase
    {
        [TestMethod]
        public void GetAllMethodOptions()
        {
            var code = @"
Namespace tests
    Class Class1☆
        Public Sub New()
            ' Constructor is a method but shouldn't match anything
        End Sub

        Public Sub OnPhotoTaken(ByVal args As CameraControlEventArgs)
        End Sub

        Public Sub ZoomIn()
            Return _zoomService?.ZoomIn()
        End Sub

        Public Sub Undo()
        End Sub

        Public Async Sub SwitchTheme(ByVal theme As ElementTheme)
        End Sub

        Public Async Sub Redo()
        End Sub

        Public Sub MethodName(ByVal name As String, ByVal amount As Integer)
        End Sub

        Private Sub DoNotMatchBecausePrivate()
        End Sub

        Public Function DoNotMatchBecauseOfReturnType() As Integer
        End Function

        Friend Sub DoNotMatchBecauseInternal()
        End Sub

        Protected Sub DoNotMatchBecauseProtected()
        End Sub

        Public Sub DoNotMatchAsGeneric(Of T)()
        End Sub
    End Class
End Namespace";

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
Namespace tests
    Class Class1☆
        Public Sub OnPhotoTaken(ByVal args As CameraControlEventArgs)
        End Sub

        Public Sub ZoomIn()
            Return _zoomService?.ZoomIn()
        End Sub

        Public Sub Undo()
        End Sub

        Public Async Sub SwitchTheme(ByVal theme As ElementTheme)
        End Sub

        Public Async Sub Redo()
        End Sub

        Public Sub MethodName(ByVal name As String, ByVal amount As Integer)
        End Sub☆
    End Class
End Namespace
";

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
Namespace tests
    Class Class1
        Public Sub Un☆do()
        End Sub
    End Class
End Namespace
";

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
        public void GetSingleMethod_OneParam()
        {
            var code = @"
Namespace tests
    Class Class1
        Public Sub OnPhotoT☆aken(ByVal args As CameraControlEventArgs)
        End Sub
    End Class
End Namespace
";

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
Namespace tests
    Class Class1
        Public Sub Metho☆dName(ByVal name As String, ByVal amount As Integer)
        End Sub
    End Class
End Namespace
";

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
Namespace tests
    Class Class1☆
        Public Sub MethodName(ByVal name As String, ByVal amount As Integer)
        End Sub

        <DoNotGenerate>
        Public Sub IgnoredMethodName()
        End Sub

        <DoNotGenerate>
        Public Sub IgnoredMethodName2(ByVal name As String)
        End Sub

        <DoNotGenerate>
        Public Sub IgnoredMethodName3(ByVal name As String, ByVal amount As Integer)
        End Sub
    End Class
End Namespace
";

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
Namespace tests
    Class Class1☆
        <Filter0>
        <Filter1>
        Public Sub IgnoredMethodName1()
        End Sub

        <Filter1>
        Public Sub IgnoredMethodName2()
        End Sub
    End Class
End Namespace
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
Namespace tests
    Class Class1☆
        <Filter1>
        Public Sub IgnoredMethodName1(ByVal id As Integer)
        End Sub

        <Filter1>
        Public Sub IgnoredMethodName2(ByVal name As String)
        End Sub

        <Filter1>
        Public Sub IgnoredMethodName3(ByVal name As String)
        End Sub
    End Class
End Namespace
";

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
Namespace tests
    Class Class1☆
        <Filter1>
        Public Sub IgnoredMethodName1(ByVal id As Integer, ByVal refId As Integer)
        End Sub

        <Filter1>
        Public Sub IgnoredMethodName2(ByVal name As String, ByVal id As Integer)
        End Sub

        <Filter1>
        Public Sub IgnoredMethodName3(ByVal id As Integer, ByVal name As String)
        End Sub

        <Filter1>
        Public Sub IgnoredMethodName4(ByVal name As String, ByVal description As String)
        End Sub
    End Class
End Namespace
";

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

        [TestMethod]
        public void MethodOutputCanBeInGrid_OneColumn()
        {
            var code = @"
Namespace tests
    Class Class1☆
        Public Sub OnPhotoTaken(ByVal args As CameraControlEventArgs)
        End Sub

        Public Sub ZoomIn()
            Return _zoomService?.ZoomIn()
        End Sub

        Public Sub Undo()
        End Sub

        Public Async Sub SwitchTheme(ByVal theme As ElementTheme)
        End Sub

        Public Async Sub Redo()
        End Sub

        Public Sub MethodName(ByVal name As String, ByVal amount As Integer)
        End Sub
    End Class
End Namespace";

            var expectedOutput = "<Grid>"
         + Environment.NewLine + "    <Grid.RowDefinitions>"
         + Environment.NewLine + "        <RowDefinition Height=\"Auto\" />"
         + Environment.NewLine + "        <RowDefinition Height=\"Auto\" />"
         + Environment.NewLine + "        <RowDefinition Height=\"Auto\" />"
         + Environment.NewLine + "        <RowDefinition Height=\"Auto\" />"
         + Environment.NewLine + "        <RowDefinition Height=\"Auto\" />"
         + Environment.NewLine + "        <RowDefinition Height=\"Auto\" />"
         + Environment.NewLine + "        <RowDefinition Height=\"*\" />"
         + Environment.NewLine + "    </Grid.RowDefinitions>"
         + Environment.NewLine + "    <TextBlock Text=\"ONEPARAM_OnPhotoTaken_args\" Grid.Row=\"0\" />"
         + Environment.NewLine + "    <TextBlock Text=\"NOPARAMS_ZoomIn\" Grid.Row=\"1\" />"
         + Environment.NewLine + "    <TextBlock Text=\"NOPARAMS_Undo\" Grid.Row=\"2\" />"
         + Environment.NewLine + "    <TextBlock Text=\"ONEPARAM_SwitchTheme_theme\" Grid.Row=\"3\" />"
         + Environment.NewLine + "    <TextBlock Text=\"NOPARAMS_Redo\" Grid.Row=\"4\" />"
         + Environment.NewLine + "    <TextBlock Text=\"TWOPARAMS_MethodName_name_amount\" Grid.Row=\"5\" />"
         + Environment.NewLine + "</Grid>";

            var expected = new ParserOutput
            {
                Name = "Class1",
                Output = expectedOutput,
                OutputType = ParserOutputType.Class,
            };

            var profile = TestProfile.CreateEmpty();
            profile.ClassGrouping = "GRID-PLUS-ROWDEFS";
            profile.Mappings.Add(new Mapping
            {
                Type = "method()",
                NameContains = "",
                Output = "<TextBlock Text=\"NOPARAMS_$method$\" Grid.Row=\"$incint$\" />",
                IfReadOnly = false,
            });
            profile.Mappings.Add(new Mapping
            {
                Type = "method(T)",
                NameContains = "",
                Output = "<TextBlock Text=\"ONEPARAM_$method$_$arg1$\" Grid.Row=\"$incint$\" />",
                IfReadOnly = false,
            });
            profile.Mappings.Add(new Mapping
            {
                Type = "method(T,T)",
                NameContains = "",
                Output = "<TextBlock Text=\"TWOPARAMS_$method$_$arg1$_$arg2$\" Grid.Row=\"$incint$\" />",
                IfReadOnly = false,
            });

            this.PositionAtStarShouldProduceExpected(code, expected, profile);
        }

        [TestMethod]
        public void MethodOutputCanBeInGrid_TwoColumns()
        {
            var code = @"
Namespace tests
    Class Class1☆
        Public Sub OnPhotoTaken(ByVal args As CameraControlEventArgs)
        End Sub

        Public Sub ZoomIn()
            Return _zoomService?.ZoomIn()
        End Sub

        Public Sub Undo()
        End Sub

        Public Async Sub SwitchTheme(ByVal theme As ElementTheme)
        End Sub

        Public Async Sub Redo()
        End Sub

        Public Sub MethodName(ByVal name As String, ByVal amount As Integer)
        End Sub
    End Class
End Namespace";

            var expectedOutput = "<Grid>"
         + Environment.NewLine + "    <Grid.ColumnDefinitions>"
         + Environment.NewLine + "        <ColumnDefinition Width=\"Auto\" />"
         + Environment.NewLine + "        <ColumnDefinition Width=\"*\" />"
         + Environment.NewLine + "    </Grid.ColumnDefinitions>"
         + Environment.NewLine + "    <Grid.RowDefinitions>"
         + Environment.NewLine + "        <RowDefinition Height=\"Auto\" />"
         + Environment.NewLine + "        <RowDefinition Height=\"Auto\" />"
         + Environment.NewLine + "        <RowDefinition Height=\"Auto\" />"
         + Environment.NewLine + "        <RowDefinition Height=\"Auto\" />"
         + Environment.NewLine + "        <RowDefinition Height=\"Auto\" />"
         + Environment.NewLine + "        <RowDefinition Height=\"Auto\" />"
         + Environment.NewLine + "        <RowDefinition Height=\"*\" />"
         + Environment.NewLine + "    </Grid.RowDefinitions>"
         + Environment.NewLine + "    <Lbl Grid.Column=\"0\" Grid.Row=\"0\" />"
         + Environment.NewLine + "    <TextBlock Text=\"ONEPARAM_OnPhotoTaken_args\" Grid.Column=\"1\" Grid.Row=\"0\" />"
         + Environment.NewLine + "    <Lbl Grid.Column=\"0\" Grid.Row=\"1\" />"
         + Environment.NewLine + "    <TextBlock Text=\"NOPARAMS_ZoomIn\" Grid.Column=\"1\" Grid.Row=\"1\" />"
         + Environment.NewLine + "    <Lbl Grid.Column=\"0\" Grid.Row=\"2\" />"
         + Environment.NewLine + "    <TextBlock Text=\"NOPARAMS_Undo\" Grid.Column=\"1\" Grid.Row=\"2\" />"
         + Environment.NewLine + "    <Lbl Grid.Column=\"0\" Grid.Row=\"3\" />"
         + Environment.NewLine + "    <TextBlock Text=\"ONEPARAM_SwitchTheme_theme\" Grid.Column=\"1\" Grid.Row=\"3\" />"
         + Environment.NewLine + "    <Lbl Grid.Column=\"0\" Grid.Row=\"4\" />"
         + Environment.NewLine + "    <TextBlock Text=\"NOPARAMS_Redo\" Grid.Column=\"1\" Grid.Row=\"4\" />"
         + Environment.NewLine + "    <Lbl Grid.Column=\"0\" Grid.Row=\"5\" />"
         + Environment.NewLine + "    <TextBlock Text=\"TWOPARAMS_MethodName_name_amount\" Grid.Column=\"1\" Grid.Row=\"5\" />"
         + Environment.NewLine + "</Grid>";

            var expected = new ParserOutput
            {
                Name = "Class1",
                Output = expectedOutput,
                OutputType = ParserOutputType.Class,
            };

            var profile = TestProfile.CreateEmpty();
            profile.ClassGrouping = "GRID-PLUS-ROWDEFS-2COLS";
            profile.Mappings.Add(new Mapping
            {
                Type = "method()",
                NameContains = "",
                Output = "<Lbl Grid.Column=\"0\" Grid.Row=\"$incint$\" /><TextBlock Text=\"NOPARAMS_$method$\" Grid.Column=\"1\" Grid.Row=\"$repint$\" />",
                IfReadOnly = false,
            });
            profile.Mappings.Add(new Mapping
            {
                Type = "method(T)",
                NameContains = "",
                Output = "<Lbl Grid.Column=\"0\" Grid.Row=\"$incint$\" /><TextBlock Text=\"ONEPARAM_$method$_$arg1$\" Grid.Column=\"1\" Grid.Row=\"$repint$\" />",
                IfReadOnly = false,
            });
            profile.Mappings.Add(new Mapping
            {
                Type = "method(T,T)",
                NameContains = "",
                Output = "<Lbl Grid.Column=\"0\" Grid.Row=\"$incint$\" /><TextBlock Text=\"TWOPARAMS_$method$_$arg1$_$arg2$\" Grid.Column=\"1\" Grid.Row=\"$repint$\" />",
                IfReadOnly = false,
            });

            this.PositionAtStarShouldProduceExpected(code, expected, profile);
        }

        [TestMethod]
        public void FilterMethodsByName()
        {
            var code = @"
Namespace tests
    Class Class1☆
        Public Sub OnPhotoTaken(ByVal args As CameraControlEventArgs)
        End Sub

        Public Sub ZoomIn()
            Return _zoomService?.ZoomIn()
        End Sub

        Public Sub Undo()
        End Sub

        Public Async Sub SwitchTheme(ByVal theme As ElementTheme)
        End Sub

        Public Async Sub Redo()
        End Sub

        Public Sub MethodName(ByVal name As String, ByVal amount As Integer)
        End Sub

        Public Async Sub OtherMethodNamez(ByVal name As String, ByVal amount As Integer)
        End Sub
    End Class
End Namespace
";

            var profile = TestProfile.CreateEmpty();
            profile.ClassGrouping = "StackPanel";
            profile.FallbackOutput = "$nooutput$";
            profile.Mappings.Add(new Mapping
            {
                Type = "method()",
                NameContains = "e",
                Output = "<TextBlock Text=\"NOPARAMS_$method$\" />",
                IfReadOnly = false,
            });
            profile.Mappings.Add(new Mapping
            {
                Type = "method(T)",
                NameContains = "e",
                Output = "<TextBlock Text=\"ONEPARAM_$method$_$arg1$\" />",
                IfReadOnly = false,
            });
            profile.Mappings.Add(new Mapping
            {
                Type = "method(T,T)",
                NameContains = "z",
                Output = "<TextBlock Text=\"TWOPARAMS_$method$_$arg1$_$arg2$\" />",
                IfReadOnly = false,
            });

            var expectedOutput = "<StackPanel>"
         + Environment.NewLine + "    <TextBlock Text=\"ONEPARAM_OnPhotoTaken_args\" />"
         + Environment.NewLine + "    <TextBlock Text=\"ONEPARAM_SwitchTheme_theme\" />"
         + Environment.NewLine + "    <TextBlock Text=\"NOPARAMS_Redo\" />"
         + Environment.NewLine + "    <TextBlock Text=\"TWOPARAMS_OtherMethodNamez_name_amount\" />"
          + Environment.NewLine + "</StackPanel>";

            var expected = new ParserOutput
            {
                Name = "Class1",
                Output = expectedOutput,
                OutputType = ParserOutputType.Class,
            };

            this.PositionAtStarShouldProduceExpected(code, expected, profile);
        }

        [TestMethod]
        public void FallbackIsNotUsedForMethods()
        {
            var code = @"
Namespace tests
    Class Class1☆
        Public Sub New()
            ' Constructor is a method but shouldn't match anything
        End Sub

        Public Sub OnPhotoTaken(ByVal args As CameraControlEventArgs)
        End Sub

        Public Sub ZoomIn()
            Return _zoomService?.ZoomIn()
        End Sub

        Public Sub Undo()
        End Sub

        Public Async Sub SwitchTheme(ByVal theme As ElementTheme)
        End Sub

        Public Async Sub Redo()
        End Sub

        Public Sub MethodName(ByVal name As String, ByVal amount As Integer)
        End Sub

        Private Sub DoNotMatchBecausePrivate()
        End Sub

        Public Function DoNotMatchBecauseOfReturnType() As Integer
        End Function

        Friend Sub DoNotMatchBecauseInternal()
        End Sub

        Protected Sub DoNotMatchBecauseProtected()
        End Sub

        Public Sub DoNotMatchAsGeneric(Of T)()
        End Sub
    End Class
End Namespace
";

            var expectedOutput = "<StackPanel>"
         + Environment.NewLine + "</StackPanel>";

            var expected = new ParserOutput
            {
                Name = "Class1",
                Output = expectedOutput,
                OutputType = ParserOutputType.Class,
            };

            var profile = TestProfile.CreateEmpty();
            profile.ClassGrouping = "StackPanel";
            profile.FallbackOutput = "<DoNotOutputForMethods />";
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
