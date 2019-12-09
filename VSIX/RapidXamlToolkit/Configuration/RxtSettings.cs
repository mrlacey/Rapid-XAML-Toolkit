// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Newtonsoft.Json.Linq;

namespace RapidXamlToolkit.Configuration
{
    public class RxtSettings
    {
        private const string AppSettingsFileName = "appsettings.json";

        public RxtSettings()
        {
            var executionPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var filePath = Path.Combine(executionPath, AppSettingsFileName);

            var rawJson = File.ReadAllText(filePath);

            if (!string.IsNullOrWhiteSpace(rawJson))
            {
                try
                {
                    JObject obj = JObject.Parse(rawJson);

                    foreach (var property in obj.Properties())
                    {
                        switch (property.Name)
                        {
                            case "TelemetryKey":
                                this.TelemetryKey = property.Value.ToString();
                                break;
                            case "LightBulbTelemetryGuid":
                                if (property.Value.ToString() != "SET HERE")
                                {
                                    this.LightBulbTelemetryGuid = new Guid(property.Value.ToString());
                                }

                                break;
                            case "ExtendedOutputEnabledByDefault":
                                this.ExtendedOutputEnabledByDefault = bool.Parse(property.Value.ToString());
                                break;
                            default:
                                break;
                        }
                    }
                }
                catch (Exception exc)
                {
                    Debug.WriteLine(exc);
                }
            }
        }

        public string TelemetryKey { get; private set; } = "DEFAULT-VALUE";

        public Guid LightBulbTelemetryGuid { get; private set; } = Guid.Empty;

        public bool ExtendedOutputEnabledByDefault { get; private set; } = true;
    }
}
