// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Drawing;

namespace RapidXamlToolkit
{
    public static class ColorHelper
    {
        // TODO: FIx, as this is broken
        public static Color? GetColor(string color)
        {
            if (!color?.TrimStart().StartsWith("#") ?? false)
            {
                color = GetHexForNamedColor(color.Trim());
            }

            try
            {
                // This should be getting the color from the hex value, not the name
                return Color.FromName(color.Trim());
            }
            catch
            {
                return null;
            }
        }

        public static string GetHexForNamedColor(string colorName)
        {
            switch (colorName?.ToLowerInvariant().Replace(" ", string.Empty) ?? string.Empty)
            {
                case "aliceblue": return "#F0F8FF";
                case "antiquewhite": return "#FAEBD7";
                case "aqua": return "#00FFFF";
                case "aquamarine": return "#7FFFD4";
                case "azure": return "#F0FFFF";
                case "beige": return "#F5F5DC";
                case "bisque": return "#FFE4C4";
                case "black": return "#000000";
                case "blanchedalmond": return "#FFEBCD";
                case "blue": return "#0000FF";
                case "blueviolet": return "#8A2BE2";
                case "brown": return "#A52A2A";
                case "burlywood": return "#DEB887";
                case "cadetblue": return "#5F9EA0";
                case "chartreuse": return "#7FFF00";
                case "chocolate": return "#D2691E";
                case "coral": return "#FF7F50";
                case "cornflowerblue": return "#6495ED";
                case "cornsilk": return "#FFF8DC";
                case "crimson": return "#DC143C";
                case "cyan": return "#00FFFF";
                case "darkblue": return "#00008B";
                case "darkcyan": return "#008B8B";
                case "darkgoldenrod": return "#B8860B";
                case "darkgray": return "#A9A9A9";
                case "darkgreen": return "#006400";
                case "darkgrey": return "#A9A9A9";
                case "darkkhaki": return "#BDB76B";
                case "darkmagenta": return "#8B008B";
                case "darkolivegreen": return "#556B2F";
                case "darkorange": return "#FF8C00";
                case "darkorchid": return "#9932CC";
                case "darkred": return "#8B0000";
                case "darksalmon": return "#E9967A";
                case "darkseagreen": return "#8FBC8B";
                case "darkslateblue": return "#483D8B";
                case "darkslategray": return "#2F4F4F";
                case "darkslategrey": return "#2F4F4F";
                case "darkturquoise": return "#00CED1";
                case "darkviolet": return "#9400D3";
                case "deeppink": return "#FF1493";
                case "deepskyblue": return "#00BFFF";
                case "dimgray": return "#696969";
                case "dimgrey": return "#696969";
                case "dodgerblue": return "#1E90FF";
                case "firebrick": return "#B22222";
                case "floralwhite": return "#FFFAF0";
                case "forestgreen": return "#228B22";
                case "fuchsia": return "#FF00FF";
                case "gainsboro": return "#DCDCDC";
                case "ghostwhite": return "#F8F8FF";
                case "gold": return "#FFD700";
                case "goldenrod": return "#DAA520";
                case "gray": return "#808080";
                case "green": return "#008000";
                case "greenyellow": return "#ADFF2F";
                case "grey": return "#808080";
                case "honeydew": return "#F0FFF0";
                case "hotpink": return "#FF69B4";
                case "indianred": return "#CD5C5C";
                case "indigo": return "#4B0082";
                case "ivory": return "#FFFFF0";
                case "khaki": return "#F0E68C";
                case "lavender": return "#E6E6FA";
                case "lavenderblush": return "#FFF0F5";
                case "lawngreen": return "#7CFC00";
                case "lemonchiffon": return "#FFFACD";
                case "lightblue": return "#ADD8E6";
                case "lightcoral": return "#F08080";
                case "lightcyan": return "#E0FFFF";
                case "lightgoldenrodyellow": return "#FAFAD2";
                case "lightgray": return "#D3D3D3";
                case "lightgreen": return "#90EE90";
                case "lightgrey": return "#d3d3d3";
                case "lightpink": return "#FFB6C1";
                case "lightsalmon": return "#FFA07A";
                case "lightseagreen": return "#20B2AA";
                case "lightskyblue": return "#87CEFA";
                case "lightslategray": return "#778899";
                case "lightslategrey": return "#778899";
                case "lightsteelblue": return "#B0C4DE";
                case "lightyellow": return "#FFFFE0";
                case "lime": return "#00FF00";
                case "limegreen": return "#32CD32";
                case "linen": return "#FAF0E6";
                case "magenta": return "#FF00FF";
                case "maroon": return "#800000";
                case "mediumaquamarine": return "#66CDAA";
                case "mediumblue": return "#0000CD";
                case "mediumorchid": return "#BA55D3";
                case "mediumpurple": return "#9370DB";
                case "mediumseagreen": return "#3CB371";
                case "mediumslateblue": return "#7B68EE";
                case "mediumspringgreen": return "#00FA9A";
                case "mediumturquoise": return "#48D1CC";
                case "mediumvioletred": return "#C71585";
                case "midnightblue": return "#191970";
                case "mintcream": return "#F5FFFA";
                case "mistyrose": return "#FFE4E1";
                case "moccasin": return "#FFE4B5";
                case "navajowhite": return "#FFDEAD";
                case "navy": return "#000080";
                case "oldlace": return "#FDF5E6";
                case "olive": return "#808000";
                case "olivedrab": return "#6B8E23";
                case "orange": return "#FFA500";
                case "orangered": return "#FF4500";
                case "orchid": return "#DA70D6";
                case "palegoldenrod": return "#EEE8AA";
                case "palegreen": return "#98FB98";
                case "paleturquoise": return "#AFEEEE";
                case "palevioletred": return "#DB7093";
                case "papayawhip": return "#FFEFD5";
                case "peachpuff": return "#FFDAB9";
                case "peru": return "#CD853F";
                case "pink": return "#FFC0CB";
                case "plum": return "#DDA0DD";
                case "powderblue": return "#B0E0E6";
                case "purple": return "#800080";
                case "rebeccapurple": return "#663399";
                case "red": return "#FF0000";
                case "rosybrown": return "#BC8F8F";
                case "royalblue": return "#4169E1";
                case "saddlebrown": return "#8B4513";
                case "salmon": return "#FA8072";
                case "sandybrown": return "#F4A460";
                case "seagreen": return "#2E8B57";
                case "seashell": return "#FFF5EE";
                case "sienna": return "#A0522D";
                case "silver": return "#C0C0C0";
                case "skyblue": return "#87CEEB";
                case "slateblue": return "#6A5ACD";
                case "slategray": return "#708090";
                case "slategrey": return "#708090";
                case "snow": return "#FFFAFA";
                case "springgreen": return "#00FF7F";
                case "steelblue": return "#4682B4";
                case "tan": return "#D2B48C";
                case "teal": return "#008080";
                case "thistle": return "#D8BFD8";
                case "tomato": return "#FF6347";
                case "turquoise": return "#40E0D0";
                case "violet": return "#EE82EE";
                case "wheat": return "#F5DEB3";
                case "white": return "#FFFFFF";
                case "whitesmoke": return "#F5F5F5";
                case "yellow": return "#FFFF00";
                case "yellowgreen": return "#9ACD32";
                default: return colorName;
            }
        }

        public static double GetLuminance(Color color)
        {
            var rs = color.R / 255D;
            var gs = color.G / 255D;
            var bs = color.B / 255D;

            var r = (rs <= 0.03928) ? rs / 12.92 : Math.Pow((rs + 0.055) / 1.055, 2.4);
            var g = (gs <= 0.03928) ? gs / 12.92 : Math.Pow((gs + 0.055) / 1.055, 2.4);
            var b = (bs <= 0.03928) ? bs / 12.92 : Math.Pow((bs + 0.055) / 1.055, 2.4);

            return (0.2126 * r) + (0.7152 * g) + (0.0722 * b);
        }

        public static double GetLuminanceRatio(double l1, double l2)
        {
            return (Math.Max(l1, l2) + 0.05) / (Math.Min(l1, l2) + 0.05);
        }
    }
}
