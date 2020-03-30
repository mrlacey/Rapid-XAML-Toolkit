// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.ComponentModel;
using Microsoft.VisualStudio.Shell;

namespace RapidXamlToolkit
{
    public class AnalysisOptionsGrid : DialogPage
    {
        [Category("Experimental")]
        [DisplayName("Enable Custom Analysis")]
        [Description("Attempt to load additional analyzers from referenced libraries.")]
        public bool EnableCustomAnalysis { get; set; } = false;

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            RapidXamlAnalysisPackage.Options = this;
        }
    }
}
