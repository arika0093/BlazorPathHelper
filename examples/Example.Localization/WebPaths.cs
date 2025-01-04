using BlazorPathHelper;
using Example.Localization.Resource;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable ClassNeverInstantiated.Global

namespace Example.Localization;

[BlazorPath]
public partial class WebPaths
{
    [Item(nameof(Localize.Home),
        Description = nameof(Localize.HomePage),
        Icon = "bi-house-door-fill")]
    public const string Home = "/";

    [Item(nameof(Localize.Sample1),
        Description = nameof(Localize.Sample1Page),
        Icon = "bi-1-circle-fill")]
    public const string Sample1 = "/sample1";

    [Item(nameof(Localize.Sample1C1),
        Description = nameof(Localize.Sample1Child1),
        Icon = "bi-1-square")]
    public const string Sample1C1 = $"{Sample1}/child1";

    [Item(nameof(Localize.Sample1C2),
        Description = nameof(Localize.Sample1Child2),
        Icon = "bi-2-square")]
    public const string Sample1C2 = $"{Sample1}/child2";

    [Item(nameof(Localize.Sample1C2C1),
        Description = nameof(Localize.Sample1Child2Sub1),
        Icon = "bi-bag-plus")]
    public const string Sample1C2C1 = $"{Sample1}/child2/child1";
}
