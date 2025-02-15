﻿// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RapidXamlToolkit
{
    public class Placeholder
    {
        public const string PropertyName = "$name$";

        public const string SafePropertyName = "$safename$";

        public const string PropertyNameWithSpaces = "$namewithspaces$";

        public const string PropertyType = "$type$";

        public const string IncrementingInteger = "$incint$";

        public const string RepeatingInteger = "$repint$";

        public const string SubProperties = "$subprops$";

        public const string EnumMembers = "$members$";

        public const string EnumElement = "$element$";

        public const string EnumElementWithSpaces = "$elementwithspaces$";

        public const string EnumPropName = "$enumname$";

        public const string NoOutput = "$nooutput$";

        public const string XName = "$xname$";

        public const string RepeatingXName = "$repxname$";

        public const string MethodName = "$method$";

        public const string Argument1 = "$arg1$";

        public const string Argument2 = "$arg2$";

        private static List<string> all = null;

        public static List<string> All()
        {
            if (all == null)
            {
                Type type = typeof(Placeholder);
                var flags = BindingFlags.Static | BindingFlags.Public;
                var fields = type.GetFields(flags).Where(f => f.IsLiteral);

                all = fields.Select(f => f.GetValue(null).ToString()).ToList();
            }

            return all;
        }
    }
}
