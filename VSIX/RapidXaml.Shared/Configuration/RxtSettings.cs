// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;

namespace RapidXamlToolkit.Configuration
{
    public class RxtSettings
    {
        public string TelemetryKey { get; private set; } = "DEFAULT-VALUE";

        public Guid LightBulbTelemetryGuid { get; private set; } = Guid.Empty;

        public bool ExtendedOutputEnabledByDefault { get; private set; } = true;
    }
}
