// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using RapidXaml;

namespace RapidXamlToolkit.XamlAnalysis.CustomAnalysis
{
    public class AddXmlnsAnalyzer : BuiltInXamlAnalyzer
    {
        public AddXmlnsAnalyzer(VisualStudioIntegration.IVisualStudioAbstraction vsa)
            : base(vsa)
        {
        }

        public override string TargetType() => "WebView";

        public override AnalysisActions Analyze(RapidXamlElement element, ExtraAnalysisDetails extraDetails)
        {
            var defaultAlias = "newns";
            var nsvalue = "using:mynewnamespace";

            var aliasToUse = defaultAlias;

            extraDetails.TryGet(KnownExtraDetails.Xmlns, out Dictionary<string, string> xmlns);

            if (xmlns != null)
            {
                if (xmlns.ContainsKey(defaultAlias))
                {
                    if (xmlns[defaultAlias] == nsvalue)
                    {
                        // What would be added is already there
                        return AnalysisActions.EmptyList;
                    }
                    else
                    {
                        if (xmlns.ContainsValue(nsvalue))
                        {
                            // It already exists with a different alias
                            // In other scenarios, this might affect what is used in other operations
                        }

                        var suffix = 1;
                        while (xmlns.ContainsKey(aliasToUse))
                        {
                            aliasToUse = defaultAlias + suffix++.ToString();
                        }
                    }
                }
            }

            return AnalysisActions.AddXmlns(
                RapidXamlErrorType.Warning,
                "addns",
                $"add xmlns ({aliasToUse})",
                "add xmlns",
                aliasToUse,
                nsvalue);
        }
    }
}
