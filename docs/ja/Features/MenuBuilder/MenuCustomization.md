---
title: メニュー項目カスタマイズ
---

## ページの説明文を追加する
メニュー項目に説明文を追加するには、`Description`プロパティを使用します。

```csharp title="WebPaths.cs"
using BlazorPathHelper;

[BlazorPath]
public partial class WebPaths
{
    [Item("Home", Description = "home page")]
    public const string Home = "/";
}
```

!!! tip "メニュー側の実装も必要です"

    あくまでデータ側の更新方法なので、実際にはメニュー側にも`Description`を表示するための実装が必要です。


## アイコンを指定する
メニュー項目にアイコンを指定するには、`Icon`プロパティを使用します。

```csharp title="WebPaths.cs"
using BlazorPathHelper;

[BlazorPath]
public partial class WebPaths
{
    [Item("Home", Icon = "home")]
    public const string Home = "/";
}
```

基本的には上記の指定で十分ですが、FluentUIのようにアイコン定義がクラスで提供されている場合はジェネリクスを使用します。

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

FluentUIの場合の詳細については [Fluent UIに組み込む](./FrameworkExamples/UsageFluentUI.md) を参照してください。

## メニュー項目を非表示にする
メニュー項目を非表示にするには、`Visible`プロパティを使用します。

```csharp title="WebPaths.cs"
using BlazorPathHelper;

[BlazorPath]
public partial class WebPaths
{
    [Item(Visible = false)]
    public const string Home = "/";
}
```

## URL構造を無視して階層構造を指定する
例として、以下のような階層構造を持つメニューを考えます。

```
├── TopPage (/)
└── Sample (/sample)
    ├── SampleC1 (/sample/child1)
    ├── SampleC2 (/sample/child2)
    └── OtherSample (/super/sub/sample)
        └── OtherSampleC1 (/super/sub/sample/child1)
```

`OtherSample`のようにURL的なつながりが直接的でない場合、`Group`プロパティを使用して親子関係を指定します。

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

上記のように、子要素の`Group`プロパティに親要素のキーを指定します。


別の例として、以下のような階層構造を持つメニューを考えます。


```
├── TopPage (/)
├── Sample (/sample)
│   ├── SampleC1 (/sample/child1)
│   └── SampleC2 (/sample/child2)
└── OtherSample (/super/sub/sample)
    └── OtherSampleC1 (/super/sub/sample/child1)
```

このように最も上位のメニューとして`OtherSample`を指定する場合、`Group`プロパティに`/`を指定します。

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

## メニュー項目の多言語対応(i10n)を行う

まずはリソースを作成します。今回は`Resources`フォルダに以下の内容で`Localize.resx`を作成します。

| Name       | English            | Japanese |
| ---------- | ------------------ | -------- |
| Sample     | Sample Text        | サンプルテキスト |
| SampleDesc | Sample Description | サンプル説明文  |

そして、以下のように指定します。

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

リソースキーが渡されるので、表示側でそのキーを利用して表示します。

```csharp title="Menu.razor"
@inject IStringLocalizer<Localize> Localizer
@foreach(var menuItem in WebPaths.MenuItem)
{
  <a href="@menuItem.Path">
    @Localizer[menuItem.Name]
  </a>
}
```

実装例は #TODO を参照してください。
