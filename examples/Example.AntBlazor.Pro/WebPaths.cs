using BlazorPathHelper;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable ClassNeverInstantiated.Global

namespace Example.AntBlazor.Pro;

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

    public const string Sample3Arg = "/sample3/{value:int}";
}
