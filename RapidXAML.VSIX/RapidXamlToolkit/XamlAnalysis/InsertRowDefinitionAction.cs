// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Imaging.Interop;
using Microsoft.VisualStudio.Text.Editor;
using RapidXamlToolkit.Commands;
using RapidXamlToolkit.Resources;

namespace RapidXamlToolkit.XamlAnalysis
{
    public class InsertRowDefinitionAction : BaseSuggestedAction
    {
        public InsertRowDefinitionTag tag;

        public override string DisplayText
        {
            get { return $"Insert new definition for row {tag.RowId}"; }
        }

        public override ImageMoniker IconMoniker => KnownMonikers.InsertClause;

        public static InsertRowDefinitionAction Create(InsertRowDefinitionTag tag, string file, ITextView view)
        {
            var result = new InsertRowDefinitionAction
            {
                tag = tag,
                File = file,
                View = view,
            };

            return result;
        }

        public override void Execute(CancellationToken cancellationToken)
        {
            // TODO: Do insertion - use code from existing command - may need to abstract for reuse

            // TODO: review simplifying this for just the bits needed here - may need to break VSA into multiple interfaces
          //  var vs = new VisualStudioAbstraction(RapidXamlPackage.Logger, this.ServiceProvider, dte);

          //  var logic = new InsertGridRowDefinitionCommandLogic(RapidXamlPackage.Logger, vs);

            // TODO Move this logic here - need to also add total number of definitions to tag (so don't need to look up)
       //     var replacements = logic.GetReplacements();

            var exclusions = GetExclusions(View.TextSnapshot.GetText());

      //      vs.StartSingleUndoOperation(StringRes.Info_UndoContextIndertRowDef);
      //      try
      //      {
      //          vs.ReplaceInActiveDoc(replacements, this.tag.GridStartPos, this.tag.GridEndPos, exclusions);
      //          vs.InsertIntoActiveDocumentOnNextLine(this.tag.XamlTag, this.tag.InsertPoint);
      //      }
      //      finally
      //      {
      //          vs.EndSingleUndoOperation();
      //      }
        }

        public static Dictionary<int, int> GetExclusions(string xaml)
        {
            const string gridOpen = "<Grid";
            const string gridOpenSpace = "<Grid ";
            const string gridOpenComplete = "<Grid>";
            const string gridClose = "</Grid>";

            var exclusions = new Dictionary<int, int>();

            var nextOpening = xaml.Substring(gridOpen.Length).FirstIndexOf(gridOpenComplete, gridOpenSpace);

            while (nextOpening > -1 && nextOpening < xaml.Length)
            {
                var endPos = xaml.IndexOf(gridClose, nextOpening, StringComparison.Ordinal) + gridClose.Length;

                exclusions.Add(nextOpening, endPos);

                var searchFrom = endPos + 1;

                nextOpening = xaml.Substring(searchFrom).FirstIndexOf(gridOpenComplete, gridOpenSpace);

                if (nextOpening > -1)
                {
                    nextOpening += searchFrom;
                }
            }

            return exclusions;
        }
    }
}
