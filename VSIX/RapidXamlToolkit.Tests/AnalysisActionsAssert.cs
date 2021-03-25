// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidXaml;

namespace RapidXamlToolkit.Tests
{
    public static class AnalysisActionsAssert
    {
        public static void HasOneActionToRemoveHardCodedString(AnalysisActions actions)
        {
            Assert.IsFalse(actions.IsNone);
            Assert.AreEqual(1, actions.Actions.Count);
            Assert.AreEqual(ActionType.CreateResource, actions.Actions[0].Action);
            Assert.AreEqual("RXT200", actions.Actions[0].Code);
            Assert.AreEqual(2, actions.Actions[0].SupplementaryActions.Count);
            Assert.AreEqual(ActionType.RemoveAttribute, actions.Actions[0].SupplementaryActions[0].Action);
            Assert.AreEqual(ActionType.AddAttribute, actions.Actions[0].SupplementaryActions[1].Action);
        }
    }
}
