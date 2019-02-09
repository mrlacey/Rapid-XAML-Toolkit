// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;

namespace RapidXamlToolkit.XamlAnalysis
{
    public static class ProjectHelpers
    {
        static ProjectHelpers()
        {
            DTE = (DTE2)Package.GetGlobalService(typeof(DTE));
        }

        public static DTE2 DTE { get; }
    }
}
