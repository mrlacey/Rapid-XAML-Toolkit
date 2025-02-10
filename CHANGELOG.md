# Rapid XAML Toolkit - ChangeLog

## 2.0 (Feb 2025)

- Support for Visual Studio 2022.
- Combined in to a single VSIX package.
- Now works with MAUI, WinUI, & WPF.
- Removed UWP & Xamarin.Forms specific functionality.

## 0.14.1

- Fix issue with looking up resources as part of BuildAnalysis.

## 0.14.0

- Update NuGet packages to have all the latest fixes.

## 0.13.2

- Fix support for "ANYOF:" prefix in Custom analyzers.
- Fix selection of profiles in the Option dialog.

## 0.13.1

- Fix analysis issue with nested grids.
- Fix editor issue that could lock the VS main thread.
- Fix tags in the Custom Analysis project.

## 0.13.0

- Expanded ability to show FontAwesome icon previews within the editor.
- Minimum supported VS version to 16.9.
- Support for WinUI3 and .NET MAUI.
- Extend symbol visualization within the XAML editor.

## 0.12.2

- Fix issue affecting telemetry.

## 0.12.1

- Fix bug affecting the detection of assigned rows and columns within grids.

## 0.12

- Fix bug where the location of items to underline was not calculated correctly.
- Performance improvements in analysis.
- Add JsonSchema support for .xamlAnalysis files.
- Add automated fixes for hard-coded strings in WPF & Xamarin.Forms.
- Add analyzers for standard Xamarin.Forms controls.

## 0.11.5

- CustomAnalysis can identify where to CreateResource(s).

## 0.11.4

- Support succinct Row and Column Definitions in RapidXaml.Analysis.

## 0.11.3

- Avoid duplicate output from BuildAnalysis.
- Increased error handling in RapidXaml.Analysis.

## 0.11.2

- Fix (harmless) error message when running BuildAnalysis.

## 0.11.1

- Fixes an issue when calling `AnalysisActions.AndAddXmlns`.

## 0.11

- Added Editor Extras package.
- Performance improvements across all areas of the toolkit.
- Enabled custom analysis using `AnyOf:`, `AnyContaining:`, and `AnyOrChildrenContaining:` prefixes.
- Refactoring to support the AutoFix NuGet package.
- Added ability to see all xmlns in Custom Analyzers.
- Added ability to add a document level xmlns from analyzer / quick action.
- Added first Uno specific XAML analyzers.

## 0.10.5

- Performance improvements and bug fixes for XAML Analysis.

---

Note: _earlier release notes are summary only_

## 0.10.*

- Added Build time XAML Analysis option.

## 0.9.*

- Add early support for Custom XAML Analysis.
- Add Templates package. (Initially to help create custom analyzers.)
- Lots of bug fixes and more XAML analyzers.

## 0.8.*

- Under new management.
- Refactored project into multiple connected packages that can either be installed together or separately.

## 0.3.*

- First version available via the marketplace.
- Included latest developments in Generation and Analysis.
- Added Roslyn Analyzers for C#.

## 0.2.*

- Introduces support for separate active profiles for UWP, WPF, & Xamarin.Forms projects.
- **BREAKING CHANGE** Configuration format/version change for anyone updating from v0.1.* versions. [more details](https://github.com/microsoft/Rapid-XAML-Toolkit/issues/224)

## 0.1.*

- Initial beta versions.
