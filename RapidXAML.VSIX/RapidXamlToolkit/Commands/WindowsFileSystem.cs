// <copyright file="WindowsFileSystem.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System.IO;

namespace RapidXamlToolkit
{
    public class WindowsFileSystem : IFileSystemAbstraction
    {
        public bool FileExists(string fileName)
        {
            return File.Exists(fileName);
        }

        public string GetAllFileText(string fileName)
        {
            return File.ReadAllText(fileName);
        }

        public string GetDirectoryName(string fileName)
        {
            return Path.GetDirectoryName(fileName);
        }

        public string GetFileExtension(string fileName)
        {
            return Path.GetExtension(fileName);
        }

        public string GetFileNameWithoutExtension(string fileName)
        {
            return Path.GetFileNameWithoutExtension(fileName);
        }

        public string PathCombine(params string[] paths)
        {
            return Path.Combine(paths);
        }
    }
}
