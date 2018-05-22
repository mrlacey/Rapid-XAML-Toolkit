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
            var textManager = await serviceProvider.GetServiceAsync(typeof(SVsTextManager)) as IVsTextManager;

            if (textManager == null)
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
            return componentModel.GetService<IVsEditorAdaptersFactoryService>();
        }
    }
}
