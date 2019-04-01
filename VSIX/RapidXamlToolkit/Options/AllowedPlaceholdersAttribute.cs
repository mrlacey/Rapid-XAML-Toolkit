// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;

namespace RapidXamlToolkit.Options
{
    internal class AllowedPlaceholdersAttribute : Attribute
    {
        public AllowedPlaceholdersAttribute(params string[] placeholders)
        {
            this.Placeholders = placeholders;
        }

        public string[] Placeholders { get; }
    }
}
