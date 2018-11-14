// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using RapidXamlToolkit.Options;

namespace RapidXamlToolkit.Tests.DragDrop
{
    public class DropHandlerTestsBase
    {
        protected Profile GetProfileForTesting()
        {
            var profile = TestProfile.CreateEmpty();
            profile.ClassGrouping = "StackPanel";
            profile.FallbackOutput = "<TextBlock Text=\"$name$\" />";

            return profile;
        }
    }
}
