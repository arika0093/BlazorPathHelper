# Automatic Installation (Beta)
You can easily install BlazorPathHelper into a specified project by running the following commands:

```bash
dotnet tool install --global BlazorPathHelper.Migration --prerelease
bph-migration
# Or
# dotnet new tool-manifest
# dotnet tool install BlazorPathHelper.Migration --prerelease
# dotnet bph-migration
```

This tool performs the following tasks:

* Searches for `*.csproj` files under the execution directory and displays options for selection.
* Installs the latest version of `BlazorPathHelper` into the selected project.
* Searches for `.razor` files under the project directory, reads the values of `@page` attributes, and generates a `WebPaths.cs` file.
    * Currently, only `.razor` files are targeted.
* (Optional) Replaces the values of `@page` attributes with the generated `const string` variables.
    * This makes URL management easier.

!!! warning "Caution!"

    This tool references and modifies existing code.
    Be sure to create a backup using git or another version control system before running it.
    (The tool will also display a warning if there are uncommitted changes in git.)
