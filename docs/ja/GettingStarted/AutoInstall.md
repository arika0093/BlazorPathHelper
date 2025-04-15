# 自動導入 (ベータ版)
以下のコマンドを実行することで、BlazorPathHelperを指定したプロジェクトに簡易導入できます。

```bash
dotnet tool install --global BlazorPathHelper.Migration --prerelease
bph-migration
# または
# dotnet new tool-manifest
# dotnet tool install BlazorPathHelper.Migration --prerelease
# dotnet bph-migration
```

このツールは以下のことを行います。

* 実行ディレクトリの下に存在する `*.csproj`ファイルを検索し、選択肢を表示します。
    * Spaceキーで選択、Enterキーで決定します。
* 選択したプロジェクトに、最新版の`BlazorPathHelper`をインストールします。
* プロジェクト配下の`.razor`ファイルを検索し、`@page`属性の値を元に`WebPaths.cs`ファイルを生成します。
    * 現在`.razor`ファイルのみを対象としています。
* (オプション) `@page`属性の値を生成した`const string`の変数に置き換えます。
    * これによりURLの管理が容易になります。
* (オプション) `#!csharp [SupplyParameterFromQuery]`属性を元に、`#!csharp [Query]`属性を追加します。
    * これにより、クエリパラメータを生成できるようになります。
    * `.razor`ファイルに記載されている値のみを読み取ります。

!!! tip "複数の@page属性がある場合"

    `@page`属性が複数存在する場合、ユーザーに選択肢を表示します。

!!! warning "注意！"

    このツールは既存のコードを参照し、一部書き換えます。
    使用する場合は、必ずgit等でバックアップを生成してから実行してください。
    (ツール上でもgitの変更が存在する場合には警告を表示します)
