# Features

Make working with XAML files faster and easier with these great features. Whether you're starting with just the ViewModel and an empty view, or already have XAML written, there are features here to help all developers.

## XAML Generation

Drag the ViewModel file onto the open XAML source file.

![XAML generation via drag-and-drop](./Assets/drag-drop-gen.gif)

Get the XAML from the ViewModel and paste it into the View.

![Copy class in ViewModel and paste into View as XAML](./Assets/Copy-Class-To-Clipboard.gif)

You don't have to copy the XAML for the whole class.

![Copy selection of properties in ViewModel and paste into View as XAML](./Assets/Copy-Selection-To-Clipboard.gif)

If you want to add the XAML in multiple places you can send them to the Toolbox and use them from there.

![Send properties to the Toolbox then drag onto the View as XAML](./Assets/Send-To-Toolbox-And-Drag-To-View.gif)

## XAML Analysis

A number of potential issues will be listed in the Error List and underlined in source. Suggested Actions are also included to help address each issue.

The image below shows some of the issues that analysis can find (and fix).

![Screenshot showing some of the issues analysis can find](./Assets/xaml-analysis-example.png)

See the [full list of all warnings](./warnings/readme.md#rapid-xaml-toolkit---warnings).

Additionaly, it's also possible to:

- Right-click anywhere in a XAML source file and select 'Rapid XAML > Move all hard-coded string to Resource file' to move all hard-coded strings to a resource file.
- Right-click on an opening `Grid` tag and easily add Row and Column definitions.

### Custom XAML Analyzers

Extend the built-in XAML Analysis by easily [adding your own analyzers](./custom-analysis.md).

## Roslyn Analyzers

Miscellaneous utility analyzers and code fixes to make it easier to work with C# and VB.Net code in your XAML and MVVM apps.

## Project and Item Templates

Additional templates providing extra options to make creating XAML apps easier and faster.
