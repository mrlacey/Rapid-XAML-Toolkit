using JetBrains.Annotations;
using JetBrains.ReSharper.Daemon.CodeFolding;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Xaml;

namespace ReSharperPlugin.RapidXaml;

[Language(typeof(XamlLanguage))]
public sealed class XamlNamespaceCodeFoldingProcessorFactory : ICodeFoldingProcessorFactory
{
    [NotNull]
    public ICodeFoldingProcessor CreateProcessor() => new XamlNamespaceCodeFoldingProcessor();
}
