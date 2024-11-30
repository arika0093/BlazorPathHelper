// ReSharper disable MemberCanBePrivate.Global


namespace BlazorPathHelper.Try;

[BlazorPath]
public partial class WebPaths
{
    [BlazorPathItem("sample")]
    public const string Sample1 = "/sample1";
    [BlazorPathItem(nameof(Localize.Sample), nameof(Localize.Sample))]
    public const string Sample2 = "/sample2";
    
}