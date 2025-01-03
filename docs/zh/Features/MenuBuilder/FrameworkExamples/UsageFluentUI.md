以下是将给定文本翻译成自然中文的版本：

## 前提条件

使用[官方模板](https://www.fluentui-blazor.net/CodeSetup)创建项目。

```bash title="FluentUI 模板的安装"
dotnet new install Microsoft.FluentUI.AspNetCore.Templates
dotnet new fluentblazorwasm --name MyApplication
```

## 创建 WebPaths.cs

创建 `WebPaths.cs` 来定义 URL 路径。与其他框架不同，FluentUI 的 `Icon` 是通过类而不是字符串定义的，因此不能直接指定。因此，需要以泛型的形式指定图标类。图标的定义可以参考 [FluentUI Icons](https://www.fluentui-blazor.net/Icon)。详细的设置方法请参阅[菜单项自定义](../MenuCustomization.md)。

```csharp title="WebPaths.cs"
using BlazorPathHelper;
// 为了简化图标定义，使用 using static
using static Microsoft.FluentUI.AspNetCore.Components.Icons.Regular.Size20;

[BlazorPath]
public partial class WebPaths
{
    [Item<Home>("主页")]
    public const string Home = "/";
    [Item<TextHeader1>("示例1")]
    public const string Sample1 = "/sample1";
    [Item<AddCircle>("示例1子项1")]
    public const string Sample1C1 = $"{Sample1}/child1";
    [Item<AddCircle>("示例1子项2")]
    public const string Sample1C2 = $"{Sample1}/child2";
    [Item<CheckmarkCircle>("示例1子项2子项1")]
    public const string Sample1C2C1 = $"{Sample1}/child2/child1";
    [Item<TextHeader2>("示例2")]
    public const string Sample2 = "/sample2";
    [Item<Star>("示例2子项1")]
    public const string Sample2C1 = $"{Sample2}/child1";
    [Item<TextHeader3>("示例3")]
    public const string Sample3 = "/sample3";
}
```

## 创建菜单组件

创建 `NavMenuItem.razor` 以显示菜单的组件。

```razor title="NavMenuItem.razor"
@using BlazorPathHelper

@foreach(var menuItem in MenuItems)
{
  <!-- 根据是否有子元素进行不同处理 -->
  @if(menuItem.HasChildren)
  {
    <!-- key 属性定义使用 menuItem.Key -->
    <!-- menuItem.Path 表示菜单项的 URL 路径 -->
    <!-- Icon 是 object? 类型，因此需要转换后使用 -->
    <FluentNavGroup @key=@menuItem.Key Href="@menuItem.Path"
        Title="@menuItem.Name" Icon="@((Icon?)menuItem.Icon)">
      <!-- 递归调用以显示子元素 -->
      <NavMenuItem MenuItems="menuItem.Children"/>
    </FluentNavGroup>
  }
  else
  {
    <!-- NavLinkMatch.All 仅适用于主页，其他使用 Prefix -->
    <FluentNavLink @key=@menuItem.Key Href="@menuItem.Path"
        Match="@(menuItem.IsHome ? NavLinkMatch.All : NavLinkMatch.Prefix)"
        Icon="@((Icon?)menuItem.Icon)" IconColor="Color.Accent">
      @menuItem.Name
    </FluentNavLink>
  }
}

@code {
  // 接收菜单项数组
  [Parameter, EditorRequired]
  public BlazorPathMenuItem[] MenuItems { get; set; } = [];
}
```

## 显示菜单

在 `MainLayout.razor` 中添加显示菜单的组件。

```razor title="MainLayout.razor"
<!-- 省略 -->
<FluentNavMenu Id="main-menu" Width="250" Collapsible="true"
               Title="导航菜单" CustomToggle="true">
  <NavMenuItem MenuItems="WebPaths.MenuItem"/>
</FluentNavMenu>
<!-- 省略 -->
```

## 运行结果

![](../../../../assets/sample-fluentui.gif){: style="width: 400px;" }

## 源代码
实现示例可以在 [Example.FluentUI](https://github.com/arika0093/BlazorPathHelper/tree/main/examples/Example.FluentUI/) 中找到。