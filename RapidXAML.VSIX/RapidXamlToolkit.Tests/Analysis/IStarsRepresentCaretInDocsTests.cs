// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using RapidXamlToolkit.Analyzers;
using RapidXamlToolkit.Options;

namespace RapidXamlToolkit.Tests.Analysis
{
    public interface IStarsRepresentCaretInDocsTests
    {
        void EachPositionBetweenStarsShouldProduceExpected(string code, AnalyzerOutput expected, Profile profileOverload);

        void PositionAtStarShouldProduceExpected(string code, AnalyzerOutput expected, Profile profileOverload);

        void SelectionBetweenStarsShouldProduceExpected(string code, AnalyzerOutput expected, Profile profileOverload);

        void PositionAtStarShouldProduceExpectedUsingAdditonalFiles(string code, AnalyzerOutput expected, Profile profileOverload, params string[] additionalCode);

        void PositionAtStarShouldProduceExpectedUsingAdditonalReferences(string code, AnalyzerOutput expected, Profile profileOverload, params string[] additionalReferences);

        void PositionAtStarShouldProduceExpectedUsingAdditonalLibraries(string code, AnalyzerOutput expected, Profile profileOverload, params string[] additionalLibraryPaths);
    }
}
