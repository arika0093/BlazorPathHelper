using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using BlazorPathHelper.Models;
using Microsoft.CodeAnalysis;

namespace BlazorPathHelper.CodeBuilders;

internal class ParseRecordToPathHelper(ParseRecordForPathHelper record)
{
    private const string QueryVarName = "__query";

    /// <summary>
    /// Generates a collection of C# code snippets for path helper methods based on the current parse record settings.
    /// </summary>
    /// <returns>
    /// An enumerable of strings representing the generated helper methods. Returns an empty collection if the record is marked as ignored; otherwise, returns query-based, argument-based, or argument-less helper methods as determined by the record’s properties.
    /// </returns>
    public IEnumerable<string> BuildPathHelpers()
    {
        if (record.IsIgnore)
        {
            return [];
        }
        if (record.IsExistQuery)
        {
            return BuildPathHelperWithQuery();
        }
        if (record.IsRequireArgs)
        {
            return BuildPathHelperWithArguments();
        }
        return BuildPathHelperWithoutArguments();
    }

    // e.g. public static string Sample() => "/sample";
    private IEnumerable<string> BuildPathHelperWithoutArguments()
    {
        yield return $"/// <summary>Build Path String: {record.PathRawValue} </summary>";
        yield return $"public static string {record.FunctionName}() => \"{record.PathRawValue}\";";
    }

    // e.g. public static string Sample(int val1, int val2) => string.Format("/sample/{0}/{1}", val1, val2);
    private IEnumerable<string> BuildPathHelperWithArguments()
    {
        yield return $"/// <summary>Build Path String: {record.PathRawValue} </summary>";
        yield return $"public static string {record.FunctionName}({GetBuilderArgs()})";
        yield return $"    => string.Format(\"{record.PathFormatterBase}\", {GetBuilderVals()});";
    }

    // e.g. public static string Sample(int val1, int val2, QueryClass query)
    //    => string.Format("/sample/{0}/{1}{2}", val1, val2, query);
    private IEnumerable<string> BuildPathHelperWithQuery()
    {
        if (!record.IsExistQuery)
        {
            yield break;
        }

        var queryType = record.QueryTypeSymbol?.ToDisplayString();
        var membersRecord = record.QueryRecords;
        // make query string placeholder.
        // want to the value of the placeholder+1 in the URL part
        // because pass the entire "?query=..." part to the format function.
        var memberQueryString = $"{{{record.Parameters.Count}}}";

        var isAnyRequired = membersRecord.Any(m => m.IsRequireInitialize);
        var eachQueryVals = membersRecord
            .Select(m => $"ToEscapedStrings(\"{m.UrlName}\", {QueryVarName}.{m.Name})")
            .ToArray();
        string[] queryArg = [$"{queryType} {QueryVarName}"];
        string[] queryValue = [$"BuildQuery([{string.Join(",", eachQueryVals)}])"];

        yield return $"/// <summary>Build Path String with Query: {record.PathRawValue} </summary>";
        yield return $"public static string {record.FunctionName}({GetBuilderArgs(queryArg)})";
        yield return $"    => string.Format(\"{record.PathFormatterBase + memberQueryString}\", {GetBuilderVals(queryValue)});";
    }

    // e.g. "int val1, int val2"
    private string GetBuilderVals(string[]? optionals = null) =>
        string.Join(", ", record.Parameters.Select(a => a.VariableString).Concat(optionals ?? []));

    // e.g. "val1, val2"
    private string GetBuilderArgs(string[]? optionals = null) =>
        string.Join(", ", record.Parameters.Select(a => a.ArgDefinition).Concat(optionals ?? []));
}

internal record ParseRecordForPathHelper
{
    public required bool IsIgnore { get; init; }
    public required string PathRawValue { get; init; }
    public required string FunctionName { get; init; }
    public required List<ParseParameterRecord> Parameters { get; init; }
    public required ITypeSymbol? QueryTypeSymbol { get; init; }
    public required List<ParseQueryRecord> QueryRecords { get; init; }

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

    public bool IsExistQuery => QueryRecords.Count > 0;
    public bool IsRequireArgs => Parameters.Count > 0;
}
