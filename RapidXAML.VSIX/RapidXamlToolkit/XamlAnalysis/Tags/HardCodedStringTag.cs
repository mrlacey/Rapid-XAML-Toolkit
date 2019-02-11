// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.Text;

namespace RapidXamlToolkit.XamlAnalysis.Tags
{
    public class HardCodedStringTag : RapidXamlWarningTag
    {
        public HardCodedStringTag(Span span, ITextSnapshot snapshot)
            : base(span, snapshot)
        {
            this.ActionType = ActionTypes.HardCodedString;
            this.ToolTip = "HardCoded string message"; // TODO: need to customize this???
            this.ErrorCode = "RXT???"; // TODO Need to assign a proper error code
        }
    }
}
