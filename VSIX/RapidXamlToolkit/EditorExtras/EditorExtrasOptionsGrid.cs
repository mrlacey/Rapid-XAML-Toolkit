// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.ComponentModel;
using Microsoft.VisualStudio.Shell;
using RapidXamlToolkit;

namespace RapidXaml.EditorExtras
{
    public class EditorExtrasOptionsGrid : DialogPage
    {
        [DisplayName("Show Symbol Icons")]
        [Description("Show symbols and glyphs where used in the editor.")]
        [DefaultValue(true)]
        public bool ShowSymbolIcons { get; set; } = true;

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            RapidXamlPackage.EditorOptions = this;
        }
    }
}
