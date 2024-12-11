namespace BlazorPathHelper.Try;

public class Icon
{
    public string Name { get; init; } = string.Empty;
}

[BlazorPath]
public partial class WebPaths
{
    public const string Sample = "/sample";
    [BlazorPathItem(Icon = typeof(Icon))]
    public const string Sample2 = "/sample2";
    public const string Sample3 = "/sample3";
    public const string BuildPathTest = "/sample/{value:int}";
    public const string BuildPathTest2 = "/sample/{value:int}/test";
}