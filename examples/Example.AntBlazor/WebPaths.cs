using BlazorPathHelper;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable ClassNeverInstantiated.Global

namespace Example.AntBlazor;

[BlazorPath]
public partial class WebPaths
{
    [BlazorPathItem("Home(Pro)", Icon = "home")]
    public const string Home = "/";
    [BlazorPathItem("Home(Standard)", Icon = "folder")]
    public const string Standard = "/standard";
    [BlazorPathItem("Sample1", Icon = "folder")]
    public const string Sample1 = "/sample1";
    [BlazorPathItem("Sample1C1", Icon = "file")]
    public const string Sample1C1 = $"{Sample1}/child1";
    [BlazorPathItem("Sample1C2", Icon = "folder")]
    public const string Sample1C2 = $"{Sample1}/child2";
    [BlazorPathItem("Sample1C2C1", Icon = "file")]
    public const string Sample1C2C1 = $"{Sample1}/child2/child1";
    [BlazorPathItem("Sample2", Icon = "folder")]
    public const string Sample2 = "/sample2";
    [BlazorPathItem("Sample2C1", Icon = "file")]
    public const string Sample2C1 = $"{Sample2}/child1";
    [BlazorPathItem("Sample3", Icon = "file")]
    public const string Sample3 = "/sample3";
    
    public const string Sample3Arg = "/sample3/{value:int}";
}