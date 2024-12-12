namespace BlazorPathHelper.Try;

public class PageSimulateClass
{
}
public record QuerySimulate
{
    public int Test1Required { get; set; }
    public int? Test2Nullable { get; set; }
    public int Test3HasDefault { get; set; } = 3;
}

[BlazorPath]
public partial class WebPaths
{
    [BlazorPathItem("tests"), BlazorPathQuery<PageSimulateClass, QuerySimulate>]
    public const string BuildPathQuery2 = "/query-test/3/{val:int}";
}