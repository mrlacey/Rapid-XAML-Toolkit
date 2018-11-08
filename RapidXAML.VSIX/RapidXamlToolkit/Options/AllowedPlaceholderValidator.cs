// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RapidXamlToolkit.Options
{
    public class AllowedPlaceholderValidator
    {
        public (bool isValid, List<string> invalidPlaceholders) ContainsOnlyValidPlaceholders(Type type, string propertyName, string output)
        {
            var propInfo = type.GetProperty(propertyName);
            var attribute = propInfo.GetCustomAttribute(typeof(AllowedPlaceholdersAttribute)) as AllowedPlaceholdersAttribute;

            var allowedPlaceholders = attribute.Placeholders;
            var usedPlaceholders = output.GetPlaceholders();

            var incorrectlyUsedPlaceholders = new List<string>();

            foreach (var usedPlaceholder in usedPlaceholders)
            {
                if (!allowedPlaceholders.Contains(usedPlaceholder))
                {
                    incorrectlyUsedPlaceholders.Add(usedPlaceholder);
                }
            }

            return (!incorrectlyUsedPlaceholders.Any(), incorrectlyUsedPlaceholders);
        }
    }
}
