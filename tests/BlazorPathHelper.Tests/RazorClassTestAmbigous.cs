using System.Linq;
using FluentAssertions;
using Xunit;

// check test for issue #20
namespace BlazorPathHelper.Tests
{
    [BlazorPath]
    public partial class AmbiguousWebPaths
    {
        [Page<Foo.Sample>]
        public const string Sample = "/sample";
    }

    public class RazorClassTestAmbiguous
    {
        [Fact]
        public void TestSample()
        {
            // check route attribute (Foo.Sample)
            var attribute = typeof(Foo.Sample)
                .GetCustomAttributesData()
                .First(attr => attr.AttributeType.Name == "RouteAttribute");
            attribute.ConstructorArguments[0].Value.Should().Be(AmbiguousWebPaths.Sample);
            // check route attribute (Foo.Bar.Sample)
            // should not be found
            var attribute2 = typeof(Foo.Bar.Sample)
                .GetCustomAttributesData()
                .FirstOrDefault(attr => attr.AttributeType.Name == "RouteAttribute");
            attribute2.Should().BeNull();
        }
    }
}

// for test
namespace BlazorPathHelper.Tests.Foo
{
    public partial class Sample;
}

namespace BlazorPathHelper.Tests.Foo.Bar
{
    public partial class Sample;
}
