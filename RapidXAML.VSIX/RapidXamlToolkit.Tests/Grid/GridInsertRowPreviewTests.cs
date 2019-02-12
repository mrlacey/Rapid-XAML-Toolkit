// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.Text;
using RapidXamlToolkit.XamlAnalysis;
using RapidXamlToolkit.XamlAnalysis.Actions;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.Tests.Grid
{
    [TestClass]
    public class GridInsertRowPreviewTests
    {
        [TestMethod]
        public void CreatePreviewCorrectly()
        {
            var original = "<Page>"
   + Environment.NewLine + "    <Grid>"
   + Environment.NewLine + "        <Grid.RowDefinitions>"
   + Environment.NewLine + "            <RowDefinition Height=\"*\">"
   + Environment.NewLine + "            ☆<RowDefinition Height=\"Auto\">"
   + Environment.NewLine + "            <RowDefinition Height=\"*\">"
   + Environment.NewLine + "        </Grid.RowDefinitions>"
   + Environment.NewLine + ""
   + Environment.NewLine + "        <TextBlock Text=\"Footer\" Grid.Row=\"2\" />"
   + Environment.NewLine + "    </Grid>"
   + Environment.NewLine + "</Page>";

            var expected = "<Page>"
   + Environment.NewLine + "    <Grid>"
   + Environment.NewLine + "        <Grid.RowDefinitions>"
   + Environment.NewLine + "            <RowDefinition Height=\"*\">"
   + Environment.NewLine + "            <RowDefinition Height=\"XXX\">"
   + Environment.NewLine + "            <RowDefinition Height=\"Auto\">"
   + Environment.NewLine + "            <RowDefinition Height=\"*\">"
   + Environment.NewLine + "        </Grid.RowDefinitions>"
   + Environment.NewLine + ""
   + Environment.NewLine + "        <TextBlock Text=\"Footer\" Grid.Row=\"3\" />"
   + Environment.NewLine + "    </Grid>"
   + Environment.NewLine + "</Page>";

            var tag = new InsertRowDefinitionTag(new Span(0, 0), new FakeTextSnapshot())
            {
                XamlTag = "<RowDefinition Height=\"XXX\">",
                InsertPoint = original.IndexOf("☆", StringComparison.Ordinal),
                GridStartPos = 12,
            };

            var actual = InsertRowDefinitionAction.GetPreviewText(original.Replace("☆", string.Empty), InsertRowDefinitionAction.GetReplacements(1, 3), null, tag);

            StringAssert.AreEqual(expected, actual);
        }
    }
}
