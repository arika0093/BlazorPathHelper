## 添加页面描述

要为菜单项添加描述，请使用`Description`属性。

```csharp title="WebPaths.cs"
using BlazorPathHelper;

[BlazorPath]
public partial class WebPaths
{
    [Item("Home", Description = "主页")]
    public const string Home = "/";
}
```

!!! tip "还需要在菜单端进行实现"

    这只是数据端的更新方法，实际上还需要在菜单端实现以显示`Description`。

## 指定图标

要为菜单项指定图标，请使用`Icon`属性。

```csharp title="WebPaths.cs"
using BlazorPathHelper;

[BlazorPath]
public partial class WebPaths
{
    [Item("Home", Icon = "home")]
    public const string Home = "/";
}
```

基本上，上述指定已经足够，但如果像FluentUI这样图标定义是通过类提供的，可以使用泛型。

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

关于FluentUI的详细信息，请参阅[集成Fluent UI](./FrameworkExamples/UsageFluentUI.md)。

## 隐藏菜单项

要隐藏菜单项，请使用`Visible`属性。

```csharp title="WebPaths.cs"
using BlazorPathHelper;

[BlazorPath]
public partial class WebPaths
{
    [Item(Visible = false)]
    public const string Home = "/";
}
```

## 忽略URL结构指定层次结构

例如，考虑以下具有层次结构的菜单。

```
├── TopPage (/)
└── Sample (/sample)
    ├── SampleC1 (/sample/child1)
    ├── SampleC2 (/sample/child2)
    └── OtherSample (/super/sub/sample)
        └── OtherSampleC1 (/super/sub/sample/child1)
```

对于像`OtherSample`这样URL连接不直接的情况，可以使用`Group`属性指定父子关系。

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

如上所示，在子元素的`Group`属性中指定父元素的键。

另一个例子，考虑以下具有层次结构的菜单。

```
├── TopPage (/)
├── Sample (/sample)
│   ├── SampleC1 (/sample/child1)
│   └── SampleC2 (/sample/child2)
└── OtherSample (/super/sub/sample)
    └── OtherSampleC1 (/super/sub/sample/child1)
```

如果要将`OtherSample`指定为最上层菜单，请在`Group`属性中指定`/`。

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

## 实现菜单项的多语言支持(i10n)

首先创建资源。在`Resources`文件夹中创建`Localize.resx`，内容如下：

| 名称       | 英文             | 日文     |
| ---------- | ---------------- | -------- |
| Sample     | Sample Text      | サンプルテキスト |
| SampleDesc | Sample Description | サンプル説明文  |

然后按如下方式指定。

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

资源键会被传递，因此在显示端可以利用该键进行显示。

```csharp title="Menu.razor"
@inject IStringLocalizer<Localize> Localizer
@foreach(var menuItem in WebPaths.MenuItem)
{
  <a href="@menuItem.Path">
    @Localizer[menuItem.Name]
  </a>
}
```

有关实现示例，请参阅#TODO。