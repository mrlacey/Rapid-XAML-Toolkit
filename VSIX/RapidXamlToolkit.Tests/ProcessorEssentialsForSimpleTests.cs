// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using RapidXamlToolkit.XamlAnalysis.Processors;

namespace RapidXamlToolkit.Tests
{
    public class ProcessorEssentialsForSimpleTests : ProcessorEssentials
    {
        public ProcessorEssentialsForSimpleTests(ProjectType projectType = ProjectType.Any)
        {
            this.ProjectType = projectType;
            this.Logger = new DefaultTestLogger();
            this.ProjectFilePath = string.Empty;
        }
    }
}
