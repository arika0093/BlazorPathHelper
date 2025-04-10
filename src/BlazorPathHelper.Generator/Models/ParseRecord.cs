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
    /// namespace of target class. if null then global namespace.
    /// </summary>
    public required string? Namespace { get; init; }

    /// <summary>
    /// access modifier of export class.
    /// </summary>
    public required string AccessModifier { get; init; }

    /// <summary>
    /// class name of export class.
    /// </summary>
    public required string ExportClassName { get; init; }

    /// <summary>
    /// root path of blazor page (trimmed last '/'). e.g. "/sample"
    /// </summary>
    public required string PathBaseValue
    {
        get => _PathBaseValue.TrimEnd('/');
        init => _PathBaseValue = value;
    }
    private string _PathBaseValue = default!;

    /// <summary>
    /// Raw string of path. contains {value:int} like. contained PathBaseValue.
    /// </summary>
    public required string PathRawValue
    {
        // concat PathBase
        get => GetFullPath(_PathRawValue);
        init => _PathRawValue = value;
    }
    private string _PathRawValue = default!;

    /// <summary>
    /// Raw string of path. contains {value:int} like. not contained PathBaseValue.
    /// </summary>
    public string PathRawValueWithoutPathBase => _PathRawValue;

    /// <summary>
    /// name of variable. used for function name of PathHelper.
    /// </summary>
    public required string VariableName { get; init; }

    /// <summary>
    /// get list of ParameterRecord for path builder.
    /// </summary>
    public required List<ParseParameterRecord> Parameters { get; init; }

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
    /// is display to menu or not.
    /// </summary>
    public bool? ForceDisplayFlag { get; set; }

    /// <summary>
    /// menu item and PathBuilder ignore flag. default: false
    /// </summary>
    public bool IsIgnore { get; set; } = false;

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
    /// Location of the attribute on the Blazor page in the source code.
    /// This is needed to present error information by the analyzer
    /// when multiple components share the same name.
    /// default: null
    /// </summary>
    public Location? PageAttributeLocation { get; init; }

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
    /// is display to menu or not.
    /// default: true on IsRequireArgs/IsExistQuery is false.
    /// you can override this value by ForceDisplayFlag.
    /// </summary>
    public bool IsDisplay => ForceDisplayFlag ?? !(IsRequireArgs || IsExistQuery);

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

    // are there any query?
    public bool IsExistQuery => QueryRecords.Count > 0;

    /// <summary>
    /// Derives the default group path from the raw path.
    /// </summary>
    /// <remarks>
    /// If the raw path contains a variable placeholder (denoted by '{'), the group path is the portion before the first occurrence of '{'.
    /// Otherwise, the group path is determined by removing the last segment of the path (i.e., the parent directory).
    /// </remarks>
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

    /// <summary>
    /// Combines the base path with a provided raw path to produce a properly formatted full path.
    /// </summary>
    /// <param name="rawPath">The raw path to append. It can be empty, a single slash ("/"), or a relative path starting with a slash.</param>
    /// <returns>
    /// The full concatenated path. If no base path is set, returns the raw path; if the raw path is "/" or empty, returns the base path;
    /// otherwise, appends the trimmed raw path to the base path with a single slash separator.
    /// </returns>
    private string GetFullPath(string rawPath)
    {
        // for example, "/" + "/sample" => "/sample"
        if (string.IsNullOrEmpty(PathBaseValue))
        {
            return rawPath;
        }
        // for example, "/sample/" + "/" => "/sample"
        else if (rawPath == "/" || rawPath == "")
        {
            return PathBaseValue;
        }
        // for example, "/sample/" + "/foo/bar" => "/sample/foo/bar"
        else
        {
            var rawPathTrim = rawPath.TrimStart('/');
            return $"{PathBaseValue}/{rawPathTrim}";
        }
    }


}