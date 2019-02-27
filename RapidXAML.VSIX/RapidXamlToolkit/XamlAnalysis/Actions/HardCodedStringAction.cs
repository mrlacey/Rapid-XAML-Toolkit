// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Imaging.Interop;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using RapidXamlToolkit.Resources;
using RapidXamlToolkit.VisualStudioIntegration;
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
            // determine which file to add to - need setting for rule on creation if none exist
            // create entry in the resource file (& open it? - configurable?)

            var vs = new VisualStudioTextManipulation(ProjectHelpers.Dte);
            vs.StartSingleUndoOperation(StringRes.Info_UndoContextMoveStringToResourceFile);

            try
            {
                var currentTag = $"Text=\"{this.tag.Value}\"";

                if (this.tag.UidExists)
                {
                    vs.RemoveInActiveDocOnLine(currentTag, this.tag.GetDesignerLineNumber());
                }
                else
                {
                    var uidTag = $"x:Uid=\"{this.tag.UidValue}\"";
                    vs.ReplaceInActiveDocOnLine(currentTag, uidTag, this.tag.GetDesignerLineNumber());
                }

                RapidXamlDocumentCache.TryUpdate(this.File);
            }
            finally
            {
                vs.EndSingleUndoOperation();
            }
        }
    }
}
