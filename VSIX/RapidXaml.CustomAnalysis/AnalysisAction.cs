// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;

namespace RapidXaml
{
    public class AnalysisAction
    {
        public RapidXamlErrorType ErrorType { get; internal set; }

        public ActionType Action { get; internal set; }

        public string Code { get; internal set; }

        public string Description { get; internal set; }

        public string ActionText { get; internal set; }

        public string Name { get; internal set; }

        public string Value { get; internal set; }

        public string Content { get; internal set; }

        public RapidXamlElement Element { get; internal set; }

        public RapidXamlSpan Location { get; internal set; }

        public string ExtendedMessage { get; internal set; }

        public string MoreInfoUrl { get; internal set; }

        public bool? IsInlineAttribute { get; internal set; }

        public List<AnalysisAction> SupplementaryActions { get; } = new List<AnalysisAction>();
    }
}
