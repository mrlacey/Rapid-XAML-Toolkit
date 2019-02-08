using System.Collections.Generic;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell.TableControl;
using Microsoft.VisualStudio.Shell.TableManager;

namespace RapidXamlToolkit.ErrorList
{
    public class TableEntriesSnapshot : WpfTableEntriesSnapshotBase
    {
        private string _projectName;
        private DTE2 _dte;

        internal TableEntriesSnapshot(ValidationResult result)
        {
            _dte = (DTE2)Package.GetGlobalService(typeof(DTE));
            _projectName = result.Project;
            Errors.AddRange(result.Errors);
            Url = result.Url;
        }

        public List<Error> Errors { get; } = new List<Error>();

        public override int VersionNumber { get; } = 1;

        public override int Count
        {
            get { return Errors.Count; }
        }

        public string Url { get; set; }

        public override bool TryGetValue(int index, string columnName, out object content)
        {
            content = null;

            if (index < 0 || index >= Errors.Count)
            {
                return false;
            }

            Error error = Errors[index];

            switch (columnName)
            {
                case StandardTableKeyNames.ErrorCategory:
                    content = vsTaskCategories.vsTaskCategoryHTML;
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
                    content = "W3C";
                    return true;
                case StandardTableKeyNames.ProjectName:
                    content = _projectName;
                    return true;
                default:
                    content = null;
                    return false;
            }
        }

        public override bool CanCreateDetailsContent(int index)
        {
            return !string.IsNullOrEmpty(Errors[index].Extract);
        }

        public override bool TryCreateDetailsStringContent(int index, out string content)
        {
            var error = Errors[index];
            content = error.Extract;
            return true;
        }
    }
}
