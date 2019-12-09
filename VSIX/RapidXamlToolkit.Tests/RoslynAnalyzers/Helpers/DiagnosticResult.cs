// Copyright (c) Matt Lacey Ltd.. All rights reserved.
// Licensed under the MIT license.

using Microsoft.CodeAnalysis;

namespace TestHelper
{
    /// <summary>
    /// Struct that stores information about a Diagnostic appearing in a source.
    /// </summary>
    public struct DiagnosticResult
    {
        private DiagnosticResultLocation[] locations;

        public DiagnosticResultLocation[] Locations
        {
            get
            {
                if (this.locations == null)
                {
                    this.locations = new DiagnosticResultLocation[] { };
                }

                return this.locations;
            }

            set
            {
                this.locations = value;
            }
        }

        public DiagnosticSeverity Severity { get; set; }

        public string Id { get; set; }

        public string Message { get; set; }

        public string Path
        {
            get
            {
                return this.Locations.Length > 0 ? this.Locations[0].Path : "";
            }
        }

        public int Line
        {
            get
            {
                return this.Locations.Length > 0 ? this.Locations[0].Line : -1;
            }
        }

        public int Column
        {
            get
            {
                return this.Locations.Length > 0 ? this.Locations[0].Column : -1;
            }
        }
    }
}
