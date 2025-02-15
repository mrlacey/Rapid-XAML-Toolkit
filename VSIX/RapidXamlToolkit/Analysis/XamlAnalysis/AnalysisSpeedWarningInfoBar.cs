// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System.Threading.Tasks;
using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using RapidXamlToolkit.Resources;
using Task = System.Threading.Tasks.Task;

namespace RapidXamlToolkit.XamlAnalysis
{
    public class AnalysisSpeedWarningInfoBar : IVsInfoBarUIEvents
    {
        private static readonly InfoBarModel InfoBarModel =
           new InfoBarModel(
               new[]
               {
                    new InfoBarTextSpan(StringRes.Info_PromptAnalysisRunningSlow),
                    new InfoBarTextSpan("   "),
                    new InfoBarHyperlink(StringRes.Info_PromptDisableAnalysisOnSave),
               },
               KnownMonikers.StatusWarningOutline,
               true);

        private bool isVisible = false;
        private IVsInfoBarUIElement uiElement;

        public void CloseInfoBar()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (this.isVisible && this.uiElement != null)
            {
                this.uiElement.Close();
            }
        }

        public async Task ShowInfoBarAsync()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            if (this.isVisible || !await this.TryCreateInfoBarUIAsync(InfoBarModel))
            {
                return;
            }

            this.uiElement.Advise(this, out _);

            var shell = await AsyncServiceProvider.GlobalProvider.GetServiceAsync<SVsShell, IVsShell>();
            shell.GetProperty((int)__VSSPROPID7.VSSPROPID_MainWindowInfoBarHost, out var obj);

            var host = obj as IVsInfoBarHost;
            host.AddInfoBar(this.uiElement);
        }

        public void OnClosed(IVsInfoBarUIElement infoBarUIElement)
        {
            this.isVisible = false;
        }

        public void OnActionItemClicked(IVsInfoBarUIElement infoBarUIElement, IVsInfoBarActionItem actionItem)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
#if VSIXNOTEXE
            RapidXamlPackage.AnalysisOptions.AnalyzeWhenDocumentSaved = false;
#endif
            infoBarUIElement.Close();
        }

        private async Task<bool> TryCreateInfoBarUIAsync(IVsInfoBar infoBar)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            IVsInfoBarUIFactory infoBarUIFactory = await AsyncServiceProvider.GlobalProvider.GetServiceAsync<SVsInfoBarUIFactory, IVsInfoBarUIFactory>();

            this.uiElement = infoBarUIFactory.CreateInfoBar(infoBar);
            return this.uiElement != null;
        }
    }
}
