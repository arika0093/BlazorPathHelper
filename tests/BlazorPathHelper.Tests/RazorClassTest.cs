using System.Linq;
using FluentAssertions;
using Xunit;
// ReSharper disable ReturnValueOfPureMethodIsNotUsed

namespace BlazorPathHelper.Tests;

public partial class PageSample1;
public partial class PageSample2;
public partial class PageSample3;
public partial class PageSample4;
public partial class PageSample5;
public record class PageQuery3
{
    public string? QueryValue1 { get; set; }
}

public class TemplateBase { }
public class AnotherTemplateBase { }

[BlazorPath(PageTemplate = typeof(TemplateBase))]
public partial class PageSampleWebPaths
{
    [Item("Sample1"), Page<PageSample1>]
    public const string Sample1 = "/sample1";
    [Item("Sample2"), Page<PageSample2>]
    public const string Sample2 = "/sample2/{val}";
    [Item("Sample3"), Page<PageSample3>, Query<PageQuery3>]
    public const string Sample3 = "/sample3/{val1:int}/{val2:long}";
    [Item("Sample4"), Page<PageSample4>(Inherits = typeof(AnotherTemplateBase))]
    public const string Sample4 = "/sample4";
    [Item("Sample5"), Page<PageSample5>(Inherits = null)]
    public const string Sample5 = "/sample5";
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
        // check the inherits directive
        var inheritsDirective = typeof(PageSample1).BaseType;
        inheritsDirective.Should().Be(typeof(TemplateBase));
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
            .First(p => p.Name == "Val");
        var exist = parameter.GetCustomAttributesData()
            .Any(attr => attr.AttributeType.Name == "ParameterAttribute");
        parameter.PropertyType.Should().Be(typeof(string));
        // check the inherits directive
        var inheritsDirective = typeof(PageSample2).BaseType;
        inheritsDirective.Should().Be(typeof(TemplateBase));
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
        parameters[0].Name.Should().Be("Val1");
        parameters[0].PropertyType.Should().Be(typeof(int));
        parameters[0].GetCustomAttributesData().Any(attr => attr.AttributeType.Name == "ParameterAttribute");
        parameters[1].Name.Should().Be("Val2");
        parameters[1].PropertyType.Should().Be(typeof(long));
        parameters[1].GetCustomAttributesData().Any(attr => attr.AttributeType.Name == "ParameterAttribute");
        // check the query
        parameters[2].Name.Should().Be("QueryValue1");
        parameters[2].PropertyType.Should().Be(typeof(string));
        parameters[2].GetCustomAttributesData().Any(attr => attr.AttributeType.Name == "SupplyParameterFromQuery");
        // check the inherits directive
        var inheritsDirective = typeof(PageSample3).BaseType;
        inheritsDirective.Should().Be(typeof(TemplateBase));
    }

    [Fact]
    public void Sample4()
    {
        PageSampleWebPaths.Helper.Sample4()
            .Should().Be("/sample4");
        // check the attribute
        var attribute = typeof(PageSample4).GetCustomAttributesData()
            .First(attr => attr.AttributeType.Name == "RouteAttribute");
        attribute.ConstructorArguments[0].Value.Should().Be(PageSampleWebPaths.Sample4);
        // check the inherits directive
        var inheritsDirective = typeof(PageSample4).BaseType;
        inheritsDirective.Should().Be(typeof(AnotherTemplateBase));
    }

    [Fact]
    public void Sample5()
    {
        PageSampleWebPaths.Helper.Sample5()
            .Should().Be("/sample5");
        // check the attribute
        var attribute = typeof(PageSample5).GetCustomAttributesData()
            .First(attr => attr.AttributeType.Name == "RouteAttribute");
        attribute.ConstructorArguments[0].Value.Should().Be(PageSampleWebPaths.Sample5);
        // check the inherits directive
        var inheritsDirective = typeof(PageSample5).BaseType;
        inheritsDirective.Should().Be(typeof(object));
    }
}
