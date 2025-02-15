using JetBrains.Application.BuildScript.Application.Zones;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.Xaml;

namespace ReSharperPlugin.RapidXaml;

[ZoneDefinition]
// [ZoneDefinitionConfigurableFeature("Title", "Description", IsInProductSection: false)]
public interface IRapidXamlZone : IZone,
    IRequire<ILanguageXamlZone>,
    IRequire<DaemonZone>
{
}