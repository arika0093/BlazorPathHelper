using FluentAssertions;
using Xunit;

namespace BlazorPathHelper.Tests;

[BlazorPath(PathBaseValue = PathBase)]
internal partial class DefinitionWithPathBase1
{
    [Item(Ignore = true)]
    internal const string PathBase = "/example/"; // with trailing slash

    public const string Home = "/";
    public const string Sample1 = "/sample1";
    public const string Sample2 = "/sample2/{value:int}";
}

[BlazorPath(PathBaseValue = PathBase)]
internal partial class DefinitionWithPathBase2
{
    [Item(Ignore = true)]
    internal const string PathBase = "/example"; // without trailing slash

    public const string Home = "/";
    public const string Sample1 = "/sample1";
    public const string Sample2 = "/sample2/{value:int}";
}

public class PathTestWithPathBase
{
    [Fact]
    public void TestPathBase1()
    {
        DefinitionWithPathBase1.Helper.Home().Should().Be($"/example");
        DefinitionWithPathBase1.Helper.Sample1().Should().Be($"/example/sample1");
        DefinitionWithPathBase1.Helper.Sample2(123).Should().Be($"/example/sample2/123");
    }

    [Fact]
    public void TestPathBase2()
    {
        DefinitionWithPathBase2.Helper.Home().Should().Be($"/example");
        DefinitionWithPathBase2.Helper.Sample1().Should().Be($"/example/sample1");
        DefinitionWithPathBase2.Helper.Sample2(123).Should().Be($"/example/sample2/123");
    }
}
