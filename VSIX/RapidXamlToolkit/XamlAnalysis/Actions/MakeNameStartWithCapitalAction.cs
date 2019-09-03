// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Threading;
using RapidXamlToolkit.Resources;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.XamlAnalysis.Actions
{
    public class MakeNameStartWithCapitalAction : ReplaceSubStringLineAction
    {
        public MakeNameStartWithCapitalAction(string file, NameTitleCaseTag tag)
            : base(
                  file,
                  StringRes.UI_CapitalizeFirstLetterOfName,
                  $"{Attributes.Name}=\"{tag.CurrentValue}\"",
                  $"{Attributes.Name}=\"{tag.DesiredValue}\"",
                  tag.Line)
        {
            this.DisplayText = StringRes.UI_CapitalizeFirstLetterOfName;
        }

        public static MakeNameStartWithCapitalAction Create(NameTitleCaseTag tag, string file)
        {
            return new MakeNameStartWithCapitalAction(file, tag);
        }

        public override void Execute(CancellationToken cancellationToken)
        {
            base.Execute(cancellationToken);
        }
    }
}
