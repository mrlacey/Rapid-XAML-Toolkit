// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Threading.Tasks;
using RapidXamlToolkit.Options;
using RapidXamlToolkit.Parsers;

namespace RapidXamlToolkit.Tests.Parsers
{
    public class CSharpTestsBase : StarsRepresentCaretInDocsTests, IStarsRepresentCaretInDocsTests
    {
        public void EachPositionBetweenStarsShouldProduceExpected(string code, ParserOutput expected, Profile profileOverload = null)
        {
            var profile = profileOverload ?? this.DefaultProfile;

            this.EachPositionBetweenStarsShouldProduceExpected(code, expected, isCSharp: true, profileOverload: profile);
        }

        public void PositionAtStarShouldProduceExpected(string code, ParserOutput expected, Profile profileOverload = null)
        {
            var profile = profileOverload ?? this.DefaultProfile;

            this.PositionAtStarShouldProduceExpected(code, expected, isCSharp: true, profileOverload: profile);
        }

        public async Task PositionAtStarShouldProduceExpectedUsingAdditionalFiles(string code, ParserOutput expected, Profile profileOverload, params string[] additionalCode)
        {
            var profile = profileOverload ?? this.DefaultProfile;

            await this.PositionAtStarShouldProduceExpectedUsingAdditionalFiles(code, expected, isCSharp: true, profileOverload: profile, additionalCode: additionalCode);
        }

        public async Task PositionAtStarShouldProduceExpectedUsingAdditionalReferences(string code, ParserOutput expected, Profile profileOverload, params string[] additionalReferences)
        {
            var profile = profileOverload ?? this.DefaultProfile;

            await this.PositionAtStarShouldProduceExpectedUsingAdditionalReferences(code, expected, isCSharp: true, profileOverload: profile, additionalReferences: additionalReferences);
        }

        public async Task PositionAtStarShouldProduceExpectedUsingAdditionalLibraries(string code, ParserOutput expected, Profile profileOverload, params string[] additionalLibraryPaths)
        {
            var profile = profileOverload ?? this.DefaultProfile;

            await this.PositionAtStarShouldProduceExpectedUsingAdditionalLibraries(code, expected, isCSharp: true, profileOverload: profile, additionalLibraryPaths: additionalLibraryPaths);
        }

        public void SelectionBetweenStarsShouldProduceExpected(string code, ParserOutput expected, Profile profileOverload = null)
        {
            var profile = profileOverload ?? this.DefaultProfile;

            this.SelectionBetweenStarsShouldProduceExpected(code, expected, isCSharp: true, profileOverload: profile);
        }
    }
}
