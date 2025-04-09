using System.Collections.Generic;
using System.Linq;
using BlazorPathHelper.Models;

namespace BlazorPathHelper.CodeBuilders;

internal class ParseRecordToPathHelper(ParseRecord record)
{
    private const string QueryVarName = "__query";

    /// <summary>
    /// build path helper function.
    /// </summary>
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
        yield return $"public static string {record.VariableName}() => \"{record.PathRawValue}\";";
    }

    // e.g. public static string Sample(int val1, int val2) => string.Format("/sample/{0}/{1}", val1, val2);
    private IEnumerable<string> BuildPathHelperWithArguments()
    {
        yield return $"/// <summary>Build Path String: {record.PathRawValue} </summary>";
        yield return $"public static string {record.VariableName}({GetBuilderArgs()})";
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
        yield return $"public static string {record.VariableName}({GetBuilderArgs(queryArg)})";
        yield return $"    => string.Format(\"{record.PathFormatterBase + memberQueryString}\", {GetBuilderVals(queryValue)});";
    }

    // e.g. "int val1, int val2"
    private string GetBuilderVals(string[]? optionals = null)
        => string.Join(", ", record.Parameters.Select(a => a.VariableString).Concat(optionals ?? []));

    // e.g. "val1, val2"
    private string GetBuilderArgs(string[]? optionals = null)
        => string.Join(", ", record.Parameters.Select(a => a.ArgDefinition).Concat(optionals ?? []));
}