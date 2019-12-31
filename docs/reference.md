# Reference

Some useful other reference documents.

## Telemetry

We use telemetry in the extension to answer some specific questions.

- Is the extension being used?
- Which features are being used?
- Is anything going wrong?

What we don't capture:

- Anything identifiable.
- The details or names of your projects, classes, or code.
- Use by individuals over time or the number of times a person uses the tools.

While we don't capture information about how you use the toolkit we'd love to hear your feedback, stories of how you use it, and any suggestions for how we can improve in the future. Please [raise an issue](https://github.com/mrlacey/Rapid-XAML-Toolkit/issues/new) with details of any bugs or feature requests.

## Testing

How testing is handled in the project.

### Automated Testing

The VSIX solution includes a test project that includes a number of automated tests. These tests are run on the server after a successful build. They should also be run (and ensure they pass) before committing any code.

### Manual Testing

Due to the level of integration with Visual Studio, the automated testing of some functionality is not practical. The following tests must be performed manually.

#### XAML Generation

##### Drag & Drop VM

- Drag a ViewModel file onto a XAML document and check that UI elements are created.

##### Copy to Clipboard

- Check copying a **class** actually adds to the clipboard.
- Check copying a **property** actually adds to the clipboard.
- Check copying a **selection** actually adds to the clipboard.
- Check that failing to copy anything does not add anything to the clipboard.

##### Send to Toolbox

- Check sending a **class** actually adds to the Toolbox.
- Check sending a **property** actually adds to the Toolbox.
- Check sending a **selection** actually adds to the Toolbox.
- Check that failing to select anything valid does not add anything to the Toolbox.

#### XAML Analysis

- Open a .xaml file and check that items are underlined (as appropriate.)
- Check that equivalent entries exist in the Error List table for the underlined items.
- Check a code fix can be invoked from the Ctrl+. menu.
- Check that the appropriate entries are added to the context menu on the XAML editor.
- Check that it's possible to 'move all hard-coded strings to Resource file' at once.

#### Roslyn Analyzers

- Add a property with basic syntax to a property that inherits from a class that has a public `SetProperty` method. Ensure correct code fix option is displayed.
- Add a property with basic syntax to a property that inherits from a class that has a public `Set` method. Ensure correct code fix option is displayed.
- Add a property with basic syntax to a property that inherits from a class that has a public `OnPropertyChanged` method. Ensure correct code fix option is displayed.
- Add a property with basic syntax to a property that inherits from a DependencyObject. Ensure correct code fix option for 'To dependency property' is displayed.

## Verifying generated VSIX

List of manual checks for verifying a VSIX before release.

- Right-click in XAML designer. In the RXT context menu select "Profiles"
- Options menu should be opened and default profiles populated.
- Export a profile. Should create the file on disk.
- Import the exported profile. Should load the duplicate profile.
- Edit the exported profile so that it is not valid JSON. Try and import the corrupt file. Ensure an appropriate error message is displayed.
- Edit a profile. Check that code highlighting is applied to appropriate fields.
- Open any ViewModel file. Highlight some properties and select "Copy to Clipboard" from the context menu. Press Ctrl+V to confirm what was added to the clipboard.
- Open the Toolbox (Ctrl+W, X). Right-click on the class name of a VM and select "Send to Toolbox." Toolbox entry should be created for that class.
- Open a XAML window. Drag a VM file from the Solution Explorer onto the XAML window and appropriate elements should be added to the editor.
- In a XAML Window add some content that will cause Analysis warnings to be displayed. Save the file and check that the appropriate underlining is applied and Error List entries are added.
