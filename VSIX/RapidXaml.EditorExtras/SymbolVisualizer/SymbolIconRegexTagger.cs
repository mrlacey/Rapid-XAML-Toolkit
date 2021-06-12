// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.Text;

namespace RapidXaml.EditorExtras.SymbolVisualizer
{
    internal abstract class SymbolIconRegexTagger : RegexTagger<SymbolIconTag>
    {
        protected SymbolIconRegexTagger(ITextBuffer buffer, string matchExpression)
            : base(buffer, new[] { new Regex(matchExpression, RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase) })
        {
        }

        protected SymbolIconTag TryCreateSymbolIconTagForMatch(Match match, int lineStart, int spanStart, string snapshotText, SymbolType symbolType, string supportedElement)
        {
            if (snapshotText.Contains(match.Value) && match.Groups.Count == 4)
            {
                var value = match.Groups[2].Value;

                int matchPos;

                if (spanStart > 0)
                {
                    // looking at a span that is smaller than the whole document
                    matchPos = snapshotText.IndexOf(match.Value, spanStart);
                }
                else
                {
                    matchPos = lineStart + match.Index;
                }

                if (matchPos >= 0)
                {
                    var openingTagPos = snapshotText.Substring(0, matchPos).LastIndexOf('<');

                    var closingTagPos = snapshotText.Substring(matchPos).IndexOf('>');

                    if (openingTagPos >= 0 && closingTagPos > 0)
                    {
                        var elementString = snapshotText.Substring(openingTagPos, closingTagPos + matchPos - openingTagPos + 1);

                        var elementName = elementString.Split(' ')[0].Split('<', ':').ToList().Last().Trim();

                        switch (symbolType)
                        {
                            case SymbolType.Glyph:

                                string fontFamily = string.Empty;
                                var attributeOpening = @"FontFamily=""";
                                var ffPos = elementString.IndexOf(attributeOpening);

                                if (ffPos > -1)
                                {
                                    var closingPos = elementString.IndexOf('"', ffPos + attributeOpening.Length);

                                    if (closingPos > 0)
                                    {
                                        fontFamily = elementString.Substring(ffPos + attributeOpening.Length, closingPos - ffPos - attributeOpening.Length);
                                    }
                                }

                                // Cruse matching to try and identify a "font awesome" font
                                // Better to show from the embedded file rather than hope it's installed on the system
                                if (fontFamily.ToLowerInvariant().Contains("f")
                                    && fontFamily.ToLowerInvariant().Contains("awesome"))
                                {
                                    if (int.TryParse(value, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out int code))
                                    {
                                        string unicodeString = char.ConvertFromUtf32(code);

                                        var icon = SymbolIconAdornment.KnownFontAwesomeIcons.FirstOrDefault(i => i.Value == unicodeString);

                                        if (icon.Key != null)
                                        {
                                            return new SymbolIconTag(icon.Key, SymbolType.FontAwesome);
                                        }
                                        else
                                        {
                                            System.Diagnostics.Debug.WriteLine($"unknown value {value}");
                                            return null;
                                        }
                                    }
                                }

                                return new SymbolIconTag(value, SymbolType.Glyph, fontFamily);

                            case SymbolType.Symbol:

                                if (elementName == supportedElement)
                                {
                                    if (SymbolIconAdornment.KnownSymbols.ContainsKey(value))
                                    {
                                        return new SymbolIconTag(value, symbolType);
                                    }
                                    else
                                    {
                                        System.Diagnostics.Debug.WriteLine($"unknown value {value}");
                                    }
                                }

                                break;

                            case SymbolType.FontAwesome:

                                if (elementName == supportedElement)
                                {
                                    if (SymbolIconAdornment.KnownFontAwesomeIcons.ContainsKey(value))
                                    {
                                        return new SymbolIconTag(value, symbolType);
                                    }
                                    else
                                    {
                                        System.Diagnostics.Debug.WriteLine($"unknown value {value}");
                                    }
                                }

                                break;
                        }
                    }
                }
            }

            return null;
        }
    }
}
