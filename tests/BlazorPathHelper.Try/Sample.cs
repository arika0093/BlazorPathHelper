namespace BlazorPathHelper.Try;

internal class PageSimulateClass
{
    internal record QuerySimulate
    {
        internal int Test1Required { get; set; }
        internal int? Test2Nullable { get; set; }
        internal int Test3HasDefault { get; set; } = 3;
    }
}

[BlazorPath]
public partial class WebPaths
{
    // test
    public const string Sample = "/sample1";
    public const string Sample2 = "/sample2";
    public const string Sample3 = "/sample3";
    public const string BuildPathTest = "/sample/{value:int}";
    //[BlazorPathItem(Query = typeof(PageSimulateClass.QuerySimulate))]
    public const string BuildPathQuery1 = "/query-test";
    //[BlazorPathItem(Query = typeof(PageSimulateClass.QuerySimulate))]
    public const string BuildPathQuery2 = "/query-test/3/{val:int}";
}