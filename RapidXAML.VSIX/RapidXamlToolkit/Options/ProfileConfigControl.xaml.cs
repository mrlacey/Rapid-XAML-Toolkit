// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Windows;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;
using Microsoft.VisualStudio.Shell;

namespace RapidXamlToolkit.Options
{
    public partial class ProfileConfigControl
    {
        private Profile viewModel;
        private ICanClose host;
        private bool disabled = false;

        public ProfileConfigControl()
        {
            this.InitializeComponent();
        }

        public void SetDataContextAndHost(Profile profile, ICanClose host)
        {
            this.viewModel = profile;
            this.DataContext = this.viewModel;

            this.viewModel.PropertyChanged += this.OnViewModelPropertyChanged;

            this.SetEditorTexts();
            this.HandleHighContrastForTextEditors();

            this.host = host;
        }

        public void DisableButtonsForEmulator()
        {
            this.disabled = true;
        }

        private void OnViewModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Profile.SelectedMapping))
            {
                this.SelectedMappingOutputEntry.Text = this.viewModel.SelectedMapping.Output;
            }
        }

        private void HandleHighContrastForTextEditors()
        {
            if (SystemParameters.HighContrast)
            {
                // Remove highlighting and default system highlighting will be applied and this includes high contrast
                this.ViewGenXamlPlchldrEntry.SyntaxHighlighting = null;
                this.ViewGenCodeBehindPlchldrEntry.SyntaxHighlighting = null;

                this.FallbackOutputEntry.SyntaxHighlighting = null;
                this.SubPropertyOutputEntry.SyntaxHighlighting = null;
                this.EnumMemberOutputEntry.SyntaxHighlighting = null;

                this.CodeBehindPageContentEntry.SyntaxHighlighting = null;
                this.CodeBehindConstructorContentEntry.SyntaxHighlighting = null;
                this.DefaultCodeBehindConstructorEntry.SyntaxHighlighting = null;

                this.SelectedMappingOutputEntry.SyntaxHighlighting = null;
            }
        }

        private void OnSelectedMappingOutputChanged(object sender, EventArgs e)
        {
            if (this.viewModel?.SelectedMapping == null)
            {
                return;
            }

            // As the editor doesn't support binding change the VM value directly
            this.viewModel.SelectedMapping.Output = (sender as TextEditor).Text;
        }

        // Use this to ensure that code is highlighted/colored appropriate to the language (C#/VB) being used
        private void OnCodeChanged(object sender, EventArgs e)
        {
            bool CodeLooksLikeCSharp(string codeSnippet)
            {
                // A very quick, hacky test - improve as/when needed.
                return codeSnippet.StartsWith("using")
                    || codeSnippet.Contains(" var ")
                    || codeSnippet.Contains("}\r\n")
                    || codeSnippet.Contains(";\r\n")
                    || codeSnippet.TrimEnd().EndsWith(";");
            }

            var editor = sender as TextEditor;

            if (editor.Text.Length > 20)
            {
                var codeIsCsharp = CodeLooksLikeCSharp(editor.Text);
                var isUsingCSharpFormatting = editor.SyntaxHighlighting.ToString() == "C#";

                if (codeIsCsharp)
                {
                    if (!isUsingCSharpFormatting)
                    {
                        editor.SyntaxHighlighting = new HighlightingDefinitionTypeConverter().ConvertFrom("C#") as IHighlightingDefinition;
                    }
                }
                else
                {
                    if (isUsingCSharpFormatting)
                    {
                        editor.SyntaxHighlighting = new HighlightingDefinitionTypeConverter().ConvertFrom("VB") as IHighlightingDefinition;
                    }
                }
            }
        }

        // Set text for editor controls that don't support binding
        private void SetEditorTexts()
        {
            this.ViewGenXamlPlchldrEntry.Text = this.viewModel.ViewGeneration.XamlPlaceholder;
            this.ViewGenCodeBehindPlchldrEntry.Text = this.viewModel.ViewGeneration.CodePlaceholder;

            this.FallbackOutputEntry.Text = this.viewModel.FallbackOutput;
            this.SubPropertyOutputEntry.Text = this.viewModel.SubPropertyOutput;
            this.EnumMemberOutputEntry.Text = this.viewModel.EnumMemberOutput;

            this.CodeBehindPageContentEntry.Text = this.viewModel.Datacontext.CodeBehindPageContent;
            this.CodeBehindConstructorContentEntry.Text = this.viewModel.Datacontext.CodeBehindConstructorContent;
            this.DefaultCodeBehindConstructorEntry.Text = this.viewModel.Datacontext.DefaultCodeBehindConstructor;
        }

        private void GetEditorTexts()
        {
            this.viewModel.ViewGeneration.XamlPlaceholder = this.ViewGenXamlPlchldrEntry.Text;
            this.viewModel.ViewGeneration.CodePlaceholder = this.ViewGenCodeBehindPlchldrEntry.Text;

            this.viewModel.FallbackOutput = this.FallbackOutputEntry.Text;
            this.viewModel.SubPropertyOutput = this.SubPropertyOutputEntry.Text;
            this.viewModel.EnumMemberOutput = this.EnumMemberOutputEntry.Text;

            this.viewModel.Datacontext.CodeBehindPageContent = this.CodeBehindPageContentEntry.Text;
            this.viewModel.Datacontext.CodeBehindConstructorContent = this.CodeBehindConstructorContentEntry.Text;
            this.viewModel.Datacontext.DefaultCodeBehindConstructor = this.DefaultCodeBehindConstructorEntry.Text;
        }

        private void OkClicked(object sender, RoutedEventArgs e)
        {
            if (this.disabled)
            {
                return;
            }

            this.GetEditorTexts();

            this.host?.Close();
        }

        private void AddClicked(object sender, RoutedEventArgs e)
        {
            if (this.disabled)
            {
                return;
            }

            try
            {
                ThreadHelper.ThrowIfNotOnUIThread();

                this.viewModel.Mappings.Add(Mapping.CreateNew());
                this.viewModel.RefreshMappings();
            }
            catch (Exception exc)
            {
                RapidXamlPackage.Logger?.RecordException(exc);
                throw;  // Remove for launch. see issue #90
            }
        }

        private void CopyClicked(object sender, RoutedEventArgs e)
        {
            if (this.disabled)
            {
                return;
            }

            try
            {
                ThreadHelper.ThrowIfNotOnUIThread();

                if (this.DisplayedMappings.SelectedIndex >= 0)
                {
                    var copy = (Mapping)this.viewModel.Mappings[this.DisplayedMappings.SelectedIndex].Clone();

                    this.viewModel.Mappings.Add(copy);
                    this.viewModel.RefreshMappings();
                }
            }
            catch (Exception exc)
            {
                RapidXamlPackage.Logger?.RecordException(exc);
                throw;  // Remove for launch. see issue #90
            }
        }

        private void DeleteClicked(object sender, RoutedEventArgs e)
        {
            if (this.disabled)
            {
                return;
            }

            try
            {
                ThreadHelper.ThrowIfNotOnUIThread();

                if (this.DisplayedMappings.SelectedIndex >= 0)
                {
                    this.viewModel.Mappings.RemoveAt(this.DisplayedMappings.SelectedIndex);
                    this.viewModel.RefreshMappings();
                }
            }
            catch (Exception exc)
            {
                RapidXamlPackage.Logger?.RecordException(exc);
                throw;  // Remove for launch. see issue #90
            }
        }

        private void DetailsClicked(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/Microsoft/Rapid-XAML-Toolkit/issues/16");
        }
    }
}
