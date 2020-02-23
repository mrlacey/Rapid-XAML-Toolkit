// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

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

        public RapidXamlSpan Location { get; internal set; }

        public string MoreInfoUrl { get; internal set; }
    }
}
