using BlazorPathHelper;
// ReSharper disable MemberCanBePrivate.Global

namespace Example.Plain;

[BlazorPath]
public partial class WebPaths
{
    [BlazorPathItem(Name="Home", Icon="bi-house-door-fill")]
    public const string Home = "/";
    [BlazorPathItem(Name="Sample1", Icon="bi-1-circle-fill")]
    public const string Sample1 = "/sample1";
    [BlazorPathItem(Name="Sample1C1", Icon="bi-1-square")]
    public const string Sample1C1 = $"{Sample1}/child1";
    [BlazorPathItem(Name="Sample1C2", Icon="bi-2-square")]
    public const string Sample1C2 = $"{Sample1}/child2";
    [BlazorPathItem(Name="Sample1C2C1", Icon="bi-bag-plus")]
    public const string Sample1C2C1 = $"{Sample1}/child2/child1";
    [BlazorPathItem(Name="Sample2", Icon="bi-2-circle-fill")]
    public const string Sample2 = "/sample2";
    [BlazorPathItem(Name="Sample2C1", Icon="bi-1-square")]
    public const string Sample2C1 = $"{Sample2}/child1";
    [BlazorPathItem(Name="Sample3", Icon="bi-3-circle-fill")]
    public const string Sample3 = "/sample3";
    // will be hidden on menu
    public const string Sample3Arg = "/sample3/{value:int}";
}