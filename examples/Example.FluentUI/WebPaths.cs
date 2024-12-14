using BlazorPathHelper;
using static Microsoft.FluentUI.AspNetCore.Components.Icons.Regular.Size20;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable ClassNeverInstantiated.Global

namespace Example.FluentUI;

[BlazorPath]
public partial class WebPaths
{
    // you need to pass the icon element in the form of Generics because FluentUI Icons require object creation.
    // ---------------------------------
    [Item<Home>("Home")]
    public const string Home = "/";
    [Item<TextHeader1>("Sample1")]
    public const string Sample1 = "/sample1";
    [Item<AddCircle>("Sample1C1")]
    public const string Sample1C1 = $"{Sample1}/child1";
    [Item<AddCircle>("Sample1C2")]
    public const string Sample1C2 = $"{Sample1}/child2";
    [Item<CheckmarkCircle>("Sample1C2C1")]
    public const string Sample1C2C1 = $"{Sample1}/child2/child1";
    [Item<TextHeader2>("Sample2")]
    public const string Sample2 = "/sample2";
    [Item<Star>("Sample2C1")]
    public const string Sample2C1 = $"{Sample2}/child1";
    [Item<TextHeader3>("Sample3")]
    public const string Sample3 = "/sample3";
    
    public const string Sample3Arg = "/sample3/{value:int}";
}