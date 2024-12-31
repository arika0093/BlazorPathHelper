---
title: AntBlazorに組み込む
---

## 前提

[公式のテンプレート](https://antblazor.com/en-US/docs/introduce)を使用してプロジェクトを作成します。

```bash title="AntBlazor テンプレートの導入"
dotnet new --install AntDesign.Templates
dotnet new antdesign -o MyAntDesignApp
```

## WebPaths.csの作成

`WebPaths.cs` を作成し、URLパスを定義します。
アイコン定義は[Ant Design Icons](https://antblazor.com/en-US/components/icon)を使用します。
詳細な設定方法は [メニュー項目カスタマイズ](../MenuCustomization.md) を参照してください。

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

## メニューコンポーネントの作成
上記のテンプレートを使用して導入すると、標準では`AntBlazor.Pro`のメニューコンポーネントが使用されます。

### `AntBlazor.Pro`を使用しない場合

`NavMenuItem.razor` を作成し、メニューを表示するコンポーネントを作成します。

```razor title="NavMenuItem.razor"
@using BlazorPathHelper

@foreach (var menuItem in MenuItems)
{
  @if (menuItem.HasChildren)
  {
    <!-- key属性の定義には menuItem.Key が使えます -->
    <SubMenu Key=@menuItem.Key>
      <TitleTemplate>
        <!-- Iconはobject?型であるため、toStringで使用します -->
        <Icon Type=@(menuItem.Icon?.ToString()) Theme="outline" />
        <span>@menuItem.Name</span>
      </TitleTemplate>
      <ChildContent>
        <!-- 子要素を表示するために、再帰的に呼び出します -->
        <NavMenuItem MenuItems="menuItem.Children" />
      </ChildContent>
    </SubMenu>
  }
  else
  {
    <!-- menuItem.Path はメニューアイテムのURLパスを表します -->
    <MenuItem RouterLink="@menuItem.Path" Key=@menuItem.Key>
      <Icon Type=@(menuItem.Icon?.ToString()) Theme="outline" />
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

### `AntBlazor.Pro`を使用する場合

標準テンプレート(Pro)にはオブジェクトからメニューを生成する機能が含まれていますが、これらの定義の生成を以下のように簡略化することができます。

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

## メニューの表示

`MainLayout.razor` にメニューを表示するコンポーネントを追加します。

```razor title="MainLayout.razor"
<!-- 省略 -->
<Menu Theme="MenuTheme.Dark" DefaultSelectedKeys=@(new[]{"1"}) Mode="MenuMode.Inline">
    <NavMenuItem MenuItems="WebPaths.MenuItem"/>
</Menu>
<!-- 省略 -->
```

## 実行結果

<img src="../image/sample-antblazor.gif" style="width:400px;">


## 注意事項

この例ではSample1のようなサブメニューを持つ要素にはリンクが指定されていません。
これはAntBlazorがサブメニューを持つ要素にリンクを設定するAPIを用意していないためです。

## ソースコード
実装例は [Example.AntBlazor.Standard](https://github.com/arika0093/BlazorPathHelper/examples/Example.AntBlazor.Standard/) および [Example.AntBlazor.Pro](https://github.com/arika0093/BlazorPathHelper/examples/Example.AntBlazor.Pro/) で利用できます。
