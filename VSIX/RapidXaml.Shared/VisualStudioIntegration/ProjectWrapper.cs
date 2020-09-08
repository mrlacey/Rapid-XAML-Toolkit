// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

namespace RapidXamlToolkit.VisualStudioIntegration
{
    public class ProjectWrapper
    {
        private string name;

        private string fileName;

        public ProjectWrapper()
        {
        }

        public ProjectWrapper(EnvDTE.Project project)
        {
            this.Project = project;
        }

        public EnvDTE.Project Project { get; set; }

        public string Name { get => this.Project?.Name ?? this.name; set => this.name = value; }

        public string FileName { get => this.Project?.FileName ?? this.fileName; set => this.fileName = value; }
    }
}
