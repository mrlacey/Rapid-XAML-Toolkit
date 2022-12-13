// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using RapidXaml.Resources;

namespace RapidXamlToolkit.Options
{
    public static class OptionsEntryValidator
    {
        public static string Validate(string enteredText, Type type, string property, bool checkforNoOutput = true)
        {
            if (checkforNoOutput && string.IsNullOrWhiteSpace(enteredText))
            {
                return OptionsStringRes.Options_Warn_UseNoOutputNotBlank.WithParams(Placeholder.NoOutput);
            }

            var apv = new AllowedPlaceholderValidator();

            var (allValid, invalidPlaceholders) = apv.ContainsOnlyValidPlaceholders(type, property, enteredText);

            if (!allValid)
            {
                var (allKnown, unknownPlaceholders) = apv.ContainsUnknownPlaceholders(enteredText);

                if (!allKnown)
                {
                    return OptionsStringRes.Options_Warn_UnknownPlaceholders.WithParams(string.Join(", ", unknownPlaceholders));
                }
                else
                {
                    return OptionsStringRes.Options_Warn_InvalidPlaceholders.WithParams(string.Join(", ", invalidPlaceholders));
                }
            }

            var placeholders = enteredText.GetPlaceholders();

            var modifiedValue = enteredText;

            foreach (var ph in placeholders)
            {
                modifiedValue = modifiedValue.Replace(ph, string.Empty);
            }

            var attribs = modifiedValue.GetAllAttributes();

            foreach (var attrib in attribs)
            {
                modifiedValue = modifiedValue.Replace(attrib, string.Empty);
            }

            var dollarSignCount = modifiedValue.OccurrenceCount("$");
            if (dollarSignCount > 0)
            {
                string invalidPlaceholder;
                if (dollarSignCount > 1)
                {
                    invalidPlaceholder = modifiedValue.Substring(modifiedValue.IndexOf("$"), modifiedValue.IndexOf("$", modifiedValue.IndexOf("$") + 1) - modifiedValue.IndexOf("$") + 1);
                }
                else
                {
                    invalidPlaceholder = modifiedValue.Substring(modifiedValue.IndexOf("$"));
                }

                return OptionsStringRes.Options_Warn_UnknownPlaceholders.WithParams(invalidPlaceholder);
            }

            return null;
        }
    }
}
