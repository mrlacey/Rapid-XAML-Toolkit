
# Rapid XAML Toolkit

[![Build status](https://ci.appveyor.com/api/projects/status/kryvt4vdvy39940m/branch/dev?svg=true)](https://ci.appveyor.com/project/mrlacey/rapid-xaml-toolkit/branch/dev)
[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)
![Works with Visual Studio 2019](https://img.shields.io/static/v1.svg?label=VS&message=2019&color=5F2E96)
![AppVeyor tests](https://img.shields.io/appveyor/tests/mrlacey/rapid-xaml-toolkit)

This is a collection of tools for making it easier for developers to work with XAML (**UWP**, **WPF**, and **Xamarin.Forms**). These tools include

- **XAML Generation** (making it easier to create a UI with XAML)
- **XAML Analysis** (helping you find and fix issues with XAML)
- **Roslyn Analyzers** (to help with code related to MVVM and XAML)
- **Project & Item Templates** (to create new apps faster)

Get it from the [VS Marketplace](https://marketplace.visualstudio.com/items?itemName=MattLaceyLtd.RapidXamlToolkit)

## XAML Generation

Creating a XAML UI can be slow and require a lot of manual effort. These tools reduce the time and effort required to get the basics working, allowing more time to customize the UI to meet the specific needs of your app.
We can't and don't try to create the whole app for you but we can make creating and working with XAML faster and easier.
The generated XAML is based on common conventions but is highly [configurable](./docs/configuration.md).

Go from this

```csharp
public class OrderDetailsViewModel : ViewModelBase
{
    public int OrderId { get; private set; }
    public Guid CustomerId { get; set; }
    public DateTimeOffset OrderDate { get; set; }
    public string OrderNotes { get; set; }
    public decimal OrderTotal { get; }
    public ObservableCollection<OrderLineItem> Items { get; set; }
}
```

to this

```xml
<StackPanel>
    <TextBlock Text="{x:Bind ViewModel.OrderId}" />
    <TextBlock Text="{x:Bind ViewModel.CustomerId}" />
    <DatePicker Date="{x:Bind ViewModel.OrderDate, Mode=TwoWay}" />
    <TextBox Text="{x:Bind ViewModel.OrderNotes, Mode=TwoWay}" />
    <TextBlock Text="{x:Bind ViewModel.OrderTotal}" />
    <ListView ItemsSource="{x:Bind ViewModel.Items}">
        <ListView.ItemTemplate>
            <DataTemplate x:DataType="OrderLineItem">
                <StackPanel>
                    <TextBlock Text="{x:Bind OrderLineId}" />
                    <TextBlock Text="{x:Bind ItemId}" />
                    <TextBlock Text="{x:Bind ItemDescription}" />
                    <TextBlock Text="{x:Bind Quantity}" />
                    <TextBlock Text="{x:Bind UnitPrice}" />
                    <TextBlock Text="{x:Bind LineTotal}" />
                </StackPanel>
            </DataTemplate>
        </ListView.ItemTemplate>
    </ListView>
</StackPanel>
```

in a couple of clicks.

There are three ways to turn your ViewModel code into XAML

- Create XAML by dragging a ViewModel file onto the designer.
- Copy a property, selection of properties, or the whole class (from the ViewModel) into the clipboard and paste into the View as XAML.
- Send a property, selection of properties, or the whole class (from the ViewModel) into the Toolbox and drag into the View as XAML.

Learn more about [features](./docs/features.md).

## XAML Analysis

Like [Roslyn Analyzers](https://docs.microsoft.com/en-us/visualstudio/code-quality/roslyn-analyzers-overview?view=vs-2019) do for C# and VB.Net, XAML Analyzers can identify potential issues in XAML files and create comments, warnings, or errors about the problems. They also provide Suggested Actions as ways to fix the issues.

![Screenshot showing some of the issues analysis can find](./docs/Assets/xaml-analysis-example.png)

## Principles guiding this project

- Doing something is better than doing nothing.
- The toolkit can't generate the final XAML as every app requires unique customization.
- Everything that is output should be configurable.
- C# and VB.Net are supported equally.
- The toolkit won't do things that Visual Studio can already do. (Without very good reason.)
- This toolkit is focused specifically on tooling for working with XAML. It will not include controls, etc.

## Installation

Please see our [**getting started guide**](./docs/getting-started.md).

## Contributing

Please see the [contribution guide](./CONTRIBUTING.md).

***

## History

The Rapid XAML Toolkit (RXT) was started as a sister project to [Windows Template Studio (WinTS)](https://aka.ms/wts).  
It was oringinally created in partnership with Microsoft (that's why [github.com/microsoft/rapid-xaml-toolkit redirects](github.com/microsoft/rapid-xaml-toolkit) here) but is now primarily cared for by [Matt Lacey](https://github.com/mrlacey).
