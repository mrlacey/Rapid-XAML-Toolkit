// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;

namespace RapidXamlToolkit.XamlAnalysis
{
    public static class ExclusionHelpers
    {
        public static bool IsInExcludedArea(this Dictionary<int, int> exclusions, int position)
        {
            foreach (var item in exclusions)
            {
                if (position >= item.Key && position <= item.Value)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
