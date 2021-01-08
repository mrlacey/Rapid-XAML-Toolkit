// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System.Linq;
using RapidXaml;

namespace RapidXamlToolkit.XamlAnalysis.CustomAnalysis
{
    public class UnoIgnorablesAnalyzer : BuiltInXamlAnalyzer
    {
        public UnoIgnorablesAnalyzer(VisualStudioIntegration.IVisualStudioAbstraction vsa)
            : base(vsa)
        {
        }

        public override string TargetType() => "Page";

        public override AnalysisActions Analyze(RapidXamlElement element, ExtraAnalysisDetails extraDetails)
        {
            if (!extraDetails.TryGet(KnownExtraDetails.Framework, out ProjectFramework framework)
             || framework != ProjectFramework.Uwp)
            {
                return AnalysisActions.None;
            }

            var result = AnalysisActions.EmptyList;

            var ignorable = element.Attributes.FirstOrDefault(a => a.Name == "mc:Ignorable");

            this.CheckForXmlns("xamarin", element, ignorable, ref result);
            this.CheckForXmlns("not_win", element, ignorable, ref result);
            this.CheckForXmlns("android", element, ignorable, ref result);
            this.CheckForXmlns("ios", element, ignorable, ref result);
            this.CheckForXmlns("wasm", element, ignorable, ref result);
            this.CheckForXmlns("macos", element, ignorable, ref result);

            return result;
        }

        private void CheckForXmlns(string alias, RapidXamlElement element, RapidXamlAttribute ignorable, ref AnalysisActions actions)
        {
            var ns = element.Attributes.FirstOrDefault(a => a.ToString() == $"xmlns:{alias}=\"http:/uno.ui/{alias}\"");

            if (ns != null && !ignorable.StringValue.Contains(alias))
            {
                actions.RemoveAttribute(
                    RapidXamlErrorType.Warning,
                    "RXT700",
                    "xmlns '{0}' should be marked as ignorable.".WithParams(alias),
                    "Mark {0} as ignorable".WithParams(alias),
                    ignorable)
                    .AndAddAttribute("mc:Ignorable", $"{ignorable.StringValue} {alias}");
            }
        }
    }
}
