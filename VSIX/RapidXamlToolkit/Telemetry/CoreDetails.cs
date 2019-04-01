// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Reflection;

namespace RapidXamlToolkit.Telemetry
{
    public static class CoreDetails
    {
        public static string GetVersion()
        {
            var assembly = Assembly.GetExecutingAssembly();

            return assembly.GetName().Version.ToString();
        }
    }
}
