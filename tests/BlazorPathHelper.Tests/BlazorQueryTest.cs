using System.Linq;
using FluentAssertions;
using Xunit;
// ReSharper disable InconsistentNaming

namespace BlazorPathHelper.Tests;

public partial class PageSample
{
    public record Query1(string test1, int test2);
    public record Query2(string? test1 = null, int? test2 = null);
    public record Query3(string[] tests);
}

[BlazorPath]
internal partial class DefinitionForQuery
{
    [BlazorPathQuery<PageSample, PageSample.Query1>]
    public const string QueryTest1 = "/query-test/1";
    [BlazorPathQuery<PageSample, PageSample.Query2>]
    public const string QueryTest2 = "/query-test/2";
    [BlazorPathQuery<PageSample, PageSample.Query3>]
    public const string QueryTest3 = "/query-test/3/{val:int}";
}

public class BlazorQueryTest
{
    [Fact]
    public void QueryTest1()
    {
        DefinitionForQuery.Helper.QueryTest1()
            .Should().Be("/query-test/1");
        DefinitionForQuery.Helper.QueryTest1(new("test", 3))
            .Should().Be("/query-test/1?test1=test&test2=3");
        DefinitionForQuery.Helper.QueryTest1(new("!?", 0))
            .Should().Be("/query-test/1?test1=%21%3F&test2=0");
    }

    [Fact]
    public void QueryTest2()
    {
        DefinitionForQuery.Helper.QueryTest2()
            .Should().Be("/query-test/2");
        DefinitionForQuery.Helper.QueryTest2(new("test", 3))
            .Should().Be("/query-test/2?test1=test&test2=3");
        DefinitionForQuery.Helper.QueryTest2(new("test"))
            .Should().Be("/query-test/2?test1=test");
        DefinitionForQuery.Helper.QueryTest2(new())
            .Should().Be("/query-test/2");
        DefinitionForQuery.Helper.QueryTest2(null)
            .Should().Be("/query-test/2");
    }

    [Fact]
    public void QueryTest3()
    {
        DefinitionForQuery.Helper.QueryTest3(0)
            .Should().Be("/query-test/3/0");
        DefinitionForQuery.Helper.QueryTest3(1,new(["hello"]))
            .Should().Be("/query-test/3/1?tests=hello");
        DefinitionForQuery.Helper.QueryTest3(2,new(["hello", "world"]))
            .Should().Be("/query-test/3/2?tests=hello&tests=world");
    }

}