// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Threading;
using RapidXamlToolkit.Resources;
using RapidXamlToolkit.VisualStudioIntegration;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.XamlAnalysis.Actions
{
    public class MissingCheckBoxEventAction : BaseSuggestedAction
    {
        public MissingCheckBoxEventAction(string file)
            : base(file)
        {
            this.DisplayText = StringRes.UI_AddMissingEvent;
        }

        public CheckBoxCheckedAndUncheckedEventsTag Tag { get; private set; }

        public static MissingCheckBoxEventAction Create(CheckBoxCheckedAndUncheckedEventsTag tag, string file)
        {
            var result = new MissingCheckBoxEventAction(file)
            {
                Tag = tag,
            };

            return result;
        }

        public override void Execute(CancellationToken cancellationToken)
        {
            var vs = new VisualStudioTextManipulation(ProjectHelpers.Dte);
            vs.StartSingleUndoOperation(StringRes.UI_AddMissingEvent);
            try
            {
                var lineNumber = this.Tag.Snapshot.GetLineNumberFromPosition(this.Tag.InsertPosition) + 1;

                string newName;

                if (this.Tag.ExistingName.ToLowerInvariant().Contains("checked"))
                {
                    if (this.Tag.ExistingIsChecked)
                    {
                        newName = this.Tag.ExistingName.Replace("Checked", "UnChecked").Replace("checked", "Unchecked");
                    }
                    else
                    {
                         newName = this.Tag.ExistingName.Replace("UnChecked", "Checked").Replace("unchecked", "checked");
                   }
                }
                else
                {
                    if (this.Tag.ExistingIsChecked)
                    {
                        newName = "OnCheckBoxUnchecked";
                    }
                    else
                    {
                        newName = "OnCheckBoxChecked";
                    }
                }

                var newEvent = this.Tag.ExistingIsChecked
                    ? $"Unchecked=\"{newName}\""
                    : $"Checked=\"{newName}\"";

                vs.ReplaceInActiveDocOnLine("<CheckBox", $"<CheckBox {newEvent} ", lineNumber);

                RapidXamlDocumentCache.TryUpdate(this.File);
            }
            finally
            {
                vs.EndSingleUndoOperation();
            }
        }
    }
}
