// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

namespace RapidXaml
{
    public enum ActionType
    {
        /// <summary>
        /// Add an attribute to the element.
        /// </summary>
        AddAttribute,

        /// <summary>
        /// Add a child to the element.
        /// </summary>
        AddChild,

        /// <summary>
        /// Add an xmlns alias to the top-level document element.
        /// </summary>
        AddXmlns,

        /// <summary>
        /// Report the error but provide no fix.
        /// </summary>
        HighlightWithoutAction,

        /// <summary>
        /// Remove an attribute from the element.
        /// </summary>
        RemoveAttribute,

        /// <summary>
        /// Remove a child from the element.
        /// </summary>
        RemoveChild,

        /// <summary>
        /// Rename the selected element.
        /// </summary>
        RenameElement,

        /// <summary>
        /// Replace the entire element.
        /// </summary>
        ReplaceElement,

        /// <summary>
        /// Create an entry in a Resource file (RESW or RESX).
        /// </summary>
        CreateResource,
    }
}
