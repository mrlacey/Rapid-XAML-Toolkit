// <copyright file="VisualBasicTestsBase.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace RapidXamlToolkit.Tests.Analysis
{
    public class VisualBasicTestsBase : StarsRepresentCaretInDocsTests, IStarsRepresentCaretInDocsTests
    {
        public void EachPositionBetweenStarsShouldProduceExpected(string code, AnalyzerOutput expected, Profile profileOverload = null)
        {
            var profile = profileOverload ?? this.DefaultProfile;

            this.EachPositionBetweenStarsShouldProduceExpected(code, expected, isCSharp: false, profileOverload: profile);
        }

        public void PositionAtStarShouldProduceExpected(string code, AnalyzerOutput expected, Profile profileOverload = null)
        {
            var profile = profileOverload ?? this.DefaultProfile;

            this.PositionAtStarShouldProduceExpected(code, expected, isCSharp: false, profileOverload: profile);
        }

        public void PositionAtStarShouldProduceExpectedUsingAdditonalFiles(string code, AnalyzerOutput expected, Profile profileOverload = null, params string[] additionalCode)
        {
            var profile = profileOverload ?? this.DefaultProfile;

            this.PositionAtStarShouldProduceExpectedUsingAdditonalFiles(code, expected, isCSharp: false, profileOverload: profile, additionalCode: additionalCode);
        }

        public void PositionAtStarShouldProduceExpectedUsingAdditonalReferences(string code, AnalyzerOutput expected, Profile profileOverload, params string[] additionalReferences)
        {
            var profile = profileOverload ?? this.DefaultProfile;

            this.PositionAtStarShouldProduceExpectedUsingAdditonalReferences(code, expected, isCSharp: false, profileOverload: profile, additionalReferences: additionalReferences);
        }

        public void PositionAtStarShouldProduceExpectedUsingAdditonalLibraries(string code, AnalyzerOutput expected, Profile profileOverload, params string[] additionalLibraryPaths)
        {
            var profile = profileOverload ?? this.DefaultProfile;

            this.PositionAtStarShouldProduceExpectedUsingAdditonalLibraries(code, expected, isCSharp: false, profileOverload: profile, additionalLibraryPaths: additionalLibraryPaths);
        }

        public void SelectionBetweenStarsShouldProduceExpected(string code, AnalyzerOutput expected, Profile profileOverload = null)
        {
            var profile = profileOverload ?? this.DefaultProfile;

            this.SelectionBetweenStarsShouldProduceExpected(code, expected, isCSharp: false, profileOverload: profile);
        }
    }
}
