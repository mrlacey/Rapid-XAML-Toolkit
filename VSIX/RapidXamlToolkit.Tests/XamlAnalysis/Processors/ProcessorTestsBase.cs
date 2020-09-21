// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using RapidXamlToolkit.XamlAnalysis;
using RapidXamlToolkit.XamlAnalysis.Processors;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.Tests.XamlAnalysis.Processors
{
    public class ProcessorTestsBase
    {
        internal List<IRapidXamlAdornmentTag> GetTags<T>(string xaml, ProjectType projectType = ProjectType.Any)
            where T : XamlElementProcessor
        {
            var outputTags = new TagList();

            var sut = (T)Activator.CreateInstance(typeof(T), new ProcessorEssentialsForSimpleTests(projectType));

            var snapshot = new FakeTextSnapshot(xaml.Length);

            sut.Process("testfile.xaml", 0, xaml, string.Empty, snapshot, outputTags);

            return outputTags;
        }
    }
}
