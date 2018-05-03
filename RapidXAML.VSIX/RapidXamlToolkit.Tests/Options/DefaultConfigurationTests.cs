// <copyright file="DefaultConfigurationTests.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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

            // This is the only things that may be null (although empty is preferred)
            Assert.IsTrue(string.IsNullOrEmpty(defSet.ActiveProfileName), nameof(Settings.ActiveProfileName));

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

                Assert.IsNotNull(profile.ViewGeneration, $"{nameof(Profile.ViewGeneration)} in profile {profile.Name}");
                Assert.IsNotNull(profile.ViewGeneration.AllInSameProject, nameof(ViewGenerationSettings.AllInSameProject));
                Assert.IsNotNull(profile.ViewGeneration.CodePlaceholder, nameof(ViewGenerationSettings.CodePlaceholder));
                Assert.IsNotNull(profile.ViewGeneration.ViewModelDirectoryName, nameof(ViewGenerationSettings.ViewModelDirectoryName));
                Assert.IsNotNull(profile.ViewGeneration.ViewModelFileSuffix, nameof(ViewGenerationSettings.ViewModelFileSuffix));
                Assert.IsNotNull(profile.ViewGeneration.ViewModelProjectSuffix, nameof(ViewGenerationSettings.ViewModelProjectSuffix));
                Assert.IsNotNull(profile.ViewGeneration.XamlFileDirectoryName, nameof(ViewGenerationSettings.XamlFileDirectoryName));
                Assert.IsNotNull(profile.ViewGeneration.XamlFileSuffix, nameof(ViewGenerationSettings.XamlFileSuffix));
                Assert.IsNotNull(profile.ViewGeneration.XamlPlaceholder, nameof(ViewGenerationSettings.XamlPlaceholder));
                Assert.IsNotNull(profile.ViewGeneration.XamlProjectSuffix, nameof(ViewGenerationSettings.XamlProjectSuffix));

                Assert.IsNotNull(profile.Datacontext, $"{nameof(Profile.Datacontext)} in profile {profile.Name}");
                Assert.IsNotNull(profile.Datacontext.CodeBehindConstructorContent, nameof(DatacontextSettings.CodeBehindConstructorContent));
                Assert.IsNotNull(profile.Datacontext.CodeBehindPageContent, nameof(DatacontextSettings.CodeBehindPageContent));
                Assert.IsNotNull(profile.Datacontext.DefaultCodeBehindConstructor, nameof(DatacontextSettings.DefaultCodeBehindConstructor));
                Assert.IsNotNull(profile.Datacontext.XamlPageAttribute, nameof(DatacontextSettings.XamlPageAttribute));
            }
        }
    }
}
