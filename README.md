
# Rapid XAML Toolkit

Creating a XAML UI can be slow or require lots of manual effort. These tools aim to reduce the time and effort required to get the basics working and allow you to customize the UI to meet your preferences or the specific needs of your app.
We can't and don't try to create the whole app for you but we can make creating and working with XAML faster easier.
The functionality of the toolkit is based common conventions but is highly [configurable](./docs/configuration.md).

## Overview

The Rapid XAML Toolkit aims to help developers with two aspects of working with XAML files.

- Creating new XAML files
- Modifying existing XAML files

### Creation

In very few apps is the UI created first. It's more common to start with the data Model and the ViewModels and then create the UI for them. Allow the Rapid XAML Toolkit to scaffold your Views based on your ViewModels. It works with different naming conventions or files and folders. The View and ViewModel don't even have to be in the same project.

![Creating a View from context menu of ViewModel in Solution Explorer](./docs/Assets/Create-View-In-Solution-Explorer.gif)

### Modification and improvement

If you already have an empty page, you can easily get the XAML for the ViewModel and paste it into the View.

![Copy class in ViewModel and paste into View as XAML](./docs/Assets/Copy-Class-To-Clipboard.gif)

You don't have to copy the XAML for the whole class.

![Copy selection of properties in ViewModel and paste into View as XAML](./docs/Assets/Copy-Selection-To-Clipboard.gif)

If you want to add the XAML for multiple properties you can send them to the Toolbox and use them from there.

![Send properties to the Toolbox then drag onto the View as XAML](./docs/Assets/Send-To-Toolbox-And-Drag-To-View.gif)

In addition to creating the XAML for the View, you can also set the DataContext (so the bindings will work.)
Do it in Code-Behind.

![Set the DataContext and related properties if not already defined in the code-behind file](./docs/Assets/Set-Datacontext-In-CodeBehind.gif)

Or in the XAML.

![Set the DataContext and related properties if not already defined in the XAML file](./docs/Assets/Set-Datacontext-In-XAML.gif)

More to follow...

## Principles guiding this project

- Doing something is better than nothing.
- We can't produce the final UI as every app may require unique customization.
- Everything that is output should be configurable.
- C# and VB.Net are supported equally.
- This toolkit is focused specifically on tooling for working with XAML. It will not include controls, etc.

## Installation

details to follow.

## Contributing

Please see the [contribution guide](./CONTRIBUTING.md).
