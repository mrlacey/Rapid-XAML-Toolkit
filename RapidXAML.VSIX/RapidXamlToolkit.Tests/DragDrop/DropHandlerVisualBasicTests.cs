// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidXamlToolkit.Commands;
using RapidXamlToolkit.DragDrop;

namespace RapidXamlToolkit.Tests.DragDrop
{
    [TestClass]
    public class DropHandlerVisualBasicTests : DropHandlerTestsBase
    {
        [TestMethod]
        public async Task FileDoesNotContainClassOrModule()
        {
            var profile = this.GetProfileForTesting();

            var fileContents = " ' Just a comment";

            (IFileSystemAbstraction fs, IVisualStudioAbstraction vsa) = this.GetVbAbstractions(fileContents);

            var sut = new DropHandlerLogic(DefaultTestLogger.Create(), vsa, fs, profile);

            var actual = await sut.ExecuteAsync("C:\\Tests\\SomeFile.vb", 8);

            string expected = null;

            StringAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public async Task FileContainsClassButNoProperties()
        {
            var profile = this.GetProfileForTesting();

            var fileContents = "Public Class TestViewModel"
       + Environment.NewLine + "    Private Property FirstProperty As String"
       + Environment.NewLine + "    Private Property SecondProperty As String"
       + Environment.NewLine + "End Class";

            (IFileSystemAbstraction fs, IVisualStudioAbstraction vsa) = this.GetVbAbstractions(fileContents);

            var sut = new DropHandlerLogic(DefaultTestLogger.Create(), vsa, fs, profile);

            var actual = await sut.ExecuteAsync("C:\\Tests\\SomeFile.vb", 8);

            var expected = "<StackPanel>"
   + Environment.NewLine + "            <!-- No accessible properties when copying as XAML -->"
   + Environment.NewLine + "        </StackPanel>";

            StringAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public async Task FileContainsClassAndPublicProperties()
        {
            var profile = this.GetProfileForTesting();

            var fileContents = "Public Class TestViewModel"
       + Environment.NewLine + "    Public Property FirstProperty As String"
       + Environment.NewLine + "    Public Property SecondProperty As String"
       + Environment.NewLine + "End Class";

            (IFileSystemAbstraction fs, IVisualStudioAbstraction vsa) = this.GetVbAbstractions(fileContents);

            var sut = new DropHandlerLogic(DefaultTestLogger.Create(), vsa, fs, profile);

            var actual = await sut.ExecuteAsync("C:\\Tests\\SomeFile.vb", 8);

            var expected = "<StackPanel>"
   + Environment.NewLine + "            <TextBlock Text=\"FirstProperty\" />"
   + Environment.NewLine + "            <TextBlock Text=\"SecondProperty\" />"
   + Environment.NewLine + "        </StackPanel>";

            StringAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public async Task FileContainsModuleButNoProperties()
        {
            var profile = this.GetProfileForTesting();

            var fileContents = "Public Module TestViewModel"
       + Environment.NewLine + "    Private Property OnlyHiddenProperty As String"
       + Environment.NewLine + "End Module";

            (IFileSystemAbstraction fs, IVisualStudioAbstraction vsa) = this.GetVbAbstractions(fileContents);

            var sut = new DropHandlerLogic(DefaultTestLogger.Create(), vsa, fs, profile);

            var actual = await sut.ExecuteAsync("C:\\Tests\\SomeFile.vb", 8);

            var expected = "<StackPanel>"
   + Environment.NewLine + "            <!-- No accessible properties when copying as XAML -->"
   + Environment.NewLine + "        </StackPanel>";

            StringAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public async Task FileContainsModuleAndPublicProperties()
        {
            var profile = this.GetProfileForTesting();

            var fileContents = "Public Module TestViewModel"
       + Environment.NewLine + "    Public Property FirstProperty As String"
       + Environment.NewLine + "    Public Property SecondProperty As String"
       + Environment.NewLine + "End Module";

            (IFileSystemAbstraction fs, IVisualStudioAbstraction vsa) = this.GetVbAbstractions(fileContents);

            var sut = new DropHandlerLogic(DefaultTestLogger.Create(), vsa, fs, profile);

            var actual = await sut.ExecuteAsync("C:\\Tests\\SomeFile.vb", 8);

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

            var fileContents = "Public Class TestViewModel"
       + Environment.NewLine + "    Public Property FirstProperty As String"
       + Environment.NewLine + "    Public Property SecondProperty As String"
       + Environment.NewLine + "End Class";

            (IFileSystemAbstraction fs, IVisualStudioAbstraction vsa) = this.GetVbAbstractions(fileContents);

            var sut = new DropHandlerLogic(DefaultTestLogger.Create(), vsa, fs, profile);

            var actual = await sut.ExecuteAsync("C:\\Tests\\SomeFile.vb", 8);

            var expected = "<TextBlock Text=\"FirstProperty\" />"
   + Environment.NewLine + "        <TextBlock Text=\"SecondProperty\" />";

            StringAssert.AreEqual(expected, actual);
        }
    }
}
