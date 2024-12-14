using BlazorPathHelper.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BlazorPathHelper.Models;

// information for parsing query string.
internal record ParseQueryRecord
{
    /// <summary>
    /// name of url parameter. ?q=1 -> q
    /// </summary>
    public required string UrlName { get; init; }

    /// <summary>
    /// type of property or field.
    /// </summary>
    public required ITypeSymbol Type { get; init; }

    /// <summary>
    /// name of property or field. string.Format("?q={0}", Name) -> "Name"
    /// </summary>
    public required string Name {get; init;}

    /// <summary>
    /// initial value of property or field.
    /// </summary>
    public required EqualsValueClauseSyntax? InitialValue {get; init; }

    /// <summary>
    /// is nullable or not.
    /// </summary>
    public required bool IsNullable {get; init;}

    /// <summary>
    /// has initializer or not.
    /// e.g. public string Name { get; set; } = "default"; -> true
    /// e.g. public string Name { get; set; } -> false
    /// </summary>
    public bool HasInitializer => InitialValue != null;

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
