// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

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
