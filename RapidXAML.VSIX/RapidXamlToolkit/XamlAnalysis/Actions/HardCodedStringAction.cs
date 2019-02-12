// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Threading;
using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Imaging.Interop;
using Microsoft.VisualStudio.Text.Editor;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.XamlAnalysis.Actions
{
    public class HardCodedStringAction : BaseSuggestedAction
    {
        private string file;
        private ITextView view;
        private HardCodedStringTag tag;

        public override ImageMoniker IconMoniker => KnownMonikers.GenerateResource;

        public override string DisplayText
        {
            get { return "Move hard coded string to resource file."; }
        }

        public static HardCodedStringAction Create(HardCodedStringTag tag, string file, ITextView view)
        {
            var result = new HardCodedStringAction
            {
                tag = tag,
                file = file,
                view = view,
            };

            return result;
        }

        public override void Execute(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
