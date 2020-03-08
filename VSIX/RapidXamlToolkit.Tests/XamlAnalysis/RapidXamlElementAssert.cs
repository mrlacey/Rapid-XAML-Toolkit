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
                        StringAssert.AreEqual(expected.Content, actual.Content, $"Evaluating content for {expected}");
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
                            var expAttr = expected.Attributes[i];
                            var actAttr = actual.Attributes[i];

                            if (expAttr.Name != actAttr.Name)
                            {
                                errorMessage = $"At index {i}, found attribute named '{actAttr.Name}' when expecting '{expAttr.Name}'.";
                            }
                            else if (expAttr.HasStringValue != actAttr.HasStringValue)
                            {
                                if (expAttr.HasStringValue)
                                {
                                    errorMessage = $"At index {i}, expected a string value ({expAttr.StringValue}) but found {actAttr.Children.Count} child elements.";
                                }
                                else
                                {
                                    errorMessage = $"At index {i}, expected children but found a string value ({actAttr.StringValue}).";
                                }
                            }
                            else if (expAttr.HasStringValue && expAttr.StringValue != actAttr.StringValue)
                            {
                                errorMessage = $"At index {i}, found attribute with value '{actAttr.StringValue}' when expecting '{expAttr.StringValue}'.";
                            }
                            else if (expAttr.IsInline != actAttr.IsInline)
                            {
                                errorMessage = $"At index {i}, found attribute with IsInline '{actAttr.IsInline}' when expecting '{expAttr.IsInline}'.";
                            }
                            else if (expAttr.Children.Count != actAttr.Children.Count)
                            {
                                errorMessage = $"At index {i}, found attribute with '{actAttr.Children.Count}' children when expecting '{expAttr.Children.Count}'.";
                            }
                            else
                            {
                                for (int j = 0; j < expAttr.Children.Count; j++)
                                {
                                    if (actAttr.Children[j].Name != expAttr.Children[j].Name)
                                    {
                                        errorMessage = $"At index {i}, found element attribute with child '{actAttr.Children[j].Name}' when expecting '{expAttr.Children[j].Name}'. (Name comparison only!)";
                                    }
                                }
                            }
                        }
                    }
                }

                // Children checks
                if (string.IsNullOrWhiteSpace(errorMessage))
                {
                    if (expected.Children.Count > 0)
                    {
                        for (int i = 0; i < expected.Children.Count; i++)
                        {
                            var expChild = expected.Children[0];
                            var actChild = actual.Children[0];

                            if (expChild.Name != actChild.Name)
                            {
                                errorMessage = $"At index {i}, found child named '{actChild.Name}' when expecting '{expChild.Name}'.";
                            }
                            else if (expChild.Attributes.Count != actChild.Attributes.Count)
                            {
                                errorMessage = $"At index {i}, found child with {actChild.Attributes.Count} attributes when expecting '{expChild.Attributes.Count}'.";
                            }
                            else if (expChild.Children.Count != actChild.Children.Count)
                            {
                                errorMessage = $"At index {i}, found child with {actChild.Children.Count} children when expecting '{expChild.Children.Count}'.";
                            }
                            else
                            {
                                // Check children recursively
                                AreEqual(expChild, actChild);
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
