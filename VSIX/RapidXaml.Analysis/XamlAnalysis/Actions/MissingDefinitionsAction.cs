// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.XamlAnalysis.Actions
{
    public abstract class MissingDefinitionsAction : BaseSuggestedAction
    {
        protected MissingDefinitionsAction(string file)
            : base(file)
        {
        }

        public string UndoOperationName { get; protected set; }

        public MissingDefinitionTag Tag { get; set; }
    }
}
