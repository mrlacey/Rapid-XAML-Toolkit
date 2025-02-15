// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using EnvDTE;
using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell.TableControl;
using Microsoft.VisualStudio.Shell.TableManager;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.ErrorList
{
    public class TableEntriesSnapshot : WpfTableEntriesSnapshotBase
    {
        private readonly string projectName;

        internal TableEntriesSnapshot(FileErrorCollection result)
        {
            this.projectName = result.Project;
            this.Errors.AddRange(result.Errors);
            this.FilePath = result.FilePath;
        }

        public List<ErrorRow> Errors { get; } = new List<ErrorRow>();

        public override int VersionNumber { get; } = 1;

        public override int Count => this.Errors.Count;

        public string FilePath { get; set; }

        public override bool TryGetValue(int index, string columnName, out object content)
        {
            try
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

                        if (error.IsInternalError)
                        {
                            content = KnownMonikers.ProcessError;
                        }
                        else
                        {
                            switch (error.ErrorType)
                            {
                                case TagErrorType.Error:
                                    content = KnownMonikers.StatusErrorOutline;
                                    break;
                                case TagErrorType.Warning:
                                    content = KnownMonikers.StatusWarningOutline;
                                    break;
                                case TagErrorType.Suggestion:
                                    content = KnownMonikers.StatusInformationOutline;
                                    break;
                                case TagErrorType.Hidden:
                                default:
                                    content = null;
                                    break;
                            }
                        }

                        return true;
                    case StandardTableKeyNames.ErrorSeverity:
                        switch (error.ErrorType)
                        {
                            case TagErrorType.Error:
                                content = __VSERRORCATEGORY.EC_ERROR;
                                break;
                            case TagErrorType.Warning:
                                content = __VSERRORCATEGORY.EC_WARNING;
                                break;
                            case TagErrorType.Suggestion:
                                content = __VSERRORCATEGORY.EC_MESSAGE;
                                break;
                            case TagErrorType.Hidden:
                            default:
                                content = -1;
                                break;
                        }

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
                        var position = error.Span.Start;
                        var line = position.GetContainingLine();
                        content = position.Position - line.Start.Position;
                        return true;
                    case StandardTableKeyNames.ErrorCodeToolTip:
                    case StandardTableKeyNames.HelpLink:
                        if (!string.IsNullOrWhiteSpace(error.MoreInfoUrl))
                        {
                            content = error.MoreInfoUrl;
                            return true;
                        }
                        else if (error.ErrorCode.StartsWith("RXT"))
                        {
                            content = $"https://github.com/mrlacey/Rapid-XAML-Toolkit/blob/main/docs/warnings/{error.ErrorCode}.md";
                            return true;
                        }
                        else
                        {
                            return false;
                        }

                    default:
                        content = null;
                        return false;
                }
            }
            catch (Exception exc)
            {
                RapidXamlPackage.Logger?.RecordException(exc);

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
            try
            {
                var error = this.Errors[index];
                content = error.ExtendedMessage;
                return true;
            }
            catch (Exception exc)
            {
                RapidXamlPackage.Logger?.RecordException(exc);

                content = null;
                return false;
            }
        }
    }
}
