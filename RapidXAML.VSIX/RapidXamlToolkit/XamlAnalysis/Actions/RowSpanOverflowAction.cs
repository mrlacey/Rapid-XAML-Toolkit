// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Threading;
using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Imaging.Interop;
using RapidXamlToolkit.Resources;
using RapidXamlToolkit.VisualStudioIntegration;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.XamlAnalysis.Actions
{

    public class RowSpanOverflowAction : AddMissingRowDefinitionsAction
    {
        public RowSpanOverflowAction(string file)
            : base(file)
        {
        }

        public static RowSpanOverflowAction Create(RowSpanOverflowTag tag, string file)
        {
            var result = new RowSpanOverflowAction(file)
            {
                Tag = tag,
            };

            return result;
        }
    }
}
