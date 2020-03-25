// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Linq;
using System.Threading;
using RapidXaml;
using RapidXamlToolkit.VisualStudioIntegration;
using RapidXamlToolkit.XamlAnalysis.Processors;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.XamlAnalysis.Actions
{
    public class CustomAnalysisAction : BaseSuggestedAction
    {
        public CustomAnalysisAction(string file, CustomAnalysisTag tag)
            : base(file)
        {
            this.Tag = tag;
            this.DisplayText = tag.ActionText;

            this.CustomFeatureUsageOverride = tag.CustomFeatureUsageOverride;
        }

        public CustomAnalysisTag Tag { get; }

        public static CustomAnalysisAction Create(CustomAnalysisTag tag, string file)
        {
            return new CustomAnalysisAction(file, tag);
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

                RapidXamlDocumentCache.TryUpdate(this.File);
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
                Snapshot = tag.Snapshot,
            };

            if (suppAction.Location == null)
            {
                catd.Span = tag.Span;
            }
            else
            {
                catd.Span = suppAction.Location.ToSpanPlusStartPos(tag.InsertPosition);
            }

            return new CustomAnalysisTag(catd);
        }

        private void InnerExecute(VisualStudioTextManipulation vs, CustomAnalysisTag tag, CancellationToken cancellationToken)
        {
            switch (tag.Action)
            {
                case RapidXaml.ActionType.AddAttribute:
                    var lineNumber = tag.Snapshot.GetLineNumberFromPosition(tag.InsertPosition) + 1;

                    // Can't rely on the original element name as this may be supplemental after it's been renamed
                    if (XamlElementProcessor.IsSelfClosing(tag.AnalyzedElement.OriginalString))
                    {
                        var before = $"/>";
                        var after = $"{tag.Name}=\"{tag.Value}\" />";

                        vs.ReplaceInActiveDocOnLine(before, after, lineNumber);
                    }
                    else
                    {
                        var before = $">";
                        var after = $"{tag.Name}=\"{tag.Value}\" /";

                        vs.ReplaceInActiveDocOnLine(before, after, lineNumber);
                    }

                    break;

                case RapidXaml.ActionType.AddChild:

                    var origXaml = tag.AnalyzedElement.OriginalString;

                    // Allow for self-closing elements
                    if (origXaml.EndsWith("/>"))
                    {
                        var replacementXaml = $">{Environment.NewLine}{tag.Content}{Environment.NewLine}</{tag.ElementName}>";

                        var insertLine = tag.Snapshot.GetLineNumberFromPosition(tag.InsertPosition) + 1;
                        vs.ReplaceInActiveDocOnLine("/>", replacementXaml, insertLine);
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
            }
        }
    }
}
