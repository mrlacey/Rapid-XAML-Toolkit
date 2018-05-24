// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Threading.Tasks;
using RapidXamlToolkit.Analyzers;
using RapidXamlToolkit.Logging;
using RapidXamlToolkit.Options;
using RapidXamlToolkit.Resources;

namespace RapidXamlToolkit.Commands
{
    public class CreateViewCommandLogic
    {
        private readonly Profile profile;
        private readonly ILogger logger;
        private readonly IFileSystemAbstraction fileSystem;
        private readonly IVisualStudioAbstraction vs;

        public CreateViewCommandLogic(Profile profile, ILogger logger, IVisualStudioAbstraction vs, IFileSystemAbstraction fileSystem = null)
        {
            this.profile = profile ?? throw new ArgumentNullException(nameof(profile));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.vs = vs ?? throw new ArgumentNullException(nameof(vs));
            this.fileSystem = fileSystem ?? new WindowsFileSystem();
        }

        public bool CreateView { get; private set; }

        public ProjectWrapper ViewProject { get; private set; }

        public string XamlFileName { get; private set; }

        public string CodeFileName { get; private set; }

        public string XamlFileContents { get; private set; }

        public string CodeFileContents { get; private set; }

        public string ViewFolder { get; private set; }

        public async Task ExecuteAsync(string selectedFileName)
        {
            var vmProj = this.vs.GetActiveProject();

            var fileExt = this.fileSystem.GetFileExtension(selectedFileName);
            var fileContents = this.fileSystem.GetAllFileText(selectedFileName);

            (var syntaxTree, var semModel) = await this.vs.GetDocumentModelsAsync(selectedFileName);

            AnalyzerBase analyzer = null;
            var codeBehindExt = string.Empty;

            switch (fileExt)
            {
                case ".cs":
                    analyzer = new CSharpAnalyzer(this.logger);
                    codeBehindExt = ((CSharpAnalyzer)analyzer).FileExtension;
                    break;
                case ".vb":
                    analyzer = new VisualBasicAnalyzer(this.logger);
                    codeBehindExt = ((VisualBasicAnalyzer)analyzer).FileExtension;
                    break;
            }

            this.CreateView = false;

            if (analyzer != null)
            {
                // IndexOf is allowing for "class " in C# and "Class " in VB
                var analyzerOutput = ((IDocumentAnalyzer)analyzer).GetSingleItemOutput(await syntaxTree.GetRootAsync(), semModel, fileContents.IndexOf("lass "), this.profile);

                var config = this.profile.ViewGeneration;

                var vmClassName = analyzerOutput.Name;

                var baseClassName = vmClassName;

                if (vmClassName.EndsWith(config.ViewModelFileSuffix))
                {
                    baseClassName = vmClassName.Substring(0, vmClassName.LastIndexOf(config.ViewModelFileSuffix, StringComparison.InvariantCulture));
                }

                var viewClassName = $"{baseClassName}{config.XamlFileSuffix}";

                var vmProjName = vmProj.Name;
                string viewProjName;

                this.ViewProject = null;

                if (config.AllInSameProject)
                {
                    this.ViewProject = vmProj;
                    viewProjName = this.ViewProject.Name;
                }
                else
                {
                    var expectedViewProjectName = vmProjName.Replace(config.ViewModelProjectSuffix, config.XamlProjectSuffix);

                    this.ViewProject = this.vs.GetProject(expectedViewProjectName);

                    if (this.ViewProject == null)
                    {
                        this.logger.RecordError(StringRes.Error_UnableToFindProjectInSolution.WithParams(expectedViewProjectName));
                    }

                    viewProjName = this.ViewProject?.Name;
                }

                if (this.ViewProject != null)
                {
                    var folder = this.fileSystem.GetDirectoryName(this.ViewProject.FileName);

                    this.ViewFolder = this.fileSystem.PathCombine(folder, config.XamlFileDirectoryName);

                    // We assume that the type name matches the file name.
                    this.XamlFileName = this.fileSystem.PathCombine(this.ViewFolder, $"{viewClassName}.xaml");
                    this.CodeFileName = this.fileSystem.PathCombine(this.ViewFolder, $"{viewClassName}.xaml.{codeBehindExt}");

                    if (this.fileSystem.FileExists(this.XamlFileName))
                    {
                        this.logger.RecordInfo(StringRes.Info_FileExists.WithParams(this.XamlFileName));

                        var overwrite = this.vs.UserConfirms(StringRes.Prompt_FileExistsTitle, StringRes.Propt_FileExistsMessage);

                        if (overwrite)
                        {
                            this.CreateView = true;
                            this.logger.RecordInfo(StringRes.Info_OverwritingFile.WithParams(this.XamlFileName));
                        }
                        else
                        {
                            this.logger.RecordInfo(StringRes.Info_NotOverwritingFile.WithParams(this.XamlFileName));
                        }
                    }
                    else
                    {
                        this.CreateView = true;
                    }

                    if (this.CreateView)
                    {
                        var viewNamespace = analyzer is CSharpAnalyzer
                                          ? $"{viewProjName}.{config.XamlFileDirectoryName}".TrimEnd('.')
                                          : $"{config.XamlFileDirectoryName}".TrimEnd('.');

                        var vmNamespace = $"{vmProjName}.{config.ViewModelDirectoryName}".TrimEnd('.');

                        var replacementValues = (viewProjName, viewNamespace, vmNamespace, viewClassName, vmClassName, analyzerOutput.Output);

                        this.XamlFileContents = this.ReplacePlaceholders(config.XamlPlaceholder, replacementValues);
                        this.CodeFileContents = this.ReplacePlaceholders(config.CodePlaceholder, replacementValues);
                    }
                }
            }
        }

        private string ReplacePlaceholders(string source, (string projName, string viewNs, string vmNs, string viewClass, string vmClass, string xaml) values)
        {
            return source.Replace(Placeholder.ViewProject, values.projName)
                         .Replace(Placeholder.ViewNamespace, values.viewNs)
                         .Replace(Placeholder.ViewModelNamespace, values.vmNs)
                         .Replace(Placeholder.ViewClass, values.viewClass)
                         .Replace(Placeholder.ViewModelClass, values.vmClass)
                         .Replace(Placeholder.GeneratedXAML, values.xaml);
        }
    }
}
