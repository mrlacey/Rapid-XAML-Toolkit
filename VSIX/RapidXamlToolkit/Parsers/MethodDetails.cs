// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using Microsoft.CodeAnalysis;

namespace RapidXamlToolkit.Parsers
{
    public class MethodDetails : MemberDetails
    {
        public MethodDetails()
            : base(TypeOfMember.Method)
        {
        }

        public static new MethodDetails Empty
        {
            get
            {
                var empty = new MethodDetails
                {
                    Name = MemberDetails.Empty.Name,
                    TypeOfMember = TypeOfMember.Method,
                };

                return empty;
            }
        }

        public string Argument1Name { get; set; }

        public ITypeSymbol Argument1Type { get; set; }

        public string Argument2Name { get; set; }

        public ITypeSymbol Argument2Type { get; set; }
    }
}
