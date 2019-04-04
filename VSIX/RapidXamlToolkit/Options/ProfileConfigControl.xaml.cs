// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;
using Microsoft.VisualStudio.PlatformUI;
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
                this.FallbackOutputEntry.SyntaxHighlighting = null;
                this.SubPropertyOutputEntry.SyntaxHighlighting = null;
                this.EnumMemberOutputEntry.SyntaxHighlighting = null;

                this.SelectedMappingOutputEntry.SyntaxHighlighting = null;

                this.FallbackOutputBorder.BorderBrush = this.ReferenceTextBox.BorderBrush;
                this.FallbackOutputBorder.BorderThickness = this.ReferenceTextBox.BorderThickness;
                this.SubPropertyOutputBorder.BorderBrush = this.ReferenceTextBox.BorderBrush;
                this.SubPropertyOutputBorder.BorderThickness = this.ReferenceTextBox.BorderThickness;
                this.EnumMemberOutputBorder.BorderBrush = this.ReferenceTextBox.BorderBrush;
                this.EnumMemberOutputBorder.BorderThickness = this.ReferenceTextBox.BorderThickness;

                this.SelectedMappingOutputBorder.BorderBrush = this.ReferenceTextBox.BorderBrush;
                this.SelectedMappingOutputBorder.BorderThickness = this.ReferenceTextBox.BorderThickness;
            }
        }

        private void OnSelectedMappingOutputChanged(object sender, EventArgs e)
        {
            this.OnEditorTextChanged(sender, e);

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
            this.OnEditorTextChanged(sender, e);

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
            this.FallbackOutputEntry.Text = this.viewModel.FallbackOutput;
            this.SubPropertyOutputEntry.Text = this.viewModel.SubPropertyOutput;
            this.EnumMemberOutputEntry.Text = this.viewModel.EnumMemberOutput;
        }

        private void GetEditorTexts()
        {
            this.viewModel.FallbackOutput = this.FallbackOutputEntry.Text;
            this.viewModel.SubPropertyOutput = this.SubPropertyOutputEntry.Text;
            this.viewModel.EnumMemberOutput = this.EnumMemberOutputEntry.Text;
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

        private void TextEditorPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab)
            {
                var shiftPressed = (e.KeyboardDevice.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift;

                if (shiftPressed)
                {
                    (sender as TextEditor).MoveFocus(new TraversalRequest(FocusNavigationDirection.Previous));
                }
                else
                {
                    (sender as TextEditor).TextArea.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                }

                e.Handled = true;
            }
        }

        private void GridPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab)
            {
                var shiftPressed = (e.KeyboardDevice.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift;

                if (shiftPressed)
                {
                    this.EnumMemberOutputEntry.Focus();
                }
                else
                {
                    this.AddMappingButton.Focus();
                }

                e.Handled = true;
            }
        }

        private void OnEditorTextChanged(object sender, EventArgs e)
        {
            string validationErrorMessage = null;

            if (sender is TextEditor te)
            {
                switch (te.Name)
                {
                    case "FallbackOutputEntry":
                        validationErrorMessage = this.viewModel.GetFallbackOutputErrorMessage(te.Text);
                        break;
                    case "SubPropertyOutputEntry":
                        validationErrorMessage = this.viewModel.GetSubPropertyOutputErrorMessage(te.Text);
                        break;
                    case "EnumMemberOutputEntry":
                        validationErrorMessage = this.viewModel.GetEnumMemberOutputErrorMessage(te.Text);
                        break;
                    case "SelectedMappingOutputEntry":
                        validationErrorMessage = this.viewModel.SelectedMapping.GetOutputErrorMessage(te.Text);
                        break;
                }

                WarningTriangle icon = null;
                try
                {
                    icon = (WarningTriangle)this.FindName($"{te.Name}Warning");
                }
                catch (Exception)
                {
                }

                if (string.IsNullOrWhiteSpace(validationErrorMessage))
                {
                    te.Background = null;
                    te.ToolTip = null;

                    if (icon != null)
                    {
                        icon.Visibility = Visibility.Collapsed;
                    }
                }
                else
                {
                    te.ToolTip = validationErrorMessage;

                    if (icon != null)
                    {
                        icon.Visibility = Visibility.Visible;
                    }

                    try
                    {
                        var drawingColor = VSColorTheme.GetThemedColor(EnvironmentColors.ControlEditRequiredBackgroundBrushKey);
                        te.Background = new SolidColorBrush(Color.FromArgb(drawingColor.A, drawingColor.R, drawingColor.G, drawingColor.B));
                    }
                    catch (Exception)
                    {
                        // VSColorTheme.GetThemedColor will fail in OptionsEmulator
                    }
                }
            }
        }
    }
}
