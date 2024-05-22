using JetBrains.Annotations;
using JetBrains.Application.Parts;
using JetBrains.ReSharper.Daemon.CodeFolding;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Xaml;

namespace ReSharperPlugin.RapidXaml;

[Language(typeof(XamlLanguage), Instantiation.DemandAnyThread)]
public sealed class XamlNamespaceCodeFoldingProcessorFactory : ICodeFoldingProcessorFactory
{
    [NotNull]
    public ICodeFoldingProcessor CreateProcessor() => new XamlNamespaceCodeFoldingProcessor();
}
