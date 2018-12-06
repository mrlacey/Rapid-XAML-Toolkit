// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidXamlToolkit.Commands;
using RapidXamlToolkit.DragDrop;

namespace RapidXamlToolkit.Tests.DragDrop
{
    [TestClass]
    public class DropHandlerLogicTests : DropHandlerTestsBase
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), "profile")]
        public void CheckConstructorRequiredParam_Profile()
        {
            var fileContents = " // Just a comment";

            (IFileSystemAbstraction fs, IVisualStudioAbstraction vsa) = this.GetVbAbstractions(fileContents);

            var sut = new DropHandlerLogic(null, DefaultTestLogger.Create(), vsa);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), "logger")]
        public void CheckConstructorRequiredParam_Logger()
        {
            var profile = TestProfile.CreateEmpty();

            var fileContents = " // Just a comment";

            (IFileSystemAbstraction fs, IVisualStudioAbstraction vsa) = this.GetVbAbstractions(fileContents);

            var sut = new DropHandlerLogic(profile, null, vsa);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), "vs")]
        public void CheckConstructorRequiredParam_VS()
        {
            var profile = TestProfile.CreateEmpty();

            var sut = new DropHandlerLogic(profile, DefaultTestLogger.Create(), null);
        }
    }
}
