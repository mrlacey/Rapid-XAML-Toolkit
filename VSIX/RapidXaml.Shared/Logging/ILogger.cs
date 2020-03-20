// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;

namespace RapidXamlToolkit.Logging
{
    public interface ILogger
    {
        bool UseExtendedLogging { get; set; }

        void RecordNotice(string message);

        void RecordInfo(string message);

        void RecordFeatureUsage(string feature);

        /// <summary>
        /// Record an error.
        /// </summary>
        /// <param name="message">Description of the error.</param>
        /// <param name="force">Try and make this error visible in VS UI.</param>
        void RecordError(string message, bool force = false);

        void RecordGeneralError(string message);

        void RecordException(Exception exception);
    }
}
