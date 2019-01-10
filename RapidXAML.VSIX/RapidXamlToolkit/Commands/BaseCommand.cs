// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TextManager.Interop;
using RapidXamlToolkit.Logging;

namespace RapidXamlToolkit.Commands
{
    public class BaseCommand
    {
        public static readonly Guid CommandSet = new Guid("8c20aab1-50b0-4523-8d9d-24d512fa8154");

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
            var componentModel = await serviceProvider.GetServiceAsync(typeof(SComponentModel)) as IComponentModel;

            if (componentModel != null)
            {
                return componentModel.GetService<IVsEditorAdaptersFactoryService>();
            }
            else
            {
                RapidXamlPackage.Logger?.RecordError("Failed to get IComponentModel in BaseCommand.GetEditorAdaptersFactoryServiceAsync");

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

                var textManager = await serviceProvider.GetServiceAsync(typeof(SVsTextManager)) as IVsTextManager4;

                if (textManager != null)
                {
                    textManager.GetUserPreferences4(pViewPrefs: null, pLangPrefs: languagePreferences, pColorPrefs: null);

                    return (int)languagePreferences[0].uIndentSize;
                }
                else
                {
                    RapidXamlPackage.Logger?.RecordError("Failed to get IVsTextManager4 in BaseCommand.GetXamlIndentAsync");
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
