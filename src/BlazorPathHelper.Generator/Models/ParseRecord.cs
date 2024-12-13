using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;

namespace BlazorPathHelper.Models;

/// <summary>
/// stored information for path builder and menu generation.
/// </summary>
internal record ParseRecord
{
    /// <summary>
    /// base file name of target class. default: inherited file location.
    /// </summary>
    public required string BaseFileName { get; init; }

    /// <summary>
    /// namespace of target class.
    /// </summary>
    public required string Namespace { get; init; }

    /// <summary>
    /// access modifier of export class.
    /// </summary>
    public required string AccessModifier { get; init; }

    /// <summary>
    /// class name of export class.
    /// </summary>
    public required string ExportClassName { get; init; }

    /// <summary>
    /// Raw string of path. contains {value:int} like.
    /// </summary>
    public required string PathRawValue { get; init; }

    /// <summary>
    /// name of variable. used for function name of PathHelper.
    /// </summary>
    public required string VariableName { get; init; }

    /// <summary>
    /// get list of ParameterRecord for path builder.
    /// </summary>
    public required List<ParseParameterRecord> Parameters { get; init; }

    /// <summary>
    /// is display to menu or not.
    /// </summary>
    public required bool IsDisplay { get; init; }

    /// <summary>
    /// display name of path. used for menu item. default: VariableName
    /// </summary>
    public required string DisplayName { get; init; }

    /// <summary>
    /// display description of path. used for menu item. default: null
    /// </summary>
    public required string? DisplayDescription { get; init; }
    
    /// <summary>
    /// icon name of menu item. e.g. string "icon-home" or `new Icon();` constructor.
    /// </summary>
    public required string? Icon { get; init; }

    /// <summary>
    /// icon symbol of menu item. used for menu item. default: null
    /// </summary>
    public ITypeSymbol? IconSymbol { get; init; }

    /// <summary>
    /// query type for use path builder. default: null
    /// </summary>
    public ITypeSymbol? QueryTypeSymbol { get; init; }

    /// <summary>
    /// query record for use path builder. default: null
    /// </summary>
    public List<ParseQueryRecord> QueryRecords { get; init; } = [];

    /// <summary>
    /// blazor page type for path builder. default: null
    /// </summary>
    public ITypeSymbol? PageTypeSymbol { get; init; }

    /// <summary>
    /// grouping path. default: parent directory of path.
    /// </summary>
    public string? GroupPath
    {
        get => _groupPathCache ??= BuildDefaultGroupPath();
        init => _groupPathCache = value?.TrimEnd('/');
    }

    private string? _groupPathCache;

    /// <summary>
    /// get string for string.Format. e.g. /sample/{0}/{1}
    /// </summary>
    public string PathFormatterBase
    {
        get
        {
            // replace e.g. {val1}/{val2} -> {0}/{1}   
            var count = 0;
            return Regex.Replace(PathRawValue, @"{[^}]+}", (_) => $"{{{count++}}}");
        }
    }

    /// <summary>
    /// get is root menu path or not.
    /// </summary>
    public bool IsRoot => GroupPath == "";

    // are there any arguments?    
    public bool IsRequireArgs => Parameters.Count > 0;

    // build path string from PathRawValue
    private string BuildDefaultGroupPath()
    {
        // Basically, the parent directory of the path should be used as the key.
        // However, if there is a variable in the path, the key should be the top part excluding the variable.
        // In other words, if there is a `{`, use the part before it. If there is no `{`, split by `/` and remove the last element.
        // e.g. / -> ""
        // e.g. /sample -> ""
        // e.g. /sample/hoge -> /sample
        // e.g. /sample/{value} -> /sample
        // e.g. /sample/{val1}/{val2} -> /sample        
        if (PathRawValue.Contains('{'))
        {
            return PathRawValue[..PathRawValue.IndexOf('{')];
        }

        var split = PathRawValue.Split('/');
        return string.Join("/", split.Take(split.Length - 1));
    }
}