// <copyright file="CSharpTestsBase.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace RapidXamlToolkit.Tests.Analysis
{
    public class CSharpTestsBase : StarsRepresentCaretInDocsTests, IStarsRepresentCaretInDocsTests
    {
        public void EachPositionBetweenStarsShouldProduceExpected(string code, AnalyzerOutput expected, Profile profileOverload = null)
        {
            var profile = profileOverload ?? this.DefaultProfile;

            this.EachPositionBetweenStarsShouldProduceExpected(code, expected, isCSharp: true, profileOverload: profile);
        }

        public void PositionAtStarShouldProduceExpected(string code, AnalyzerOutput expected, Profile profileOverload = null)
        {
            var profile = profileOverload ?? this.DefaultProfile;

            this.PositionAtStarShouldProduceExpected(code, expected, isCSharp: true, profileOverload: profile);
        }

        public void SelectionBetweenStarsShouldProduceExpected(string code, AnalyzerOutput expected, Profile profileOverload = null)
        {
            var profile = profileOverload ?? this.DefaultProfile;

            this.SelectionBetweenStarsShouldProduceExpected(code, expected, isCSharp: true, profileOverload: profile);
        }
    }
}
