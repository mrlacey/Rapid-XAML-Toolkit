// Copyright (c) Matt Lacey Ltd.. All rights reserved.
// Licensed under the MIT license.

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidXamlToolkit.Commands;
using RapidXamlToolkit.DragDrop;
using RapidXamlToolkit.VisualStudioIntegration;

namespace RapidXamlToolkit.Tests.DragDrop
{
    [TestClass]
    public class DropHandlerLogicTests : DropHandlerTestsBase
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), "logger")]
        public void CheckConstructorRequiredParam_Logger()
        {
            var fileContents = " // Just a comment";

            (IFileSystemAbstraction _, IVisualStudioAbstraction vsa) = this.GetVbAbstractions(fileContents);

            var sut = new DropHandlerLogic(null, vsa);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), "vs")]
        public void CheckConstructorRequiredParam_VS()
        {
            var sut = new DropHandlerLogic(DefaultTestLogger.Create(), null);
        }
    }
}
