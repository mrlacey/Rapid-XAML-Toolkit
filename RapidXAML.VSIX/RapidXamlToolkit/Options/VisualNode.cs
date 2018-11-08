// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;

namespace RapidXamlToolkit.Options
{
    public class VisualNode
    {
        public VisualNode(string caption = "")
        {
            this.Caption = caption;
            this.ChildNodes = new List<VisualNode>();
        }

        public string Caption { get; set; }

        public List<VisualNode> ChildNodes { get; set; }
    }
}
