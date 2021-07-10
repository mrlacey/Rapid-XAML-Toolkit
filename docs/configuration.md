# Configuration of XAML Generation

The Rapid XAML Toolkit is highly configurable so that it can be used regardless of the platform your building for, the naming conventions you use, or the way you structure your solutions or code. While the number of configurable options may seem complex we've tried to keep it as simple as possible.

## Profiles

[Profiles](./profiles.md) are the way XAML generation is configured.

Only one profile can be used at a time. The profile being used is called the **active** profile. You can set different active profiles for UWP, WPF, Xamarin.Forms, WinUI3, and .NET MAUI projects. The toolkit works out the project type to use based on the currently loaded solution.

You can also specify a **fallback** profile which will be used if it's not possible to determine the project type required.

A number of profiles are provided by default. These can be used as they are or customized to meet your needs.

Learn [more about profiles](./profiles.md).

## General settings

General settings apply regardless of the active profile.

### Extended Logging

When extended logging is enabled, details of what the toolkit does internally is logged to a pane in the output window. If you experience an error or find the generated XAML is not as you expect, the information in the log may help explain what's going on.

When extended logging is not enabled, only exceptions are logged in the output window.
