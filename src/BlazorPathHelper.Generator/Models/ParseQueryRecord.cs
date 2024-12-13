using BlazorPathHelper.Utils;

namespace BlazorPathHelper.Models;

// information for parsing query string.
internal record ParseQueryRecord
{
    /// <summary>
    /// name of url parameter. ?q=1 -> q
    /// </summary>
    public required string UrlName { get; init; }

    /// <summary>
    /// name of property or field. string.Format("?q={0}", Name) -> "Name"
    /// </summary>
    public required string Name {get; init;}

    /// <summary>
    /// has initializer or not.
    /// e.g. public string Name { get; set; } = "default"; -> true
    /// e.g. public string Name { get; set; } -> false
    /// </summary>
    public required bool HasInitializer {get; init;}

    /// <summary>
    /// is nullable or not.
    /// </summary>
    public required bool IsNullable {get; init;}

    /// <summary>
    /// is required initialize or not.
    /// e.g. public string Name { get; set; } = "default"; -> false
    /// e.g. public string Name { get; set; } -> true
    /// e.g. public string? Name { get; set; } -> false
    /// </summary>
    public bool IsRequireInitialize => (!HasInitializer && !IsNullable);

    // check is enable variable name.
    public string PageVariableName => RoslynGeneratorUtilities.GetValidVariableName(Name);
}
