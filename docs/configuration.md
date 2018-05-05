# Configuration

The Rapid XAML Toolkit is highly configurable so that it can be used regardless of the platform your building for, the naming conventions you use, or the way you structure your solutions or code. While the number of configurable options may seem complex we've tried to keep things as simple as possible.

## Profiles

The basis of configuration are [profiles](./profiles.md). A profile is a complete set of configurable values for how the toolkit works.

Only one profile can be used at a time. The profile being used is called the ""active"" profile. If no profile is active you will be prompted to make one active before you can use any of the functionality of the toolkit. If you work on different platforms or projects that are very different, it is expected you will switch between profiles as necessary.

A number of profiles are provided by default. These can be used as they are or customized to meet your needs.

Learn more about profiles [here](./profiles.md).

## General settings

General settings apply regardless of the active profile.

### Extended Logging

When extended logging is enabled, details of what the toolkit does internally is logged to a pane in the output window. If you experience an error or find the generated XAML is not as you expect, the information in the log may help explain what's going on.

When extended logging is not enabled, only exceptions will be logged in the output window.
