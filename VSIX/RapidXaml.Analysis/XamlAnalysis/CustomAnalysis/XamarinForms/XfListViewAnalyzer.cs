// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Linq;
using RapidXaml;
using RapidXamlToolkit.Resources;

namespace RapidXamlToolkit.XamlAnalysis.CustomAnalysis
{
    public class XfListViewAnalyzer : BuiltInXamlAnalyzer
    {
        public override string TargetType() => Elements.ListView;

        public override AnalysisActions Analyze(RapidXamlElement element, ExtraAnalysisDetails extraDetails)
        {
            if (!extraDetails.TryGet("framework", out ProjectFramework framework)
             || framework != ProjectFramework.XamarinForms)
            {
                return AnalysisActions.None;
            }

            return AnalysisActions.HighlightDescendantWithoutAction(
            errorType: RapidXamlErrorType.Suggestion,
            code: "RXT330",
            description: "CollectionView is a more flexible, and performant alternative to ListView. Consider changing it.",
            moreInfoUrl: "https://docs.microsoft.com/en-us/xamarin/xamarin-forms/user-interface/collectionview/",
            descendant: element);
        }
    }
}
