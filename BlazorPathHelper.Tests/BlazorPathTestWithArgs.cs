using System;
using Xunit;
using FluentAssertions;

namespace BlazorPathHelper.Tests;

public class BlazorPathTestWithArgs
{
    [BlazorPath]
    public partial class DefinitionWithArgs
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
    }

    [Fact]
    public void PathBuildableTest()
    {
        PathHelper.SampleWithString("test")
            .Should().Be("/string/test");
        PathHelper.SampleWithStringNullable("test")
            .Should().Be("/string-null/test");
        PathHelper.SampleWithStringNullable(null)
            .Should().Be("/string-null/");
        PathHelper.SampleWithBool(true)
            .Should().Be("/bool/True");
        PathHelper.SampleWithDate(new DateTime(2021, 1, 1))
            .Should().Be("/datetime/2021-01-01T00:00:00");
        PathHelper.SampleWithDecimal(1.1m)
            .Should().Be("/decimal/1.1");
        PathHelper.SampleWithDouble(1.1)
            .Should().Be("/double/1.1");
        PathHelper.SampleWithFloat(1.1f)
            .Should().Be("/float/1.1");
        PathHelper.SampleWithGuid(new Guid("00000000-0000-0000-0000-000000000000"))
            .Should().Be("/guid/00000000-0000-0000-0000-000000000000");
        PathHelper.SampleWithInt(1)
            .Should().Be("/int/1");
        PathHelper.SampleWithLong(1)
            .Should().Be("/long/1");
        PathHelper.SampleWithMultiple(1, 2)
            .Should().Be("/multi/1/2");
        PathHelper.SampleWithSuperMultiple("test", 2, 3.3)
            .Should().Be("/something/test/2/3.3");
        PathHelper.SampleWithSuperMultiple("test", 2)
            .Should().Be("/something/test/2/");
    }
    
    
}