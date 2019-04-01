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
    public class ColumnSpanOverflowAction : AddMissingColumnDefinitionsAction
    {
        private ColumnSpanOverflowAction(string file)
            : base(file)
        {
        }

        public static ColumnSpanOverflowAction Create(ColumnSpanOverflowTag tag, string file)
        {
            var result = new ColumnSpanOverflowAction(file)
            {
                Tag = tag,
            };

            return result;
        }
    }
}
