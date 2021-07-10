// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.ComponentModel;

namespace RapidXamlToolkit
{
    public static class ProjectTypeExtensions
    {
        public static string GetDescription(this ProjectType source)
        {
            var type = source.GetType();
            var memInfo = type.GetMember(source.ToString());

            if (memInfo != null && memInfo.Length > 0)
            {
                var attrs = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), inherit: false);

                if (attrs != null && attrs.Length > 0)
                {
                    return ((DescriptionAttribute)attrs[0]).Description;
                }
            }

            return source.ToString();
        }

        public static bool Matches(this ProjectType source, ProjectType comparewith)
        {
            return (source & comparewith) == comparewith;
        }

        public static ProjectType AsProjectTypeEnum(this string source)
        {
            if (ProjectType.Uwp.GetDescription().Equals(source, StringComparison.InvariantCultureIgnoreCase))
            {
                return ProjectType.Uwp;
            }
            else if (ProjectType.Wpf.GetDescription().Equals(source, StringComparison.InvariantCultureIgnoreCase))
            {
                return ProjectType.Wpf;
            }
            else if (ProjectType.XamarinForms.GetDescription().Equals(source, StringComparison.InvariantCultureIgnoreCase))
            {
                return ProjectType.XamarinForms;
            }
            else if (ProjectType.WinUI.GetDescription().Equals(source, StringComparison.InvariantCultureIgnoreCase))
            {
                return ProjectType.WinUI;
            }
            else if (ProjectType.MAUI.GetDescription().Equals(source, StringComparison.InvariantCultureIgnoreCase))
            {
                return ProjectType.MAUI;
            }
            else
            {
                return ProjectType.Unknown;
            }
        }
    }
}
