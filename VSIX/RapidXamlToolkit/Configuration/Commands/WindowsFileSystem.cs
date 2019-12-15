// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System.IO;

namespace RapidXamlToolkit.Commands
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
