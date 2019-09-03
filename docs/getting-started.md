# Getting Started

Whether you just want to use the extension or help make it even better--this is the place to start.

## Using the extension

Before you can do anything, you need to install the extesion.
While it's being installed you might learn how to configure it to meet your needs. Or just try it out and then adjust the configuration to your prefered way of working.

### Installing the extension

There are two versions of the extension. We recommend using the release version unless you want to test something that is only in the beta version. While it is possible to have both versions installed at once, some functionality may not work correctly if both are enabled (Extensions > Manage Extensions) at the same time.

#### Release version

Coming soon.

### Beta version

Please see the [installation guide](https://github.com/mrlacey/Rapid-XAML-Toolkit/blob/dev/docs/installation.md#nightly-dev-builds).

### Configuration

[Control the XAML Generation process](./configuration.md)
[Configure how issues are reported during XAML Analysis](./configuring-analysis.md)

## Working with source

Here's what you need to know if you want to [contribute](../CONTRIBUTING.md) or understand the code-base.

### Inside Visual Studio

Open `RapidXamlToolkit.sln` in the 'VSIX' folder.
The solution contains five projects:

- `RapidXamlToolkit`is the code for the extension. Run/debug this project and an experimental instance of VS will be launched that has the extension already installed.
- `RapidXamlToolkit.Tests` is a test project containing the automated tests.
- `RapidXamlToolkit.Tests.Manual` is a test project containing tests that require additional configuration or manual verification. You will need to make changes to the code to run these tests. Look for comments in the code for details of what to change/specify.
- `Tools/LocalizationHelper` is a console app that contains helper functionality related to localizing content in the extension.
- `Tools/OptionsEmulator` is a WPF app that allows viewing the UI that is displayed in the Options dialogs without having to start an instance of VisualStudio.

There is another solution (`RapidXamlToolkit.PRBuild.sln`) which only contains the main extension project and the automated tests project. This solution is used by the PR and CI pipelines. Depending on what you do with the source, you _may_ be able to get by just with using this solution.

Please note that **only Visual Studio 2019** is supported for opening the solution and running the extension. This is due to the tight levels of integration needed with Visual Studio by the code in this extension.

### Extending XAML Analysis

The area where contributions are most expected (and appreciated) is in creating more functionality for XAML Analysis. To help with this, there is a [guide to extending XAML Analysis](./extending-xaml-analysis.md) which contributors are encouraged to read.

### Localization

Because of the many components of the codebase and the way they interact, the localization of content is not as straight-forward as in other projects. Specifics are detailed in the [localizaiton document](./localization.md).
