// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using RapidXamlToolkit.Options;
using RapidXamlToolkit.Parsers;

namespace RapidXamlToolkit.Tests.Analysis
{
    public class VisualBasicTestsBase : StarsRepresentCaretInDocsTests, IStarsRepresentCaretInDocsTests
    {
        public void EachPositionBetweenStarsShouldProduceExpected(string code, ParserOutput expected, Profile profileOverload = null)
        {
            var profile = profileOverload ?? this.DefaultProfile;

            this.EachPositionBetweenStarsShouldProduceExpected(code, expected, isCSharp: false, profileOverload: profile);
        }

        public void PositionAtStarShouldProduceExpected(string code, ParserOutput expected, Profile profileOverload = null)
        {
            var profile = profileOverload ?? this.DefaultProfile;

            this.PositionAtStarShouldProduceExpected(code, expected, isCSharp: false, profileOverload: profile);
        }

        public void PositionAtStarShouldProduceExpectedUsingAdditonalFiles(string code, ParserOutput expected, Profile profileOverload = null, params string[] additionalCode)
        {
            var profile = profileOverload ?? this.DefaultProfile;

            this.PositionAtStarShouldProduceExpectedUsingAdditonalFiles(code, expected, isCSharp: false, profileOverload: profile, additionalCode: additionalCode);
        }

        public void PositionAtStarShouldProduceExpectedUsingAdditonalReferences(string code, ParserOutput expected, Profile profileOverload, params string[] additionalReferences)
        {
            var profile = profileOverload ?? this.DefaultProfile;

            this.PositionAtStarShouldProduceExpectedUsingAdditonalReferences(code, expected, isCSharp: false, profileOverload: profile, additionalReferences: additionalReferences);
        }

        public void PositionAtStarShouldProduceExpectedUsingAdditonalLibraries(string code, ParserOutput expected, Profile profileOverload, params string[] additionalLibraryPaths)
        {
            var profile = profileOverload ?? this.DefaultProfile;

            this.PositionAtStarShouldProduceExpectedUsingAdditonalLibraries(code, expected, isCSharp: false, profileOverload: profile, additionalLibraryPaths: additionalLibraryPaths);
        }

        public void SelectionBetweenStarsShouldProduceExpected(string code, ParserOutput expected, Profile profileOverload = null)
        {
            var profile = profileOverload ?? this.DefaultProfile;

            this.SelectionBetweenStarsShouldProduceExpected(code, expected, isCSharp: false, profileOverload: profile);
        }
    }
}
