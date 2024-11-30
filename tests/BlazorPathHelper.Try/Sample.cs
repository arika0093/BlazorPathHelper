// ReSharper disable MemberCanBePrivate.Global

namespace BlazorPathHelper.Try;

[BlazorPath]
public partial class WebPaths
{
    // ルートページはメニューに表示しない
    [BlazorPathItem(Visible = false)] public const string Index = "/";

    // ページ名とアイコンを指定する
    [BlazorPathItem(Name = "サンプルA", Icon = "fas fa-cog")]
    public const string Sample = "/sample";

    [BlazorPathItem(Name = "サンプルA-1", Icon = "fas fa-star")]
    public const string SampleChild = "/sample/child";

    // URL的に繋がりがないが子要素として認識させたい場合は、GroupKeyを指定
    [BlazorPathItem(Name = "サンプルA-2", Group = Sample)]
    public const string SampleChild2 = "/sample2";

    // どことも繋がっていない奥深くの階層をメニューに表示したい場合は、RootForceを指定
    [BlazorPathItem(Name = "入れ子ページ", RootForce = true)]
    public const string SuperInnerItem = "/hoge/fuga/piyo";

    // 上記のように指定しておけば、その子ページもメニューに表示される
    [BlazorPathItem(Name = "入れ子ページの子")] public const string SuperInnerItemChild = "/hoge/fuga/piyo/child";

    public static readonly BlazorPathMenuItem[] MenuItems =
    [
        new BlazorPathMenuItem()
        {
            Index = -1, GroupKey = "", GroupIndex = 0, GroupLevel = 0, Name = "Index", Path = "/", Icon = "",
            Children = []
        },
        new BlazorPathMenuItem()
        {
            Index = 0, GroupKey = "", GroupIndex = 1, GroupLevel = 0, Name = "サンプルA", Path = "/sample",
            Icon = "fas fa-cog",
            Children =
            [
                new BlazorPathMenuItem()
                {
                    Index = 1, GroupKey = "/sample", GroupIndex = 0, GroupLevel = 1, Name = "サンプルA-1",
                    Path = "/sample/child", Icon = "fas fa-star", Children = []
                },
                new BlazorPathMenuItem()
                {
                    Index = 2, GroupKey = "/sample", GroupIndex = 1, GroupLevel = 1, Name = "サンプルA-2",
                    Path = "/sample2", Icon = "", Children = []
                }
            ]
        },
        new BlazorPathMenuItem()
        {
            Index = 3, GroupKey = "/hoge/fuga", GroupIndex = 2, GroupLevel = 0, Name = "入れ子ページ",
            Path = "/hoge/fuga/piyo", Icon = "",
            Children =
            [
                new BlazorPathMenuItem()
                {
                    Index = 4, GroupKey = "/hoge/fuga/piyo", GroupIndex = 0, GroupLevel = 1, Name = "入れ子ページの子",
                    Path = "/hoge/fuga/piyo/child", Icon = "", Children = []
                }
            ]
        }
    ];
}