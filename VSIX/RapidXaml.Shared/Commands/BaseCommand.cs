// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TextManager.Interop;
using RapidXamlToolkit.Logging;
using RapidXamlToolkit.Resources;

namespace RapidXamlToolkit.Commands
{
    public class BaseCommand
    {
        private readonly AsyncPackage package;

        public BaseCommand(AsyncPackage package, ILogger logger)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            this.Logger = logger;
        }

        protected ILogger Logger { get; }

        protected AsyncPackage AsyncPackage => this.package;

        protected IAsyncServiceProvider ServiceProvider => this.package;

        protected static async Task<Microsoft.VisualStudio.Text.Editor.IWpfTextView> GetTextViewAsync(IAsyncServiceProvider serviceProvider)
        {
            if (!(await serviceProvider.GetServiceAsync(typeof(SVsTextManager)) is IVsTextManager textManager))
            {
                return null;
            }

            textManager.GetActiveView(1, null, out IVsTextView textView);

            if (textView == null)
            {
                return null;
            }
            else
            {
                var provider = await GetEditorAdaptersFactoryServiceAsync(serviceProvider);
                return provider.GetWpfTextView(textView);
            }
        }

        protected static async Task<IVsEditorAdaptersFactoryService> GetEditorAdaptersFactoryServiceAsync(IAsyncServiceProvider serviceProvider)
        {
            if (await serviceProvider.GetServiceAsync(typeof(SComponentModel)) is IComponentModel componentModel)
            {
                return componentModel.GetService<IVsEditorAdaptersFactoryService>();
            }
            else
            {
                SharedRapidXamlPackage.Logger?.RecordGeneralError(StringRes.Error_FailedToGetIComponentModel);

                return null;
            }
        }

        protected async Task<int> GetXamlIndentAsync(IAsyncServiceProvider serviceProvider)
        {
            try
            {
                var xamlLanguageGuid = new Guid("CD53C9A1-6BC2-412B-BE36-CC715ED8DD41");
                var languagePreferences = new LANGPREFERENCES3[1];

                languagePreferences[0].guidLang = xamlLanguageGuid;

                if (await serviceProvider.GetServiceAsync(typeof(SVsTextManager)) is IVsTextManager4 textManager)
                {
                    textManager.GetUserPreferences4(pViewPrefs: null, pLangPrefs: languagePreferences, pColorPrefs: null);

                    return (int)languagePreferences[0].uIndentSize;
                }
                else
                {
                    SharedRapidXamlPackage.Logger?.RecordGeneralError(StringRes.Error_FailedToGetIVsTextManager4);
                }
            }
            catch (Exception exc)
            {
                this.Logger.RecordException(exc);
            }

            var indent = new Microsoft.VisualStudio.Text.Editor.IndentSize();

            return indent.Default;
        }

        protected void SuppressAnyException(Action action)
        {
            try
            {
                action.Invoke();
            }
            catch (Exception)
            {
            }
        }
    }
}
