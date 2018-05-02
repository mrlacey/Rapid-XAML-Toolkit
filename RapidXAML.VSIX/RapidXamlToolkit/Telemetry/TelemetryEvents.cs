// <copyright file="TelemetryEvents.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace RapidXamlToolkit.Telemetry
{
    public class TelemetryEvents
    {
        public const string Prefix = "Rxt";

        public static string SessionStart { get; private set; } = Prefix + "SessionStart";
    }
}
