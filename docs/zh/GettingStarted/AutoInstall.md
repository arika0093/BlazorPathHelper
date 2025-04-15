# 自动导入 (测试版)
通过执行以下命令，可以将BlazorPathHelper轻松导入到指定的项目中。

```bash
dotnet tool install --global BlazorPathHelper.Migration --prerelease
# 或者
# dotnet new tool-manifest
# dotnet tool install BlazorPathHelper.Migration --prerelease
dotnet bph-migration
```

此工具将执行以下操作：

* 搜索当前目录下的`*.csproj`文件，并显示选择列表。
* 将最新版的`BlazorPathHelper`安装到选定的项目中。
* 搜索项目下的`.razor`文件，读取`@page`属性的值，并生成`WebPaths.cs`文件。
  * 当前仅支持`.razor`文件。
* （可选）将`@page`属性的值替换为生成的`const string`变量。
  * 这样可以更方便地管理URL。

!!! warning "注意！"

    此工具会参考并部分修改现有代码。
    使用前，请务必通过git等工具备份代码。
    （工具在检测到git有未提交的更改时也会显示警告）
