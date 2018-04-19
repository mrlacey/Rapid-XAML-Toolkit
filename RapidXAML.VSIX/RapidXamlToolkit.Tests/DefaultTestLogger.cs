// <copyright file="DefaultTestLogger.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;

namespace RapidXamlToolkit.Tests
{
    public class DefaultTestLogger : ILogger
    {
        public static DefaultTestLogger Create()
        {
            return new DefaultTestLogger();
        }

        public void RecordError(string message)
        {
        }

        public void RecordException(Exception exception)
        {
        }

        public void RecordInfo(string message)
        {
        }
    }
}
