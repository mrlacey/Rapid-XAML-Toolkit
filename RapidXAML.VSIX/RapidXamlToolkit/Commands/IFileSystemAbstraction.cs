// <copyright file="IFileSystemAbstraction.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace RapidXamlToolkit
{
    public interface IFileSystemAbstraction
    {
        bool FileExists(string fileName);

        string GetAllFileText(string fileName);

        string GetFileExtension(string fileName);

        string GetDirectoryName(string fileName);

        string PathCombine(params string[] paths);
    }
}
