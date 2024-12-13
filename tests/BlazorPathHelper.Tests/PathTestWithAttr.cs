using System.Globalization;
using System.Linq;
using System.Threading;
using BlazorPathHelper.Tests.Resources;
using Xunit;
using FluentAssertions;
// ReSharper disable InconsistentNaming
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable MemberCanBePrivate.Global

namespace BlazorPathHelper.Tests;

internal class IconFromClass;

[BlazorPath]
internal partial class DefinitionWithAttr
{
    // To hide from the menu, set Visible = false
    [BlazorPathItem(Visible = false)]
    public const string Index = "/";

    // Specify page name and icon (CSS class)
    [BlazorPathItem("SampleA", Icon = "fas fa-cog")]
    public const string Sample = "/sample";
    [BlazorPathItem("SampleA-1", Icon = "fas fa-star")]
    public const string SampleChild = "/sample/child";

    // Add descriptions using the second argument
    [BlazorPathItem("SampleA-2", Description = "Description of the A-2 page")]
    public const string SampleWithDesc = "/sample/child2";

    // To group unrelated paths as child elements, specify Group
    [BlazorPathItem("SampleA-3", Group = Sample)]
    public const string SampleChild3 = "/sample-3";

    // To display deeply nested items as top-level menu items, specify Group = "/"
    [BlazorPathItem("SampleB", Group = Index)]
    public const string SuperInnerItem = "/hoge/fuga/piyo";

    // Child pages will also appear in the menu
    [BlazorPathItem("SampleB-1")]
    public const string SuperInnerItemChild = "/hoge/fuga/piyo/child";
    
    // Use the generic type to specify the icon class
    [BlazorPathItem<IconFromClass>("SampleC1")]
    public const string SampleC1 = "/sample-c1";

    // For multilingual support, use nameof to specify resource keys and IStringLocalizer in components
    [BlazorPathItem(nameof(Localize.Sample))]
    public const string SampleLocalize = "/sample-l10n";
    [BlazorPathItem(nameof(Localize.Sample), Description = nameof(Localize.SampleDesc))]
    public const string SampleLocalizeWithDesc = "/sample-l10n-plus";
}

public class PathTestWithAttr
{
    [Fact]
    public void IsVisiblyFalseItemHidden()
    {
        var menuItem = DefinitionWithAttr.MenuItem;
        menuItem.Should().NotContain(item => item.Path == DefinitionWithAttr.Index);
    }
    
    [Fact]
    public void HasIconClass()
    {
        var menuItem = DefinitionWithAttr.MenuItemFlatten;
        var sampleItem = menuItem.First(item => item.Path == DefinitionWithAttr.Sample);
        sampleItem.Icon.Should().Be("fas fa-cog");
    }

    [Fact]
    public void HasName()
    {
        var menuItem = DefinitionWithAttr.MenuItemFlatten;
        var sampleA1Item = menuItem.First(item => item.Path == DefinitionWithAttr.SampleChild);
        sampleA1Item.Name.Should().Be("SampleA-1");
    }

    [Fact]
    public void HasDescription()
    {
        var menuItem = DefinitionWithAttr.MenuItemFlatten;
        var sampleA2Item = menuItem.First(item => item.Path == DefinitionWithAttr.SampleWithDesc);
        sampleA2Item.Description.Should().Be("Description of the A-2 page");
    }
    
    [Fact]
    public void HasGroupedItem()
    {
        var menuItem = DefinitionWithAttr.MenuItemFlatten;
        var sampleA3Item = menuItem.First(item => item.Path == DefinitionWithAttr.SampleChild3);
        sampleA3Item.GroupKey.Should().Be(DefinitionWithAttr.Sample);
    }
    
    [Fact]
    public void HasTopLevelItem()
    {
        var menuItem = DefinitionWithAttr.MenuItemFlatten;
        var sampleBItem = menuItem.First(item => item.Path == DefinitionWithAttr.SuperInnerItem);
        var sampleB1Item = menuItem.First(item => item.Path == DefinitionWithAttr.SuperInnerItemChild);
        sampleBItem.GroupKey.Should().Be("");
        sampleBItem.Children.Should().NotBeEmpty();
        sampleBItem.Children.Should().Contain(sampleB1Item);
    }
    
    [Fact]
    public void HasIconClassFromGenericType()
    {
        var menuItem = DefinitionWithAttr.MenuItemFlatten;
        var sampleC1Item = menuItem.First(item => item.Path == DefinitionWithAttr.SampleC1);
        sampleC1Item.Icon.Should().BeAssignableTo(typeof(IconFromClass));
    }

    [Fact]
    public void HasLocalizedKeyCheck()
    {
        var menuItem = DefinitionWithAttr.MenuItemFlatten;
        var l10n = menuItem.First(item => item.Path == DefinitionWithAttr.SampleLocalize);
        var l10nDesc = menuItem.First(item => item.Path == DefinitionWithAttr.SampleLocalizeWithDesc);
        l10n.Name.Should().Be(nameof(Localize.Sample));
        l10nDesc.Description.Should().Be(nameof(Localize.SampleDesc));
    }
}