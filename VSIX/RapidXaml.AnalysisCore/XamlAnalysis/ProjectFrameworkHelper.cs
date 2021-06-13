// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using RapidXaml;

namespace RapidXamlToolkit.XamlAnalysis
{
    public class ProjectFrameworkHelper
    {
        public static ProjectFramework FromType(ProjectType type)
        {
            switch (type)
            {
                case ProjectType.Uwp:
                    return ProjectFramework.Uwp;

                case ProjectType.Wpf:
                    return ProjectFramework.Wpf;

                case ProjectType.XamarinForms:
                    return ProjectFramework.XamarinForms;

                case ProjectType.WinUI:
                    // TODO: Update CustomAnalysis lib for this to work
                    return ProjectFramework.WinUI;

                case ProjectType.MAUI:
                    // TODO: Update CustomAnalysis lib for this to work
                    return ProjectFramework.MAUI;

                case ProjectType.Unknown:
                case ProjectType.UwpOrWpf:
                case ProjectType.Any:
                default:
                    return ProjectFramework.Unknown;
            }
        }
    }
}
