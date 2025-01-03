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

!!! tip "Implementation Needed on the Menu Side"

    This is just a method for updating the data. You will also need to implement the display of `Description` on the menu side.

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

Generally, the above specification is sufficient, but if icon definitions are provided as classes, like in FluentUI, you can use generics.

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

Specify the parent element's key in the `Group` property of child elements.

For another example, consider the following hierarchical structure:

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

## Implementing Multilingual Support (i10n) for Menu Items

First, create a resource file. In this example, create `Localize.resx` in the `Resources` folder with the following content:

| Name       | English            | Japanese |
| ---------- | ------------------ | -------- |
| Sample     | Sample Text        | サンプルテキスト |
| SampleDesc | Sample Description | サンプル説明文  |

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

The resource key is passed, so use it for display on the UI side.

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