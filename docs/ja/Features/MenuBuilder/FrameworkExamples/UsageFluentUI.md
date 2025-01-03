---
title: Fluent UIに組み込む
---

## 前提

[公式のテンプレート](https://www.fluentui-blazor.net/CodeSetup)を使用してプロジェクトを作成します。

```bash title="FluentUI テンプレートの導入"
dotnet new install Microsoft.FluentUI.AspNetCore.Templates
dotnet new fluentblazorwasm --name MyApplication
```

## WebPaths.csの作成

`WebPaths.cs` を作成し、URLパスを定義します。
他のフレームワークと異なる点として、FluentUIの`Icon`は文字列でなくクラスで定義されておりそのままでは指定できません。
そのため、Genericの形式でアイコンクラスを指定します。
Iconの定義は[FluentUI Icons](https://www.fluentui-blazor.net/Icon)を使用します。
詳細な設定方法は [メニュー項目カスタマイズ](../MenuCustomization.md) を参照してください。

```csharp title="WebPaths.cs"
using BlazorPathHelper;
// アイコン定義を簡略化するためにusing staticを使用
using static Microsoft.FluentUI.AspNetCore.Components.Icons.Regular.Size20;

[BlazorPath]
public partial class WebPaths
{
    [Item<Home>("Home")]
    public const string Home = "/";
    [Item<TextHeader1>("Sample1")]
    public const string Sample1 = "/sample1";
    [Item<AddCircle>("Sample1C1")]
    public const string Sample1C1 = $"{Sample1}/child1";
    [Item<AddCircle>("Sample1C2")]
    public const string Sample1C2 = $"{Sample1}/child2";
    [Item<CheckmarkCircle>("Sample1C2C1")]
    public const string Sample1C2C1 = $"{Sample1}/child2/child1";
    [Item<TextHeader2>("Sample2")]
    public const string Sample2 = "/sample2";
    [Item<Star>("Sample2C1")]
    public const string Sample2C1 = $"{Sample2}/child1";
    [Item<TextHeader3>("Sample3")]
    public const string Sample3 = "/sample3";
}
```

## メニューコンポーネントの作成

`NavMenuItem.razor` を作成し、メニューを表示するコンポーネントを作成します。

```razor title="NavMenuItem.razor"
@using BlazorPathHelper

@foreach(var menuItem in MenuItems)
{
  <!-- 子要素がある場合とない場合で処理を分ける -->
  @if(menuItem.HasChildren)
  {
    <!-- key属性の定義には menuItem.Key が使えます -->
    <!-- menuItem.Path はメニューアイテムのURLパスを表します -->
    <!-- Iconはobject?型であるため、キャストして使用します -->
    <FluentNavGroup @key=@menuItem.Key Href="@menuItem.Path"
        Title="@menuItem.Name" Icon="@((Icon?)menuItem.Icon)">
      <!-- 子要素を表示するために、再帰的に呼び出します -->
      <NavMenuItem MenuItems="menuItem.Children"/>
    </FluentNavGroup>
  }
  else
  {
    <!-- NavLinkMatch.All はトップページにのみ適用し、それ以外はPrefixを使用 -->
    <FluentNavLink @key=@menuItem.Key Href="@menuItem.Path"
        Match="@(menuItem.IsHome ? NavLinkMatch.All : NavLinkMatch.Prefix)"
        Icon="@((Icon?)menuItem.Icon)" IconColor="Color.Accent">
      @menuItem.Name
    </FluentNavLink>
  }
}

@code {
  // メニューアイテムの配列を受け取ります
  [Parameter, EditorRequired]
  public BlazorPathMenuItem[] MenuItems { get; set; } = [];
}
```

## メニューの表示

`MainLayout.razor` にメニューを表示するコンポーネントを追加します。

```razor title="MainLayout.razor"
<!-- 省略 -->
<FluentNavMenu Id="main-menu" Width="250" Collapsible="true"
               Title="Navigation menu" CustomToggle="true">
  <NavMenuItem MenuItems="WebPaths.MenuItem"/>
</FluentNavMenu>
<!-- 省略 -->
```

## 実行結果

![](../../../../assets/sample-fluentui.gif){: style="width: 400px;" }


## ソースコード
実装例は [Example.FluentUI](https://github.com/arika0093/BlazorPathHelper/tree/main/examples/Example.FluentUI/) で利用できます。
