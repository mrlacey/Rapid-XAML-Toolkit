// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WindowsTestHelpers;

namespace RapidXamlToolkit.Tests.Manual
{
    public class TestsUsingWinAppDriver
    {
        [TestInitialize]
        public void SetUp()
        {
            WinAppDriverHelper.CheckIsInstalled();

            WinAppDriverHelper.StartIfNotRunning();

            VirtualKeyboard.MinimizeAllWindows();
        }

        [TestCleanup]
        public void TearDown()
        {
            WinAppDriverHelper.StopIfRunning();
        }
    }
}
