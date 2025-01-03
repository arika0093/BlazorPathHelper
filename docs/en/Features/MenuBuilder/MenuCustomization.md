# Customizing Menu Items

## Adding Descriptions to Pages
To add a description to a menu item, use the `Description` property.

```csharp title="WebPaths.cs"
using BlazorPathHelper;

[BlazorPath]
public partial class WebPaths
{
    [Item("Home", Description = "home page")]
    public const string Home = "/";
}
```

!!! tip "Implementation on the Menu Side is Also Required"

    This only updates the data side, so you'll need to implement the display of `Description` on the menu side as well.

## Specifying Icons
To assign an icon to a menu item, use the `Icon` property.

```csharp title="WebPaths.cs"
using BlazorPathHelper;

[BlazorPath]
public partial class WebPaths
{
    [Item("Home", Icon = "home")]
    public const string Home = "/";
}
```

The above specification is generally sufficient, but if icon definitions are provided as classes, like in FluentUI, you can use generics.

```csharp title="WebPaths.cs"
using BlazorPathHelper;
using static Microsoft.FluentUI.AspNetCore.Components.Icons.Regular.Size20;

[BlazorPath]
public partial class WebPaths
{
    [Item<Home>("Home")]
    public const string Home = "/";
}
```

For more details on using FluentUI, refer to [Integrating with Fluent UI](./FrameworkExamples/UsageFluentUI.md).

## Hiding Menu Items
To hide a menu item, use the `Visible` property.

```csharp title="WebPaths.cs"
using BlazorPathHelper;

[BlazorPath]
public partial class WebPaths
{
    [Item(Visible = false)]
    public const string Home = "/";
}
```

## Specifying Hierarchical Structure Ignoring URL Structure
Consider a menu with the following hierarchical structure:

```
├── TopPage (/)
└── Sample (/sample)
    ├── SampleC1 (/sample/child1)
    ├── SampleC2 (/sample/child2)
    └── OtherSample (/super/sub/sample)
        └── OtherSampleC1 (/super/sub/sample/child1)
```

When the URL connection is not direct, like `OtherSample`, use the `Group` property to specify parent-child relationships.

```csharp title="WebPaths.cs"
using BlazorPathHelper;

[BlazorPath]
public partial class WebPaths
{
    [Item("TopPage")]
    public const string Home = "/";
    [Item("Sample")]
    public const string Sample = "/sample";
    [Item("SampleC1", Group = "Sample")]
    public const string SampleC1 = "/sample/child1";
    [Item("SampleC2", Group = "Sample")]
    public const string SampleC2 = "/sample/child2";
    [Item("OtherSample", Group = Sample)]
    public const string OtherSample = "/super/sub/sample";
    [Item("OtherSampleC1")]
    public const string OtherSampleC1 = "/super/sub/sample/child1";
}
```

Specify the parent element's key in the `Group` property of the child elements.

Consider another example with the following structure:

```
├── TopPage (/)
├── Sample (/sample)
│   ├── SampleC1 (/sample/child1)
│   └── SampleC2 (/sample/child2)
└── OtherSample (/super/sub/sample)
    └── OtherSampleC1 (/super/sub/sample/child1)
```

To specify `OtherSample` as the top-level menu, set the `Group` property to `/`.

```csharp title="WebPaths.cs"
using BlazorPathHelper;

[BlazorPath]
public partial class WebPaths
{
    [Item("TopPage")]
    public const string Home = "/";
    [Item("Sample")]
    public const string Sample = "/sample";
    [Item("SampleC1", Group = "Sample")]
    public const string SampleC1 = "/sample/child1";
    [Item("SampleC2", Group = "Sample")]
    public const string SampleC2 = "/sample/child2";
    [Item("OtherSample", Group = Home)]
    public const string OtherSample = "/super/sub/sample";
    [Item("OtherSampleC1")]
    public const string OtherSampleC1 = "/super/sub/sample/child1";
}
```

## Implementing Multilingual Support (i18n) for Menu Items

First, create a resource. In this example, create `Localize.resx` in the `Resources` folder with the following content.

![resource keys](resource-keys.png)

Then, specify it as follows:

```csharp title="WebPaths.cs"
using BlazorPathHelper;

[BlazorPath]
public partial class WebPaths
{
    [Item(nameof(Localize.Sample))]
    public const string SampleLocalize = "/sample-l10n";
    [Item(nameof(Localize.Sample), Description = nameof(Localize.SampleDesc))]
    public const string SampleLocalizeWithDesc = "/sample-l10n-plus";
}
```

Resource keys are passed, so use those keys for display.

```csharp title="Menu.razor"
@inject IStringLocalizer<Localize> Localizer
@foreach(var menuItem in WebPaths.MenuItem)
{
  <a href="@menuItem.Path">
    @Localizer[menuItem.Name]
  </a>
}
```

For implementation examples, refer to #TODO.