// Copyright (c) Matt Lacey Ltd.. All rights reserved.
// Licensed under the MIT license.

namespace RapidXamlToolkit.Telemetry
{
    public class TelemetryProperties
    {
        public static string RoleInstanceName { get; } = TelemetryEvents.Prefix + "Vsix";

        public static string VisualStudioExeVersion { get; } = TelemetryEvents.Prefix + "VsExeVersion";

        public static string VisualStudioEdition { get; } = TelemetryEvents.Prefix + "VsEdition";

        public static string VisualStudioCulture { get; } = TelemetryEvents.Prefix + "VsCulture";

        public static string VisualStudioManifestId { get; } = TelemetryEvents.Prefix + "VsManifestId";
    }
}
