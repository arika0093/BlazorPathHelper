
![Logo](https://dev-to-uploads.s3.amazonaws.com/uploads/articles/th5xamgrr6se0x5ro4g6.png)

# BlazorPathHelper

<div>
	<a href="https://www.nuget.org/packages/BlazorPathHelper/">
		<img alt="NuGet Version" src="https://img.shields.io/nuget/v/BlazorPathHelper?style=for-the-badge">
	</a>
	<img alt="GitHub License" src="https://img.shields.io/github/license/arika0093/BlazorPathHelper?style=for-the-badge">

</div>

![GitHub Actions Workflow Status](https://img.shields.io/github/actions/workflow/status/arika0093/BlazorPathHelper/test.yaml?branch=main&label=Test) ![GitHub Actions Workflow Status](https://img.shields.io/github/actions/workflow/status/arika0093/BlazorPathHelper/release.yaml?branch=main&label=Release) ![GitHub last commit (branch)](https://img.shields.io/github/last-commit/arika0093/BlazorPathHelper/main)

## 機能

- パス定義からURLビルダーを生成
- パス定義からメニューアイテム一覧を生成
- シンプルな実装方法でメニューを生成可能
- メニュー生成部分を自由にカスタマイズ可能・各種フレームワークに対応

## 使い方例
以下のクラスを作成します。

```csharp
using BlazorPathHelper;

[BlazorPath]
public partial class WebPaths
{
    [BlazorPathItem("ホーム")]
    public const string Index = "/";
    [BlazorPathItem("サンプル1")]
    public const string Sample = "/sample";
    [BlazorPathItem("サンプル1-a")]
    public const string SampleChild = "/sample/child";
    [BlazorPathItem("サンプル2")]
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
            Name = "ホーム",  // メニュー名。標準では変数名
            Path = "/",      // ページのパス
            Children = []    // 子メニュー
            // その他メニューを生成するのに便利なプロパティを自動生成
        },
        new BlazorPathMenuItem(){
            Name = "サンプル1",
            Path = "/sample", 
            // URLを解析して、よしなに子メニューを生成する
            Children = [
                new BlazorPathMenuItem(){
                    Name = "サンプル1-a",
                    Path = "/sample/child", 
                    Children = []
                }
            ]
        },
        new BlazorPathMenuItem(){
            Name = "サンプル2",
            Path = "/counter",
            // パラメータを持つページはメニューに表示されない
            Children = []
        }
    ];
}
```

詳細な利用方法は [ドキュメント](/docs/001-GettingStart.md) を参照してください。

## TODO
- [x] CI/CD設定
- [x] nuget公開
- [x] 最小限のドキュメント作成
- [ ] 各種例を追加
  - [ ] 最小限の例
  - [ ] 多言語化を行った例
  - [ ] Blazorテンプレートを使った例
  - [ ] FluentUI
  - [ ] AntBlazor
  - [ ] MudBlazor
  - [ ] MASA Blazor
- [ ] ReadMeを整備
  - [ ] ドキュメント整備
  - [ ] 英語のドキュメント追加
- [ ] ドキュメントサイト追加
- [ ] コード綺麗にする
