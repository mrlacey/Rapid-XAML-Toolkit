// <copyright file="ILogger.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace RapidXamlToolkit
{
    public interface ILogger
    {
        void RecordInfo(string message);

        void RecordError(string message);
    }
}
