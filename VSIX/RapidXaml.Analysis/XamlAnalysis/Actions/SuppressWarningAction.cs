// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.IO;
using System.Threading;
using Microsoft.VisualStudio.Shell;
using Newtonsoft.Json;
using RapidXamlToolkit.ErrorList;
using RapidXamlToolkit.Resources;
using RapidXamlToolkit.VisualStudioIntegration;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.XamlAnalysis.Actions
{
    internal class SuppressWarningAction : BaseSuggestedAction
    {
        public SuppressWarningAction(string file)
            : base(file)
        {
        }

        public string ErrorCode { get; private set; }

        public RapidXamlDisplayedTag Tag { get; private set; }

        public SuggestedActionsSource Source { get; private set; }

        public static SuppressWarningAction Create(RapidXamlDisplayedTag tag, string file, SuggestedActionsSource suggestedActionsSource)
        {
            var result = new SuppressWarningAction(file)
            {
                Tag = tag,
                DisplayText = StringRes.UI_SuggestedActionDoNotWarn.WithParams(tag.ErrorCode),
                ErrorCode = tag.ErrorCode,
                Source = suggestedActionsSource,
            };

            return result;
        }

        public override void Execute(CancellationToken cancellationToken)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            this.SetAsHiddenInSettingsFile(this.Tag);
            this.Source.Refresh();
            RapidXamlDocumentCache.RemoveTags(this.Tag.FileName, this.ErrorCode);
            RapidXamlDocumentCache.Invalidate(this.Tag.FileName);
            RapidXamlDocumentCache.TryUpdate(this.Tag.FileName);
            TableDataSource.Instance.CleanErrors(this.Tag.FileName);
        }

        public void SetAsHiddenInSettingsFile(RapidXamlDisplayedTag tag)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();

            if (string.IsNullOrWhiteSpace(tag.FileName))
            {
                SharedRapidXamlPackage.Logger?.RecordInfo(StringRes.Info_FileNameMissingFromTag);
                return;
            }

            var proj = ProjectHelpers.Dte.Solution.GetProjectContainingFile(tag.FileName);

            if (proj == null)
            {
                SharedRapidXamlPackage.Logger?.RecordInfo(StringRes.Info_UnableToFindProjectContainingFile.WithParams(tag.FileName));
                return;
            }

            var settingsFile = Path.Combine(Path.GetDirectoryName(proj.FullName), RapidXamlDisplayedTag.SettingsFileName);

            Dictionary<string, string> settings;

            bool addToProject = false;

            if (System.IO.File.Exists(settingsFile))
            {
                var json = System.IO.File.ReadAllText(settingsFile);
                settings = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            }
            else
            {
                settings = new Dictionary<string, string>();
                addToProject = true;
            }

            settings[this.ErrorCode] = nameof(TagErrorType.Hidden);

            var jsonSettings = new JsonSerializerSettings { Formatting = Formatting.Indented };

            System.IO.File.WriteAllText(settingsFile, JsonConvert.SerializeObject(settings, settings: jsonSettings));

            if (addToProject)
            {
                proj.ProjectItems.AddFromFile(settingsFile);
            }
        }
    }
}
