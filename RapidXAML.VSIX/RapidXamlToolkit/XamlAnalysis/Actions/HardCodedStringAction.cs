// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Xml.Linq;
using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Imaging.Interop;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using RapidXamlToolkit.Resources;
using RapidXamlToolkit.VisualStudioIntegration;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.XamlAnalysis.Actions
{
    public class HardCodedStringAction : BaseSuggestedAction
    {
        private HardCodedStringTag tag;

        public HardCodedStringAction(string file)
            : base(file)
        {
            this.DisplayText = StringRes.UI_MoveHardCodedString;
        }

        public override ImageMoniker IconMoniker => KnownMonikers.GenerateResource;

        public static HardCodedStringAction Create(HardCodedStringTag tag, string file, ITextView view)
        {
            var result = new HardCodedStringAction(file)
            {
                tag = tag,
                View = view,
            };

            return result;
        }

        // TODO: Need to deal with whether attribute/child-element-attribute/default-value
        public override void Execute(CancellationToken cancellationToken)
        {
            var vs = new VisualStudioTextManipulation(ProjectHelpers.Dte);
            vs.StartSingleUndoOperation(StringRes.Info_UndoContextMoveStringToResourceFile);

            try
            {
                var resPath = this.GetResourceFilePath();

                // TODO: work out what to do if the resfile is open and has unsaved changes - can't force save, so what?

                this.AddResource(resPath, $"{this.tag.UidValue}.Text", this.tag.Value);

                if (this.tag.AttributeType == AttributeType.Inline)
                {
                    var currentAttribute = $"Text=\"{this.tag.Value}\"";

                    if (this.tag.UidExists)
                    {
                        vs.RemoveInActiveDocOnLine(currentAttribute, this.tag.GetDesignerLineNumber());
                    }
                    else
                    {
                        var uidTag = $"x:Uid=\"{this.tag.UidValue}\"";
                        vs.ReplaceInActiveDocOnLine(currentAttribute, uidTag, this.tag.GetDesignerLineNumber());
                    }
                }
                else if (this.tag.AttributeType == AttributeType.Element)
                {
                    var currentAttribute = $"<TextBlock.Text>{this.tag.Value}</TextBlock.Text>";

                    vs.RemoveInActiveDocOnLine(currentAttribute, this.tag.GetDesignerLineNumber());

                    if (!this.tag.UidExists)
                    {
                        var uidTag = $"<TextBlock x:Uid=\"{this.tag.UidValue}\"";
                        vs.ReplaceInActiveDocOnLineOrAbove("<TextBlock", uidTag, this.tag.GetDesignerLineNumber());
                    }
                }

                RapidXamlDocumentCache.TryUpdate(this.File);
            }
            finally
            {
                vs.EndSingleUndoOperation();
            }
        }

        private void AddResource(string resPath, string name, string value)
        {
            var xdoc = XDocument.Load(resPath);

            // Don't want to create a duplicate entry.
            var alreadyExists = false;

            foreach (var element in xdoc.Descendants("data"))
            {
                if (element.Attribute("name")?.Value == name)
                {
                    alreadyExists = true;
                    break;
                }
            }

            if (!alreadyExists)
            {
                var newData = new XElement("data");

                newData.Add(new XAttribute("name", name));

                newData.Add(new XAttribute(XNamespace.Xml + "space", "preserve"));
                newData.Add(new XElement("value", value));
                xdoc.Element("root").Add(newData);
                xdoc.Save(resPath);
            }
        }

        // TODO: support .resx too - will need to know which to use of project includes both
        private string GetResourceFilePath()
        {
            var reswFiles = new List<string>();

            // See also https://docs.microsoft.com/en-us/dotnet/api/microsoft.visualstudio.shell.interop.ivssolution.getprojectenum?view=visualstudiosdk-2017#Microsoft_VisualStudio_Shell_Interop_IVsSolution_GetProjectEnum_System_UInt32_System_Guid__Microsoft_VisualStudio_Shell_Interop_IEnumHierarchies__
            void IterateProjItems(EnvDTE.ProjectItem projectItem)
            {
                var item = projectItem.ProjectItems.GetEnumerator();

                while (item.MoveNext())
                {
                    if (item.Current is EnvDTE.ProjectItem projItem)
                    {
                        if (projItem.ProjectItems.Count > 0)
                        {
                            IterateProjItems(projItem);
                        }
                        else if (projItem.Name.EndsWith(".resw"))
                        {
                            reswFiles.Add(projItem.FileNames[0]);
                        }
                    }
                }
            }

            void IterateProject(EnvDTE.Project project)
            {
                var item = project.ProjectItems.GetEnumerator();

                while (item.MoveNext())
                {
                    if (item.Current is EnvDTE.ProjectItem projItem)
                    {
                        IterateProjItems(projItem);
                    }
                }
            }

            var proj = ProjectHelpers.Dte.Solution.GetProjectContainingFile(this.File);

            // For very large projects this can be very inefficient. When an issue, consider caching or querying the file system directly.
            IterateProject(proj);

            if (reswFiles.Count == 0)
            {
                // Don't try and create one if none exists.
                // TODO: work out how to provide feedback to the user that none exists and they first need to create one
                return null;
            }
            else if (reswFiles.Count == 1)
            {
                return reswFiles.First();
            }
            else
            {
                // TODO: If multiples, find the default locale and use that
                return @"C:\Users\matt\source\repos\RxtDevTesting\UwpApp\Strings\en-us\Resources.resw";
            }
        }
    }
}
