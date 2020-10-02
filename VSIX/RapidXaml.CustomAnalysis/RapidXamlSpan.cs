// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

namespace RapidXaml
{
    public class RapidXamlSpan
    {
        public RapidXamlSpan()
        {
        }

        public RapidXamlSpan(int start, int length)
        {
            this.Start = start;
            this.Length = length;
        }

        public int Start { get; set; } = -1;

        public int Length { get; set; } = -1;

        public RapidXamlSpan CloneWithAdjustedLocationStart(int startChange)
        {
            return new RapidXamlSpan(this.Start + startChange, this.Length);
        }

        public override string ToString()
        {
            return $"({this.Start}, {this.Length})";
        }
    }
}
