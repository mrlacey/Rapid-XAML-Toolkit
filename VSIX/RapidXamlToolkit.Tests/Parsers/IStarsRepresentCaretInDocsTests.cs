// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System.Threading.Tasks;
using RapidXamlToolkit.Options;
using RapidXamlToolkit.Parsers;

namespace RapidXamlToolkit.Tests.Parsers
{
    public interface IStarsRepresentCaretInDocsTests
    {
        void EachPositionBetweenStarsShouldProduceExpected(string code, ParserOutput expected, Profile profileOverload);

        void PositionAtStarShouldProduceExpected(string code, ParserOutput expected, Profile profileOverload);

        void SelectionBetweenStarsShouldProduceExpected(string code, ParserOutput expected, Profile profileOverload);

        Task PositionAtStarShouldProduceExpectedUsingAdditionalFiles(string code, ParserOutput expected, Profile profileOverload, params string[] additionalCode);

        Task PositionAtStarShouldProduceExpectedUsingAdditionalReferences(string code, ParserOutput expected, Profile profileOverload, params string[] additionalReferences);

        Task PositionAtStarShouldProduceExpectedUsingAdditionalLibraries(string code, ParserOutput expected, Profile profileOverload, params string[] additionalLibraryPaths);
    }
}
