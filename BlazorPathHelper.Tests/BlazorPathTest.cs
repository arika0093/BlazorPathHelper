using Xunit;
using FluentAssertions;

namespace BlazorPathHelper.Tests;

public class BlazorPathTest
{
    [BlazorPath]
    public partial class DefinitionBase
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
        // ignore parameter test
        [BlazorPathItem(Visible = false)]
        public const string SampleSub4 = $"{SampleTop}/sub4";
        // multiple root item
        public const string Sample2Top = "/sample2";
        public const string Sample2Sub1 = $"{Sample2Top}/sub1";
        public const string Sample3Top = "/sample3";
    }

    [Fact]
    public void PathRoutingTest()
    {
        PathHelper.TopPage().Should().Be(DefinitionBase.TopPage);
        PathHelper.SampleTop().Should().Be(DefinitionBase.SampleTop);
        PathHelper.SampleSub1().Should().Be(DefinitionBase.SampleSub1);
        PathHelper.SampleSub1C1().Should().Be(DefinitionBase.SampleSub1C1);
        PathHelper.SampleSub2().Should().Be(DefinitionBase.SampleSub2);
        PathHelper.SampleSub2C1().Should().Be(DefinitionBase.SampleSub2C1);
        PathHelper.SampleSub2C2().Should().Be(DefinitionBase.SampleSub2C2);
        PathHelper.SampleSub3().Should().Be(DefinitionBase.SampleSub3);
        PathHelper.SampleSub4().Should().Be(DefinitionBase.SampleSub4);
    }


    [Fact]
    public void MenuStructureTest()
    {
        var menuStructure = PathHelper.MenuItem.DefinitionBase;
        // menuStructure should be 
        // [
        //   Path = "/", Children = [],
        //   Path = "/sample", Children = [
        //     Path = "/sample/sub1", Children = [
        //       Path = "/sample/sub1/child1", Children = []
        //     ],
        //     Path = "/sample/sub2", Children = [
        //       Path = "/sample/sub2/child1", Children = [],
        //       Path = "/sample/sub2/child2", Children = []
        //     ],
        //     Path = "/sample/sub3", Children = []
        //   ],
        //   Path = "/sample2", Children = [
        //     Path = "/sample2/sub1", Children = []
        //   ],
        //   Path = "/sample3", Children = []
        // ];
        menuStructure.Should().HaveCount(4);
        menuStructure[0].Path.Should().Be(DefinitionBase.TopPage);
        menuStructure[0].Children.Should().BeEmpty();
        menuStructure[1].Path.Should().Be(DefinitionBase.SampleTop);
        menuStructure[1].Children.Should().HaveCount(3);
        menuStructure[1].Children[0].Path.Should().Be(DefinitionBase.SampleSub1);
        menuStructure[1].Children[0].Children.Should().HaveCount(1);
        menuStructure[1].Children[0].Children[0].Path.Should().Be(DefinitionBase.SampleSub1C1);
        menuStructure[1].Children[0].Children[0].Children.Should().BeEmpty();
        menuStructure[1].Children[1].Path.Should().Be(DefinitionBase.SampleSub2);
        menuStructure[1].Children[1].Children.Should().HaveCount(2);
        menuStructure[1].Children[1].Children[0].Path.Should().Be(DefinitionBase.SampleSub2C1);
        menuStructure[1].Children[1].Children[0].Children.Should().BeEmpty();
        menuStructure[1].Children[1].Children[1].Path.Should().Be(DefinitionBase.SampleSub2C2);
        menuStructure[1].Children[1].Children[1].Children.Should().BeEmpty();
        menuStructure[1].Children[2].Path.Should().Be(DefinitionBase.SampleSub3);
        menuStructure[1].Children[2].Children.Should().BeEmpty();
        menuStructure[2].Path.Should().Be(DefinitionBase.Sample2Top);
        menuStructure[2].Children.Should().HaveCount(1);
        menuStructure[2].Children[0].Path.Should().Be(DefinitionBase.Sample2Sub1);
        menuStructure[2].Children[0].Children.Should().BeEmpty();
        menuStructure[3].Path.Should().Be(DefinitionBase.Sample3Top);
        menuStructure[3].Children.Should().BeEmpty();

    }

}