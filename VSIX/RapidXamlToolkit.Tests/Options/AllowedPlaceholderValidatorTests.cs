// Copyright (c) Matt Lacey Ltd.. All rights reserved.
// Licensed under the MIT license.

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidXamlToolkit.Options;

namespace RapidXamlToolkit.Tests.Options
{
    [TestClass]
    public class AllowedPlaceholderValidatorTests
    {
        [TestMethod]
        public void NoOutputPlaceholder_IncorrectUse()
        {
            var sut = new AllowedPlaceholderValidator();

            var use = $"something {Placeholder.NoOutput} something";

            var result = sut.ContainsIncorrectUseOfNoOutputPlaceholder(use);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void NoOutputPlaceholder_CorrectUse_Padding()
        {
            var sut = new AllowedPlaceholderValidator();

            var use = $"   {Placeholder.NoOutput}     ";

            var result = sut.ContainsIncorrectUseOfNoOutputPlaceholder(use);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void NoOutputPlaceholder_CorrectUse_NoPadding()
        {
            var sut = new AllowedPlaceholderValidator();

            var use = $"{Placeholder.NoOutput}";

            var result = sut.ContainsIncorrectUseOfNoOutputPlaceholder(use);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void UnknownPlaceholder_NoUnknowns_NoKnowns()
        {
            var sut = new AllowedPlaceholderValidator();

            var use = $"FOO Blah blah blah  BAR";

            var (isValid, unknownList) = sut.ContainsUnknownPlaceholders(use);

            Assert.IsTrue(isValid);
            Assert.IsNotNull(unknownList);
            Assert.IsFalse(unknownList.Any());
        }

        [TestMethod]
        public void UnknownPlaceholder_NoUnknowns_SomeKnowns()
        {
            var sut = new AllowedPlaceholderValidator();

            var use = $"FOO  {Placeholder.PropertyName}, {Placeholder.PropertyNameWithSpaces}, {Placeholder.PropertyType}  BAR";

            var (isValid, unknownList) = sut.ContainsUnknownPlaceholders(use);

            Assert.IsTrue(isValid);
            Assert.IsNotNull(unknownList);
            Assert.IsFalse(unknownList.Any());
        }

        [TestMethod]
        public void UnknownPlaceholder_OneUnknown_NoKnowns()
        {
            var sut = new AllowedPlaceholderValidator();

            var unknownPlaceholder = "$unknown$";

            var use = $"blah blah {unknownPlaceholder} $$ bjekwbfjkewbj";

            var (isValid, unknownList) = sut.ContainsUnknownPlaceholders(use);

            Assert.IsFalse(isValid);
            Assert.IsNotNull(unknownList);
            Assert.AreEqual(1, unknownList.Count);
            Assert.AreEqual(unknownPlaceholder, unknownList[0]);
        }

        [TestMethod]
        public void UnknownPlaceholder_OneUnknown_SomeKnowns()
        {
            var sut = new AllowedPlaceholderValidator();

            var unknownPlaceholder = "$unknown$";

            var use = $"{unknownPlaceholder},  {Placeholder.PropertyName}, {Placeholder.PropertyNameWithSpaces}, {Placeholder.PropertyType}";

            var (isValid, unknownList) = sut.ContainsUnknownPlaceholders(use);

            Assert.IsFalse(isValid);
            Assert.IsNotNull(unknownList);
            Assert.AreEqual(1, unknownList.Count);
            Assert.AreEqual(unknownPlaceholder, unknownList[0]);
        }

        [TestMethod]
        public void UnknownPlaceholder_TwoUnknows_NoKnowns()
        {
            var sut = new AllowedPlaceholderValidator();

            var unknownPlaceholder1 = "$unknown1$";
            var unknownPlaceholder2 = "$unknown2$";

            var use = $"{unknownPlaceholder1}, {unknownPlaceholder2}, $$ bjekwbfjkewbj";

            var (isValid, unknownList) = sut.ContainsUnknownPlaceholders(use);

            Assert.IsFalse(isValid);
            Assert.IsNotNull(unknownList);
            Assert.AreEqual(2, unknownList.Count);
            Assert.AreEqual(unknownPlaceholder1, unknownList[0]);
            Assert.AreEqual(unknownPlaceholder2, unknownList[1]);
        }

        [TestMethod]
        public void UnknownPlaceholder_TwoUnknowns_SomeKnowns()
        {
            var sut = new AllowedPlaceholderValidator();

            var unknownPlaceholder1 = "$unknown1$";
            var unknownPlaceholder2 = "$unknown2$";

            var use = $"{unknownPlaceholder1}, {unknownPlaceholder2}, {Placeholder.PropertyName}, {Placeholder.PropertyNameWithSpaces}, {Placeholder.PropertyType}";

            var (isValid, unknownList) = sut.ContainsUnknownPlaceholders(use);

            Assert.IsFalse(isValid);
            Assert.IsNotNull(unknownList);
            Assert.AreEqual(2, unknownList.Count);
            Assert.AreEqual(unknownPlaceholder1, unknownList[0]);
            Assert.AreEqual(unknownPlaceholder2, unknownList[1]);
        }

        [TestMethod]
        public void UnknownPlaceholder_DuplicateUnknowns_RepeatedInResults()
        {
            var sut = new AllowedPlaceholderValidator();

            var unknownPlaceholder = "$unknown$";

            var use = $"{unknownPlaceholder}, {unknownPlaceholder}, {Placeholder.PropertyName}, {Placeholder.PropertyNameWithSpaces}, {Placeholder.PropertyType}";

            var (isValid, unknownList) = sut.ContainsUnknownPlaceholders(use);

            Assert.IsFalse(isValid);
            Assert.IsNotNull(unknownList);
            Assert.AreEqual(2, unknownList.Count);
            Assert.AreEqual(unknownPlaceholder, unknownList[0]);
            Assert.AreEqual(unknownPlaceholder, unknownList[1]);
        }

        [TestMethod]
        public void InvalidPlaceholders_NoInvalid_NoValid()
        {
            var sut = new AllowedPlaceholderValidator();

            var use = $"for bar  blah blah blah";

            var (isValid, invalidList) = sut.ContainsOnlyValidPlaceholders(typeof(Mapping), nameof(Mapping.Output), use);

            Assert.IsTrue(isValid);
            Assert.IsFalse(invalidList.Any());
        }

        [TestMethod]
        public void InvalidPlaceholders_NoInvalid_SomeValid()
        {
            var sut = new AllowedPlaceholderValidator();

            var use = $"  {Placeholder.PropertyName}, {Placeholder.PropertyNameWithSpaces}, {Placeholder.PropertyType}";

            var (isValid, invalidList) = sut.ContainsOnlyValidPlaceholders(typeof(Mapping), nameof(Mapping.Output), use);

            Assert.IsTrue(isValid);
            Assert.IsFalse(invalidList.Any());
        }

        [TestMethod]
        public void InvalidPlaceholders_OneInvalid_NoValid()
        {
            var sut = new AllowedPlaceholderValidator();

            var invalidPlaceholder = Placeholder.EnumElement;

            var use = $"for bar {invalidPlaceholder} blah blah blah";

            var (isValid, invalidList) = sut.ContainsOnlyValidPlaceholders(typeof(Mapping), nameof(Mapping.Output), use);

            Assert.IsFalse(isValid);
            Assert.IsNotNull(invalidList);
            Assert.AreEqual(1, invalidList.Count);
            Assert.AreEqual(invalidPlaceholder, invalidList[0]);
        }

        [TestMethod]
        public void InvalidPlaceholders__OneInvalid_SomeValid()
        {
            var sut = new AllowedPlaceholderValidator();

            var invalidPlaceholder = Placeholder.EnumElement;

            var use = $"{invalidPlaceholder}, {Placeholder.PropertyName}, {Placeholder.PropertyNameWithSpaces}, {Placeholder.PropertyType}";

            var (isValid, invalidList) = sut.ContainsOnlyValidPlaceholders(typeof(Mapping), nameof(Mapping.Output), use);

            Assert.IsFalse(isValid);
            Assert.IsNotNull(invalidList);
            Assert.AreEqual(1, invalidList.Count);
            Assert.AreEqual(invalidPlaceholder, invalidList[0]);
        }

        [TestMethod]
        public void InvalidPlaceholders_TwoInvalid_NoValid()
        {
            var sut = new AllowedPlaceholderValidator();

            var invalidPlaceholder1 = Placeholder.EnumElement;
            var invalidPlaceholder2 = Placeholder.EnumElementWithSpaces;

            var use = $"{invalidPlaceholder1}, {invalidPlaceholder2} ";

            var (isValid, invalidList) = sut.ContainsOnlyValidPlaceholders(typeof(Mapping), nameof(Mapping.Output), use);

            Assert.IsFalse(isValid);
            Assert.IsNotNull(invalidList);
            Assert.AreEqual(2, invalidList.Count);
            Assert.AreEqual(invalidPlaceholder1, invalidList[0]);
            Assert.AreEqual(invalidPlaceholder2, invalidList[1]);
        }

        [TestMethod]
        public void InvalidPlaceholders_TwoInvalid_SomeValid()
        {
            var sut = new AllowedPlaceholderValidator();

            var invalidPlaceholder1 = Placeholder.EnumElement;
            var invalidPlaceholder2 = Placeholder.EnumElementWithSpaces;

            var use = $"{invalidPlaceholder1}, {invalidPlaceholder2}, {Placeholder.PropertyName}, {Placeholder.PropertyNameWithSpaces}, {Placeholder.PropertyType}";

            var (isValid, invalidList) = sut.ContainsOnlyValidPlaceholders(typeof(Mapping), nameof(Mapping.Output), use);

            Assert.IsFalse(isValid);
            Assert.IsNotNull(invalidList);
            Assert.AreEqual(2, invalidList.Count);
            Assert.AreEqual(invalidPlaceholder1, invalidList[0]);
            Assert.AreEqual(invalidPlaceholder2, invalidList[1]);
        }

        [TestMethod]
        public void InvalidPlaceholders_DuplicateInvalid_RepeatedInResults()
        {
            var sut = new AllowedPlaceholderValidator();

            var invalidPlaceholder = Placeholder.EnumPropName;

            var use = $"{invalidPlaceholder}, {invalidPlaceholder}, {Placeholder.PropertyName}, {Placeholder.PropertyNameWithSpaces}, {Placeholder.PropertyType}";

            var (isValid, invalidList) = sut.ContainsOnlyValidPlaceholders(typeof(Mapping), nameof(Mapping.Output), use);

            Assert.IsFalse(isValid);
            Assert.IsNotNull(invalidList);
            Assert.AreEqual(2, invalidList.Count);
            Assert.AreEqual(invalidPlaceholder, invalidList[0]);
            Assert.AreEqual(invalidPlaceholder, invalidList[1]);
        }
    }
}
