// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace RapidXamlToolkit.Options
{
    public class ViewGenerationSettings : CanNotifyPropertyChanged
    {
        private bool allInSameProject;
        private string xamlProjectSuffix;
        private string xamlFileDirectoryName;
        private string xamlFileSuffix;
        private string viewModelProjectSuffix;
        private string viewModelDirectoryName;
        private string viewModelFileSuffix;

        [AllowedPlaceholders(Placeholder.ViewProject, Placeholder.ViewNamespace, Placeholder.ViewModelNamespace, Placeholder.ViewClass, Placeholder.ViewModelClass, Placeholder.GeneratedXAML)]
        public string XamlPlaceholder { get; set; }

        [AllowedPlaceholders(Placeholder.ViewProject, Placeholder.ViewNamespace, Placeholder.ViewModelNamespace, Placeholder.ViewClass, Placeholder.ViewModelClass, Placeholder.GeneratedXAML)]
        public string CodePlaceholder { get; set; }

        public bool AllInSameProject
        {
            get => this.allInSameProject;
            set
            {
                this.allInSameProject = value;
                this.OnPropertyChanged();
                this.OnPropertyChanged(nameof(ViewGenerationSettings.Visualization));
            }
        }

        public string XamlFileSuffix
        {
            get => this.xamlFileSuffix;
            set
            {
                this.xamlFileSuffix = value;
                this.OnPropertyChanged();
                this.OnPropertyChanged(nameof(ViewGenerationSettings.Visualization));
            }
        }

        public string ViewModelFileSuffix
        {
            get => this.viewModelFileSuffix;
            set
            {
                this.viewModelFileSuffix = value;
                this.OnPropertyChanged();
                this.OnPropertyChanged(nameof(ViewGenerationSettings.Visualization));
            }
        }

        public string XamlFileDirectoryName
        {
            get => this.xamlFileDirectoryName;
            set
            {
                this.xamlFileDirectoryName = value;
                this.OnPropertyChanged();
                this.OnPropertyChanged(nameof(ViewGenerationSettings.Visualization));
            }
        }

        public string ViewModelDirectoryName
        {
            get => this.viewModelDirectoryName;
            set
            {
                this.viewModelDirectoryName = value;
                this.OnPropertyChanged();
                this.OnPropertyChanged(nameof(ViewGenerationSettings.Visualization));
            }
        }

        public string XamlProjectSuffix
        {
            get => this.xamlProjectSuffix;
            set
            {
                this.xamlProjectSuffix = value;
                this.OnPropertyChanged();
                this.OnPropertyChanged(nameof(ViewGenerationSettings.Visualization));
            }
        }

        public string ViewModelProjectSuffix
        {
            get => this.viewModelProjectSuffix;
            set
            {
                this.viewModelProjectSuffix = value;
                this.OnPropertyChanged();
                this.OnPropertyChanged(nameof(ViewGenerationSettings.Visualization));
            }
        }

        [JsonIgnore]
        [IgnoreDataMember]
        public Collection<VisualNode> Visualization
        {
            get
            {
                var solution = new VisualNode("solution");

                var mainView = new VisualNode($"Main{this.XamlFileSuffix}.xaml");
                var detailView = new VisualNode($"Detail{this.XamlFileSuffix}.xaml");

                var viewFolder = new VisualNode($"{this.XamlFileDirectoryName}");
                viewFolder.ChildNodes.Add(mainView);
                viewFolder.ChildNodes.Add(detailView);

                var mainVM = new VisualNode($"Main{this.ViewModelFileSuffix}.cs");
                var detailVM = new VisualNode($"Detail{this.ViewModelFileSuffix}.cs");

                var vmFolder = new VisualNode($"{this.ViewModelDirectoryName}");
                vmFolder.ChildNodes.Add(mainVM);
                vmFolder.ChildNodes.Add(detailVM);

                if (this.AllInSameProject)
                {
                    var app = new VisualNode("MyApp");
                    app.ChildNodes.Add(viewFolder);
                    app.ChildNodes.Add(vmFolder);

                    solution.ChildNodes.Add(app);
                }
                else
                {
                    var viewDot = string.IsNullOrWhiteSpace(this.XamlProjectSuffix) ? string.Empty : ".";
                    var viewProject = new VisualNode($"MyApp{viewDot}{this.XamlProjectSuffix}");
                    viewProject.ChildNodes.Add(viewFolder);

                    var vmDot = string.IsNullOrWhiteSpace(this.ViewModelProjectSuffix) ? string.Empty : ".";
                    var vmProject = new VisualNode($"MyApp{vmDot}{this.ViewModelProjectSuffix}");
                    vmProject.ChildNodes.Add(vmFolder);

                    solution.ChildNodes.Add(viewProject);
                    solution.ChildNodes.Add(vmProject);
                }

                return new Collection<VisualNode>(new List<VisualNode>() { solution });
            }
        }

        public string GetXamlPlaceholderErrorMessage(string placeholder)
        {
            var apv = new AllowedPlaceholderValidator();

            var result = apv.ContainsOnlyValidPlaceholders(typeof(ViewGenerationSettings), nameof(ViewGenerationSettings.XamlPlaceholder), placeholder);

            // TODO: also test for blank
            // TODO: also test for unknown placeholders
            // TODO: Localize these responses
            if (!result.isValid)
            {
                return "there is an invalid palceholder";
            }

            if (!placeholder.IsValidXamlOutput())
            {
                return "This desn't look like valid XAML";
            }

            return null;
        }

        internal string GetCodeBehindPlaceholderErrorMessage(string placeholder)
        {
            var apv = new AllowedPlaceholderValidator();

            var result = apv.ContainsOnlyValidPlaceholders(typeof(ViewGenerationSettings), nameof(ViewGenerationSettings.CodePlaceholder), placeholder);

            // TODO: also test for blank
            // TODO: also test for unknown placeholders
            // TODO: Localize these responses
            if (!result.isValid)
            {
                return "there is an invalid placeholder";
            }

            return null;
        }
    }
}
