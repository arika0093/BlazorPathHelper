using System.Linq;
using FluentAssertions;
using Microsoft.AspNetCore.Components;
using Xunit;
// ReSharper disable InconsistentNaming

namespace BlazorPathHelper.Tests;

// normal pattern
public record Query1(string test1, int test2);
// optional pattern
public record Query2(string? test1 = null, int? test2 = null);
// array pattern
public record Query3(string[] tests);
// alternative name pattern
public record Query4
{
    [SupplyParameterFromQuery(Name = "short")]
    public required string CustomTest { get; set; }
}
// field pattern
public record Query5
{
    public required string fieldTest;
}
// ------------------------------------------------

[BlazorPath]
internal partial class DefinitionForQuery
{
    [BlazorPathQuery<Query1>]
    public const string QueryTest1 = "/query-test/1";
    [BlazorPathQuery<Query2>]
    public const string QueryTest2 = "/query-test/2";
    [BlazorPathQuery<Query3>]
    public const string QueryTest3 = "/query-test/3/{val:int}";
    [BlazorPathQuery<Query4>]
    public const string QueryTest4 = "/query-test/4";
    [BlazorPathQuery<Query5>]
    public const string QueryTest5 = "/query-test/5";
}

public class QueryTest
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
        DefinitionForQuery.Helper.QueryTest2(new(test1: "test"))
            .Should().Be("/query-test/2?test1=test");
        DefinitionForQuery.Helper.QueryTest2(new(test2: 5))
            .Should().Be("/query-test/2?test2=5");
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
        DefinitionForQuery.Helper.QueryTest3(1, new(["hello"]))
            .Should().Be("/query-test/3/1?tests=hello");
        DefinitionForQuery.Helper.QueryTest3(2, new(["hello", "world"]))
            .Should().Be("/query-test/3/2?tests=hello&tests=world");
    }

    [Fact]
    public void QueryTest4()
    {
        DefinitionForQuery.Helper.QueryTest4()
            .Should().Be("/query-test/4");
        DefinitionForQuery.Helper.QueryTest4(new() { CustomTest = "hello" })
            .Should().Be("/query-test/4?short=hello");
    }

    [Fact]
    public void QueryTest5()
    {
        DefinitionForQuery.Helper.QueryTest5()
            .Should().Be("/query-test/5");
        DefinitionForQuery.Helper.QueryTest5(new() { fieldTest = "field" })
            .Should().Be("/query-test/5?fieldTest=field");
    }
}