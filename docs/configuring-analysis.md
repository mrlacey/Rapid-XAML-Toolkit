# Configuring XAML Analysis

The issues reported via XAML Analysis have a default ErrorType ('Error', 'Warning', 'Suggestion', or 'Hidden'). The ErrorType for each error can be configured on a project level basis or suppressed so it is not displayed.

## Changing ErrorType

The ErrorType of a tag determines how it is displayed in Visual Studio.

- **Error** is underlined in code and listed in the 'Errors' tab in the Error List.
- **Warning** is underlined in code and listed in the 'Warnings' tab in the Error List.
- **Suggestion** is underlined in code and listed in the 'Messages' tab in the Error List.
- **Hidden** has an underline hint in code and is not included in the Error List.

To change the ErrorType of a warning, create a file called `settings.xamlanalysis` in the root of the project.

The file must contain a JSON formatted `Dictionary<string, string>`, like this:

```js
{
    "RTX150": "None",
    "RTX160": "Error"
}
```

The key is the TagErrorCode of the warning to change.
The value is a string representing the ErrorType to treat the warning as.

Valid values are (case insensitive)

- None/Hidden
- Suggestion
- Warning
- Error

## Suppressing errors

It is possible to suppress all occurrences of a warning, or a specific instance.

Configure suppressions in a file called `suppressions.xamlanalysis` in the root of the project you wish this to apply to.

The file must contain a JSON formatted list of suppression objects.

Each object is made up of:

- **FileName** [required] Matching is case sensitive and done from the end of a full file path. No wildcards are supported but you can specify the FileName as '.xaml' for it to be applied to all XAML files.
- **TagErrorCode** [optional] The warning to suppress. If not specified the suppression will apply to all tags in the file.
- **ElementIdentifier** [optional] Used in a `Contains()` match on the full text of the element, and only those that match are suppressed. If not specified (or left blank) then all tags with the errorcode in the specified file will be suppressed.
- **Reason** [optional] Nothing is done with this, it's simply a place to justify why the suppression has been added and allows developers to tell their future selves, or others working on the project, why they did this.

A valid file looks like this:

```js
[
  {
    "FileName": "MainPage.xaml",
    "TagErrorCode": "RXT200",
    "ElementIdentifier": "Text=\"menu1\"",
    "Reason": "Just for testing"
  }
]
```

Will prevent the following being reported as a hard-coded string.

`Views\MainPage.xaml`

```xml
   <MenuFlyoutItem Text="menu1" />
```

A suppression file can contain multiple entries for the same, or different, files and will prevent the warning being displayed if any of them match.
