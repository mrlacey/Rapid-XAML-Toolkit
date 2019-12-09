// Copyright (c) Matt Lacey Ltd.. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;

namespace RapidXamlToolkit.Parsers
{
    public class MemberDetails
    {
        public MemberDetails(TypeOfMember type)
        {
            this.TypeOfMember = type;
        }

        public static MemberDetails Empty => new MemberDetails(TypeOfMember.None)
        {
            Name = string.Empty,
        };

        public string Name { get; set; }

        public List<AttributeDetails> Attributes { get; set; } = new List<AttributeDetails>();

        public TypeOfMember TypeOfMember { get; set; }
    }
}
