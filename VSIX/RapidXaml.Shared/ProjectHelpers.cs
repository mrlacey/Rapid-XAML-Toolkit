// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text.Operations;

namespace RapidXamlToolkit
{
    public static class ProjectHelpers
    {
        static ProjectHelpers()
        {
            Dte = (DTE)Package.GetGlobalService(typeof(DTE));
            Dte2 = (DTE2)Package.GetGlobalService(typeof(DTE));
        }

        public static DTE Dte { get; }

        public static DTE2 Dte2 { get; }
    }
}
