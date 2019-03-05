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
        public HardCodedStringAction(string file, ITextView view, string elementName, string attributeName)
            : base(file)
        {
            this.View = view;
            this.DisplayText = StringRes.UI_MoveHardCodedString;
            this.ElementName = elementName;
            this.AttributeName = attributeName;
        }

        public override ImageMoniker IconMoniker => KnownMonikers.GenerateResource;

        public HardCodedStringTag Tag { get; protected set; }

        public string ElementName { get; }

        public string AttributeName { get; }

        public override void Execute(CancellationToken cancellationToken)
        {
            var vs = new VisualStudioTextManipulation(ProjectHelpers.Dte);
            vs.StartSingleUndoOperation(StringRes.Info_UndoContextMoveStringToResourceFile);

            try
            {
                var resPath = this.GetResourceFilePath();

                // If the resource file is open with unsaved changes VS will prompt about data being lost.
                this.AddResource(resPath, $"{this.Tag.UidValue}.{this.AttributeName}", this.Tag.Value);

                if (this.Tag.AttributeType == AttributeType.Inline)
                {
                    var currentAttribute = $"{this.AttributeName}=\"{this.Tag.Value}\"";

                    if (this.Tag.UidExists)
                    {
                        vs.RemoveInActiveDocOnLine(currentAttribute, this.Tag.GetDesignerLineNumber());
                    }
                    else
                    {
                        var uidTag = $"x:Uid=\"{this.Tag.UidValue}\"";
                        vs.ReplaceInActiveDocOnLine(currentAttribute, uidTag, this.Tag.GetDesignerLineNumber());
                    }
                }
                else if (this.Tag.AttributeType == AttributeType.Element)
                {
                    var currentAttribute = $"<{this.ElementName}.{this.AttributeName}>{this.Tag.Value}</{this.ElementName}.{this.AttributeName}>";

                    vs.RemoveInActiveDocOnLine(currentAttribute, this.Tag.GetDesignerLineNumber());

                    if (!this.Tag.UidExists)
                    {
                        var uidTag = $"<{this.ElementName} x:Uid=\"{this.Tag.UidValue}\"";
                        vs.ReplaceInActiveDocOnLineOrAbove($"<{this.ElementName}", uidTag, this.Tag.GetDesignerLineNumber());
                    }
                }
                else if (this.Tag.AttributeType == AttributeType.DefaultValue)
                {
                    var current = $">{this.Tag.Value}</{this.ElementName}>";
                    var replaceWith = this.Tag.UidExists ? " />" : $" x:Uid=\"{this.Tag.UidValue}\" />";

                    vs.ReplaceInActiveDocOnLine(current, replaceWith, this.Tag.GetDesignerLineNumber());
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

        // TODO: support .resx too - will need to know which to use if project includes both
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
