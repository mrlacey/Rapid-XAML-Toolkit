// <copyright file="Mapping.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;

namespace RapidXamlToolkit
{
    public class Mapping : ICloneable
    {
        public string Type { get; set; }

        public string NameContains { get; set; }

        public bool IfReadOnly { get; set; }

        public string Output { get; set; }

        public static Mapping CreateNew()
        {
            return new Mapping
            {
                Type = string.Empty,
                NameContains = string.Empty,
                Output = string.Empty,
                IfReadOnly = false,
            };
        }

        public object Clone()
        {
            return new Mapping
            {
                Type = this.Type,
                NameContains = this.NameContains,
                Output = this.Output,
                IfReadOnly = this.IfReadOnly,
            };
        }
    }
}
