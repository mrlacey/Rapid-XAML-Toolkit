// <copyright file="ILogger.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;

namespace RapidXamlToolkit
{
    public interface ILogger
    {
        void RecordInfo(string message);

        void RecordFeatureUsage(string feature);

        void RecordError(string message);

        void RecordException(Exception exception);
    }
}
