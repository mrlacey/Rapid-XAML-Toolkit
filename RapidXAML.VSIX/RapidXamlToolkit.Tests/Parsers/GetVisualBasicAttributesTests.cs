// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidXamlToolkit.Options;
using RapidXamlToolkit.Parsers;

namespace RapidXamlToolkit.Tests.Parsers
{
    [TestClass]
    public class GetVisualBasicAttributesTests : VisualBasicTestsBase
    {
        private readonly Profile maxLengthProfile = new Profile
        {
            Name = "maxLength=Profile",
            ClassGrouping = "Grid",
            FallbackOutput = "<TextBlock Text=\"$name$\" $att:MaxLength:MaxLength=\"[1]\"$ />",
            SubPropertyOutput = string.Empty,
            EnumMemberOutput = string.Empty,
            Mappings = new ObservableCollection<Mapping>
            {
            },
        };

        private readonly Profile maxLengthAttributeProfile = new Profile
        {
            Name = "maxLengthAttributeProfile",
            ClassGrouping = "Grid",
            FallbackOutput = "<TextBlock Text=\"$name$\" $att:MaxLengthAttribute:MaxLength=\"[1]\"$ />",
            SubPropertyOutput = string.Empty,
            EnumMemberOutput = string.Empty,
            Mappings = new ObservableCollection<Mapping>
            {
            },
        };

        private readonly Profile rangeProfile = new Profile
        {
            Name = "rangeAttributeProfile",
            ClassGrouping = "Grid",
            FallbackOutput = "<Slider Name=\"$name$\"$att:Range: Minimum=\"[1]\"$$att:Range: Maximum=\"[2]\"$ />",
            SubPropertyOutput = string.Empty,
            EnumMemberOutput = string.Empty,
            Mappings = new ObservableCollection<Mapping>
            {
            },
        };

        private readonly Profile displayNameProfile = new Profile
        {
            Name = "displayNameProfile",
            ClassGrouping = "Grid",
            FallbackOutput = "<TextBox Text=\"$name$\" $att:Display: Header=\"[Name]\"$ />",
            SubPropertyOutput = string.Empty,
            EnumMemberOutput = string.Empty,
            Mappings = new ObservableCollection<Mapping>
            {
            },
        };

        private readonly Profile displayNameAndMaxLengthProfile = new Profile
        {
            Name = "displayNameAndMaxLengthProfile",
            ClassGrouping = "Grid",
            FallbackOutput = "<TextBox Text=\"$name$\"$att:Display: Header=\"[Name]\"$$att:MaxLength: MaxLength=\"[1]\"$ />",
            SubPropertyOutput = string.Empty,
            EnumMemberOutput = string.Empty,
            Mappings = new ObservableCollection<Mapping>
            {
            },
        };

        [TestMethod]
        public void Display_NamedParam()
        {
            var code = @"
Namespace tests
    Class Class1
        <Display(Name:= ShortName)>
        Public Property ☆Name As String
    End Class
End Namespace";

            var expectedOutput = "<TextBox Text=\"Name\" Header=\"ShortName\" />";

            var expected = new ParserOutput
            {
                Name = "Name",
                Output = expectedOutput,
                OutputType = ParserOutputType.Property,
            };

            this.PositionAtStarShouldProduceExpected(code, expected, this.displayNameProfile);
        }

        [TestMethod]
        public void DisplayAndMaxLength_SeparateAttributes()
        {
            var code = @"
Namespace tests
    Class Class1
        <Display(Name:= ShortName)>
        <MaxLength(50)>
        Public Property ☆Name As String
    End Class
End Namespace";

            var expectedOutput = "<TextBox Text=\"Name\" Header=\"ShortName\" MaxLength=\"50\" />";

            var expected = new ParserOutput
            {
                Name = "Name",
                Output = expectedOutput,
                OutputType = ParserOutputType.Property,
            };

            this.PositionAtStarShouldProduceExpected(code, expected, this.displayNameAndMaxLengthProfile);
        }

        [TestMethod]
        public void DisplayAndMaxLength_SingleAttribute()
        {
            var code = @"
Namespace tests
    Class Class1
        <Display(Name:= ShortName), MaxLength(50)>
        Public Property ☆Name As String
    End Class
End Namespace";

            var expectedOutput = "<TextBox Text=\"Name\" Header=\"ShortName\" MaxLength=\"50\" />";

            var expected = new ParserOutput
            {
                Name = "Name",
                Output = expectedOutput,
                OutputType = ParserOutputType.Property,
            };

            this.PositionAtStarShouldProduceExpected(code, expected, this.displayNameAndMaxLengthProfile);
        }

        [TestMethod]
        public void MaxLength_SingleNumericParam()
        {
            var code = @"
Namespace tests
    Class Class1
        <MaxLength(50)>
        Public Property ☆Name As String
    End Class
End Namespace";

            var expectedOutput = "<TextBlock Text=\"Name\" MaxLength=\"50\" />";

            var expected = new ParserOutput
            {
                Name = "Name",
                Output = expectedOutput,
                OutputType = ParserOutputType.Property,
            };

            this.PositionAtStarShouldProduceExpected(code, expected, this.maxLengthProfile);
        }

        [TestMethod]
        public void MaxLengthAttribute_SingleNumericParam()
        {
            var code = @"
Namespace tests
    Class Class1
        <MaxLengthAttribute(50)>
        Public Property ☆Name As String

    End Class
End Namespace";

            var expectedOutput = "<TextBlock Text=\"Name\" MaxLength=\"50\" />";

            var expected = new ParserOutput
            {
                Name = "Name",
                Output = expectedOutput,
                OutputType = ParserOutputType.Property,
            };

            this.PositionAtStarShouldProduceExpected(code, expected, this.maxLengthProfile);
        }

        [TestMethod]
        public void MaxLength_AttributeIncludedInProfile_SingleNumericParam()
        {
            var code = @"
Namespace tests
    Class Class1
        <MaxLength(50)>
        Public Property ☆Name As String
    End Class
End Namespace";

            var expectedOutput = "<TextBlock Text=\"Name\" MaxLength=\"50\" />";

            var expected = new ParserOutput
            {
                Name = "Name",
                Output = expectedOutput,
                OutputType = ParserOutputType.Property,
            };

            this.PositionAtStarShouldProduceExpected(code, expected, this.maxLengthAttributeProfile);
        }

        [TestMethod]
        public void RangeAttribute_MultipleNumericParams()
        {
            var code = @"
Namespace tests
    Class Class1
        <Range(1, 5)>
        Public Property ☆Rating As Integer
    End Class
End Namespace";

            var expectedOutput = "<Slider Name=\"Rating\" Minimum=\"1\" Maximum=\"5\" />";

            var expected = new ParserOutput
            {
                Name = "Rating",
                Output = expectedOutput,
                OutputType = ParserOutputType.Property,
            };

            this.PositionAtStarShouldProduceExpected(code, expected, this.rangeProfile);
        }

        [TestMethod]
        public void RangeAttribute_InProfileButNotOnProperty_NoOutput()
        {
            var code = @"
Namespace tests
    Class Class1
        Public Property ☆Rating As Integer
    End Class
End Namespace";

            var expectedOutput = "<Slider Name=\"Rating\" />";

            var expected = new ParserOutput
            {
                Name = "Rating",
                Output = expectedOutput,
                OutputType = ParserOutputType.Property,
            };

            this.PositionAtStarShouldProduceExpected(code, expected, this.rangeProfile);
        }

        [TestMethod]
        public void MaxLengthAttribute_SingleNumericParam_PropertyBlock()
        {
            var code = @"
Namespace tests
    Class Class

        Private _name As String

        <MaxLengthAttribute(50)>
        Public Property Name☆ As String
            Get
                Return _name
            End Get

            Set(ByVal value As String)
                _name = value
            End Set
        End Property

    End Class
End Namespace";

            var expectedOutput = "<TextBlock Text=\"Name\" MaxLength=\"50\" />";

            var expected = new ParserOutput
            {
                Name = "Name",
                Output = expectedOutput,
                OutputType = ParserOutputType.Property,
            };

            this.PositionAtStarShouldProduceExpected(code, expected, this.maxLengthProfile);
        }

    }
}


