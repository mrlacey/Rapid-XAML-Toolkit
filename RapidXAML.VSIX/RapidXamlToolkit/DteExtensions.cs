// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using EnvDTE;

namespace RapidXamlToolkit
{
    public static class DteExtensions
    {
        public static void FormatDocument(this DTE dte, Options.Profile profile)
        {
            // Profile passed in to ensure the check for running this command is always performed
            if (profile?.General.AttemptAutomaticDocumentFormatting == true)
            {
                dte.ExecuteCommand("Edit.FormatDocument");
            }
        }
    }
}
