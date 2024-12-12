namespace BlazorPathHelper.Try;

internal class PageSimulateClass
{
    internal record QuerySimulate
    {

    }
}

[BlazorPath]
public partial class WebPaths
{
    public const string Sample = "/sample";
    public const string Sample2 = "/sample2";
    public const string Sample3 = "/sample3";
    public const string BuildPathTest = "/sample/{value:int}";
    [BlazorPathItem(Query = typeof(string))]
    public const string BuildPathQuery1 = "/query-test1/{test:int}";
    [BlazorPathItem(Query = typeof(PageSimulateClass.QuerySimulate))]
    public const string BuildPathQuery2 = "/query-test2/{test:int}";
}