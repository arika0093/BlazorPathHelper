## 最小限的使用方法

以下是最小限必要的内容：

* 准备一个带有 `#!csharp [BlazorPath]` 属性的类。在此过程中，类定义需要使用 `#!csharp partial` 属性。
* 在该类中定义一个 `#!csharp const string` 类型的常量。
* 作为成员的属性，添加 `#!csharp [Page<PageComponent>]`。

BlazorPathHelper 会自动扫描满足上述条件的类定义，并生成 URL 构建函数。

```csharp title="WebPaths.cs"
using BlazorPathHelper;
using Microsoft.AspNetCore.Components;

[BlazorPath]
public partial class WebPaths
{
  [Page<Home>]
  public const string Index = "/";
}

// 各组件的定义（实际上在各组件中描述）
public partial class Home : ComponentBase;
```

??? abstract "生成的代码"

    ```csharp title="Auto Generated Code"
    // <auto-generated />
    [Route("/")]
    public partial class Home;
    ```

结果是，`Home` 组件会自动获得与 `@page` 属性相同的效果。

!!! tip "注意！"

    最后需要删除原本定义的 `@page` 属性。

## 参数与查询支持

如果 URL 中包含[参数定义](../UrlBuilder/index.md)，则会自动添加 `[Parameter]` 属性。

```csharp title="WebPaths.cs"
using BlazorPathHelper;

[BlazorPath]
public partial class WebPaths
{
  [Page<Counter>]
  public const string CounterWithState = "/counter/{count:int}";
}
```

??? abstract "生成的代码"

    ```csharp title="Auto Generated Code"
    // <auto-generated />
    [Route("/counter/{count:int}")]
    public partial class Counter
    {
      [Parameter]
      public int Count { get; set; }
    }
    ```

同样，通过[添加](../UrlBuilder/QuerySupport.md) `Query<QueryRecord>` 属性，可以自动添加 `[SupplyParameterFromQuery]` 属性。

```csharp title="WebPaths.cs"
using BlazorPathHelper;

[BlazorPath]
public partial class WebPaths
{
  [Query<QueryRecord>, Page<Counter2>]
  public const string CounterWithQuery = "/counter/query";
}

public record QueryRecord(string query = "hello", int page = 0, bool? opt = null);
```

??? abstract "生成的代码"

    ```csharp title="Auto Generated Code"
    // <auto-generated />
    [Route("/counter/query")]
    public partial class Counter2
    {
      [SupplyParameterFromQuery]
      public string Query { get; set; }

      [SupplyParameterFromQuery]
      public int Page { get; set; }

      [SupplyParameterFromQuery]
      public bool? Opt { get; set; }
    }
    ```

也支持查询名称的缩写。

```csharp title="WebPaths.cs"
using BlazorPathHelper;

[BlazorPath]
public partial class WebPaths
{
  [Query<QueryRecord>, Page<Counter3>]
  public const string CounterWithQuery = "/counter/query";
}

public record QueryRecord
{
  [SupplyParameterFromQuery(Name = "q")]
  public string Query { get; init; } = "hello";

  [SupplyParameterFromQuery(Name = "p")]
  public int Page { get; init; } = 0;

  [SupplyParameterFromQuery(Name = "o")]
  public bool? Opt { get; init; }
}
```

??? abstract "生成的代码"

    ```csharp title="Auto Generated Code"
    // <auto-generated />
    [Route("/counter/query")]
    public partial class Counter3
    {
      [SupplyParameterFromQuery(Name = "q")]
      public string Query { get; set; }

      [SupplyParameterFromQuery(Name = "p")]
      public int Page { get; set; }

      [SupplyParameterFromQuery(Name = "o")]
      public bool? Opt { get; set; }
    }
    ```

## 工作原理
TODO