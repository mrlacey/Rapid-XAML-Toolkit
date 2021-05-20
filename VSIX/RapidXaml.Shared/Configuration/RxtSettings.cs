// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;

namespace RapidXamlToolkit.Configuration
{
    public class RxtSettings
    {
        public string TelemetryKey
        {
            get
            {
                return Properties.Settings.Default.TelemetryKey;
            }
        }

        public Guid LightBulbTelemetryGuid
        {
            get
            {
                var configValue = Properties.Settings.Default.LightBulbTelemetryGuid;

                if (!string.IsNullOrWhiteSpace(configValue))
                {
                    return new Guid(configValue);
                }
                else
                {
                    return Guid.Empty;
                }
            }
        }
    }
}
