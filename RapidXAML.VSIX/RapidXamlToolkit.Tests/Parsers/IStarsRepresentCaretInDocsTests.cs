// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using RapidXamlToolkit.Options;
using RapidXamlToolkit.Parsers;

namespace RapidXamlToolkit.Tests.Analysis
{
    public interface IStarsRepresentCaretInDocsTests
    {
        void EachPositionBetweenStarsShouldProduceExpected(string code, ParserOutput expected, Profile profileOverload);

        void PositionAtStarShouldProduceExpected(string code, ParserOutput expected, Profile profileOverload);

        void SelectionBetweenStarsShouldProduceExpected(string code, ParserOutput expected, Profile profileOverload);

        void PositionAtStarShouldProduceExpectedUsingAdditonalFiles(string code, ParserOutput expected, Profile profileOverload, params string[] additionalCode);

        void PositionAtStarShouldProduceExpectedUsingAdditonalReferences(string code, ParserOutput expected, Profile profileOverload, params string[] additionalReferences);

        void PositionAtStarShouldProduceExpectedUsingAdditonalLibraries(string code, ParserOutput expected, Profile profileOverload, params string[] additionalLibraryPaths);
    }
}
