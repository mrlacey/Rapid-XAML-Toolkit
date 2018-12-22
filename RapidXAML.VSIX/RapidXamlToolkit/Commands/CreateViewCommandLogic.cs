// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Threading.Tasks;
using RapidXamlToolkit.Logging;
using RapidXamlToolkit.Options;
using RapidXamlToolkit.Parsers;
using RapidXamlToolkit.Resources;

namespace RapidXamlToolkit.Commands
{
    public class CreateViewCommandLogic
    {
        private readonly ILogger logger;
        private readonly IFileSystemAbstraction fileSystem;
        private readonly Profile profileOverride;
        private readonly IVisualStudioAbstraction vs;

        public CreateViewCommandLogic(ILogger logger, IVisualStudioAbstraction vs, IFileSystemAbstraction fileSystem = null, Profile profileOverride = null)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.vs = vs ?? throw new ArgumentNullException(nameof(vs));
            this.fileSystem = fileSystem ?? new WindowsFileSystem();
            this.profileOverride = profileOverride;
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

            CodeParserBase parser = null;
            var codeBehindExt = string.Empty;
            var indent = await this.vs.GetXamlIndentAsync();

            switch (fileExt)
            {
                case ".cs":
                    parser = new CSharpParser(this.logger, indent, this.profileOverride);
                    codeBehindExt = ((CSharpParser)parser).FileExtension;
                    break;
                case ".vb":
                    parser = new VisualBasicParser(this.logger, indent, this.profileOverride);
                    codeBehindExt = ((VisualBasicParser)parser).FileExtension;
                    break;
            }

            this.CreateView = false;

            if (parser != null)
            {
                // IndexOf is allowing for "class " in C# and "Class " in VB
                var cursorPos = fileContents.IndexOf("lass ");

                if (cursorPos == -1 && codeBehindExt == "vb")
                {
                    // If not a class, there may be a module
                    cursorPos = fileContents.IndexOf("odule ");
                }

                if (cursorPos < 0)
                {
                    this.logger.RecordInfo(StringRes.Info_CouldNotFindClassInFile.WithParams(selectedFileName));
                    return;
                }

                (var syntaxTree, var semModel) = await this.vs.GetDocumentModelsAsync(selectedFileName);

                var syntaxRoot = await syntaxTree.GetRootAsync();

                var parserOutput = ((IDocumentParser)parser).GetSingleItemOutput(syntaxRoot, semModel, cursorPos);

                var config = parser.Profile.ViewGeneration;

                var vmClassName = parserOutput.Name;

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
                        // Allow for different namespace conventions
                        var viewNamespace = parser is CSharpParser
                                          ? $"{viewProjName}.{config.XamlFileDirectoryName}".TrimEnd('.')
                                          : $"{config.XamlFileDirectoryName}".TrimEnd('.');

                        var vmNamespace = $"{vmProjName}.{config.ViewModelDirectoryName}".TrimEnd('.');

                        var replacementValues = (viewProjName, viewNamespace, vmNamespace, viewClassName, vmClassName);

                        this.XamlFileContents = this.ReplacePlaceholders(config.XamlPlaceholder, replacementValues);

                        var placeholderPos = this.XamlFileContents.IndexOf(Placeholder.GeneratedXAML);
                        var startOfPlaceholderLine = this.XamlFileContents.Substring(0, placeholderPos).LastIndexOf(Environment.NewLine);

                        var insertIndent = placeholderPos - startOfPlaceholderLine - Environment.NewLine.Length;

                        this.XamlFileContents = this.XamlFileContents.Replace(Placeholder.GeneratedXAML, parserOutput.Output.Replace(Environment.NewLine, Environment.NewLine + new string(' ', insertIndent)).Trim());

                        this.CodeFileContents = this.ReplacePlaceholders(config.CodePlaceholder, replacementValues);
                    }
                }
            }
        }

        private string ReplacePlaceholders(string source, (string projName, string viewNs, string vmNs, string viewClass, string vmClass) values)
        {
            return source.Replace(Placeholder.ViewProject, values.projName)
                         .Replace(Placeholder.ViewNamespace, values.viewNs)
                         .Replace(Placeholder.ViewModelNamespace, values.vmNs)
                         .Replace(Placeholder.ViewClass, values.viewClass)
                         .Replace(Placeholder.ViewModelClass, values.vmClass);
        }
    }
}
