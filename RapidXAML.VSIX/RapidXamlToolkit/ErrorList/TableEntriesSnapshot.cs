// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using EnvDTE;
using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell.TableControl;
using Microsoft.VisualStudio.Shell.TableManager;

namespace RapidXamlToolkit.ErrorList
{
    public class TableEntriesSnapshot : WpfTableEntriesSnapshotBase
    {
        private string _projectName;

        internal TableEntriesSnapshot(FileErrorCollection result)
        {
            _projectName = result.Project;
            Errors.AddRange(result.Errors);
            FilePath = result.FilePath;
        }

        public List<ErrorRow> Errors { get; } = new List<ErrorRow>();

        public override int VersionNumber { get; } = 1;

        public override int Count
        {
            get { return Errors.Count; }
        }

        public string FilePath { get; set; }

        public override bool TryGetValue(int index, string columnName, out object content)
        {
            content = null;

            if (index < 0 || index >= Errors.Count)
            {
                return false;
            }

            var error = Errors[index];

            switch (columnName)
            {
                case StandardTableKeyNames.ErrorCategory:
                    content = vsTaskCategories.vsTaskCategoryMisc;
                    return true;
                case StandardTableKeyNames.BuildTool:
                    content = "RXT";
                    return true;
                case StandardTableKeyNames.Text:
                    content = error.Message;
                    return true;
                case StandardTableKeyNames.PriorityImage:
                case StandardTableKeyNames.ErrorSeverityImage:
                    content = KnownMonikers.ReportWarning;
                    return true;
                case StandardTableKeyNames.ErrorSeverity:
                    content = __VSERRORCATEGORY.EC_WARNING;
                    return true;
                case StandardTableKeyNames.Priority:
                    content = vsTaskPriority.vsTaskPriorityMedium;
                    return true;
                case StandardTableKeyNames.ErrorSource:
                    content = ErrorSource.Other;
                    return true;
                case StandardTableKeyNames.ErrorCode:
                    content = "RXT00?";
                    return true;
                case StandardTableKeyNames.ProjectName:
                    content = _projectName;
                    return true;
                case StandardTableKeyNames.DocumentName:
                    content = FilePath;
                    return true;
                case StandardTableKeyNames.Line:
                    content = error.Span.Start.GetContainingLine().LineNumber;
                    return true;
                case StandardTableKeyNames.Column:
                    var position = this.Errors[index].Span.Start;
                    var line = position.GetContainingLine();
                    content = position.Position - line.Start.Position;
                    return true;
                default:
                    content = null;
                    return false;
            }
        }

        public override bool CanCreateDetailsContent(int index)
        {
            return !string.IsNullOrEmpty(Errors[index].ExtendedMessage);
        }

        public override bool TryCreateDetailsStringContent(int index, out string content)
        {
            var error = Errors[index];
            content = error.ExtendedMessage;
            return true;
        }
    }
}
