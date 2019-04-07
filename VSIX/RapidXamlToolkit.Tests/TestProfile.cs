// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.ObjectModel;
using RapidXamlToolkit.Options;

namespace RapidXamlToolkit.Tests
{
    public static class TestProfile
    {
        public static Profile CreateEmpty()
        {
            return new Profile
            {
                Name = string.Empty,
                ClassGrouping = string.Empty,
                FallbackOutput = string.Empty,
                SubPropertyOutput = string.Empty,
                EnumMemberOutput = string.Empty,
                Mappings = new ObservableCollection<Mapping>(),
                AttemptAutomaticDocumentFormatting = true,
            };
        }
    }
}
