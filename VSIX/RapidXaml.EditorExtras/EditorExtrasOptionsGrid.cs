// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Shell;

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

            RapidXamlEditorExtrasPackage.Options = this;
        }
    }
}
