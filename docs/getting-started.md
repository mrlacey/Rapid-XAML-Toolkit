# Getting Started

Whether you want to use the extension or help make it even better--this is the place to start.

## Using the extension

Before you can do anything, you need to install the extension.
While it's being installed you might learn how to configure it to meet your needs. Or try the defaults and then adjust the configuration to your preferred way of working.

### Installing the extension

The latest version is available from the [Visual Studio Marketplace](https://marketplace.visualstudio.com/items?itemName=MattLaceyLtd.RapidXamlToolkit) or by searching inside Visual Studio (Extensions > Manage Extensions).

![Manage Extensions search dialog showing the Rapid XAML Toolkit listing](./Assets/extension-search-dialog.png)

Please see the [installation guide](https://github.com/mrlacey/Rapid-XAML-Toolkit/blob/dev/docs/installation.md) for more details.

### Configuration

[Control the XAML Generation process](./configuration.md)
[Configure how issues are reported during XAML Analysis](./configuring-analysis.md)

## Working with source

Here's what you need to know if you want to [contribute](../CONTRIBUTING.md) or understand the code-base.

### Inside Visual Studio

Open `RapidXamlToolkit.sln` in the 'VSIX' folder.
The solution contains these projects:

- `RapidXaml.Analysis` is a shipping VSIX package that contains the XAML Analysis functionality.
- `RapidXaml.Common` is a shipping VSIX package that contains Visual Studio context menus that are shared by other extensions.
- `RapidXaml.CustomAnalysis` is the NuGet package that contains the shared logic for custom XAML Analysis.
- `RapidXaml.Generation` is a shipping VSIX package that contains the functionality for generating XAML from C# and VB.NET code.
- `RapidXaml.RoslynAnalyzers` is a shipping VSIX package that contains Roslyn Analyzers and code fixes.
- `RapidXaml.Shared` is a library containing code shared between other projects.
- `RapidXamlToolkit` contains the extension pack for bundling all separate tools through a single extension.
- `RapidXamlToolkit.Tests` is a test project containing the automated tests.
- `RapidXamlToolkit.Tests.Manual` is a test project containing tests that require additional configuration or manual verification. You will need to make changes to the code to run these tests. Look for comments in the code for details of what to change/specify.
- `Tools/OptionsEmulator` is a WPF app that allows viewing the UI that is displayed in the Options dialogs without having to start an instance of Visual Studio.
- `Tools/RapidXaml.InternalAnalyzers` contains Roslyn code anlayzers for enforcing code patterns and requirements within the solution.

There is another solution (`RapidXamlToolkit.PRBuild.sln`) in the 'VSIX' folder which only contains the main extension projects and the automated tests project. This solution is used by the PR and CI pipelines. Depending on what you do with the source, you _may_ be able to get by with using this solution.

The `Templates` folder includes `RapidXaml.Templates.sln` which contains the project and item templates packaged in a VSIX package.

All warnings are treated as errors for the release build.  
'TODO' comments that do not reference an issue number will create warnings.

### Extending XAML Analysis

The area where contributions are most expected (and appreciated) is in creating more functionality for XAML Analysis. To help with this, there is a [guide to extending XAML Analysis](./extending-xaml-analysis.md) which contributors are encouraged to read.

### Localization

Because of the many components of the codebase and the way they interact, the localization of content is not as straight-forward as in other projects. Specifics are detailed in the [localization document](./localization.md).
