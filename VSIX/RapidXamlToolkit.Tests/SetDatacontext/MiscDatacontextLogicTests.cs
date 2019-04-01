// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidXamlToolkit.Commands;

namespace RapidXamlToolkit.Tests.SetDatacontext
{
    [TestClass]
    public class MiscDatacontextLogicTests
    {
        [TestMethod]
        public void CanGetLineEndPos_FirstLine()
        {
            var file = "abc"
+ Environment.NewLine + "678"
+ Environment.NewLine + "123";

            var testResult = SetDataContextCommandLogic.GetLineEndPos(file, 1);

            Assert.AreEqual(3 + (Environment.NewLine.Length * 1), testResult);
        }

        [TestMethod]
        public void CanGetLineEndPos_SecondLine()
        {
            var file = "abc"
+ Environment.NewLine + "67"
+ Environment.NewLine + "012";

            var testResult = SetDataContextCommandLogic.GetLineEndPos(file, 2);

            Assert.AreEqual(5 + (Environment.NewLine.Length * 2), testResult);
        }

        [TestMethod]
        public void CanGetLineEndPos_ThirdLine()
        {
            var file = "1"
+ Environment.NewLine + "45"
+ Environment.NewLine + "890"
+ Environment.NewLine + "abcd";

            var testResult = SetDataContextCommandLogic.GetLineEndPos(file, 3);

            Assert.AreEqual(6 + (Environment.NewLine.Length * 3), testResult);
        }

        [TestMethod]
        public void CanGetLineEndPos_FifthLine()
        {
            var file = "1"
+ Environment.NewLine + ""
+ Environment.NewLine + "123"
+ Environment.NewLine + "abcd"
+ Environment.NewLine + "-|-|-|-"
+ Environment.NewLine + "12345678";

            var testResult = SetDataContextCommandLogic.GetLineEndPos(file, 5);

            Assert.AreEqual(15 + (Environment.NewLine.Length * 5), testResult);
        }

        [TestMethod]
        public void CanGetLineEndPos_BlankFirstLine()
        {
            var file = ""
+ Environment.NewLine + "678"
+ Environment.NewLine + "123";

            var testResult = SetDataContextCommandLogic.GetLineEndPos(file, 1);

            Assert.AreEqual(Environment.NewLine.Length, testResult);
        }

        [TestMethod]
        public void CanGetLineEndPos_BlankThirdLine()
        {
            var file = "abc"
+ Environment.NewLine + ""
+ Environment.NewLine + ""
+ Environment.NewLine + "012";

            var testResult = SetDataContextCommandLogic.GetLineEndPos(file, 3);

            Assert.AreEqual(3 + (Environment.NewLine.Length * 3), testResult);
        }

        [TestMethod]
        public void CorrectlyInferViewModelName_WhenNoSuffixes()
        {
            var sut = new SetDataContextCommandLogic(
                TestProfile.CreateEmpty(),
                DefaultTestLogger.Create(),
                new TestVisualStudioAbstraction(),
                new TestFileSystem());

            var (view, viewModel, _) = sut.InferViewModelNameFromFileName("Test.xaml.cs");

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

            var (view, viewModel, _) = sut.InferViewModelNameFromFileName("TestPage.xaml.cs");

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

            var (view, viewModel, _) = sut.InferViewModelNameFromFileName("TestPage.xaml.cs");

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

            var (view, viewModel, _) = sut.InferViewModelNameFromFileName("TestPage.xaml.cs");

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

            var (view, viewModel, _) = sut.InferViewModelNameFromFileName("TestPage.xaml.cs");

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

            var (view, viewModel, _) = sut.InferViewModelNameFromFileName("TestPage.xaml.cs");

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

            var (view, viewModel, _) = sut.InferViewModelNameFromFileName("TestFoo.xaml.cs");

            Assert.AreEqual("TestFoo", view);
            Assert.AreEqual("TestBar", viewModel);
        }

        [TestMethod]
        public void CorrectlyInferViewModelNameSpace()
        {
            var profile = TestProfile.CreateEmpty();
            profile.ViewGeneration.ViewModelDirectoryName = "VMS";

            var sut = new SetDataContextCommandLogic(
                profile,
                DefaultTestLogger.Create(),
                new TestVisualStudioAbstraction
                {
                    ActiveProject = new ProjectWrapper { Name = "TestApp" },
                },
                new TestFileSystem());

            var (_, _, vmNamespace) = sut.InferViewModelNameFromFileName("Test.xaml.cs");

            Assert.AreEqual("TestApp.VMS", vmNamespace);
        }
    }
}
