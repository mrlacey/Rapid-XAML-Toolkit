// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System.Threading;
using RapidXamlToolkit.Resources;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.XamlAnalysis.Actions
{
    public class MakeUidStartWithCapitalAction : ReplaceSubStringLineAction
    {
        public MakeUidStartWithCapitalAction(string file, UidTitleCaseTag tag)
            : base(
                  file,
                  StringRes.UI_CapitalizeFirstLetterOfUid,
                  $"{Attributes.Uid}=\"{tag.CurrentValue}\"",
                  $"{Attributes.Uid}=\"{tag.DesiredValue}\"",
                  tag.Line)
        {
            this.DisplayText = StringRes.UI_CapitalizeFirstLetterOfUid;
        }

        public static MakeUidStartWithCapitalAction Create(UidTitleCaseTag tag, string file)
        {
            return new MakeUidStartWithCapitalAction(file, tag);
        }

        public override void Execute(CancellationToken cancellationToken)
        {
            base.Execute(cancellationToken);
        }
    }
}
