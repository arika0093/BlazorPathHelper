using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace BlazorPathHelper.Models;

internal static class BuilderArgumentInfoFactory
{
    // https://regex101.com/r/HcMv3Z/1
    private const string SampleExtractArgsPattern = @"{([^\/\\:?]+)(:([^\/\\:?]+))?(\?)?}";

    public static IEnumerable<BuilderArgumentInfo> Parse(string path)
    {
        var pathRegex = new Regex(SampleExtractArgsPattern);
        var matches = pathRegex.Matches(path);
        foreach (Match match in matches)
        {
            // {value:int} -> value
            var variable = match.Groups[1].Value;
            // {value:int} -> int
            var typeString = match.Groups.Count > 3 ? match.Groups[3].Value : "";
            // {value:int?} -> true
            var isNullable = match.Groups.Count > 4 && match.Groups[4].Value == "?";
            var definition = new BuilderArgumentInfo()
            {
                VariableName = variable,
                Type = typeString,
                IsNullable = isNullable
            };
            yield return definition;
        }
    }
}

/// <summary>
/// argument information for builder method.
/// </summary>
/// <remarks>
/// e.g. path = "/sample/{value:int?}" so, VariableName = "value", Type = "int", IsNullable = true
/// </remarks>
internal record BuilderArgumentInfo
{
    public required string VariableName { get; init; }
    public required string Type { get; init; }
    public required bool IsNullable { get; init; }
    
    public string ArgDefinition => $"{Type}{NullChar} {VariableName}{(IsNullable ? " = null" : "")}";
    private string NullChar => IsNullable ? "?" : "";
    public string VariableString => (Type == nameof(DateTime))
        ? $"{VariableName}.ToString(\"yyyy-MM-ddTHH:mm:ss\")"
        : VariableName;
}
