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
        private string projectName;

        internal TableEntriesSnapshot(FileErrorCollection result)
        {
            this.projectName = result.Project;
            this.Errors.AddRange(result.Errors);
            this.FilePath = result.FilePath;
        }

        public List<ErrorRow> Errors { get; } = new List<ErrorRow>();

        public override int VersionNumber { get; } = 1;

        public override int Count
        {
            get { return this.Errors.Count; }
        }

        public string FilePath { get; set; }

        public override bool TryGetValue(int index, string columnName, out object content)
        {
            content = null;

            if (index < 0 || index >= this.Errors.Count)
            {
                return false;
            }

            var error = this.Errors[index];

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
                    content = error.IsMessage ? KnownMonikers.StatusInformationOutline : error.IsError ? KnownMonikers.ProcessError : KnownMonikers.ReportWarning;
                    return true;
                case StandardTableKeyNames.ErrorSeverity:
                    content = error.IsMessage ? __VSERRORCATEGORY.EC_MESSAGE : error.IsError ? __VSERRORCATEGORY.EC_ERROR : __VSERRORCATEGORY.EC_WARNING;
                    return true;
                case StandardTableKeyNames.Priority:
                    content = vsTaskPriority.vsTaskPriorityMedium;
                    return true;
                case StandardTableKeyNames.ErrorSource:
                    content = ErrorSource.Other;
                    return true;
                case StandardTableKeyNames.ErrorCode:
                    content = error.ErrorCode;
                    return true;
                case StandardTableKeyNames.ProjectName:
                    content = this.projectName;
                    return true;
                case StandardTableKeyNames.DocumentName:
                    content = this.FilePath;
                    return true;
                case StandardTableKeyNames.Line:
                    content = error.Span.Start.GetContainingLine().LineNumber;
                    return true;
                case StandardTableKeyNames.Column:
                    var position = this.Errors[index].Span.Start;
                    var line = position.GetContainingLine();
                    content = position.Position - line.Start.Position;
                    return true;
                case StandardTableKeyNames.ErrorCodeToolTip:
                case StandardTableKeyNames.HelpLink:
                    content = $"https://github.com/microsoft/Rapd-XAML-Toolkit/docs/warnings/{error.ErrorCode}.md";
                    return true;
                default:
                    content = null;
                    return false;
            }
        }

        public override bool CanCreateDetailsContent(int index)
        {
            return !string.IsNullOrEmpty(this.Errors[index].ExtendedMessage);
        }

        public override bool TryCreateDetailsStringContent(int index, out string content)
        {
            var error = this.Errors[index];
            content = error.ExtendedMessage;
            return true;
        }
    }
}
