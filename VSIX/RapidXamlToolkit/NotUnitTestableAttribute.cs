// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;

namespace RapidXamlToolkit
{
    /// <summary>
    /// Informational attribute used to indicate why a piece of code is not covered by unit tests.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class NotUnitTestableAttribute : Attribute
    {
        public NotUnitTestableAttribute(string reason)
        {
            this.Reason = reason;
        }

        public string Reason { get; }
    }
}
