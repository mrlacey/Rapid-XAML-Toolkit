// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RapidXamlToolkit.Tests
{
    // Provides more detailed output when compared strings are not equal.
    // Intended to make spotting differences easier.
    public static class StringAssert
    {
        public static void AreEqual(string expected, string actual, string message = "")
        {
            if (expected == actual)
            {
                Assert.IsTrue(true);
            }
            else
            {
                var errorMessage = string.Empty;

                if (expected is null)
                {
                    errorMessage = "Expected is null.";
                }
                else if (actual is null)
                {
                    errorMessage = "Actual is null.";
                }

                if (string.IsNullOrWhiteSpace(errorMessage))
                {
                    if (expected.TrimStart() == actual)
                    {
                        errorMessage = "Whitespace missing at start of output.";
                    }
                    else if (expected == actual.TrimStart())
                    {
                        errorMessage = "Additional whitespace at start of output.";
                    }
                    else if (expected.TrimEnd() == actual)
                    {
                        errorMessage = "Whitespace missing at end of output.";
                    }
                    else if (expected == actual.TrimEnd())
                    {
                        errorMessage = "Additional whitespace at end of output.";
                    }

                    if (string.IsNullOrWhiteSpace(errorMessage))
                    {
                        var aLines = actual.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
                        var eLines = expected.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

                        if (aLines.Length != eLines.Length)
                        {
                            var lines = eLines.Length > 1 ? "lines" : "line";

                            errorMessage = $"Expected {eLines.Length} {lines} in output but had {aLines.Length}.";

                            var aNoLineEndings = actual.Replace("\r", string.Empty).Replace("\n", string.Empty);
                            var eNoLineEndings = expected.Replace("\r", string.Empty).Replace("\n", string.Empty);

                            if (aNoLineEndings == eNoLineEndings)
                            {
                                errorMessage += " Difference is just line endings.";
                            }
                        }
                        else
                        {
                            for (int i = 0; i < eLines.Length; i++)
                            {
                                if (eLines[i] == aLines[i])
                                {
                                    continue;
                                }

                                if (eLines[i].TrimStart() == aLines[i])
                                {
                                    errorMessage = $"Whitespace missing at start of line {i + 1} of output.";
                                    break;
                                }
                                else if (eLines[i] == aLines[i].TrimStart())
                                {
                                    errorMessage = $"Additional whitespace at start of line {i + 1} of output.";
                                    break;
                                }
                                else if (eLines[i].TrimEnd() == aLines[i])
                                {
                                    errorMessage = $"Whitespace missing at end of line {i + 1} of output.";
                                    break;
                                }
                                else if (eLines[i] == aLines[i].TrimEnd())
                                {
                                    errorMessage = $"Additional whitespace at end of line {i + 1} of output.";
                                    break;
                                }
                            }
                        }
                    }
                }

                if (string.IsNullOrWhiteSpace(errorMessage))
                {
                    var i = 0;
                    while (i < actual.Length && i < expected.Length)
                    {
                        if (expected[i] != actual[i])
                        {
                            string FormatChar(char c)
                            {
                                if (string.IsNullOrWhiteSpace(c.ToString()))
                                {
                                    return $"chr({(int)c})";
                                }
                                else
                                {
                                    return c.ToString();
                                }
                            }

                            errorMessage = $"Output first differs at position {i} ({FormatChar(expected[i])}-{FormatChar(actual[i])}).";

                            if (actual.Contains(Environment.NewLine))
                            {
                                var lineNo = actual.Substring(0, i).Split(new[] { Environment.NewLine }, StringSplitOptions.None).Length;
                                errorMessage += $" On line {lineNo}.";
                            }

                            break;
                        }

                        i++;
                    }
                }

                Assert.Fail($"{Environment.NewLine}{errorMessage}{Environment.NewLine}Expected:<{expected}>.{Environment.NewLine}Actual<{actual}>.{Environment.NewLine}{message}");
            }
        }
    }
}
