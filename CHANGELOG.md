# Rapid XAML Toolkit - ChangeLog

## Upcoming

- Fix bug where the location of items to underline was not calculated correctly.
- Performance improvements in analysis.

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

_earlier release notes are summary only_

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
- Included latest developements in Generation and Analysis.
- Added Rsolyn ANalyzers for C#.

## 0.2.*

- Introduces support for separate active profiles for UWP, WPF, & Xamarin.Forms projects.
- **BREAKING CHANGE** Configuration format/version change for anyone updating from v0.1.* versions. [more details](https://github.com/microsoft/Rapid-XAML-Toolkit/issues/224)

## 0.1.*

- Initial beta versions.
