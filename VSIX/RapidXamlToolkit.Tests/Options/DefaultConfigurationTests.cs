// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidXamlToolkit.Options;

namespace RapidXamlToolkit.Tests.Options
{
    [TestClass]
    public class DefaultConfigurationTests
    {
        [TestMethod]
        public void EnsureNoNullsInDefaultConfig()
        {
            var defSet = ConfiguredSettings.GetDefaultSettings();

            Assert.IsNotNull(defSet.ExtendedOutputEnabled, nameof(Settings.ExtendedOutputEnabled));

            Assert.IsFalse(string.IsNullOrEmpty(defSet.FallBackProfileName), nameof(Settings.FallBackProfileName));

            Assert.AreEqual(3, defSet.ActiveProfileNames.Count, "Incorrect number of active profiles configured by default.");

            Assert.IsFalse(string.IsNullOrEmpty(defSet.ActiveProfileNames[ProjectType.Uwp.GetDescription()]), "Active UWP profile is not set.");
            Assert.IsFalse(string.IsNullOrEmpty(defSet.ActiveProfileNames[ProjectType.Wpf.GetDescription()]), "Active WPF profile is not set.");
            Assert.IsFalse(string.IsNullOrEmpty(defSet.ActiveProfileNames[ProjectType.XamarinForms.GetDescription()]), "Active Xamarin.Forms profile is not set.");

            Assert.IsNotNull(defSet.Profiles, nameof(Settings.Profiles));

            foreach (var profile in defSet.Profiles)
            {
                Assert.IsNotNull(profile.Name, "There's a profile without a name");
                Assert.IsNotNull(profile.ClassGrouping, $"{nameof(Profile.ClassGrouping)} in profile {profile.Name}");
                Assert.IsNotNull(profile.FallbackOutput, $"{nameof(Profile.FallbackOutput)} in profile {profile.Name}");
                Assert.IsNotNull(profile.SubPropertyOutput, $"{nameof(Profile.SubPropertyOutput)} in profile {profile.Name}");
                Assert.IsNotNull(profile.EnumMemberOutput, $"{nameof(Profile.EnumMemberOutput)} in profile {profile.Name}");

                Assert.IsNotNull(profile.Mappings, $"{nameof(Profile.Mappings)} in profile {profile.Name}");

                foreach (var mapping in profile.Mappings)
                {
                    Assert.IsNotNull(mapping.IfReadOnly, nameof(Mapping.IfReadOnly));
                    Assert.IsNotNull(mapping.NameContains, nameof(Mapping.NameContains));
                    Assert.IsNotNull(mapping.Output, nameof(Mapping.Output));
                    Assert.IsNotNull(mapping.Type, nameof(Mapping.Type));
                }

                Assert.IsNotNull(profile.AttemptAutomaticDocumentFormatting, $"{nameof(Profile.AttemptAutomaticDocumentFormatting)} in profile {profile.Name}");
            }
        }

        [TestMethod]
        public void EnsureValidXamlInDefaultConfig()
        {
            var defSet = ConfiguredSettings.GetDefaultSettings();

            foreach (var profile in defSet.Profiles)
            {
                Assert.IsTrue(profile.FallbackOutput.IsValidXamlOutput(), $"{nameof(Profile.FallbackOutput)} in profile '{profile.Name}' is not valid XAML.");
                Assert.IsTrue(profile.SubPropertyOutput.IsValidXamlOutput(), $"{nameof(Profile.SubPropertyOutput)} in profile '{profile.Name}' is not valid XAML.");
                Assert.IsTrue(profile.EnumMemberOutput.IsValidXamlOutput(), $"{nameof(Profile.EnumMemberOutput)} in profile '{profile.Name}' is not valid XAML.");

                foreach (var mapping in profile.Mappings)
                {
                    if (mapping.Output != Placeholder.NoOutput)
                    {
                        Assert.IsTrue(mapping.Output.IsValidXamlOutput(), $"Invalid output: {mapping.Output}");
                    }
                }
            }
        }

        [TestMethod]
        public void EnsureOnlyValidPlaceholdersInDefaultConfig()
        {
            var defSet = ConfiguredSettings.GetDefaultSettings();
            var apv = new AllowedPlaceholderValidator();

            foreach (var profile in defSet.Profiles)
            {
                var checkResult = apv.ContainsOnlyValidPlaceholders(typeof(Profile), nameof(Profile.FallbackOutput), profile.FallbackOutput);

                if (!checkResult.isValid)
                {
                    Assert.Fail($"{nameof(Profile.FallbackOutput)} in profile '{profile.Name}' contained invalid placeholder(s): {string.Join(", ", checkResult.invalidPlaceholders)}");
                }

                checkResult = apv.ContainsOnlyValidPlaceholders(typeof(Profile), nameof(Profile.SubPropertyOutput), profile.SubPropertyOutput);

                if (!checkResult.isValid)
                {
                    Assert.Fail($"{nameof(Profile.SubPropertyOutput)} in profile '{profile.Name}' contained invalid placeholder(s): {string.Join(", ", checkResult.invalidPlaceholders)}");
                }

                checkResult = apv.ContainsOnlyValidPlaceholders(typeof(Profile), nameof(Profile.EnumMemberOutput), profile.EnumMemberOutput);

                if (!checkResult.isValid)
                {
                    Assert.Fail($"{nameof(Profile.EnumMemberOutput)} in profile '{profile.Name}' contained invalid placeholder(s): {string.Join(", ", checkResult.invalidPlaceholders)}");
                }

                foreach (var mapping in profile.Mappings)
                {
                    checkResult = apv.ContainsOnlyValidPlaceholders(typeof(Mapping), nameof(Mapping.Output), mapping.Output);

                    if (!checkResult.isValid)
                    {
                        Assert.Fail($"Mapping output in profile '{profile.Name}' contained invalid placeholder(s): {string.Join(", ", checkResult.invalidPlaceholders)}");
                    }
                }
            }
        }

        [TestMethod]
        public async Task EnsureDefaultProfilesCanBeExportedAndImported()
        {
            var defSet = ConfiguredSettings.GetDefaultSettings();

            foreach (var profile in defSet.Profiles)
            {
                var json = profile.AsJson();

                var parser = new ApiAnalysis.SimpleJsonAnalyzer();

                var parserResults = await parser.AnalyzeJsonAsync(json, typeof(Profile));

                Assert.AreEqual(1, parserResults.Count);
                Assert.AreEqual(parser.MessageBuilder.AllGoodMessage, parserResults.First());
            }
        }
    }
}
