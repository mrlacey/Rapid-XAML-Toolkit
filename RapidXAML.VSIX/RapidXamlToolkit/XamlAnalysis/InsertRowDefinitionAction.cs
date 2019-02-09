// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Threading;
using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Imaging.Interop;
using Microsoft.VisualStudio.Text.Editor;

namespace RapidXamlToolkit.XamlAnalysis
{
    public class InsertRowDefinitionAction : BaseSuggestedAction
    {
        public InsertRowDefinitionTag tag;

        public override string DisplayText
        {
            get { return $"Insert new definition for row {tag.RowId}"; }
        }

        public override ImageMoniker IconMoniker
        {
            get { return KnownMonikers.InsertClause; }
        }

        public override void Execute(CancellationToken cancellationToken)
        {
            // TODO: Do insertion - use code from existing command - may need to abstract for reuse
        }

        public static InsertRowDefinitionAction Create(InsertRowDefinitionTag tag, string file, ITextView view)
        {
            var result = new InsertRowDefinitionAction
            {
                // linkUrl = errorTag.Url,
                tag = tag,
                File = file,
                View = view,
            };

            return result;
        }
    }
}
