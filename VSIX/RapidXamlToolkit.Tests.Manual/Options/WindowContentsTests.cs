// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Appium.Windows;
using WindowsTestHelpers;

namespace RapidXamlToolkit.Tests.Manual.Options
{
    [TestClass]
    public class WindowContentsTests : TestsUsingWinAppDriver
    {
        private const string PathToExe = @"..\..\..\OptionsEmulator\bin\Debug\OptionsEmulator.exe";

        private const string ArtifactDir = @"C:\UIT\RXT\";

        private readonly TestSettings testSettings;

        public WindowContentsTests()
        {
            this.testSettings = new TestSettings();

            if (string.IsNullOrWhiteSpace(this.testSettings.SubscriptionKey)
             || this.testSettings.SubscriptionKey == "MUST-BE-SET")
            {
                Assert.Fail("Set subscription key in testsettings.json");
            }

            // Need to be admin to save screenshots
            ExecutionEnvironment.CheckRunningAsAdmin();
        }

        [TestMethod]
        public async Task AllTextVisibleInHighContrastModesAsync()
        {
            if (!Directory.Exists(ArtifactDir))
            {
                Directory.CreateDirectory(ArtifactDir);
            }

            string imagePathFormat = ArtifactDir + "{0}_{1}.png";

            async Task GetScreenshotImagesAsync(string identifier)
            {
                async Task TakeScreenshotThenExtractTextAsync(WindowsDriver<WindowsElement> session, string imageId)
                {
                    var imgFile = string.Format(imagePathFormat, imageId, identifier);
                    var linesFile = $"{imgFile}.lines.txt";
                    var wordsFile = $"{imgFile}.words.txt";

                    CursorHelper.MoveToTopLeftOfScreen(); // To avoid cursor being over any controls that changes their visuals

                    session.SaveScreenshot(imgFile);

                    if (this.testSettings.IsFreeSubscription)
                    {
                        // Therefore rate limited
                        await Task.Delay(30000); // This pause to avoid querying the server too often
                    }

                    // TODO ISSUE #243 get cognitive service to extract text from image working again.
                    //// var (lines, words) = await CognitiveServicesHelpers.GetTextFromImageAsync(imgFile, this.testSettings.SubscriptionKey, this.testSettings.AzureRegion);

                    //// File.WriteAllLines(linesFile, lines);
                    //// File.WriteAllLines(wordsFile, words);
                }

                var appSession = WinAppDriverHelper.LaunchExe(PathToExe);

                appSession.Manage().Window.Maximize();

                await TakeScreenshotThenExtractTextAsync(appSession, "Settings");

                appSession.FindElementByName("Profile").Click();
                await Task.Delay(1000);

                await TakeScreenshotThenExtractTextAsync(appSession, "Profile");

                appSession.FindElementByName("Mappings").Click();
                await Task.Delay(1000);

                appSession.ClickElement("Close");
            }

            // TODO ISSUE #243 get changing high contrast settings working again.
            var sysSettings = new SystemSettingsHelper();

            await sysSettings.TurnOffHighContrastAsync();
            VirtualKeyboard.MinimizeAllWindows();
            await GetScreenshotImagesAsync("normal");

            await sysSettings.SwitchToHighContrastNumber1Async();
            VirtualKeyboard.MinimizeAllWindows();
            await GetScreenshotImagesAsync("HiContrast1");

            await sysSettings.SwitchToHighContrastNumber2Async();
            VirtualKeyboard.MinimizeAllWindows();
            await GetScreenshotImagesAsync("HiContrast2");

            await sysSettings.SwitchToHighContrastBlackAsync();
            VirtualKeyboard.MinimizeAllWindows();
            await GetScreenshotImagesAsync("HiContrastBlack");

            await sysSettings.SwitchToHighContrastWhiteAsync();
            VirtualKeyboard.MinimizeAllWindows();
            await GetScreenshotImagesAsync("HiContrastWhite");

            void CompareWordsText(string refIdentifier, string compIdentifier)
            {
                // These pages have text that goes beyond the TextBox bounds
                // - as text size changes this affects the number of lines shown which results in different text on the screen
                var pagesToIgnore = new[] { "Mapping", "Profile" };

                foreach (var refFile in Directory.EnumerateFiles(ArtifactDir, $"*{refIdentifier}*.words.txt"))
                {
                    bool skipCheck = false;
                    foreach (var toIgnore in pagesToIgnore)
                    {
                        if (refFile.Contains(toIgnore))
                        {
                            skipCheck = true;
                            break;
                        }
                    }

                    if (skipCheck)
                    {
                        continue;
                    }

                    var compFile = refFile.Replace(refIdentifier, compIdentifier);
                    if (!File.Exists(compFile))
                    {
                        Assert.Fail($"Expected file not found: '{compFile}'");
                    }
                    else
                    {
                        var refText = File.ReadAllLines(refFile);
                        var compText = File.ReadAllLines(compFile);

                        List<string> RemoveKnownExceptions(string[] origin)
                        {
                            // Close and Maximize buttons are sometimes detected as text
                            return origin.Where(t => t != "X" && t != "O").ToList();
                        }

                        var filteredRefText = RemoveKnownExceptions(refText);
                        var filteredCompText = RemoveKnownExceptions(compText);

                        List<string> TrimNonAlphaNumerics(List<string> origin)
                        {
                            string TrimNANStart(string source)
                            {
                                var trimIndex = 0;
                                for (int i = 0; i < source.Length; i++)
                                {
                                    if (char.IsLetterOrDigit(source[i]))
                                    {
                                        trimIndex = i;
                                        break;
                                    }
                                }

                                return source.Substring(trimIndex);
                            }

                            string TrimNANEnd(string source)
                            {
                                var trimIndex = -1;

                                for (int i = source.Length - 1; i >= 0; i--)
                                {
                                    if (char.IsLetterOrDigit(source[i]))
                                    {
                                        trimIndex = i;
                                        break;
                                    }
                                }

                                return source.Substring(0, trimIndex + 1);
                            }

                            return origin.Select(o => TrimNANStart(TrimNANEnd(o))).Where(t => !string.IsNullOrWhiteSpace(t)).ToList();
                        }

                        // Allow for text detection being inconsistent with punctuation
                        var trimmedRefText = TrimNonAlphaNumerics(filteredRefText);
                        var trimmedCompText = TrimNonAlphaNumerics(filteredCompText);

                        List<string> AccountForKnownExceptions(List<string> origin)
                        {
                            string RemoveKnownSubstringExceptions(string source)
                            {
                                if (source.EndsWith("$()"))
                                {
                                    return source.Substring(0, source.Length - 3);
                                }

                                if (source.EndsWith("$0"))
                                {
                                    return source.Substring(0, source.Length - 2);
                                }

                                return source;
                            }

                            return origin.Select(t => RemoveKnownSubstringExceptions(t)).ToList();
                        }

                        var testableRefText = AccountForKnownExceptions(trimmedRefText);
                        var testableCompText = AccountForKnownExceptions(trimmedCompText);

                        ListOfStringAssert.AssertAreEqualIgnoringOrder(
                            testableRefText,
                            testableCompText,
                            caseSensitive: false,
                            message: $"Difference between '{refFile}' and '{compFile}'.");
                    }
                }
            }

            // Now we've got all images and extracted the text, actually do the comparison
            // It's easier to restart the tests and just do these checks if something fails.
            // It's also easier to have all versions available if needing to do manual checks.
            CompareWordsText("normal", "HiContrast1");
            CompareWordsText("normal", "HiContrast2");
            CompareWordsText("normal", "HiContrastBlack");
            CompareWordsText("normal", "HiContrastWhite");
        }

        [TestCleanup]
        public async Task CleanUpAsync()
        {
            var sysSettings = new SystemSettingsHelper();
            await sysSettings.TurnOffHighContrastAsync();
        }
    }
}
