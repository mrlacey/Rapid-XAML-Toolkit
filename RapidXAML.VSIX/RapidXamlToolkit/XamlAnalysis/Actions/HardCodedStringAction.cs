// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Threading;
using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Imaging.Interop;
using Microsoft.VisualStudio.Text.Editor;
using RapidXamlToolkit.Resources;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.XamlAnalysis.Actions
{
    public class HardCodedStringAction : BaseSuggestedAction
    {
        private HardCodedStringTag tag;

        public HardCodedStringAction(string file)
            : base(file)
        {
            this.DisplayText = StringRes.UI_MoveHardCodedString;
        }

        public override ImageMoniker IconMoniker => KnownMonikers.GenerateResource;

        public static HardCodedStringAction Create(HardCodedStringTag tag, string file, ITextView view)
        {
            var result = new HardCodedStringAction(file)
            {
                tag = tag,
                View = view,
            };

            return result;
        }

        public override void Execute(CancellationToken cancellationToken)
        {
            // TODO: need to create missing resource
            // assign UID (name if not UWP) to element if it doesn't have one - how to determine this
            // determine which file to add to - need setting for rule on creation if none exist
            // remove existing tag
            // create entry in the resource file (& open it? - configurable?)
        }
    }
}
