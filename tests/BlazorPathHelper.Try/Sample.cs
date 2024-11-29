namespace BlazorPathHelper.Try;

[BlazorPath]
internal partial class Test
{
    public const string TopPage = "/";
    // sample 
    public const string SampleTop = "/sample";
    public const string SampleSub1 = $"{SampleTop}/sub1";
    public const string SampleSub1C1 = $"{SampleSub1}/child1";
    public const string SampleSub2 = $"{SampleTop}/sub2";
    public const string SampleSub2C1 = $"{SampleSub2}/child1";
    public const string SampleSub2C2 = $"{SampleSub2}/child2";
    public const string SampleSub3 = $"{SampleTop}/sub3";
    // ignore menu parameter
    [BlazorPathItem(Name = "test")]
    public const string SampleSub4 = $"{SampleTop}/sub4";
    // multiple root item
    public const string Sample2Top = "/sample2";
    public const string Sample2Sub1 = $"{Sample2Top}/sub1";
    public const string Sample3Top = "/sample3";
}