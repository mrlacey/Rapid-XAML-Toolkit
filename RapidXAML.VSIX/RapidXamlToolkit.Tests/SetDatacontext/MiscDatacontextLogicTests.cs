// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RapidXamlToolkit.Tests.SetDatacontext
{
    [TestClass]
    public class MiscDatacontextLogicTests
    {
        [TestMethod]
        public void CanGetLineEndPos_FirstLine()
        {
            var file = @"abc
678
123";

            var testResult = SetDataContextCommandLogic.GetLineEndPos(file, 1);

            Assert.AreEqual(5, testResult);
        }

        [TestMethod]
        public void CanGetLineEndPos_SecondLine()
        {
            var file = @"abc
67
012";

            var testResult = SetDataContextCommandLogic.GetLineEndPos(file, 2);

            Assert.AreEqual(9, testResult);
        }

        [TestMethod]
        public void CanGetLineEndPos_ThirdLine()
        {
            var file = @"1
45
890
abcd";

            var testResult = SetDataContextCommandLogic.GetLineEndPos(file, 3);

            Assert.AreEqual(12, testResult);
        }

        [TestMethod]
        public void CanGetLineEndPos_FifthLine()
        {
            var file = @"1

123
abcd
-|-|-|-
12345678";

            var testResult = SetDataContextCommandLogic.GetLineEndPos(file, 5);

            Assert.AreEqual(25, testResult);
        }

        [TestMethod]
        public void CanGetLineEndPos_BlankFirstLine()
        {
            var file = @"
678
123";

            var testResult = SetDataContextCommandLogic.GetLineEndPos(file, 1);

            Assert.AreEqual(2, testResult);
        }

        [TestMethod]
        public void CanGetLineEndPos_BlankThirdLine()
        {
            var file = @"abc


012";

            var testResult = SetDataContextCommandLogic.GetLineEndPos(file, 3);

            Assert.AreEqual(9, testResult);
        }

        [TestMethod]
        public void CorrectlyInferViewModelName_WhenNoSuffixes()
        {
            var sut = new SetDataContextCommandLogic(
                TestProfile.CreateEmpty(),
                DefaultTestLogger.Create(),
                new TestVisualStudioAbstraction(),
                new TestFileSystem());

            var (view, viewModel) = sut.InferViewModelNameFromFileName("Test.xaml.cs");

            Assert.AreEqual("Test", view);
            Assert.AreEqual("Test", viewModel);
        }

        [TestMethod]
        public void CorrectlyInferViewModelName_WhenNoConfiguredSuffixes()
        {
            var sut = new SetDataContextCommandLogic(
                TestProfile.CreateEmpty(),
                DefaultTestLogger.Create(),
                new TestVisualStudioAbstraction(),
                new TestFileSystem());

            var (view, viewModel) = sut.InferViewModelNameFromFileName("TestPage.xaml.cs");

            Assert.AreEqual("TestPage", view);
            Assert.AreEqual("TestPage", viewModel);
        }

        [TestMethod]
        public void CorrectlyInferViewModelName_OnlyPageSuffixConfigured()
        {
            var profile = TestProfile.CreateEmpty();
            profile.ViewGeneration.XamlFileSuffix = "Page";

            var sut = new SetDataContextCommandLogic(
                profile,
                DefaultTestLogger.Create(),
                new TestVisualStudioAbstraction(),
                new TestFileSystem());

            var (view, viewModel) = sut.InferViewModelNameFromFileName("TestPage.xaml.cs");

            Assert.AreEqual("TestPage", view);
            Assert.AreEqual("Test", viewModel);
        }

        [TestMethod]
        public void CorrectlyInferViewModelName_OnlyPageSuffixConfigured_ButNonMatching()
        {
            var profile = TestProfile.CreateEmpty();
            profile.ViewGeneration.XamlFileSuffix = "View";

            var sut = new SetDataContextCommandLogic(
                profile,
                DefaultTestLogger.Create(),
                new TestVisualStudioAbstraction(),
                new TestFileSystem());

            var (view, viewModel) = sut.InferViewModelNameFromFileName("TestPage.xaml.cs");

            Assert.AreEqual("TestPage", view);
            Assert.AreEqual("TestPage", viewModel);
        }

        [TestMethod]
        public void CorrectlyInferViewModelName_OnlyCodeBehindSuffixConfigured()
        {
            var profile = TestProfile.CreateEmpty();
            profile.ViewGeneration.ViewModelFileSuffix = "ViewModel";

            var sut = new SetDataContextCommandLogic(
                profile,
                DefaultTestLogger.Create(),
                new TestVisualStudioAbstraction(),
                new TestFileSystem());

            var (view, viewModel) = sut.InferViewModelNameFromFileName("TestPage.xaml.cs");

            Assert.AreEqual("TestPage", view);
            Assert.AreEqual("TestPageViewModel", viewModel);
        }

        [TestMethod]
        public void CorrectlyInferViewModelName_BothSuffixesConfigured()
        {
            var profile = TestProfile.CreateEmpty();
            profile.ViewGeneration.XamlFileSuffix = "Page";
            profile.ViewGeneration.ViewModelFileSuffix = "ViewModel";

            var sut = new SetDataContextCommandLogic(
                profile,
                DefaultTestLogger.Create(),
                new TestVisualStudioAbstraction(),
                new TestFileSystem());

            var (view, viewModel) = sut.InferViewModelNameFromFileName("TestPage.xaml.cs");

            Assert.AreEqual("TestPage", view);
            Assert.AreEqual("TestViewModel", viewModel);
        }

        [TestMethod]
        public void CorrectlyInferViewModelName_BothSuffixesConfigured_NonStandard()
        {
            var profile = TestProfile.CreateEmpty();
            profile.ViewGeneration.XamlFileSuffix = "Foo";
            profile.ViewGeneration.ViewModelFileSuffix = "Bar";

            var sut = new SetDataContextCommandLogic(
                profile,
                DefaultTestLogger.Create(),
                new TestVisualStudioAbstraction(),
                new TestFileSystem());

            var (view, viewModel) = sut.InferViewModelNameFromFileName("TestFoo.xaml.cs");

            Assert.AreEqual("TestFoo", view);
            Assert.AreEqual("TestBar", viewModel);
        }
    }
}
