// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.ComponentModel;
using Microsoft.VisualStudio.Shell;

namespace RapidXamlToolkit
{
    public class AnalysisOptionsGrid : DialogPage
    {
        [DisplayName("Analyze On Saved")]
        [Description("(Re)Ananlyze the XAML document every time it is saved.")]
        public bool AnalyzeWhenDocumentSaved { get; set; } = true;

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
