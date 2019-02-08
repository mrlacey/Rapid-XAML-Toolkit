using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;

namespace RapidXamlToolkit.Suggestions
{
    public static class ProjectHelpers
    {
        static ProjectHelpers()
        {
            DTE = (DTE2)Package.GetGlobalService(typeof(DTE));
        }

        public static DTE2 DTE { get; }
    }
}
