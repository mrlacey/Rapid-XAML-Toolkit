// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using RapidXamlToolkit.Logging;

namespace RapidXamlToolkit.XamlAnalysis.Processors
{
    public class ProcessorEssentials
    {
        public ProcessorEssentials()
        {
        }

        public ProcessorEssentials(ProjectType projectType, ILogger logger, string projectFilePath)
        {
            this.ProjectType = projectType;
            this.Logger = logger;
            this.ProjectFilePath = projectFilePath;
        }

        public ProjectType ProjectType { get; set; }

        public ILogger Logger { get; set; }

        public string ProjectFilePath { get; set; }
    }
}
