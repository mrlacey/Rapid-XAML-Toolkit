// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.WebTools.Languages.Json.Schema;

namespace RapidXamlToolkit.JSON
{
    [Export(typeof(IJsonSchemaSelector))]
    internal class RapidXamlAnalysisJsonSchemaSelector : IJsonSchemaSelector
    {
#pragma warning disable CS0067 // Event is never used - But defined as part of Interface
        public event EventHandler AvailableSchemasChanged;
#pragma warning restore CS0067

        public Task<IEnumerable<string>> GetAvailableSchemasAsync()
        {
            return Task.FromResult(Enumerable.Empty<string>());
        }

        public string GetSchemaFor(string fileLocation)
        {
            string fileExt = Path.GetExtension(fileLocation);

            if (!fileExt.Equals(".xamlanalysis", StringComparison.InvariantCultureIgnoreCase))
            {
                return null;
            }

            string fileName = Path.GetFileName(fileLocation);

            string assemblyLoc = Assembly.GetExecutingAssembly().Location;
            string folder = Path.GetDirectoryName(assemblyLoc);

            switch (fileName.ToLowerInvariant())
            {
                case "settings.xamlanalysis":
                    return Path.Combine(folder, "JSON", "settings-schema.json");
                case "suppressions.xamlanalysis":
                    return Path.Combine(folder, "JSON", "suppressions-schema.json");
                default:
                    return null;
            }
        }
    }
}
