// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

namespace RapidXamlToolkit.XamlAnalysis.Tags
{
    public class UnexpectedErrorTag : RapidXamlDisplayedTag
    {
        public UnexpectedErrorTag(TagDependencies tagDeps)
            : base(tagDeps, "RXT999", TagErrorType.Error)
        {
            this.ToolTip = string.Empty;
            this.IsInternalError = true;
        }
    }
}
