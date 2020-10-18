// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System.IO;
using RapidXamlToolkit.Utils.IO;

namespace RapidXaml
{
    public class NetStandardFileSystemAccess : IFileSystemAbstraction
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

        public string[] ReadAllLines(string path)
        {
            return File.ReadAllLines(path);
        }

        public void WriteAllFileText(string fileName, string fileContents)
        {
            File.WriteAllText(fileName, fileContents);
        }
    }
}
