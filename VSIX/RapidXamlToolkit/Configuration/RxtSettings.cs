// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using RapidXaml.Properties;

namespace RapidXamlToolkit.Configuration
{
    public class RxtSettings
    {
        public string TelemetryKey
        {
            get
            {
                return Settings.Default.TelemetryKey;
            }
        }

        public string AppInsightsConnectionString
        {
            get
            {
                // TODO: fix
                ////return Properties.Settings.Default.AppInsightsConnStr;
                return string.Empty;
            }
        }

        public Guid LightBulbTelemetryGuid
        {
            get
            {
                var configValue = Settings.Default.LightBulbTelemetryGuid;

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
