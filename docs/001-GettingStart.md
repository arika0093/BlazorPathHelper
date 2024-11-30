## GettingStart

### 導入
Blazorプロジェクト内に `BlazorPathHelper` をインストールします。
```
$ dotnet add BlazorPathHelper
```

### 下準備
`WebPaths.cs` ファイルを作成します。
このファイルにプログラム内で使用するパスをまとめて定義します。
最後に `[BlazorPath]` 属性を付与しておきます。

```csharp
using BlazorPathHelper;

[BlazorPath]
public partial class WebPaths // partialを忘れずに付与する
{
    public const string Index = "/";
    public const string Sample = "/sample";
    public const string SampleChild = "/sample/child";
    public const string Counter = "/counter";
    public const string CounterWithState = "/counter/{count:int}";
}
```

すると、以下のようなクラス定義が自動で生成されます。

```csharp
// 元々のクラスに追記されます
public partial class WebPaths
{
    // URLビルドを行う際に便利なヘルパー関数
    public partial class Helper
    {
        public static string Index() => "/";
        public static string Sample() => "/sample";
        public static string SampleChild() => "/sample/child";
        public static string Counter() => "/counter";
        public static string CounterWithState(int count) => string.Format("/counter/{0}", count);
    }
    
    // メニュー項目を動的に作成するのに便利な配列
    public static readonly BlazorMenuItem[] MenuItems = [
        new BlazorPathMenuItem(){
            Name = "Index",  // メニュー名。標準では変数名
            Path = "/",      // ページのパス
            Children = []    // 子メニュー
            // その他メニューを生成するのに便利なプロパティを自動生成
        },
        new BlazorPathMenuItem(){
            Name = "Sample",
            Path = "/sample", 
            // URLを解析して、よしなに子メニューを生成する
            Children = [
                new BlazorPathMenuItem(){
                    Name = "SampleChild",
                    Path = "/sample/child", 
                    Children = []
                }
            ]
        },
        new BlazorPathMenuItem(){
            Name = "Counter",
            Path = "/counter",
            // パラメータを持つページはメニューに表示されない
            Children = []
        }
    ];
}
```

最後に、ページ側の定義を `@page` から `@attribute`+`WebPaths` を使うように修正します。  
こうすることで、パス定義を変更した際にページ側の修正を忘れることがなくなります。

```diff
- @page "/counter/{count:int}"
+ @attribute [Route(WebPaths.CounterWithState)]
```

### 使い方
#### URLビルド
以下のように呼び出します。
```razor
@inject NavigationManager NavigationManager
@code {
    private void GoCounter(int value)
    {
        NavigationManager.NavigateTo(
            WebPaths.Helper.CounterWithState(value)
        );
    }
}
```

#### メニュー生成

まずは、`BlazorMenuItem` を元にメニューへ変換するためのコンポーネントを新規作成します。

```razor
@* NavMenuItem.razor *@
@using BlazorPathHelper

@* メニュー項目を再帰的に表示するコンポーネント *@
@* Key属性にMenuItem.Indexを指定することで、再描画時に正しく動作する *@
<div @key="@MenuItem.Index">
    @* メニュー項目 *@
    <NavLink href="@MenuItem.Path" Match="@NavLinkMatch.All">
        @* アイコンとメニュー名 *@
        <span class="@MenuItem.Icon" aria-hidden="true"></span>
        @MenuItem.Name
    </NavLink>

    @* 子メニュー *@
    @foreach(var childMenuItem in MenuItem.Children)
    {
        <NavMenuItem MenuItem="childMenuItem"/>
    }
</div>

@code {
    [Parameter, EditorRequired]
    public BlazorPathMenuItem MenuItem { get; set; } = default!;
}
```


次に、`Paths.MenuItems` を呼び出して上記のコンポーネントを呼び出します。

```razor
@* NavMenu.razor *@
<nav>
    @foreach(var menuItem in Paths.MenuItem)
    {
        <NavMenuItem MenuItem="menuItem"/>
    }
</nav>
```

### メニューのカスタマイズ

`[BlazorPathItem]` 属性を付与することで、メニューの表示をカスタマイズできます。

```csharp
[BlazorPath]
public partial class WebPaths
{
    // メニューに表示しないようにする場合は、Visible = false を指定
    [BlazorPathItem(Visible = false)]
    public const string Index = "/";

    // ページ名とアイコンを指定する
    [BlazorPathItem("サンプルA", Icon = "fas fa-cog")]
    public const string Sample = "/sample";
    [BlazorPathItem("サンプルA-1", Icon = "fas fa-star")]
    public const string SampleChild = "/sample/child";

    // 説明文を追加する場合は、第二引数で指定可能
    [BlazorPathItem("サンプルA-2", "A-2ページの説明文")]
    public const string SampleComplex = "/sample/child2";

    // URL的に繋がりがないが子要素として認識させたい場合は、Groupを指定
    [BlazorPathItem("サンプルA-3", Group = Sample)]
    public const string SampleChild2 = "/sample-3";

    // どことも繋がっていない奥深くの階層を最上位メニューに表示したい場合は、Group = "/" を指定
    [BlazorPathItem("入れ子ページ", Group = Index)]
    public const string SuperInnerItem = "/hoge/fuga/piyo";

    // 上記のように指定しておけば、その子ページもメニューに表示される
    [BlazorPathItem("入れ子ページの子")]
    public const string SuperInnerItemChild = "/hoge/fuga/piyo/child";
}
```

以下のように生成されます。

```csharp
public partial class WebPaths
{
    public static readonly BlazorPathMenuItem[] MenuItem = [
        new BlazorPathMenuItem() {
            Name = "サンプルA", Path = "/sample", Icon = "fas fa-cog",
            Children = [
                new BlazorPathMenuItem() {
                    Name = "サンプルA-1", Path = "/sample/child", Icon = "fas fa-star", 
                },
                new BlazorPathMenuItem() {
                    Name = "サンプルA-2", Path = "/sample/child2",Description = "A-2ページの説明文"
                },
                new BlazorPathMenuItem() {
                    Name = "サンプルA-3", Path = "/sample-3"
                }
            ]
        },
        new BlazorPathMenuItem() {
            Name = "入れ子ページ", Path = "/hoge/fuga/piyo", 
            Children = [
                new BlazorPathMenuItem() {
                    Name = "入れ子ページの子", Path = "/hoge/fuga/piyo/child"
                }
            ]
        }
    ];
}
```

### 多言語対応

`[BlazorPathItem]` を指定する際、以下のように記述します。

```csharp
[BlazorPath]
public partial class WebPaths
{
    [BlazorPathItem(nameof(Localize.SampleA))]
    public const string Sample = "/sample";
}
```

呼び出す際のコードは以下のようになります。

```razor
@inject IStringLocalizer<Localize> Localizer

@* nameof(...)の形で名称を指定した場合HasLocalizeNameがTrueとなるので、それを使う *@
@(MenuItem.HasLocalizeName ? Localizer[MenuItem.Name] : MenuItem.Name)

@code {
    [Parameter, EditorRequired]
    public BlazorPathMenuItem MenuItem { get; set; } = default!;
}
```


### 実装例
