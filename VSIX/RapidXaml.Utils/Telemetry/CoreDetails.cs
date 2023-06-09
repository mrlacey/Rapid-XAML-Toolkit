// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System.Reflection;

namespace RapidXamlToolkit.Telemetry
{
    public static class CoreDetails
    {
        public static string GetVersion()
        {
            var assembly = Assembly.GetCallingAssembly();

            return assembly.GetName().Version.ToString();
        }
    }
}
