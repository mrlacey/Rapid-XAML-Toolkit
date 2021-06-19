// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidXaml;

namespace RapidXamlToolkit.Tests.AutoFix
{
    public class BespokeTestFileSystem : TestFileSystem
    {
        public Dictionary<string, string> FilesAndContents { get; set; } = new Dictionary<string, string>();

        public Dictionary<string, string> WrittenFiles { get; set; } = new Dictionary<string, string>();

        public string[] FileLines { get; set; }

        public override string GetAllFileText(string fileName)
        {
            if (this.FilesAndContents.ContainsKey(fileName))
            {
                return this.FilesAndContents[fileName];
            }
            else
            {
                return string.Empty;
            }
        }

        public override void WriteAllFileText(string fileName, string fileContents)
        {
            this.WrittenFiles.Add(fileName, fileContents);
        }

        public override string[] ReadAllLines(string path)
        {
            return this.FileLines;
        }
    }
}
