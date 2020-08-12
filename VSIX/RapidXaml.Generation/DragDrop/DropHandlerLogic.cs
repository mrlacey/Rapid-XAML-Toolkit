// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Threading.Tasks;
using RapidXamlToolkit.Commands;
using RapidXamlToolkit.Logging;
using RapidXamlToolkit.Options;
using RapidXamlToolkit.Parsers;
using RapidXamlToolkit.Resources;
using RapidXamlToolkit.VisualStudioIntegration;

namespace RapidXamlToolkit.DragDrop
{
    public class DropHandlerLogic
    {
        private readonly ILogger logger;
        private readonly IFileSystemAbstraction fileSystem;
        private readonly Profile profileOverride;
        private readonly IVisualStudioAbstraction vs;

        public DropHandlerLogic(ILogger logger, IVisualStudioAbstraction vs, IFileSystemAbstraction fileSystem = null, Profile profileOverride = null)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.vs = vs ?? throw new ArgumentNullException(nameof(vs));
            this.fileSystem = fileSystem ?? new WindowsFileSystem();
            this.profileOverride = profileOverride;
        }

        public async Task<string> ExecuteAsync(string draggedFilename, int insertLinePadding, ProjectType projectType)
        {
            var fileContents = this.fileSystem.GetAllFileText(draggedFilename);
            var fileExt = this.fileSystem.GetFileExtension(draggedFilename);

            var indent = await this.vs.GetXamlIndentAsync();

            var parser = fileExt == ".cs" ? new CSharpParser(this.logger, projectType, indent, this.profileOverride)
                                          : (IDocumentParser)new VisualBasicParser(this.logger, projectType, indent, this.profileOverride);

            // IndexOf is allowing for "class " in C# and "Class " in VB
            var cursorPos = fileContents.AsSpan().FirstIndexOf(" class ", " Class ");

            if (cursorPos == -1 && fileExt == ".vb")
            {
                // If not a class, there may be a module
                cursorPos = fileContents.IndexOf("odule ");
            }

            if (cursorPos < 0)
            {
                this.logger.RecordInfo(StringRes.Info_CouldNotFindClassInFile.WithParams(draggedFilename));
                return null;
            }

            (var syntaxTree, var semModel) = await this.vs.GetDocumentModelsAsync(draggedFilename);

            var treeRoot = await syntaxTree.GetRootAsync();

            var parserOutput = parser.GetSingleItemOutput(treeRoot, semModel, cursorPos);

            string textOutput = parserOutput.Output;

            if (insertLinePadding > 0)
            {
                textOutput = textOutput.Replace(Environment.NewLine, Environment.NewLine + new string(' ', insertLinePadding));
            }

            return textOutput;
        }
    }
}
