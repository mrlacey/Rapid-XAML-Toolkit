// <copyright file="SetDatacontextInXamlTests.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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

            var fs = new TestFileSystem { };

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

            var fs = new TestFileSystem { };

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

            var vs = new TestVisualStudioAbstraction
            {
                ActiveDocumentFileName = "test.xaml",
                ActiveDocumentText = @"<Page
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    >
    <!-- Content would go here -->
</Page>",
            };

            var fs = new TestFileSystem { };

            var sut = new SetDataContextCommandLogic(profile, logger, vs, fs);

            var result = sut.GetPageAttributeToAdd("TestViewModel");

            Assert.IsTrue(result.anythingToAdd);
            Assert.AreEqual(2, result.lineNoToAddAfter);
            Assert.AreEqual($"{Environment.NewLine}    DataContext=\"HasBeenSet\"", result.contentToAdd);
        }
    }
}
