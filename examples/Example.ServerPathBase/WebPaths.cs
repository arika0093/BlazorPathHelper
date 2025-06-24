using BlazorPathHelper;
using Example.ServerPathBase.Components.Pages;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable ClassNeverInstantiated.Global

namespace Example.ServerPathBase;

[BlazorPath(PathBaseValue = PathBase)]
public partial class WebPaths
{
    [Item(Ignore = true)]
    internal const string PathBase = "/example/";

    // Native Blazor Template does not include icons.
    // In this example, Bootstrap Icons are added to App.razor as follows:
    // <link href="https://cdn.jsdelivr.net/npm/bootstrap-icons/font/bootstrap-icons.css" rel="stylesheet">
    // ---------------------------------
    [Item("Home", Icon = "bi-house-door-fill")]
    [Page<Home>]
    public const string Home = "/";

    [Item("Sample1", Icon = "bi-1-circle-fill")]
    [Page<Sample1>]
    public const string Sample1 = "/sample1";

    [Item("Sample2", Icon = "bi-2-circle-fill")]
    [Page<Sample2>]
    public const string Sample2 = "/sample2";

    [Item("Sample3", Icon = "bi-3-circle-fill")]
    [Page<Sample3>]
    public const string Sample3 = "/sample3";

    [Page<Sample3Value>]
    public const string Sample3Arg = "/sample3/{value:int}";
}
