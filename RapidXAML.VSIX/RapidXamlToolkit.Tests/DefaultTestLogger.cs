// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

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

        public void RecordFeatureUsage(string feature)
        {
        }

        public void RecordInfo(string message)
        {
        }
    }
}
