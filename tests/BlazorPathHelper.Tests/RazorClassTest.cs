using System.Linq;
using FluentAssertions;
using Xunit;
// ReSharper disable ReturnValueOfPureMethodIsNotUsed

namespace BlazorPathHelper.Tests;

public partial class PageSample1;
public partial class PageSample2;
public partial class PageSample3;
public record class PageQuery3
{
    public string? QueryValue1 { get; set; }
}

[BlazorPath]
public partial class PageSampleWebPaths
{
    [BlazorPathItem("Sample1", Page = typeof(PageSample1))]
    public const string Sample1 = "/sample1";
    [BlazorPathItem("Sample2", Page = typeof(PageSample2))]
    public const string Sample2 = "/sample2/{val}";
    [BlazorPathItem("Sample3", Page = typeof(PageSample3)), BlazorPathQuery<PageQuery3>]
    public const string Sample3 = "/sample3/{val1:int}/{val2:long}";
}

public class RazorClassTest
{
    [Fact]
    public void Sample1()
    {
        PageSampleWebPaths.Helper.Sample1() 
            .Should().Be("/sample1");
        // check the attribute
        var attribute = typeof(PageSample1).GetCustomAttributesData()
            .First(attr => attr.AttributeType.Name == "RouteAttribute");
        attribute.ConstructorArguments[0].Value.Should().Be(PageSampleWebPaths.Sample1);
    }

    [Fact]
    public void Sample2()
    {
        PageSampleWebPaths.Helper.Sample2("test")
            .Should().Be("/sample2/test");
        // check the attribute
        var attribute = typeof(PageSample2).GetCustomAttributesData()
            .First(attr => attr.AttributeType.Name == "RouteAttribute");
        attribute.ConstructorArguments[0].Value.Should().Be(PageSampleWebPaths.Sample2);
        // check the parameter
        var parameter = typeof(PageSample2).GetProperties()
            .First(p => p.Name == "val");
        var exist = parameter.GetCustomAttributesData()
            .Any(attr => attr.AttributeType.Name == "ParameterAttribute");
        parameter.PropertyType.Should().Be(typeof(string));
    }

    [Fact]
    public void Sample3()
    {
        PageSampleWebPaths.Helper.Sample3(1, 2, new PageQuery3 { QueryValue1 = "test" })
            .Should().Be("/sample3/1/2?QueryValue1=test");
        // check the attribute
        var attribute = typeof(PageSample3).GetCustomAttributesData()
            .First(attr => attr.AttributeType.Name == "RouteAttribute");
        attribute.ConstructorArguments[0].Value.Should().Be(PageSampleWebPaths.Sample3);
        // check the parameter
        var parameters = typeof(PageSample3).GetProperties();
        parameters[0].Name.Should().Be("val1");
        parameters[0].PropertyType.Should().Be(typeof(int));
        parameters[0].GetCustomAttributesData().Any(attr => attr.AttributeType.Name == "ParameterAttribute");
        parameters[1].Name.Should().Be("val2");
        parameters[1].PropertyType.Should().Be(typeof(long));
        parameters[1].GetCustomAttributesData().Any(attr => attr.AttributeType.Name == "ParameterAttribute");
        // check the query
        parameters[2].Name.Should().Be("QueryValue1");
        parameters[2].PropertyType.Should().Be(typeof(string));
        parameters[2].GetCustomAttributesData().Any(attr => attr.AttributeType.Name == "SupplyParameterFromQuery");
    }
}