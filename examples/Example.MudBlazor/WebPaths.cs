using BlazorPathHelper;
using static MudBlazor.Icons.Material.Filled;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable ClassNeverInstantiated.Global

namespace Example.MudBlazor;

[BlazorPath]
public partial class WebPaths
{
    [Item("Home", Icon = House)]
    public const string Home = "/";
    [Item("Sample1", Icon = Filter1)]
    public const string Sample1 = "/sample1";
    [Item("Sample1C1", Icon = ExposurePlus1)]
    public const string Sample1C1 = $"{Sample1}/child1";
    [Item("Sample1C2", Icon = ExposurePlus2)]
    public const string Sample1C2 = $"{Sample1}/child2";
    [Item("Sample1C2C1", Icon = StarBorder)]
    public const string Sample1C2C1 = $"{Sample1}/child2/child1";
    [Item("Sample2", Icon = Filter2)]
    public const string Sample2 = "/sample2";
    [Item("Sample2C1", Icon = _1xMobiledata)]
    public const string Sample2C1 = $"{Sample2}/child1";
    [Item("Sample3", Icon = Filter3)]
    public const string Sample3 = "/sample3";

    public const string Sample3Arg = "/sample3/{value:int}" ; 
}
