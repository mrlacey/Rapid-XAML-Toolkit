// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using RapidXaml;
using RapidXamlToolkit.Resources;
using RapidXamlToolkit.VisualStudioIntegration;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.XamlAnalysis.Actions
{
    public class CustomAnalysisAction : BaseSuggestedAction
    {
        public CustomAnalysisAction(string file, CustomAnalysisTag tag)
            : base(file)
        {
            this.Tag = tag;

            if (string.IsNullOrWhiteSpace(tag.ActionText))
            {
                this.DisplayText = StringRes.UI_XamlAnalysisFixUnavailable;
                this.IsEnabled = false;
            }
            else
            {
                this.DisplayText = tag.ActionText;
            }

            this.CustomFeatureUsageOverride = tag.CustomFeatureUsageOverride;
        }

        public CustomAnalysisTag Tag { get; }

        public static CustomAnalysisAction[] Create(CustomAnalysisTag tag, string file)
        {
            var result = new List<CustomAnalysisAction>
            {
                new CustomAnalysisAction(file, tag),
            };

            foreach (var altAction in tag.AlternativeActions)
            {
                result.Add(new CustomAnalysisAction(file, tag.RecreateForAlternativeAction(altAction)));
            }

            return result.ToArray();
        }

        public override void Execute(CancellationToken cancellationToken)
        {
            var vs = new VisualStudioTextManipulation(ProjectHelpers.Dte);
            vs.StartSingleUndoOperation(this.DisplayText);
            try
            {
                this.InnerExecute(vs, this.Tag, cancellationToken);

                foreach (var suppAction in this.Tag.SupplementaryActions)
                {
                    var sat = this.RepurposeTagForSupplementaryAction(this.Tag, suppAction);
                    this.InnerExecute(vs, sat, cancellationToken);
                }
            }
            finally
            {
                vs.EndSingleUndoOperation();
            }
        }

        private CustomAnalysisTag RepurposeTagForSupplementaryAction(CustomAnalysisTag tag, AnalysisAction suppAction)
        {
            var catd = new CustomAnalysisTagDependencies
            {
                AnalyzedElement = tag.AnalyzedElement,
                Action = suppAction,
                ElementName = tag.ElementName,
                FileName = tag.FileName,
                InsertPos = tag.InsertPosition,
                Logger = tag.Logger,
                ProjectFilePath = tag.ProjectFilePath,
                Snapshot = tag.Snapshot,
                //// Don't need to set VsAbstraction as tags only need it for referencing settings but supplementary actions don't need to know about settings.
            };

            if (suppAction.Location == null)
            {
                catd.Span = new RapidXamlSpan(tag.Span.Start, tag.Span.Length);
            }
            else
            {
                catd.Span = suppAction.Location.AddStartPos(tag.InsertPosition);
            }

            return new CustomAnalysisTag(catd);
        }

#pragma warning disable IDE0060 // Remove unused parameter - cancellationToken
        private void InnerExecute(VisualStudioTextManipulation vs, CustomAnalysisTag tag, CancellationToken cancellationToken)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            switch (tag.Action)
            {
                case RapidXaml.ActionType.AddAttribute:
                    var lineNumber = tag.Snapshot.GetLineNumberFromPosition(tag.InsertPosition) + 1;

                    if (tag.AnalyzedElement.IsSelfClosing())
                    {
                        var before = $"/>";
                        var after = $"{tag.Name}=\"{tag.Value}\" />";

                        vs.ReplaceInActiveDocOnLine(before, after, lineNumber);
                    }
                    else
                    {
                        var before = $">";
                        var after = $" {tag.Name}=\"{tag.Value}\" >";

                        vs.ReplaceInActiveDocOnLine(before, after, lineNumber);
                    }

                    break;

                case RapidXaml.ActionType.AddChild:

                    // Allow for self-closing elements
                    if (tag.AnalyzedElement.IsSelfClosing())
                    {
                        var replacementXaml = $">{Environment.NewLine}{tag.Content}{Environment.NewLine}</{tag.ElementName}>";

                        var insertLine = tag.Snapshot.GetLineNumberFromPosition(tag.InsertPosition) + 1;
                        vs.ReplaceInActiveDocOnLine("/>", replacementXaml, insertLine);
                        tag.AnalyzedElement.OverrideIsSelfClosing(false);
                    }
                    else
                    {
                        // Allows for opening and closing tags on same or different lines
                        var insertLine = tag.Snapshot.GetLineNumberFromPosition(tag.InsertPosition) + 1;
                        vs.InsertIntoActiveDocOnLineAfterClosingTag(insertLine, tag.Content);
                    }

                    break;

                case RapidXaml.ActionType.HighlightWithoutAction:
                    // As the name implies, do nothing.
                    break;

                case RapidXaml.ActionType.RemoveAttribute:
                    if (tag.IsInlineAttribute ?? false)
                    {
                        var currentAttribute = $" {tag.Name}=\"{tag.Value}\"";
                        vs.RemoveInActiveDocOnLine(currentAttribute, tag.GetDesignerLineNumber());
                    }
                    else
                    {
                        var attrs = tag.AnalyzedElement.GetAttributes(tag.Name).ToList();

                        if (attrs.Count() == 1)
                        {
                            var attr = attrs.First();
                            var toRemove =
                                tag.AnalyzedElement.OriginalString.Substring(
                                    attr.Location.Start - tag.InsertPosition,
                                    attr.Location.Length);

                            vs.RemoveInActiveDocOnLine(toRemove, tag.GetDesignerLineNumber());
                        }
                    }

                    break;

                case RapidXaml.ActionType.RemoveChild:
                    vs.RemoveInActiveDocOnLine(tag.Element.OriginalString, tag.GetDesignerLineNumber());
                    break;

                case RapidXaml.ActionType.ReplaceElement:
                    vs.ReplaceInActiveDocOnLine(
                        tag.AnalyzedElement.OriginalString,
                        tag.Content,
                        tag.Snapshot.GetLineNumberFromPosition(tag.AnalyzedElement.Location.Start));
                    break;

                case RapidXaml.ActionType.RenameElement:
                    // Just change opening tags as Visual Studio will change closing tags automatically
                    var renameLineNumber = tag.Snapshot.GetLineNumberFromPosition(tag.InsertPosition);
                    vs.ReplaceInActiveDocOnLine(tag.ElementName, tag.Name, renameLineNumber);

                    foreach (var childAttr in tag.AnalyzedElement.ChildAttributes)
                    {
                        renameLineNumber = tag.Snapshot.GetLineNumberFromPosition(childAttr.Location.Start);
                        vs.ReplaceInActiveDocOnLine($"{tag.ElementName}.{childAttr.Name}", $"{tag.Name}.{childAttr.Name}", renameLineNumber);
                    }

                    break;
                case RapidXaml.ActionType.AddXmlns:
                    vs.AddXmlnsAliasToActiveDoc(tag.Name, tag.Value);

                    break;

                case RapidXaml.ActionType.CreateResource:
                    vs.AddResource(tag.Content, tag.Name, tag.Value);

                    break;

                case RapidXaml.ActionType.RemoveContent:
                    var current = $">{this.Tag.Value}</{this.Tag.ElementName}>";
                    var replaceWith = $" />";
                    vs.ReplaceInActiveDocOnLine(current, replaceWith, this.Tag.GetDesignerLineNumber());
                    tag.AnalyzedElement.OverrideIsSelfClosing(true);
                    break;

                default:
                    // Using a newer version of CustomAnalysis than the VSIX knows about
                    // Doing nothing is a suitable fallback.
                    tag.Logger?.RecordInfo(StringRes.Info_UnknownCustomActionType);
                    break;
            }
        }
    }
}
