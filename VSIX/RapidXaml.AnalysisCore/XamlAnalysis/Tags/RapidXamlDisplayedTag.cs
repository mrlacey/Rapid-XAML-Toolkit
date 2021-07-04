// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
#if !AUTOFIX
using Newtonsoft.Json;
#endif
using RapidXamlToolkit.VisualStudioIntegration;

namespace RapidXamlToolkit.XamlAnalysis.Tags
{
    public abstract class RapidXamlDisplayedTag : RapidXamlAdornmentTag, IRapidXamlErrorListTag
    {
        public const string SettingsFileName = "settings.xamlAnalysis";

        protected RapidXamlDisplayedTag(TagDependencies deps, string errorCode, TagErrorType defaultErrorType)
            : base(deps.Span, deps.Snapshot, deps.FileName, deps.Logger)
        {
            if (deps.Span.Start > deps.Snapshot.Length)
            {
                if (deps.Snapshot.Length > 0)
                {
                    deps.Logger?.RecordError(
                        $"Span calculated outside snapshot for ErrorCode={errorCode}",
                        deps.TelemetryProperties);
                }

                // Reset the span location to something that's definitely valid
                deps.Span = (0, 0);
            }

            var (startPos, lineNumber) = deps.Snapshot.GetLineDetailsFromPosition(deps.Span.Start);
            var col = deps.Span.Start - startPos;

            this.ErrorCode = errorCode;
            this.Line = lineNumber;
            this.Column = col;
            this.DefaultErrorType = defaultErrorType;
            this.VsPfp = deps.VsPfp;
            this.ProjectFilePath = deps.ProjectFilePath;
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

        public string ProjectFilePath { get; private set; }

        public IVisualStudioProjectFilePath VsPfp { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the tag is for something that should show in the Errors tab of the error list.
        /// This should never need setting.
        /// </summary>
        public bool IsInternalError { get; protected set; }

        public new TagErrorType ConfiguredErrorType
        {
            get
            {
#if AUTOFIX
                return this.DefaultErrorType;
#else
                if (this.TryGetConfiguredErrorType(this.ErrorCode, out TagErrorType configuredType))
                {
                    return configuredType;
                }
                else
                {
                    return this.DefaultErrorType;
                }
#endif
            }
        }

#if !AUTOFIX
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

                if (string.IsNullOrWhiteSpace(this.ProjectFilePath))
                {
                    var projFileName = this.VsPfp.GetPathOfProjectContainingFile(this.FileName);

                    if (string.IsNullOrWhiteSpace(projFileName))
                    {
                        tagErrorType = this?.DefaultErrorType ?? TagErrorType.Warning;
                        return false;
                    }

                    this.ProjectFilePath = projFileName;
                }

                var settingsFile = Path.Combine(Path.GetDirectoryName(this.ProjectFilePath), SettingsFileName);

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
                        if (TagErrorTypeParser.TryParse(settings[errorCode], base.Logger, out tagErrorType))
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
#endif
    }
}
