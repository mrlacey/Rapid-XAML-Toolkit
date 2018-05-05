// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
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

        protected static Microsoft.VisualStudio.Text.Editor.IWpfTextView GetTextView(IAsyncServiceProvider serviceProvider)
        {
            var textManager = (IVsTextManager)serviceProvider.GetServiceAsync(typeof(SVsTextManager)).Result;

            if (textManager == null)
            {
                return null;
            }

            textManager.GetActiveView(1, null, out IVsTextView textView);

            return textView == null ? null : GetEditorAdaptersFactoryService(serviceProvider).GetWpfTextView(textView);
        }

        protected static IVsEditorAdaptersFactoryService GetEditorAdaptersFactoryService(IAsyncServiceProvider serviceProvider)
        {
            var componentModel = (IComponentModel)serviceProvider.GetServiceAsync(typeof(SComponentModel)).Result;
            return componentModel.GetService<IVsEditorAdaptersFactoryService>();
        }
    }
}
