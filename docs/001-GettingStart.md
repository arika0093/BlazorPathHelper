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
            Index = 0,       // メニュー全体のNo。@key属性に使用すると便利
            GroupKey = "",   // グループのキー。同じグループキーのメニューはグループ化される
            GroupIndex = 0,  // グループ内のNo
            GroupLevel = 0,  // グループの階層
            Name = "Index",  // メニュー名。標準では変数名
            Path = "/",      // ページのパス
            Icon = "",       // アイコン指定。主にclass名を指定して、MenuItemコンポーネントで表示する
            Children = []    // 子メニュー
        },
        new BlazorPathMenuItem(){
            Index = 1,
            GroupKey = "",
            GroupIndex = 1,
            GroupLevel = 0,
            Name = "Sample",
            Path = "/sample", 
            Icon = "",
            // URLを解析して、よしなに子メニューを生成する
            Children = [
                new BlazorPathMenuItem(){
                    Index = 2,
                    GroupKey = "/sample", 
                    GroupIndex = 0,
                    GroupLevel = 1, 
                    Name = "SampleChild",
                    Path = "/sample/child", 
                    Icon = "", 
                    Children = []
                }
            ]
        },
        new BlazorPathMenuItem(){
            Index = 3,
            GroupKey = "",
            GroupIndex = 2,
            GroupLevel = 0,
            Name = "Counter",
            Path = "/counter",
            Icon = "",
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
    // ルートページはメニューに表示しない
    [BlazorPathItem(Visible = false)]
    public const string Index = "/";

    // ページ名とアイコンを指定する
    [BlazorPathItem(Name = "サンプルA", Icon = "fas fa-cog")]
    public const string Sample = "/sample";
    [BlazorPathItem(Name = "サンプルA-1", Icon = "fas fa-star")]
    public const string SampleChild = "/sample/child";

    // URL的に繋がりがないが子要素として認識させたい場合は、Groupを指定
    [BlazorPathItem(Name = "サンプルA-2", Group = Sample)]
    public const string SampleChild2 = "/sample2";

    // どことも繋がっていない奥深くの階層を最上位メニューに表示したい場合は、RootForceを指定
    [BlazorPathItem(Name = "入れ子ページ", RootForce = true)]
    public const string SuperInnerItem = "/hoge/fuga/piyo";

    // 上記のように指定しておけば、その子ページもメニューに表示される
    [BlazorPathItem(Name = "入れ子ページの子")]
    public const string SuperInnerItemChild = "/hoge/fuga/piyo/child";
}
```

以下のように生成されます。

```csharp
public partial class WebPaths
{
    public static readonly BlazorPathMenuItem[] MenuItem = [
        new BlazorPathMenuItem() {
            Index = 0, GroupKey = "", GroupIndex = 1, GroupLevel = 0,
            Name = "サンプルA", Path = "/sample", Icon = "fas fa-cog",
            Children = [
                new BlazorPathMenuItem() {
                    Index = 1, GroupKey = "/sample", GroupIndex = 0, GroupLevel = 1,
                    Name = "サンプルA-1", Path = "/sample/child", Icon = "fas fa-star", Children = []
                },
                new BlazorPathMenuItem() {
                    Index = 2, GroupKey = "/sample", GroupIndex = 1, GroupLevel = 1,
                    Name = "サンプルA-2", Path = "/sample2", Icon = "", Children = []
                }
            ]
        },
        new BlazorPathMenuItem() {
            Index = 3, GroupKey = "/hoge/fuga", GroupIndex = 2, GroupLevel = 0,
            Name = "入れ子ページ", Path = "/hoge/fuga/piyo", Icon = "",
            Children = [
                new BlazorPathMenuItem() {
                    Index = 4, GroupKey = "/hoge/fuga/piyo", GroupIndex = 0, GroupLevel = 1,
                    Name = "入れ子ページの子", Path = "/hoge/fuga/piyo/child", Icon = "", Children = []
                }
            ]
        }
    ];
}
```

### 多言語対応

`[BlazorPathItem]` 属性の `Name`を指定する際、以下のように記述することでリソースファイルから動的取得が可能です。

```csharp
[BlazorPath]
public partial class WebPaths
{
    [BlazorPathItem(Name = "@Localize:SampleA")]
    public const string Sample = "/sample";
}
```


### 実装例
