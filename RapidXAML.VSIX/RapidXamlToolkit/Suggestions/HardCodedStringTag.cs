// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using RapidXamlToolkit.Suggestions;

namespace RapidXamlToolkit.Tagging
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
