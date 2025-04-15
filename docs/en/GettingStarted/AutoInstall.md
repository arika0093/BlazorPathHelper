# Auto Installation (Beta Version)
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

* Searches for `*.csproj` files under the execution directory and displays options.
    * Use the Space key to select and the Enter key to confirm.
* Installs the latest version of `BlazorPathHelper` into the selected project.
* Searches for `.razor` files under the project and generates a `WebPaths.cs` file based on the values of the `@page` attributes.
    * Currently, only `.razor` files are targeted.
* (Optional) Replaces the values of the `@page` attributes with the generated `const string` variables.
    * This makes URL management easier.
* (Optional) Adds the `#!csharp [Query]` attribute based on the `#!csharp [SupplyParameterFromQuery]` attribute.
    * This enables query parameter generation.
    * Only values specified in the `.razor` files are read.

!!! tip "When Multiple @page Attributes Exist"

    If multiple `@page` attributes exist, the tool will display options for the user to select.

!!! warning "Caution!"

    This tool references and modifies existing code.
    Be sure to create a backup using git or other tools before running it.
    (The tool will also display a warning if there are existing git changes.)
