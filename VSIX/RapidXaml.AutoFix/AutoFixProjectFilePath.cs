// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using RapidXamlToolkit.VisualStudioIntegration;

namespace RapidXaml
{
    public class AutoFixProjectFilePath : IVisualStudioProjectFilePath
    {
        private string projFilePath;

        public AutoFixProjectFilePath(string projectFilePath)
        {
            this.projFilePath = projectFilePath;
        }

        public string GetPathOfProjectContainingFile(string fileName)
        {
            // The passed in value is irrelevant as we already know the value
            // and so don't need to look anything up.
            return this.projFilePath;
        }
    }
}
