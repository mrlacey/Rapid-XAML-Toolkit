// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using RapidXamlToolkit.Resources;

namespace RapidXamlToolkit.Options
{
    public static class OptionsEntryValidator
    {
        public static string Validate(string enteredText, Type type, string property, bool checkforNoOutput = true)
        {
            if (checkforNoOutput && string.IsNullOrWhiteSpace(enteredText))
            {
                return StringRes.Options_Warn_UseNoOutputNotBlank.WithParams(Placeholder.NoOutput);
            }

            var apv = new AllowedPlaceholderValidator();

            var (allValid, invalidPlaceholders) = apv.ContainsOnlyValidPlaceholders(type, property, enteredText);

            if (!allValid)
            {
                var (allKnown, unknownPlaceholders) = apv.ContainsUnknownPlaceholders(enteredText);

                if (!allKnown)
                {
                    return StringRes.Options_Warn_UnknownPlaceholders.WithParams(string.Join(", ", unknownPlaceholders));
                }
                else
                {
                    return StringRes.Options_Warn_InvalidPlaceholders.WithParams(string.Join(", ", invalidPlaceholders));
                }
            }

            return null;
        }
    }
}
