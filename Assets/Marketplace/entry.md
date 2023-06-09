# Rapid XAML Toolkit - VS Marketplace Entry details

- **Logo** - _should come from package_
- **Description** - _markdown below the line_
- **Type** - Tools
- **Categories** - Coding, Other, Scaffolding
- **Tags** - XAML, UWP, WPF, Xamarin.Forms, MVVM, Xamarin
- **Pricing Category** - Free
- **Source code repository** - https://github.com/mrlacey/rapid-xaml-toolkit
- **Allow Q&A for your extension** - True

---

A collection of tools for making it easier to work with XAML (UWP, WPF, and Xamarin.Forms).
These tools fall into two categories

- **XAML Generation** (making it easier to create a UI with XAML)
- **XAML Analysis** (helping you find and fix issues with XAML)

## XAML Generation

Creating a XAML UI can be slow and require a lot of manual effort. Reduce the time and effort required to get the basics working, allowing you more time to customize the UI to meet the specific needs of your app.

In a couple of clicks (or with a drag and drop), take this

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

and generate this

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

What is generated is based on common conventions but is highly [configurable](https://github.com/mrlacey/Rapid-XAML-Toolkit/blob/main/docs/configuration.md) and with multiple profiles provided by default.

## XAML Analysis

Like [Roslyn Analyzers](https://docs.microsoft.com/en-us/visualstudio/code-quality/roslyn-analyzers-overview?view=vs-2019) do for C# and VB&#183;Net, this toolkit can identify potential issues in XAML files and create suggestions, warnings, or errors about the problems. Suggested Actions are also provided to easily fix what is found.

![Screenshot showing some of the issues that analysis can find](https://github.com/mrlacey/Rapid-XAML-Toolkit/blob/main/docs/Assets/xaml-analysis-example.png?raw=true)

## Get Involved

[Feedback](https://github.com/mrlacey/Rapid-XAML-Toolkit/issues/new/choose) and [contributions](https://github.com/mrlacey/Rapid-XAML-Toolkit/blob/main/CONTRIBUTING.md) welcome.
