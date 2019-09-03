// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.Linq;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.XamlAnalysis
{
    public class TagList : List<IRapidXamlAdornmentTag>
    {
        public bool TryAdd(IRapidXamlAdornmentTag tag, string xamlElement, List<TagSuppression> suppressions)
        {
            // Only suppress tags that might be displayed as only they have error codes
            if (tag is RapidXamlDisplayedTag displayedTag)
            {
                if (suppressions != null)
                {
                    var relevantSuppressions = suppressions.Where(s => s.TagErrorCode == displayedTag.ErrorCode);

                    foreach (var suppression in relevantSuppressions)
                    {
                        // if identifier isn't specified match with everything
                        if (string.IsNullOrWhiteSpace(suppression.ElementIdentifier)
                         || xamlElement.Contains(suppression.ElementIdentifier))
                        {
                            return false;
                        }
                    }
                }
            }

            this.Add(tag);
            return true;
        }
    }
}
