// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;

namespace RapidXamlToolkit.Logging
{
    public interface ILogger
    {
        bool UseExtendedLogging { get; set; }

        void RecordNotice(string message);

        void RecordInfo(string message);

        /// <summary>
        /// Record that a feature was used.
        /// </summary>
        /// <param name="feature">Name of the feature.</param>
        /// <param name="quiet">If true, this won't be shown to the user.</param>
        void RecordFeatureUsage(string feature, bool quiet = false);

        /// <summary>
        /// Record an error.
        /// </summary>
        /// <param name="message">Description of the error.</param>
        /// <param name="properties">Additional properties to enable easier debugging.</param>
        /// <param name="force">Try and make this error visible in VS UI.</param>
        void RecordError(string message, Dictionary<string, string> properties = null, bool force = false);

        void RecordGeneralError(string message);

        void RecordException(Exception exception);
    }
}
