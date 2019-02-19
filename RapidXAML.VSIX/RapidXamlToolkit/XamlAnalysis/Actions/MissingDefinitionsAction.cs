// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.XamlAnalysis.Actions
{
    public abstract class MissingDefinitionsAction : BaseSuggestedAction
    {
        public string UndoOperationName { get; protected set; }

        public MissingDefinitionTag Tag { get; set; }
    }
}
