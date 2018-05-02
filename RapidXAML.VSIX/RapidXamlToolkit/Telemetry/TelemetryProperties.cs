// <copyright file="TelemetryProperties.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace RapidXamlToolkit.Telemetry
{
    public class TelemetryProperties
    {
        public static string RoleInstanceName { get; private set; } = TelemetryEvents.Prefix + "Vsix";

        public static string VisualStudioExeVersion { get; private set; } = TelemetryEvents.Prefix + "VsExeVersion";

        public static string VisualStudioEdition { get; private set; } = TelemetryEvents.Prefix + "VsEdition";

        public static string VisualStudioCulture { get; private set; } = TelemetryEvents.Prefix + "VsCulture";

        public static string VisualStudioManifestId { get; private set; } = TelemetryEvents.Prefix + "VsManifestId";
    }
}
