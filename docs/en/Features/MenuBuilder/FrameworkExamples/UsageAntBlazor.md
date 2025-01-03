# Integrating Ant Blazor

## Prerequisites

Create a project using the [official template](https://antblazor.com/en-US/docs/introduce).

```bash title="Setting up the AntBlazor Template"
dotnet new --install AntDesign.Templates
dotnet new antdesign -o MyAntDesignApp
```

## Creating WebPaths.cs

Create `WebPaths.cs` to define URL paths. Use [Ant Design Icons](https://antblazor.com/en-US/components/icon) for icon definitions. For detailed configuration, refer to [Menu Item Customization](../MenuCustomization.md).

```csharp title="WebPaths.cs"
using BlazorPathHelper;

[BlazorPath]
public partial class WebPaths
{
    [Item("Home", Icon = "home")]
    public const string Home = "/";
    [Item("Sample1", Icon = "folder")]
    public const string Sample1 = "/sample1";
    [Item("Sample1C1", Icon = "file")]
    public const string Sample1C1 = $"{Sample1}/child1";
    [Item("Sample1C2", Icon = "folder")]
    public const string Sample1C2 = $"{Sample1}/child2";
    [Item("Sample1C2C1", Icon = "file")]
    public const string Sample1C2C1 = $"{Sample1}/child2/child1";
    [Item("Sample2", Icon = "folder")]
    public const string Sample2 = "/sample2";
    [Item("Sample2C1", Icon = "file")]
    public const string Sample2C1 = $"{Sample2}/child1";
    [Item("Sample3", Icon = "file")]
    public const string Sample3 = "/sample3";
}
```

## Creating the Menu Component

When you use the above template, the default menu component from `AntBlazor.Pro` is used.

### If Not Using `AntBlazor.Pro`

Create `NavMenuItem.razor` to display the menu component.

```razor title="NavMenuItem.razor"
@using BlazorPathHelper

@foreach (var menuItem in MenuItems)
{
  @if (menuItem.HasChildren)
  {
    <!-- Use menuItem.Key for defining the key attribute -->
    <SubMenu Key=@menuItem.Key>
      <TitleTemplate>
        <!-- Icon is of type object?, so use ToString -->
        <Icon Type=@(menuItem.Icon?.ToString()) Theme="IconThemeType.Outline" />
        <span>@menuItem.Name</span>
      </TitleTemplate>
      <ChildContent>
        <!-- Recursively call to display child elements -->
        <NavMenuItem MenuItems="menuItem.Children" />
      </ChildContent>
    </SubMenu>
  }
  else
  {
    <!-- menuItem.Path represents the URL path of the menu item -->
    <MenuItem RouterLink="@menuItem.Path" Key=@menuItem.Key>
      <Icon Type=@(menuItem.Icon?.ToString()) Theme="IconThemeType.Outline" />
      <span>@menuItem.Name</span> 
    </MenuItem>    
  }
}

@inject NavigationManager NavigationManager
@code {
  [Parameter, EditorRequired]
  public BlazorPathMenuItem[] MenuItems { get; set; } = default!;
}
```

### If Using `AntBlazor.Pro`

The standard template (Pro) includes a feature to generate menus from objects, but you can simplify the definition generation as follows:

```csharp title="Layout/BasicLayout.razor.cs"
protected override async Task OnInitializedAsync()
{
  _menuData = ConverterMenuDataItem(WebPaths.MenuItem);
}

private MenuDataItem[] ConverterMenuDataItem(BlazorPathMenuItem[] items)
{
  return items.Select(item => new MenuDataItem {
    Path = item.Path,
    Name = item.Name,
    Key = item.Index.ToString(),
    Icon = item.Icon?.ToString(),
    Children = item.HasChildren
      ? ConverterMenuDataItem(item.Children) : null
  }).ToArray();
}
```

## Displaying the Menu

Add a component to display the menu in `MainLayout.razor`.

```razor title="MainLayout.razor"
<!-- Omitted -->
<Menu Theme="MenuTheme.Dark" DefaultSelectedKeys=@(new[]{"1"}) Mode="MenuMode.Inline">
    <NavMenuItem MenuItems="WebPaths.MenuItem"/>
</Menu>
<!-- Omitted -->
```

## Result

![](../../../../assets/sample-antblazor.gif){: style="width: 400px;" }

## Notes

In this example, elements like Sample1 that have submenus do not have links specified. This is because AntBlazor does not provide an API to set links for elements with submenus.

## Source Code

You can find implementation examples at [Example.AntBlazor.Standard](https://github.com/arika0093/BlazorPathHelper/tree/main/examples/Example.AntBlazor.Standard/) and [Example.AntBlazor.Pro](https://github.com/arika0093/BlazorPathHelper/tree/main/examples/Example.AntBlazor.Pro/).