// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System.Linq;
using RapidXaml;
using RapidXamlToolkit.Logging;
using RapidXamlToolkit.Resources;

namespace RapidXamlToolkit.XamlAnalysis.CustomAnalysis
{
    public class ScrollViewerAnalyzer : BuiltInXamlAnalyzer
    {
        public ScrollViewerAnalyzer(VisualStudioIntegration.IVisualStudioAbstraction vsa, ILogger logger)
            : base(vsa, logger)
        {
        }

        public override string TargetType() => Elements.ScrollView;

        public override AnalysisActions Analyze(RapidXamlElement element, ExtraAnalysisDetails extraDetails)
        {
            if (element.Children.Any(c => c.Name == Elements.CollectionView || c.Name == Elements.WebView))
            {
                var result = AnalysisActions.EmptyList;

                foreach (var child in element.Children.Where(c => c.Name == Elements.CollectionView))
                {
                    result.Add(AnalysisActions.HighlightDescendantWithoutAction(
                        RapidXamlErrorType.Warning,
                        code: "RXM001",
                        description: "Do not put a CollectionView inside a ScrollView.",
                        child,
                        extendedMessage: "Putting a CollectionView inside a ScrollView gives the CollectionView infinite space and causes all items to be loaded which can slow page loading.",
                        moreInfoUrl: "https://learn.microsoft.com/en-us/dotnet/maui/user-interface/controls/scrollview?view=net-maui-9.0"));
                }

                foreach (var child in element.Children.Where(c => c.Name == Elements.WebView))
                {
                    result.Add(AnalysisActions.HighlightDescendantWithoutAction(
                        RapidXamlErrorType.Warning,
                        code: "RXM002",
                        description: "Do not put a WebView inside a ScrollView.",
                        child,
                        moreInfoUrl: "https://learn.microsoft.com/en-us/dotnet/maui/user-interface/controls/scrollview?view=net-maui-9.0"));
                }

                return result;
            }
            else
            {
                return AnalysisActions.None;
            }
        }
    }
}
