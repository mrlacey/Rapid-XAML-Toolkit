// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidXamlToolkit.Commands;

namespace RapidXamlToolkit.Tests.SetDatacontext
{
    [TestClass]
    public class SetDatacontextInXamlTests
    {
        [TestMethod]
        public void CanDetectIfAlreadySet()
        {
            var profile = TestProfile.CreateEmpty();
            profile.Datacontext.XamlPageAttribute = "DataContext=\"HasBeenSet\"";

            var logger = DefaultTestLogger.Create();

            var vs = new TestVisualStudioAbstraction
            {
                ActiveDocumentFileName = "test.xaml",
                ActiveDocumentText = @"<Page
    DataContext=""HasBeenSet""
    >
    <!-- Content would go here -->
</Page>",
            };

            var fs = new TestFileSystem();

            var sut = new SetDataContextCommandLogic(profile, logger, vs, fs);

            var result = sut.ShouldEnableCommand();

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void CanDetectIfNotAlreadySet()
        {
            var profile = TestProfile.CreateEmpty();
            profile.Datacontext.XamlPageAttribute = "DataContext=\"HasBeenSet\"";

            var logger = DefaultTestLogger.Create();

            var vs = new TestVisualStudioAbstraction
            {
                ActiveDocumentFileName = "test.xaml",
                ActiveDocumentText = @"<Page
    >
    <!-- Content would go here -->
</Page>",
            };

            var fs = new TestFileSystem();

            var sut = new SetDataContextCommandLogic(profile, logger, vs, fs);

            var result = sut.ShouldEnableCommand();

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void CanDetectWhereAndWhatToInsert()
        {
            var profile = TestProfile.CreateEmpty();
            profile.Datacontext.XamlPageAttribute = "DataContext=\"HasBeenSet\"";

            var logger = DefaultTestLogger.Create();

            var activeDocText = "<Page"
        + Environment.NewLine + "    xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\""
        + Environment.NewLine + "    >"
        + Environment.NewLine + "    <!-- Content would go here -->"
        + Environment.NewLine + "</Page>";

            var vs = new TestVisualStudioAbstraction
            {
                ActiveDocumentFileName = "test.xaml",
                ActiveDocumentText = activeDocText,
            };

            var fs = new TestFileSystem();

            var sut = new SetDataContextCommandLogic(profile, logger, vs, fs);

            var (anythingToAdd, lineNoToAddAfter, contentToAdd) = sut.GetPageAttributeToAdd("TestViewModel", "Tests");

            Assert.IsTrue(anythingToAdd);
            Assert.AreEqual(2, lineNoToAddAfter);
            Assert.AreEqual($"{Environment.NewLine}    DataContext=\"HasBeenSet\"", contentToAdd);
        }
    }
}
