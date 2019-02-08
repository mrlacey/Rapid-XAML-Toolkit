// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using RapidXamlToolkit.Suggestions;

namespace RapidXamlToolkit.Tagging
{
    public class UnexpectedErrorTag : RapidXamlViewTag
    {
        public UnexpectedErrorTag()
        {
            this.ActionType = ActionTypes.UnexpectedError;
            this.ToolTip = string.Empty;
            this.ErrorCode = "RXT999";
            this.IsFatal = true;
        }
    }
}
