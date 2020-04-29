// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using Newtonsoft.Json;
using RapidXamlToolkit.ErrorList;
using RapidXamlToolkit.Resources;
using RapidXamlToolkit.VisualStudioIntegration;

namespace RapidXamlToolkit.XamlAnalysis.Tags
{
    public abstract class RapidXamlDisplayedTag : RapidXamlAdornmentTag, IRapidXamlErrorListTag
    {
        private const string SettingsFileName = "settings.xamlAnalysis";

        protected RapidXamlDisplayedTag(TagDependencies deps, string errorCode, TagErrorType defaultErrorType)
            : base(deps.Span, deps.Snapshot, deps.FileName, deps.Logger)
        {
            var line = deps.Snapshot.GetLineFromPosition(deps.Span.Start);
            var col = deps.Span.Start - line.Start.Position;

            this.ErrorCode = errorCode;
            this.Line = line.LineNumber;
            this.Column = col;
            this.DefaultErrorType = defaultErrorType;
            this.VsAbstraction = deps.VsAbstraction;
            this.ProjectPath = deps.ProjectPath;
            this.MoreInfoUrl = deps.MoreInfoUrl;
            this.CustomFeatureUsageOverride = deps.FeatureUsageOverride;
        }

        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the message shown when the error row is expanded.
        /// </summary>
        public string ExtendedMessage { get; set; }

        public string MoreInfoUrl { get; set; }

        public string CustomFeatureUsageOverride { get; set; }

        public int Line { get; }

        public int Column { get; }

        public TagErrorType DefaultErrorType { get; }

        public string ProjectPath { get; private set; }

        public IVisualStudioAbstraction VsAbstraction { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the tag is for something that should show in the Errors tab of the error list.
        /// This should never need setting.
        /// </summary>
        public bool IsInternalError { get; protected set; }

        public TagErrorType ConfiguredErrorType
        {
            get
            {
                if (this.TryGetConfiguredErrorType(this.ErrorCode, out TagErrorType configuredType))
                {
                    return configuredType;
                }
                else
                {
                    return this.DefaultErrorType;
                }
            }
        }

        private static Dictionary<string, (DateTime timeStamp, Dictionary<string, string> settings)> SettingsCache { get; }
            = new Dictionary<string, (DateTime timeStamp, Dictionary<string, string> settings)>();

        public bool TryGetConfiguredErrorType(string errorCode, out TagErrorType tagErrorType)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(this.FileName))
                {
                    tagErrorType = this?.DefaultErrorType ?? TagErrorType.Warning;
                    return false;
                }

                if (string.IsNullOrWhiteSpace(this.ProjectPath))
                {
                    var proj = this.VsAbstraction?.GetProjectContainingFile(this.FileName);

                    if (proj == null)
                    {
                        tagErrorType = this?.DefaultErrorType ?? TagErrorType.Warning;
                        return false;
                    }

                    this.ProjectPath = proj.FullName;
                }

                var settingsFile = Path.Combine(Path.GetDirectoryName(this.ProjectPath), SettingsFileName);

                if (File.Exists(settingsFile))
                {
                    Dictionary<string, string> settings = null;
                    var fileTime = File.GetLastWriteTimeUtc(settingsFile);

                    if (SettingsCache.ContainsKey(settingsFile))
                    {
                        if (SettingsCache[settingsFile].timeStamp == fileTime)
                        {
                            settings = SettingsCache[settingsFile].settings;
                        }
                    }

                    if (settings == null)
                    {
                        var json = File.ReadAllText(settingsFile);
                        settings = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                    }

                    SettingsCache[settingsFile] = (fileTime, settings);

                    if (settings.ContainsKey(errorCode))
                    {
                        if (TagErrorTypeParser.TryParse(settings[errorCode], out tagErrorType))
                        {
                            return true;
                        }
                    }
                }
                else
                {
                    // If settings file is removed need to remove any cached reference to it.
                    if (SettingsCache.ContainsKey(settingsFile))
                    {
                        SettingsCache.Remove(settingsFile);
                    }
                }
            }
            catch (Exception exc)
            {
                this?.Logger?.RecordException(exc);
            }

            // Set to default if no override in file
            tagErrorType = this?.DefaultErrorType ?? TagErrorType.Warning;

            return false;
        }

        public void SetAsHiddenInSettingsFile()
        {
            if (string.IsNullOrWhiteSpace(this.FileName))
            {
                this.Logger.RecordInfo(StringRes.Info_FileNameMissingFromTag);
                return;
            }

            var proj = ProjectHelpers.Dte.Solution.GetProjectContainingFile(this.FileName);

            if (proj == null)
            {
                this.Logger.RecordInfo(StringRes.Info_UnableToFindProjectContainingFile.WithParams(this.FileName));
                return;
            }

            var settingsFile = Path.Combine(Path.GetDirectoryName(proj.FullName), SettingsFileName);

            Dictionary<string, string> settings;

            bool addToProject = false;

            if (File.Exists(settingsFile))
            {
                var json = File.ReadAllText(settingsFile);
                settings = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            }
            else
            {
                settings = new Dictionary<string, string>();
                addToProject = true;
            }

            settings[this.ErrorCode] = nameof(TagErrorType.Hidden);

            var jsonSettings = new JsonSerializerSettings { Formatting = Formatting.Indented };

            File.WriteAllText(settingsFile, JsonConvert.SerializeObject(settings, settings: jsonSettings));

            if (addToProject)
            {
                proj.ProjectItems.AddFromFile(settingsFile);
            }
        }

        public override ITagSpan<IErrorTag> AsErrorTag()
        {
            var span = new SnapshotSpan(this.Snapshot, this.Span);

            return new TagSpan<IErrorTag>(
                span,
                new RapidXamlWarningAdornmentTag(
                    this.ToolTip,
                    this.ConfiguredErrorType.AsVsAdornmentErrorType()));
        }

        public ErrorRow AsErrorRow()
        {
            return new ErrorRow
            {
                ExtendedMessage = this.ExtendedMessage,
                Span = new SnapshotSpan(this.Snapshot, this.Span),
                Message = this.Description,
                ErrorCode = this.ErrorCode,
                IsInternalError = this.IsInternalError,
                ErrorType = this.ConfiguredErrorType,
                MoreInfoUrl = this.MoreInfoUrl,
            };
        }
    }
}
