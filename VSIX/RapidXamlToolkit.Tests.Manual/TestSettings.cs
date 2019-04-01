// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json.Linq;

namespace RapidXamlToolkit.Tests.Manual
{
    public class TestSettings
    {
        private const string TestSettingsFileName = "testsettings.json";

        public TestSettings()
        {
            var rawJson = File.ReadAllText(TestSettingsFileName);

            if (!string.IsNullOrWhiteSpace(rawJson))
            {
                try
                {
                    JObject obj = JObject.Parse(rawJson);

                    foreach (var property in obj.Properties())
                    {
                        switch (property.Name)
                        {
                            case "TextServiceSubscriptionKey":
                                this.SubscriptionKey = property.Value.ToString();
                                break;
                            case "TextServiceAzureRegion":
                                this.AzureRegion = property.Value.ToString();
                                break;
                            case "TextServiceIsFreeSubscription":
                                this.IsFreeSubscription = bool.Parse(property.Value.ToString());
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

        // Set these values in testsettings.json
        public string SubscriptionKey { get; private set; } = string.Empty;

        public string AzureRegion { get; private set; } = string.Empty;

        public bool IsFreeSubscription { get; private set; } = true;
    }
}
