// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

namespace RapidXamlToolkit.XamlAnalysis
{
    public class HardCodedStringTag : RapidXamlViewTag
    {
        public HardCodedStringTag()
        {
            this.ActionType = ActionTypes.HardCodedString;
            this.ToolTip = "HardCoded string message";
            this.ErrorCode = "RXT???"; // TODO Need to assign a proper error code
        }
    }
}
