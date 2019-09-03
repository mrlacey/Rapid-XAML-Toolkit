// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

namespace RapidXamlToolkit.XamlAnalysis
{
    public class TagSuppression
    {
        // Optional.
        // If specified, tags with this ErrorCode will be supressed.
        public string TagErrorCode { get; set; }

        // Required.
        // Used to identify the file to which the suppresion applies.
        public string FileName { get; set; }

        // Optional.
        // A substring of the elements definition that can be used to limit the elements the suppression is applied to.
        // Only applies if TagErrorCode is also specified.
        public string ElementIdentifier { get; set; }

        // Optional.
        // For documentation purposes only. Keep a record of why the supression exists.
        public string Reason { get; set; }
    }
}
