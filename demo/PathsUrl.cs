using BlazorPathHelper.Demo.Pages;

namespace BlazorPathHelper.Demo;

[BlazorPath]
public partial class PathsUrl
{
    [Page<WithParam>]
    public const string WithParam = "/with-param/{value:int}";
    [Page<WithQuery>, Query<WithQuery.QueryParameters>]
    public const string WithQuery = "/with-query";
}
