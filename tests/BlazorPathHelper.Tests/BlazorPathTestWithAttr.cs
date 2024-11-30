// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable MemberCanBePrivate.Global
using System.Globalization;
using System.Linq;
using System.Threading;
using Xunit;
using FluentAssertions;

namespace BlazorPathHelper.Tests;

[BlazorPath]
internal partial class DefinitionWithAttr
{
    // ルートページはメニューに表示しない
    [BlazorPathItem(Visible = false)]
    public const string Index = "/";

    // ページ名とアイコンを指定する
    [BlazorPathItem("サンプルA", Icon = "fas fa-cog")]
    public const string Sample = "/sample";
    [BlazorPathItem("サンプルA-1", Icon = "fas fa-star")]
    public const string SampleChild = "/sample/child";

    // URL的に繋がりがないが子要素として認識させたい場合は、Groupを指定
    [BlazorPathItem("サンプルA-2", Group = Sample)]
    public const string SampleChild2 = "/sample2";

    // どことも繋がっていない奥深くの階層を最上位メニューに表示したい場合は、Group = "/" を指定
    [BlazorPathItem("サンプルB", Group = Index)]
    public const string SuperInnerItem = "/hoge/fuga/piyo";

    // 上記のように指定しておけば、その子ページもメニューに表示される
    [BlazorPathItem("サンプルBの子")]
    public const string SuperInnerItemChild = "/hoge/fuga/piyo/child";

    // ページ名と説明文を用意する
    [BlazorPathItem("サンプルC", "サンプルCの説明文")]
    public const string SampleC = "/sample-c";
}

public class DefinitionWithAttrTest
{
    [Fact]
    public void PathLocalizeJaTest()
    {
        const string lang = "ja";
        Thread.CurrentThread.CurrentCulture = new CultureInfo(lang);
        Thread.CurrentThread.CurrentUICulture = new CultureInfo(lang);
        var sampleMenu = DefinitionWithAttr.MenuItem.First();
        sampleMenu.Name.Should().Be("サンプルページ");
    }
    
    [Fact]
    public void PathLocalizeEnTest()
    {
        const string lang = "en";
        Thread.CurrentThread.CurrentCulture = new CultureInfo(lang);
        Thread.CurrentThread.CurrentUICulture = new CultureInfo(lang);
        var sampleMenu = DefinitionWithAttr.MenuItem.First();
        sampleMenu.Name.Should().Be("Sample Page");
    }
}