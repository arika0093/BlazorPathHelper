---
title: MudBlazorに組み込む
---

## 前提

[公式のテンプレート](https://mudblazor.com/getting-started/installation#using-templates)を使用してプロジェクトを作成します。

```bash title="MudBlazor テンプレートの導入"
dotnet new install MudBlazor.Templates
dotnet new mudblazor --interactivity Auto --name MyApplication --all-interactive
```

## WebPaths.csの作成

`WebPaths.cs` を作成し、URLパスを定義します。
アイコン定義は[MudBlazor Icons](https://mudblazor.com/features/icons)を使用します。
詳細な設定方法は [メニュー項目カスタマイズ](../MenuCustomization.md) を参照してください。

```csharp title="WebPaths.cs"
using BlazorPathHelper;
// アイコン定義を簡略化するためにusing staticを使用
using static MudBlazor.Icons.Material.Filled;

[BlazorPath]
public partial class WebPaths
{
    [Item("Home", Icon = House)]
    public const string Home = "/";
    [Item("Sample1", Icon = Filter1)]
    public const string Sample1 = "/sample1";
    [Item("Sample1C1", Icon = ExposurePlus1)]
    public const string Sample1C1 = $"{Sample1}/child1";
    [Item("Sample1C2", Icon = ExposurePlus2)]
    public const string Sample1C2 = $"{Sample1}/child2";
    [Item("Sample1C2C1", Icon = StarBorder)]
    public const string Sample1C2C1 = $"{Sample1}/child2/child1";
    [Item("Sample2", Icon = Filter2)]
    public const string Sample2 = "/sample2";
    [Item("Sample2C1", Icon = _1xMobiledata)]
    public const string Sample2C1 = $"{Sample2}/child1";
    [Item("Sample3", Icon = Filter3)]
    public const string Sample3 = "/sample3";
}
```

## メニューコンポーネントの作成

`NavMenu.razor` を作成し、メニューを表示するコンポーネントを作成します。

```razor title="NavMenu.razor"
@using BlazorPathHelper
@using global::MudBlazor

@foreach(var menuItem in MenuItems)
{
  @if (menuItem.HasChildren)
  {
    <MudNavGroup Title="@menuItem.Name" Icon="@menuItem.Icon?.ToString()" 
                 Expanded="true" ExpandIcon="@Icons.Material.Filled.ExpandMore">
      <NavMenu MenuItems="@menuItem.Children" />
    </MudNavGroup>
  }
  else
  {
    <MudNavLink Href="@menuItem.Path" Icon="@menuItem.Icon?.ToString()" 
                Match="@(menuItem.IsHome ? NavLinkMatch.All : NavLinkMatch.Prefix)">
      @menuItem.Name
    </MudNavLink>
  }
}

@code {
  [Parameter, EditorRequired]
  public BlazorPathMenuItem[] MenuItems { get; set; } = [];
}
```

## メニューの表示

`MainLayout.razor` にメニューを表示するコンポーネントを追加します。

```razor title="MainLayout.razor"
<!-- 省略 -->
<MudDrawer @bind-Open="_drawerOpen" ClipMode="DrawerClipMode.Always" Elevation="2">
  <MudNavMenu>
    <NavMenu MenuItems="WebPaths.MenuItem"/>
  </MudNavMenu>
</MudDrawer>
<!-- 省略 -->
```

## 実行結果

<img src="../image/sample-mudblazor.gif" style="width:400px;">

## 注意事項

この例ではSample1のようなサブメニューを持つ要素にはリンクが指定されていません。
これはMudBlazorがサブメニューを持つ要素にリンクを設定するAPIを用意していないためです。

## ソースコード
実装例は [Example.MudBlazor](https://github.com/arika0093/BlazorPathHelper/examples/Example.MudBlazor/) で利用できます。
