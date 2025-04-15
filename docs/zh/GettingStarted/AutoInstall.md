# 自动导入 (测试版)
通过执行以下命令，可以将 BlazorPathHelper 简单地导入到指定的项目中。

```bash
dotnet tool install --global BlazorPathHelper.Migration --prerelease
bph-migration
# 或者
# dotnet new tool-manifest
# dotnet tool install BlazorPathHelper.Migration --prerelease
# dotnet bph-migration
```

此工具将执行以下操作：

* 搜索当前目录下的 `*.csproj` 文件，并显示选择列表。
    * 使用 Space 键选择，Enter 键确认。
* 在选定的项目中安装最新版本的 `BlazorPathHelper`。
* 搜索项目下的 `.razor` 文件，并根据 `@page` 属性的值生成 `WebPaths.cs` 文件。
    * 当前仅支持 `.razor` 文件。
* （可选）将 `@page` 属性的值替换为生成的 `const string` 变量。
    * 这样可以更方便地管理 URL。
* （可选）根据 `#!csharp [SupplyParameterFromQuery]` 属性，添加 `#!csharp [Query]` 属性。
    * 这样可以生成查询参数。
    * 仅读取 `.razor` 文件中定义的值。

!!! tip "当存在多个 @page 属性时"

    如果存在多个 `@page` 属性，将向用户显示选择列表。

!!! warning "注意！"

    此工具会参考并部分修改现有代码。
    使用前请务必通过 git 等工具生成备份。
    （工具在检测到 git 中存在未提交的更改时会显示警告）
