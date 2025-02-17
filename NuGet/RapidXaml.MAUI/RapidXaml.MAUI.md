# RapidXaml.MAUI

Tools to make working with XAML easier in .NET MAUI projects.

## Includes

The following classes are included:

### XamlThickness

A bindable version of `Microsoft.Maui.Thickness`. Use it like a `Thickness` but with the ability to bind properties when creating in XAML.

### GestureRecognizer

A class with the following attached property to make it easier to bind commands to gesture events:

- `SingleTap`
- `Tap` (The same as SingleTap but with a shorter name)
- `DoubleTap`
- `SecondaryTap`
- `Pan` (Fires when the `PanUpdated`gesture is detected and passes `PanUpdatedEventArgs` to the command)
- `PanCompleted` (Fires when the pan gesture is completed and passes `PanUpdatedEventArgs` to the command)

Can also be used with the class name `Gesture` (because shorter names are often better.)

### PickerOptions

A class with the following attached properties to make it easier to set the items in a picker:

- `Csv` (Takes a comma delimited list of strings to set as the items.)
