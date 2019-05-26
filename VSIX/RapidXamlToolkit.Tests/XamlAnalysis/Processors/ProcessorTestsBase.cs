// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidXamlToolkit.XamlAnalysis;
using RapidXamlToolkit.XamlAnalysis.Processors;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.Tests.XamlAnalysis.Processors
{
    public class ProcessorTestsBase
    {
        internal List<IRapidXamlAdornmentTag> GetTags<T>(string xaml)
            where T : XamlElementProcessor
        {
            var outputTags = new TagList();

            var sut = (T)Activator.CreateInstance<T>();

            var snapshot = new FakeTextSnapshot();

            sut.Process("testfile.xaml", 0, xaml, string.Empty, snapshot, outputTags);

            return outputTags;
        }
    }
}
