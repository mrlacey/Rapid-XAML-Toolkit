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
    public class GetCSharpAttributesTests : CSharpTestsBase
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
        public void Display_NameEqualsParam()
        {
            var code = @"
namespace tests
{
    class Class1
    {
        [Display(Name = ""ShortName"")]
        public ☆string Name { get; set; }
    }
}";

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
        public void Display_NameColonParam()
        {
            var code = @"
namespace tests
{
    class Class1
    {
        [Display(Name: ""ShortName"")]
        public ☆string Name { get; set; }
    }
}";

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
        public void Fallback_SimpleFallback()
        {
            var code = @"
namespace tests
{
    class Class1
    {
        //[Display(Name = ""ShortName"")]
        public ☆string UserName { get; set; }
    }
}";

            var expectedOutput = "<TextBox Text=\"UserName\" Header=\"unknown\" />";

            var expected = new ParserOutput
            {
                Name = "UserName",
                Output = expectedOutput,
                OutputType = ParserOutputType.Property,
            };

            var profile = this.displayNameProfile;
            profile.FallbackOutput = "<TextBox Text=\"$name$\" $att:Display: Header=\"[Name]\"::Header=\"unknown\"$ />";

            this.PositionAtStarShouldProduceExpected(code, expected, profile);
        }

        [TestMethod]
        public void Fallback_InterpretedFallback()
        {
            var code = @"
namespace tests
{
    class Class1
    {
        //[Display(Name = ""ShortName"")]
        public ☆string UserName { get; set; }
    }
}";

            var expectedOutput = "<TextBox Text=\"UserName\" Header=\"User Name\" />";

            var expected = new ParserOutput
            {
                Name = "UserName",
                Output = expectedOutput,
                OutputType = ParserOutputType.Property,
            };

            var profile = this.displayNameProfile;
            profile.FallbackOutput = "<TextBox Text=\"$name$\" $att:Display: Header=\"[Name]\"::Header=\"@namewithspaces@\"$ />";

            this.PositionAtStarShouldProduceExpected(code, expected, profile);
        }

        [TestMethod]
        public void Fallback_InterpretedFallback_OtherAttribute()
        {
            var code = @"
namespace tests
{
    class Class1
    {
        [NotDisplay(Name = ""ShortName"")]
        public ☆string UserName { get; set; }
    }
}";

            var expectedOutput = "<TextBox Text=\"NAME\" Header=\"User Name\" />";

            var expected = new ParserOutput
            {
                Name = "UserName",
                Output = expectedOutput,
                OutputType = ParserOutputType.Property,
            };

            var profile = this.displayNameProfile;
            profile.FallbackOutput = "<TextBox Text=\"$att:ToShow:[0]::NAME$\" $att:Display: Header=\"[Name]\"::Header=\"@namewithspaces@\"$ />";

            this.PositionAtStarShouldProduceExpected(code, expected, profile);
        }

        [TestMethod]
        public void Fallback_OneAttributeOneFallback()
        {
            var code = @"
namespace tests
{
    class Class1
    {
        [Display(Name = ""ShortName"")]
        public ☆string UserName { get; set; }
    }
}";

            var expectedOutput = "<TextBox Text=\"User Name\" Header=\"ShortName\" />";

            var expected = new ParserOutput
            {
                Name = "UserName",
                Output = expectedOutput,
                OutputType = ParserOutputType.Property,
            };

            var profile = this.displayNameProfile;
            profile.FallbackOutput = "<TextBox Text=\"$namewithspaces$\" $att:Display: Header=\"[Name]\"::Header=\"@namewithspaces@\"$ />";

            this.PositionAtStarShouldProduceExpected(code, expected, profile);
        }

        [TestMethod]
        public void DisplayAndMaxLength_SeparateAttributes()
        {
            var code = @"
namespace tests
{
    class Class1
    {
        [Display(Name: ""ShortName"")]
        [MaxLength(50)]
        public ☆string Name { get; set; }
    }
}";

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
namespace tests
{
    class Class1
    {
        [Display(Name: ""ShortName""), MaxLength(50)]
        public ☆string Name { get; set; }
    }
}";

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
namespace tests
{
    class Class1
    {
        [MaxLength(50)]
        public ☆string Name { get; set; }
    }
}";

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
namespace tests
{
    class Class1
    {
        [MaxLengthAttribute(50)]
        public string Name☆ { get; set; }
    }
}";

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
namespace tests
{
    class Class1
    {
        [MaxLength(50)]
        public string Name☆ { get; set; }
    }
}";

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
namespace tests
{
    class Class1
    {
        [Range(1, 5)]
        public int ☆Rating { get; set; }
    }
}";

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
namespace tests
{
    class Class1
    {
        public int ☆Rating { get; set; }
    }
}";

            var expectedOutput = "<Slider Name=\"Rating\" />";

            var expected = new ParserOutput
            {
                Name = "Rating",
                Output = expectedOutput,
                OutputType = ParserOutputType.Property,
            };

            this.PositionAtStarShouldProduceExpected(code, expected, this.rangeProfile);
        }
    }
}
