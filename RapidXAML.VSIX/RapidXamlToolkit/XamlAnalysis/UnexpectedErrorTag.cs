// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

namespace RapidXamlToolkit.XamlAnalysis
{
    public class UnexpectedErrorTag : RapidXamlWarningTag
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
