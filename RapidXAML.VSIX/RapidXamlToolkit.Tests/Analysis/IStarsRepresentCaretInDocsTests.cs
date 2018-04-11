// <copyright file="IStarsRepresentCaretInDocsTests.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace RapidXamlToolkit.Tests.Analysis
{
    public interface IStarsRepresentCaretInDocsTests
    {
        void EachPositionBetweenStarsShouldProduceExpected(string code, AnalyzerOutput expected, Profile profileOverload);

        void PositionAtStarShouldProduceExpected(string code, AnalyzerOutput expected, Profile profileOverload);

        void SelectionBetweenStarsShouldProduceExpected(string code, AnalyzerOutput expected, Profile profileOverload);

        void PositionAtStarShouldProduceExpectedUsingAdditonalFiles(string code, AnalyzerOutput expected, Profile profileOverload, params string[] additionalCode);

        void PositionAtStarShouldProduceExpectedUsingAdditonalReferences(string code, AnalyzerOutput expected, Profile profileOverload, params string[] additionalReferences);
    }
}
