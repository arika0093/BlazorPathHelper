// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable MemberCanBePrivate.Global
using FluentAssertions;
using Xunit;

namespace BlazorPathHelper.Tests;

[BlazorPath]
internal partial class DefinitionBase
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
    [BlazorPathItem(Visible = false)]
    public const string SampleSub4 = $"{SampleTop}/sub4";
    // be able customize menu item
    [BlazorPathItem(Name = "test5")]
    public const string SampleSub5 = $"{SampleTop}/sub5";
    // multiple root item
    public const string Sample2Top = "/sample2";
    public const string Sample2Sub1 = $"{Sample2Top}/sub1";
    public const string Sample3Top = "/sample3";
    // ... and specify root item
    [BlazorPathItem(RootForce = true)]
    public const string Sample4ForceRoot = "/sample4/hoge";
    public const string Sample4Child = $"{Sample4ForceRoot}/fuga";
}

public class BlazorPathTest
{
    [Fact]
    public void PathRoutingTest()
    {
        DefinitionBase.Helper.TopPage().Should().Be(DefinitionBase.TopPage);
        DefinitionBase.Helper.SampleTop().Should().Be(DefinitionBase.SampleTop);
        DefinitionBase.Helper.SampleSub1().Should().Be(DefinitionBase.SampleSub1);
        DefinitionBase.Helper.SampleSub1C1().Should().Be(DefinitionBase.SampleSub1C1);
        DefinitionBase.Helper.SampleSub2().Should().Be(DefinitionBase.SampleSub2);
        DefinitionBase.Helper.SampleSub2C1().Should().Be(DefinitionBase.SampleSub2C1);
        DefinitionBase.Helper.SampleSub2C2().Should().Be(DefinitionBase.SampleSub2C2);
        DefinitionBase.Helper.SampleSub3().Should().Be(DefinitionBase.SampleSub3);
        // be able to build path even if it is not visible
        DefinitionBase.Helper.SampleSub4().Should().Be(DefinitionBase.SampleSub4);
    }
    
    [Fact]
    public void MenuStructureTest()
    {
        var menuStructure = DefinitionBase.MenuItem;
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
        //     Path = "/sample/sub3", Children = [],
        //     Path = "/sample/sub5", Name="test5", Children = []
        //   ],
        //   Path = "/sample2", Children = [
        //     Path = "/sample2/sub1", Children = []
        //   ],
        //   Path = "/sample3", Children = [],
        //   Path = "/sample4/hoge", Children = [
        //     Path = "/sample4/hoge/fuga", Children = []
        //   ]
        // ];
        menuStructure.Should().HaveCount(5);
        menuStructure[0].Path.Should().Be(DefinitionBase.TopPage);
        menuStructure[0].Name.Should().Be(nameof(DefinitionBase.TopPage));
        menuStructure[0].Children.Should().BeEmpty();
        menuStructure[1].Path.Should().Be(DefinitionBase.SampleTop);
        menuStructure[1].Name.Should().Be(nameof(DefinitionBase.SampleTop));
        menuStructure[1].Children.Should().HaveCount(4);
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
        menuStructure[1].Children[3].Path.Should().Be(DefinitionBase.SampleSub5);
        menuStructure[1].Children[3].Name.Should().Be("test5");
        menuStructure[1].Children[3].Children.Should().BeEmpty();
        menuStructure[2].Path.Should().Be(DefinitionBase.Sample2Top);
        menuStructure[2].Children.Should().HaveCount(1);
        menuStructure[2].Children[0].Path.Should().Be(DefinitionBase.Sample2Sub1);
        menuStructure[2].Children[0].Children.Should().BeEmpty();
        menuStructure[3].Path.Should().Be(DefinitionBase.Sample3Top);
        menuStructure[3].Children.Should().BeEmpty();
        menuStructure[4].Path.Should().Be(DefinitionBase.Sample4ForceRoot);
        menuStructure[4].Children.Should().HaveCount(1);
        menuStructure[4].Children[0].Path.Should().Be(DefinitionBase.Sample4Child);
        menuStructure[4].Children[0].Children.Should().BeEmpty();
    }

    [Fact]
    public void MenuItemTest()
    {
        var sampleSub2 = DefinitionBase.MenuItem[1].Children[1];
        sampleSub2.Index.Should().Be(1);
        sampleSub2.GroupIndex.Should().Be(1);
        sampleSub2.GroupLevel.Should().Be(1);
        
    }

    [Fact]
    public void MenuStructureIndexTest()
    {
        var menuStructure = DefinitionBase.MenuItem;
        // menuStructure should be 
        // [
        //   Index = 0, GroupIndex = 0, Children = [],
        //   Index = 1, GroupIndex = 1, Children = [
        //     Index = 2, GroupIndex = 0, Children = [
        //       Index = 3, GroupIndex = 0, Children = []
        //     ],
        //     Index = 4, GroupIndex = 1, Children = [
        //       Index = 5, GroupIndex = 0, Children = [],
        //       Index = 6, GroupIndex = 1, Children = []
        //     ],
        //     Index = 7, GroupIndex = 2, Children = [],
        //     Index = 8, GroupIndex = 3, Children = []
        //   ],
        //   Index = 9, GroupIndex = 2, Children = [
        //     Index = 10, GroupIndex = 0, Children = []
        //   ],
        //   Index = 11, GroupIndex = 3, Children = [],
        //   Index = 12, GroupIndex = 4, Children = [
        //     Index = 13, GroupIndex = 0, Children = []
        //   ]
        // ];
        menuStructure[0].Index.Should().Be(0);
        menuStructure[0].GroupIndex.Should().Be(0);
        menuStructure[0].Children.Should().BeEmpty();
        menuStructure[1].Index.Should().Be(1);
        menuStructure[1].GroupIndex.Should().Be(1);
        menuStructure[1].Children[0].Index.Should().Be(2);
        menuStructure[1].Children[0].GroupIndex.Should().Be(0);
        menuStructure[1].Children[0].Children[0].Index.Should().Be(3);
        menuStructure[1].Children[0].Children[0].GroupIndex.Should().Be(0);
        menuStructure[1].Children[1].Index.Should().Be(4);
        menuStructure[1].Children[1].GroupIndex.Should().Be(1);
        menuStructure[1].Children[1].Children[0].Index.Should().Be(5);
        menuStructure[1].Children[1].Children[0].GroupIndex.Should().Be(0);
        menuStructure[1].Children[1].Children[1].Index.Should().Be(6);
        menuStructure[1].Children[1].Children[1].GroupIndex.Should().Be(1);
        menuStructure[1].Children[2].Index.Should().Be(7);
        menuStructure[1].Children[2].GroupIndex.Should().Be(2);
        menuStructure[1].Children[3].Index.Should().Be(8);
        menuStructure[1].Children[3].GroupIndex.Should().Be(3);
        menuStructure[2].Index.Should().Be(9);
        menuStructure[2].GroupIndex.Should().Be(2);
        menuStructure[2].Children[0].Index.Should().Be(10);
        menuStructure[2].Children[0].GroupIndex.Should().Be(0);
        menuStructure[3].Index.Should().Be(11);
        menuStructure[3].GroupIndex.Should().Be(3);
        menuStructure[4].Index.Should().Be(12);
        menuStructure[4].GroupIndex.Should().Be(4);
        menuStructure[4].Children[0].Index.Should().Be(13);
        menuStructure[4].Children[0].GroupIndex.Should().Be(0);
        
        
    }
    
}