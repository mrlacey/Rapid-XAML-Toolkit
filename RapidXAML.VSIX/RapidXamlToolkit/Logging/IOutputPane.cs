// <copyright file="IOutputPane.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace RapidXamlToolkit
{
    public interface IOutputPane
    {
        void Write(string message);

        void Activate();
    }
}
