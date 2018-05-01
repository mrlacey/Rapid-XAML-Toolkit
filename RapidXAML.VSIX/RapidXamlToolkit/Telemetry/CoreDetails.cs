// <copyright file="CoreDetails.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

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
