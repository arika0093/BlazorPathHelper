using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using BlazorPathHelper.Utils;
using Microsoft.CodeAnalysis;

namespace BlazorPathHelper.Models;

/// <summary>
/// stored information for path builder and menu generation.
/// </summary>
internal record ParseRecord
{
    /// <summary>
    /// create instance from BlazorPathAttribute.
    /// </summary>
    /// <param name="rootSymbol">symbol of class with BlazorPathAttribute</param>
    public static IEnumerable<ParseRecord> GenerateRecordsFromPathAttr(INamedTypeSymbol rootSymbol)
    {
        return rootSymbol.GetMembers()
            .OfType<IFieldSymbol>()
            // extract "const string" field with BlazorPathItemAttribute
            .Where(f => f.IsConst && f.Type.SpecialType == SpecialType.System_String)
            // and create instance from BlazorPathItemAttribute
            .Select(f => GenerateRecordFromPathAttr(rootSymbol, f));
    }

    /// <summary>
    /// create instance from BlazorPathAttribute and BlazorPathItemAttribute.
    /// </summary>
    /// <param name="rootSymbol">symbol of class with BlazorPathAttribute</param>
    /// <param name="pathItemSymbol">symbol of field/parameter with BlazorPathItemAttribute</param>
    public static ParseRecord GenerateRecordFromPathAttr(
        INamedTypeSymbol rootSymbol,
        IFieldSymbol pathItemSymbol
    )
    {
        // get member value of BlazorPathAttribute
        var rootAttribute = rootSymbol.GetAttributes()
            .First(a => a.AttributeClass?.Name == nameof(BlazorPathAttribute));
        var rootFileName = rootSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
            .Replace("global::", "")
            .Replace("<", "_")
            .Replace(">", "_");
        var rootNamespace = rootAttribute.ToStringFromKey(nameof(BlazorPathAttribute.Namespace));
        var rootClassName = rootAttribute.ToStringFromKey(nameof(BlazorPathAttribute.ClassName));

        // get member value of BlazorPathItemAttribute
        var pathItemAttr = pathItemSymbol.GetAttributes()
            .FirstOrDefault(a => a.AttributeClass?.Name == nameof(BlazorPathItemAttribute));
        var itemVisible = pathItemAttr?.ToStringFromKey(nameof(BlazorPathItemAttribute.Visible));
        var itemName = pathItemAttr?.ToStringFromKey(nameof(BlazorPathItemAttribute.Name));
        var itemDescription = pathItemAttr?.ToStringFromKey(nameof(BlazorPathItemAttribute.Description));
        var itemGroup = pathItemAttr?.ToStringFromKey(nameof(BlazorPathItemAttribute.Group));

        return new ParseRecord()
        {
            BaseFileName = rootFileName,
            Namespace = rootNamespace ?? rootSymbol.ContainingNamespace.ToDisplayString(),
            AccessModifier = rootSymbol.DeclaredAccessibility.ToString().ToLower(),
            ExportClassName = rootClassName ?? rootSymbol.Name,
            VariableName = pathItemSymbol.Name,
            PathRawValue = (string)pathItemSymbol.ConstantValue!,
            IsDisplay = (itemVisible != "false"),
            DisplayName = itemName ?? pathItemSymbol.Name,
            DisplayDescription = itemDescription,
            _groupPathCache = itemGroup ?? "",
        };
    }

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
    /// get list of BuilderArgumentInfo for path builder.
    /// </summary>
    public List<BuilderArgumentInfo> Arguments => _argumentsCache ??= ExtractArgumentInfos().ToList();

    private List<BuilderArgumentInfo>? _argumentsCache;

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
    /// grouping path. default: parent directory of path.
    /// </summary>
    public string GroupPath
    {
        get => _groupPathCache ??= BuildDefaultGroupPath();
        init => _groupPathCache = value.TrimEnd('/');
    }

    private string? _groupPathCache;

    /// <summary>
    /// get string for string.Format. e.g. /sample/{0}/{1}
    /// </summary>
    public string PathFormattableBase
    {
        get
        {
            // replace e.g. {val1}/{val2} -> {0}/{1}   
            var count = 0;
            return Regex.Replace(PathRawValue, @"{[^}]+}", (_) => $"{{{count++}}}");
        }
    }

    /// <summary>
    /// get is home menu path or not.
    /// </summary>
    public bool IsHome => PathRawValue == "/";

    /// <summary>
    /// get is root menu path or not.
    /// </summary>
    public bool IsRoot => GroupPath == "";

    /// <summary>
    /// build path helper function.
    /// </summary>
    public IEnumerable<string> BuildPathHelpers()
    {
        // return default definition
        yield return IsRequireArgs
            ? BuildPathHelperWithArguments()
            : BuildPathHelperWithoutArguments();
    }

    // e.g. public static string Sample() => "/sample";
    private string BuildPathHelperWithoutArguments()
    {
        return $"""
                /// <summary>Build Path String: {PathRawValue} </summary>
                public static string {VariableName}() => "{PathRawValue}";
                """;
    }

    // e.g. public static string Sample(int val1, int val2) => string.Format("/sample/{0}/{1}", val1, val2);
    private string BuildPathHelperWithArguments()
    {
        // e.g. "int val1, int val2"
        var builderArgs = string.Join(", ", Arguments.Select(a => a.ArgDefinition));
        // e.g. "val1, val2"
        var builderVals = string.Join(", ", Arguments.Select(a => a.VariableString));
        // e.g. "/// <param name="val1">int</param>\n ..."
        var builderArgsComments = string.Join("\n",
            Arguments.Select(a => $"/// <param name=\"{a.VariableName}\">{a.Type}</param>"));
        return $"""
                /// <summary>Build Path String: {PathRawValue} </summary>
                {builderArgsComments}
                public static string {VariableName}({builderArgs}) => string.Format("{PathFormattableBase}", {builderVals});
                """;
    }

    // is there any arguments?    
    private bool IsRequireArgs => Arguments.Count > 0;

    // pick up arugments information from PathRawValue.
    private IEnumerable<BuilderArgumentInfo> ExtractArgumentInfos() => BuilderArgumentInfoFactory.Parse(PathRawValue);

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