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
    // TODO: remove when not needed - this exists for testing SuggestionTags/Actions
    public class OtherHardCodedStringAction : BaseSuggestedAction
    {
        private string file;
        private ITextView view;
        private OtherHardCodedStringTag tag;

        public OtherHardCodedStringAction()
        {
            this.DisplayText = StringRes.UI_MoveHardCodedString;
        }

        public override ImageMoniker IconMoniker => KnownMonikers.GenerateResource;

        public static OtherHardCodedStringAction Create(OtherHardCodedStringTag tag, string file, ITextView view)
        {
            var result = new OtherHardCodedStringAction
            {
                tag = tag,
                file = file,
                view = view,
            };

            return result;
        }

        public override void Execute(CancellationToken cancellationToken)
        {
            // Not really doing anything here as this action is just for development reference.
            System.Diagnostics.Debug.WriteLine("Executing OtherHardCodedStringAction");
        }
    }
}
