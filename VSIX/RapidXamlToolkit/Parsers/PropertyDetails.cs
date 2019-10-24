// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.CodeAnalysis;

namespace RapidXamlToolkit.Parsers
{
    public class PropertyDetails : MemberDetails
    {
        public PropertyDetails()
            : base(TypeOfMember.Property)
        {
        }

        public static new PropertyDetails Empty
        {
            get
            {
                var empty = new PropertyDetails
                {
                    Name = MemberDetails.Empty.Name,
                    TypeOfMember = TypeOfMember.Property,
                    PropertyType = string.Empty,
                    IsReadOnly = false,
                    Symbol = null,
                };

                return empty;
            }
        }

        public string PropertyType { get; set; }

        public bool IsReadOnly { get; set; }

        public ITypeSymbol Symbol { get; set; }
    }
}
