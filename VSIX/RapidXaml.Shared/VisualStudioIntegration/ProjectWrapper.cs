// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

namespace RapidXamlToolkit.VisualStudioIntegration
{
    public class ProjectWrapper : IProjectWrapper
    {
        private readonly EnvDTE.Project project;

        private string name;

        private string fileName;

        public ProjectWrapper()
        {
        }

        public ProjectWrapper(EnvDTE.Project project)
        {
            this.project = project;
        }

        public string Name { get => this.project?.Name ?? this.name; set => this.name = value; }

        public string FileName { get => this.project?.FileName ?? this.fileName; set => this.fileName = value; }
    }
}
