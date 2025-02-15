// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System.Linq;
using RapidXaml;
using RapidXamlToolkit.Logging;

namespace RapidXamlToolkit.XamlAnalysis.CustomAnalysis
{
    public class UnoIgnorablesAnalyzer : BuiltInXamlAnalyzer
    {
        public UnoIgnorablesAnalyzer(VisualStudioIntegration.IVisualStudioAbstraction vsa, ILogger logger)
            : base(vsa, logger)
        {
        }

        public override string TargetType() => "Page";

        public override AnalysisActions Analyze(RapidXamlElement element, ExtraAnalysisDetails extraDetails)
        {
            if (!extraDetails.TryGet(KnownExtraDetails.Framework, out ProjectFramework framework)
             || framework != ProjectFramework.WinUI)
            {
                return AnalysisActions.None;
            }

            var result = AnalysisActions.EmptyList;

            var ignorable = element.Attributes.FirstOrDefault(a => a.Name == "mc:Ignorable");

            // https://platform.uno/docs/articles/platform-specific-xaml.html#available-prefixes
            this.CheckForXmlns("xamarin", element, ignorable, ref result);
            this.CheckForXmlns("not_win", element, ignorable, ref result);
            this.CheckForXmlns("android", element, ignorable, ref result);
            this.CheckForXmlns("ios", element, ignorable, ref result);
            this.CheckForXmlns("wasm", element, ignorable, ref result);
            this.CheckForXmlns("macos", element, ignorable, ref result);
            this.CheckForXmlns("skia", element, ignorable, ref result);
            this.CheckForXmlns("netstdref", element, ignorable, ref result);
            this.CheckForXmlns("not_netstdref", element, ignorable, ref result);

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
