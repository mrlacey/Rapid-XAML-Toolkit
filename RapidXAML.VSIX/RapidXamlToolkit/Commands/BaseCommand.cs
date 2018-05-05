// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TextManager.Interop;

namespace RapidXamlToolkit
{
    public class BaseCommand
    {
        public static readonly Guid CommandSet = new Guid("8c20aab1-50b0-4523-8d9d-24d512fa8154");

        private readonly ILogger logger;

        private readonly AsyncPackage package;

        public BaseCommand(AsyncPackage package, ILogger logger)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            this.logger = logger;
        }

        protected ILogger Logger => this.logger;

        protected AsyncPackage AsyncPackage => this.package;

        protected Microsoft.VisualStudio.Shell.IAsyncServiceProvider ServiceProvider
        {
            get
            {
                return this.package;
            }
        }

        protected static Microsoft.VisualStudio.Text.Editor.IWpfTextView GetTextView(Microsoft.VisualStudio.Shell.IAsyncServiceProvider serviceProvider)
        {
            var textManager = (IVsTextManager)serviceProvider.GetServiceAsync(typeof(SVsTextManager)).Result;

            if (textManager == null)
            {
                return null;
            }

            textManager.GetActiveView(1, null, out IVsTextView textView);

            if (textView == null)
            {
                return null;
            }

            return GetEditorAdaptersFactoryService(serviceProvider).GetWpfTextView(textView);
        }

        protected static IVsEditorAdaptersFactoryService GetEditorAdaptersFactoryService(Microsoft.VisualStudio.Shell.IAsyncServiceProvider serviceProvider)
        {
            IComponentModel componentModel = (IComponentModel)serviceProvider.GetServiceAsync(typeof(SComponentModel)).Result;
            return componentModel.GetService<IVsEditorAdaptersFactoryService>();
        }
    }
}
