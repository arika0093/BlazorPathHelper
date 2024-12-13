using Microsoft.AspNetCore.Components;

namespace BlazorPathHelper.Try;

public partial class PageSimulateClass
{
}
public record QuerySimulate
{
    public required int Test1Required { get; set; }
    public int? Test2Nullable { get; set; }
    public int Test3HasDefault { get; set; } = 3;
}

[BlazorPath]
public partial class WebPaths
{
    [BlazorPathItem("tests", Page = typeof(PageSimulateClass)), BlazorPathQuery<QuerySimulate>]
    public const string BuildPathQuery = "/query-test/{val:int}";
}