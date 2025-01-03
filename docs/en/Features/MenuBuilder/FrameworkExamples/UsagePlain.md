# Integrating with Blazor Templates

## Prerequisites

Create a project using the Blazor template in Visual Studio 2022.

![alt text](blazor-template.png)

## Preparations

The standard template does not include icons, so add the following to `wwwroot/index.html`:

```html
<head>
    <!-- Addition -->
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/font/bootstrap-icons.min.css">
</head>
```

This will enable the use of Bootstrap Icons.

## Creating WebPaths.cs

Create `WebPaths.cs` to define URL paths. Use [Bootstrap Icons](https://icons.getbootstrap.com/) for icon definitions. For detailed configuration, refer to [Menu Item Customization](../MenuCustomization.md).

```csharp title="WebPaths.cs"
using BlazorPathHelper;

[BlazorPath]
public partial class WebPaths
{
    [Item("Home", Icon = "bi-house-door-fill")]
    public const string Home = "/";
    [Item("Sample1", Icon = "bi-1-circle-fill")]
    public const string Sample1 = "/sample1";
    [Item("Sample1C1", Icon = "bi-1-square")]
    public const string Sample1C1 = $"{Sample1}/child1";
    [Item("Sample1C2", Icon = "bi-2-square")]
    public const string Sample1C2 = $"{Sample1}/child2";
    [Item("Sample1C2C1", Icon = "bi-bag-plus")]
    public const string Sample1C2C1 = $"{Sample1}/child2/child1";
    [Item("Sample2", Icon = "bi-2-circle-fill")]
    public const string Sample2 = "/sample2";
    [Item("Sample2C1", Icon = "bi-1-square")]
    public const string Sample2C1 = $"{Sample2}/child1";
    [Item("Sample3", Icon = "bi-3-circle-fill")]
    public const string Sample3 = "/sample3";
}
```

## Creating the Menu Component

Create `NavMenuItem.razor` to display the menu component.

```razor title="NavMenuItem.razor"
@using BlazorPathHelper

@foreach(var menuItem in MenuItems)
{
  <!-- Use menuItem.Key for defining the key attribute -->
  <div @key=menuItem.Key class="nav-item ps-3 py-1">
    <!-- menuItem.Path represents the URL path of the menu item -->
    <!-- NavLinkMatch.All applies only to the homepage, otherwise use Prefix -->
    <NavLink class="nav-link" href="@menuItem.Path"
             Match="@(menuItem.IsHome ? NavLinkMatch.All : NavLinkMatch.Prefix)">
      <!-- Icons and menu names are passed as strings -->
      <span class="me-2 fs-5 @menuItem.Icon" aria-hidden="true"></span>
      @menuItem.Name
    </NavLink>
    <!-- Recursively call to display child elements -->
    <nav class="flex-column">
      <NavMenuItem MenuItems="menuItem.Children"/>
    </nav>
  </div>
}

@code {
  // Accepts an array of menu items
  [Parameter, EditorRequired]
  public BlazorPathMenuItem[] MenuItems { get; set; } = [];
}
```

## Displaying the Menu

Add the menu component to `MainLayout.razor`.

```razor title="MainLayout.razor"
<!-- Omitted -->
<div class="@NavMenuCssClass nav-scrollable">
    <nav class="flex-column pe-3">
        <NavMenuItem MenuItems="WebPaths.MenuItem"/>
    </nav>
</div>
<!-- Omitted -->
```

## Execution Result

<img src="sample-plain.gif" style="width:400px;">

## Source Code

You can find an implementation example at [Example.Plain](https://github.com/arika0093/BlazorPathHelper/tree/main/examples/Example.Plain/).