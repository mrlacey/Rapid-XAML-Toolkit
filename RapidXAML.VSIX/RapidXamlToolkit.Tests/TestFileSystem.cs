// <copyright file="TestFileSystem.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System.IO;

namespace RapidXamlToolkit.Tests
{
    public class TestFileSystem : IFileSystemAbstraction
    {
        public bool FileExistsResponse { get; set; } = false;

        public string FileText { get; set; } = null;

        public bool FileExists(string fileName)
        {
            return this.FileExistsResponse;
        }

        public string GetAllFileText(string fileName)
        {
            return this.FileText;
        }

        public string GetDirectoryName(string fileName)
        {
            return Path.GetDirectoryName(fileName);
        }

        public string GetFileExtension(string fileName)
        {
            return Path.GetExtension(fileName);
        }

        public string PathCombine(params string[] paths)
        {
            return Path.Combine(paths);
        }
    }
}
