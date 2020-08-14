// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RapidXaml.AutoFixExe
{
    class Program
    {
        static void Main(string[] args)
        {
            // Expect
            // - first arg = path to assembly containing fixes
            // - second arg = path to *.xaml file or directory

            // TODO: check args
            // - if files, check exists
            // - if directory, find xaml files in that and sub-directories

            // TODO: Get fixes from assembly
            // - This can use all AnalysisActions but Highlight* will do nothing

            // TODO: Parse each file in turn
            // - parse to get tags
            // XamlElementExtractor.Parse
            // Ignore ProjectType
            // A version of CustomProcessorWrapper.Process should do this
            // - then fix each tagged instance
            // Can base this on CustomAnalysisAction.InnerExecute
            //  but will need a new VsAbstraction (that doesn't need to worry about line number offset)


        }
    }
}
