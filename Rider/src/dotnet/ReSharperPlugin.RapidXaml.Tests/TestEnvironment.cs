using System.Threading;
using JetBrains.Application.BuildScript.Application.Zones;
using JetBrains.ReSharper.Feature.Services;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using JetBrains.TestFramework;
using JetBrains.TestFramework.Application.Zones;
using NUnit.Framework;

[assembly: Apartment(ApartmentState.STA)]

namespace ReSharperPlugin.RapidXaml.Tests
{
    [ZoneDefinition]
    public class RapidXamlTestEnvironmentZone : ITestsEnvZone, IRequire<PsiFeatureTestZone>, IRequire<IRapidXamlZone> { }

    [ZoneMarker]
    public class ZoneMarker : IRequire<ICodeEditingZone>, IRequire<ILanguageCSharpZone>, IRequire<RapidXamlTestEnvironmentZone> { }

    [SetUpFixture]
    public class RapidXamlTestsAssembly : ExtensionTestEnvironmentAssembly<RapidXamlTestEnvironmentZone> { }
}
