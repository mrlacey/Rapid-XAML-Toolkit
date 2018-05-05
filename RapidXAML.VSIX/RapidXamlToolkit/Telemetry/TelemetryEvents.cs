// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

namespace RapidXamlToolkit.Telemetry
{
    public class TelemetryEvents
    {
        public const string Prefix = "Rxt";

        public static string SessionStart { get; private set; } = Prefix + "SessionStart";
    }
}
