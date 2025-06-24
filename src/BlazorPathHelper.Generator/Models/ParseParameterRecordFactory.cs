using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace BlazorPathHelper.Models;

/// <summary>
/// Factory for creating ParseParameterRecord.
/// </summary>
internal static class ParseParameterRecordFactory
{
    // https://regex101.com/r/HcMv3Z/1
    private const string SampleExtractArgsPattern = @"{([^\/\\:?]+)(:([^\/\\:?]+))?(\?)?}";

    /// <summary>
    /// create BuilderArgumentInfo list from path.
    /// </summary>
    public static IEnumerable<ParseParameterRecord> CreateFromPath(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            throw new ArgumentException("Path cannot be null or empty", nameof(path));
        }
        var pathRegex = new Regex(SampleExtractArgsPattern);
        var matches = pathRegex.Matches(path);
        foreach (Match match in matches)
        {
            yield return CreateFromRegexMatch(match);
        }
    }

    /// <summary>
    /// create BuilderArgumentInfo from regex match.
    /// </summary>
    private static ParseParameterRecord CreateFromRegexMatch(Match match)
    {
        // {value:int} -> value
        var variable = match.Groups[1].Value;
        if (string.IsNullOrEmpty(variable))
        {
            throw new ArgumentException("Parameter name cannot be empty");
        }
        // {value:int} -> int
        var typeString = match.Groups.Count > 3 ? match.Groups[3].Value : "";
        // {value:int?} -> true
        var isNullable = match.Groups.Count > 4 && match.Groups[4].Value == "?";
        return new ParseParameterRecord()
        {
            VariableName = variable.TrimStart('*'),
            Type = ConvertType(typeString),
            IsNullable = isNullable,
            IsCatchAll = variable[0] == '*',
        };
    }

    private static string ConvertType(string type)
    {
        // ReSharper disable once StringLiteralTypo
        return type switch
        {
            "datetime" => nameof(DateTime),
            "guid" => nameof(Guid),
            "nonfile" => "string",
            "" => "string",
            null => "string",
            _ => type,
        };
    }
}
