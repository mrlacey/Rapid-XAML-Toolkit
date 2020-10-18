// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

namespace RapidXamlToolkit.Utils.IO
{
    public interface IFileSystemAbstraction
    {
        bool FileExists(string fileName);

        string GetAllFileText(string fileName);

        string GetFileExtension(string fileName);

        string GetDirectoryName(string fileName);

        string[] ReadAllLines(string path);

        string PathCombine(params string[] paths);

        string GetFileNameWithoutExtension(string fileName);

        void WriteAllFileText(string fileName, string fileContents);
    }
}
