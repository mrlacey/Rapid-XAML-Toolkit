// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

namespace RapidXamlToolkit.VisualStudioIntegration
{
    public interface IVisualStudioProjectFilePath
    {
        string GetPathOfProjectContainingFile(string fileName);
    }
}
