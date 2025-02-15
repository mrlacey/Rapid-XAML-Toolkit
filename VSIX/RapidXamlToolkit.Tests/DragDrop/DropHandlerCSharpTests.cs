﻿// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidXamlToolkit.DragDrop;
using RapidXamlToolkit.Utils.IO;
using RapidXamlToolkit.VisualStudioIntegration;

namespace RapidXamlToolkit.Tests.DragDrop
{
    [TestClass]
    public class DropHandlerCSharpTests : DropHandlerTestsBase
    {
        [TestMethod]
        public async Task FileDoesNotContainClass()
        {
            var profile = this.GetProfileForTesting();

            var fileContents = " // Just a comment";

            (IFileSystemAbstraction fs, IVisualStudioAbstractionAndDocumentModelAccess vsa) = this.GetCSAbstractions(fileContents);

            var sut = new DropHandlerLogic(DefaultTestLogger.Create(), vsa, fs, profile);

            var actual = await sut.ExecuteAsync("C:\\Tests\\SomeFile.cs", 8, profile.ProjectType);

            string expected = null;

            StringAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public async Task FileContainsClassButNoProperties()
        {
            var profile = this.GetProfileForTesting();

            var fileContents = "public class TestViewModel"
       + Environment.NewLine + "{"
       + Environment.NewLine + "    private string FirstProperty { get; set; }"
       + Environment.NewLine + "    private string SecondProperty { get; set; }"
       + Environment.NewLine + "}";

            (IFileSystemAbstraction fs, IVisualStudioAbstractionAndDocumentModelAccess vsa) = this.GetCSAbstractions(fileContents);

            var sut = new DropHandlerLogic(DefaultTestLogger.Create(), vsa, fs, profile);

            var actual = await sut.ExecuteAsync("C:\\Tests\\SomeFile.cs", 8, profile.ProjectType);

            var expected = "<StackPanel>"
   + Environment.NewLine + "            <!-- No accessible properties found for XAML in 'TestViewModel' -->"
   + Environment.NewLine + "        </StackPanel>";

            StringAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public async Task FileContainsClassAndPublicProperties()
        {
            var profile = this.GetProfileForTesting();

            var fileContents = "public class TestViewModel"
       + Environment.NewLine + "{"
       + Environment.NewLine + "    public string FirstProperty { get; set; }"
       + Environment.NewLine + "    public string SecondProperty { get; set; }"
       + Environment.NewLine + "}";

            (IFileSystemAbstraction fs, IVisualStudioAbstractionAndDocumentModelAccess vsa) = this.GetCSAbstractions(fileContents);

            var sut = new DropHandlerLogic(DefaultTestLogger.Create(), vsa, fs, profile);

            var actual = await sut.ExecuteAsync("C:\\Tests\\SomeFile.cs", 8, profile.ProjectType);

            var expected = "<StackPanel>"
   + Environment.NewLine + "            <TextBlock Text=\"FirstProperty\" />"
   + Environment.NewLine + "            <TextBlock Text=\"SecondProperty\" />"
   + Environment.NewLine + "        </StackPanel>";

            StringAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public async Task FileContainsClassAndPublicPropertiesButNoClassGrouping()
        {
            var profile = this.GetProfileForTesting();
            profile.ClassGrouping = string.Empty;

            var fileContents = "public class TestViewModel"
       + Environment.NewLine + "{"
       + Environment.NewLine + "    public string FirstProperty { get; set; }"
       + Environment.NewLine + "    public string SecondProperty { get; set; }"
       + Environment.NewLine + "}";

            (IFileSystemAbstraction fs, IVisualStudioAbstractionAndDocumentModelAccess vsa) = this.GetCSAbstractions(fileContents);

            var sut = new DropHandlerLogic(DefaultTestLogger.Create(), vsa, fs, profile);

            var actual = await sut.ExecuteAsync("C:\\Tests\\SomeFile.cs", 8, profile.ProjectType);

            var expected = "<TextBlock Text=\"FirstProperty\" />"
   + Environment.NewLine + "        <TextBlock Text=\"SecondProperty\" />";

            StringAssert.AreEqual(expected, actual);
        }
    }
}
