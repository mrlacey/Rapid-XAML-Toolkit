// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;

namespace RapidXaml
{
    public class ExtraAnalysisDetails : Dictionary<string, object>
    {
        public ExtraAnalysisDetails()
        {
        }

        public ExtraAnalysisDetails(string filePath, ProjectFramework framework)
        {
            // All keys lowercase for easier searching
            this.Add(KnownExtraDetails.FilePath, filePath);
            this.Add(KnownExtraDetails.Framework, framework);
        }

        public bool TryGet<T>(string itemName, out T detail)
        {
            var iName = itemName.Trim().ToLowerInvariant();

            if (!string.IsNullOrWhiteSpace(iName)
             && this.ContainsKey(iName))
            {
                var item = this[iName];

                if (item != null && item is T typed)
                {
                    detail = typed;
                    return true;
                }
            }

            detail = default;
            return false;
        }

        public bool IsFramework(ProjectFramework framework)
        {
            if (!this.TryGet(KnownExtraDetails.Framework, out ProjectFramework projectFramework))
            {
                return false;
            }

            return projectFramework == framework;
        }
    }
}
