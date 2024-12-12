using System;
using Xunit;
using FluentAssertions;
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable InconsistentNaming

namespace BlazorPathHelper.Tests;

internal class PageSimulateClass
{
    internal record QuerySimulate
    {

    }
}

[BlazorPath]
internal partial class DefinitionWithArgs
{
    // string
    public const string SampleWithString = $"/string/{{val}}";
    // nullable string
    public const string SampleWithStringNullable = $"/string-null/{{val?}}";
    // bool
    public const string SampleWithBool = $"/bool/{{val:bool}}";
    // datetime
    public const string SampleWithDate = $"/datetime/{{val:datetime}}";
    // decimal
    public const string SampleWithDecimal = $"/decimal/{{val:decimal}}";
    // double
    public const string SampleWithDouble = $"/double/{{val:double}}";
    // float
    public const string SampleWithFloat = $"/float/{{val:float}}";
    // guid
    public const string SampleWithGuid = $"/guid/{{val:guid}}";
    // int
    public const string SampleWithInt = $"/int/{{val:int}}";
    // long
    public const string SampleWithLong = $"/long/{{val:long}}";
    // multiple pattern
    public const string SampleWithMultiple = $"/multi/{{val1:int}}/{{val2:int}}";
    // super multiple pattern
    public const string SampleWithSuperMultiple = $"/something/{{val1:string}}/{{val2:int}}/{{val3:double?}}";
    // catch all pattern
    public const string SampleWithCatchAll = $"/catch-all/{{*rest}}";
}

public class BlazorPathTestWithArgs
{
    [Fact]
    public void PathBuildableTest()
    {
        DefinitionWithArgs.Helper.SampleWithString("test")
            .Should().Be("/string/test");
        DefinitionWithArgs.Helper.SampleWithStringNullable("test")
            .Should().Be("/string-null/test");
        DefinitionWithArgs.Helper.SampleWithStringNullable(null)
            .Should().Be("/string-null/");
        DefinitionWithArgs.Helper.SampleWithBool(true)
            .Should().Be("/bool/True");
        DefinitionWithArgs.Helper.SampleWithDate(new DateTime(2021, 1, 1, 0, 0, 0, DateTimeKind.Utc))
            .Should().Be("/datetime/2021-01-01T00:00:00Z");
        DefinitionWithArgs.Helper.SampleWithDecimal(1.1m)
            .Should().Be("/decimal/1.1");
        DefinitionWithArgs.Helper.SampleWithDouble(1.1)
            .Should().Be("/double/1.1");
        DefinitionWithArgs.Helper.SampleWithFloat(1.1f)
            .Should().Be("/float/1.1");
        DefinitionWithArgs.Helper.SampleWithGuid(new Guid("00000000-0000-0000-0000-000000000000"))
            .Should().Be("/guid/00000000-0000-0000-0000-000000000000");
        DefinitionWithArgs.Helper.SampleWithInt(1)
            .Should().Be("/int/1");
        DefinitionWithArgs.Helper.SampleWithLong(1)
            .Should().Be("/long/1");
        DefinitionWithArgs.Helper.SampleWithMultiple(1, 2)
            .Should().Be("/multi/1/2");
        DefinitionWithArgs.Helper.SampleWithSuperMultiple("test", 2, 3.3)
            .Should().Be("/something/test/2/3.3");
        DefinitionWithArgs.Helper.SampleWithSuperMultiple("test", 2)
            .Should().Be("/something/test/2/");
        DefinitionWithArgs.Helper.SampleWithCatchAll("test/1/2/3")
            .Should().Be("/catch-all/test/1/2/3");
    }


}