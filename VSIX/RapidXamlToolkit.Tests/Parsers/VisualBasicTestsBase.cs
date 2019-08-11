// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Threading.Tasks;
using RapidXamlToolkit.Options;
using RapidXamlToolkit.Parsers;

namespace RapidXamlToolkit.Tests.Parsers
{
    public class VisualBasicTestsBase : StarsRepresentCaretInDocsTests, IStarsRepresentCaretInDocsTests
    {
        public void EachPositionBetweenStarsShouldProduceExpected(string code, ParserOutput expected, Profile profileOverload = null)
        {
            var profile = profileOverload ?? this.FallBackProfile;

            this.EachPositionBetweenStarsShouldProduceExpected(code, expected, isCSharp: false, profileOverload: profile);
        }

        public void PositionAtStarShouldProduceExpected(string code, ParserOutput expected, Profile profileOverload = null)
        {
            var profile = profileOverload ?? this.FallBackProfile;

            this.PositionAtStarShouldProduceExpected(code, expected, isCSharp: false, profileOverload: profile);
        }

        public async Task PositionAtStarShouldProduceExpectedUsingAdditionalFiles(string code, ParserOutput expected, Profile profileOverload = null, params string[] additionalCode)
        {
            var profile = profileOverload ?? this.FallBackProfile;

            await this.PositionAtStarShouldProduceExpectedUsingAdditionalFiles(code, expected, isCSharp: false, profileOverload: profile, additionalCode: additionalCode);
        }

        public async Task PositionAtStarShouldProduceExpectedUsingAdditionalReferences(string code, ParserOutput expected, Profile profileOverload, params string[] additionalReferences)
        {
            var profile = profileOverload ?? this.FallBackProfile;

            await this.PositionAtStarShouldProduceExpectedUsingAdditionalReferences(code, expected, isCSharp: false, profileOverload: profile, additionalReferences: additionalReferences);
        }

        public async Task PositionAtStarShouldProduceExpectedUsingAdditionalLibraries(string code, ParserOutput expected, Profile profileOverload, params string[] additionalLibraryPaths)
        {
            var profile = profileOverload ?? this.FallBackProfile;

            await this.PositionAtStarShouldProduceExpectedUsingAdditionalLibraries(code, expected, isCSharp: false, profileOverload: profile, additionalLibraryPaths: additionalLibraryPaths);
        }

        public void SelectionBetweenStarsShouldProduceExpected(string code, ParserOutput expected, Profile profileOverload = null)
        {
            var profile = profileOverload ?? this.FallBackProfile;

            this.SelectionBetweenStarsShouldProduceExpected(code, expected, isCSharp: false, profileOverload: profile);
        }
    }
}
