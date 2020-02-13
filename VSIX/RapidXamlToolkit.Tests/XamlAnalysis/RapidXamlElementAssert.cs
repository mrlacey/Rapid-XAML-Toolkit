// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidXaml;

namespace RapidXamlToolkit.Tests.XamlAnalysis
{
    public static class RapidXamlElementAssert
    {
        public static void AreEqual(RapidXamlElement expected, RapidXamlElement actual, string message = "")
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

                // Basic count checks
                if (string.IsNullOrWhiteSpace(errorMessage))
                {
                    if (expected.Name != actual.Name)
                    {
                        errorMessage = "Element names do not match.";
                    }
                    else if (expected.Content != actual.Content)
                    {
                        errorMessage = "Content does not match.";
                    }
                    else if (expected.Attributes.Count != actual.Attributes.Count)
                    {
                        errorMessage = "Attribute counts do not match.";
                    }
                    else if (expected.Children.Count != actual.Children.Count)
                    {
                        errorMessage = "Children counts do not match.";
                    }
                }

                // Attribute checks
                if (string.IsNullOrWhiteSpace(errorMessage))
                {
                    if (expected.Attributes.Count > 0)
                    {
                        for (int i = 0; i < expected.Attributes.Count; i++)
                        {
                            var expAttr = expected.Attributes[0];
                            var actAttr = actual.Attributes[0];

                            if (expAttr.Name != actAttr.Name)
                            {
                                errorMessage = $"At index {i}, found attribute named '{actAttr.Name}' when expecting '{expAttr.Name}'.";
                            }
                            else if (expAttr.Value != actAttr.Value)
                            {
                                errorMessage = $"At index {i}, found attribute with value '{actAttr.Value}' when expecting '{expAttr.Value}'.";
                            }
                        }
                    }
                }

                if (!string.IsNullOrWhiteSpace(errorMessage))
                {
                    Assert.Fail($"{Environment.NewLine}{errorMessage}{Environment.NewLine}Expected:<{expected}>.{Environment.NewLine}Actual<{actual}>.{Environment.NewLine}{message}");
                }
            }
        }
    }
}
