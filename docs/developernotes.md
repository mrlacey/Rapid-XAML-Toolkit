# Rapid XAML Toolkit - Developer Notes

The following is important information to know for anyone working with this codebase.

## XAML Analysis

To create a XAML analyzer requires:

- A processor for a UIElement that inherits from `XamlElementProcessor` and is registered in `RapidXamlDocument.Create`. The processor must create Tags for anything of interest and add them to the `tags` collection in the `Process` method.
- A tag that is used to identify the span in the editor that covers the appropriate text. Tags must reference the type of the `SuggestedAction` to execute to address the discovered issue.
- An action that is executed to address the highlighted issue. The action and tag should be registered in `SuggestedActionSource.GetSuggestedActions`.

All tags must inherit from one of either `RapidXamlOptionalTag`, `RapidXamlSuggestionTag`, or `RapidXamlWarningTag`.

- **Optional** : These tags have an underline hint in the UI and no entry in the error list.
- **Suggestion** : These tags have an underline hint in the UI and create an entry in the Messages tab of the error list.
- **Warning** : These tags have a complete underline in the UI and create an entry in the Warnings tab of the error list.

If the tag introduces a new `ErrorCode`, an accompanying document must also be created.
