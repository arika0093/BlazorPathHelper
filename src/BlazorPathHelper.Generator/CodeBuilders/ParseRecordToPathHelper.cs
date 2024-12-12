using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using BlazorPathHelper.Models;
using Microsoft.CodeAnalysis;

namespace BlazorPathHelper.CodeBuilders;

internal class ParseRecordToPathHelper(ParseRecord record)
{
    /// <summary>
    /// build path helper function.
    /// </summary>
    public IEnumerable<string> BuildPathHelpers()
    {
        // return default definition
        var baseFunction = record.IsRequireArgs
            ? BuildPathHelperWithArguments()
            : BuildPathHelperWithoutArguments();
        // return query definition
        var queryFunction = BuildPathHelperWithQuery();

        // concat collections
        return baseFunction.Concat(queryFunction);
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
        yield return $"public static string {record.VariableName}({GetBuilderArgs()}) => string.Format(\"{record.PathFormatterBase}\", {GetBuilderVals()});";
    }

    // e.g. public static string Sample(int val1, int val2, int val3) => string.Format("/sample/{0}/{1}?val3={2}", val1, val2, val3);
    private IEnumerable<string> BuildPathHelperWithQuery()
    {
        string queryUrl = "";
        // build query string
        var queryType = record.QueryTypeSymbol;
        if (queryType == null && queryType?.DeclaredAccessibility != Accessibility.Public)
        {
            // nothing
            yield break;
        }
        // extract contain members
        var members = queryType.GetMembers()
            .OfType<IPropertySymbol>()
            .Where(m => m.IsDefinition && m is { IsReadOnly: false, DeclaredAccessibility: Accessibility.Public })
            .ToArray();
        if(members.Length == 0)
        {
            // nothing
            yield break;
        }

        var membersRecord = members.Select(m => new QueryMembersRecord()
        {
            Name = m.Name,
            TypeName = m.Type.ToDisplayString(),
            IsNullable = m.NullableAnnotation == NullableAnnotation.Annotated,
            IsRequired = m.IsRequired
        }).ToList();

        // make query string: ?param1={2}&param2={3}&...
        // index will be start from Arguments count
        var defaultArgCount = record.Arguments.Count;
        var memberQueryString = "?" + string.Join("&", membersRecord.Select(m => $"{m.Name}={{{defaultArgCount++}}}"));

        var queryArg = $"{queryType}? query = null";
        var queryVals = string.Join(",", membersRecord.Select(m => $"Uri.EscapeDataString(query)"));

        yield return $"/// <summary>Build Path String with Query: {record.PathRawValue} </summary>";
        yield return $"public static string {record.VariableName}({GetBuilderArgs()}, {queryArg})";
        yield return $"    => string.Format(\"{record.PathFormatterBase + memberQueryString}\", {GetBuilderVals()}, {queryVals});";
    }

    record QueryMembersRecord()
    {
        public required string Name { get; init; }
        public required string TypeName { get; init; }
        public required bool IsRequired { get; init; }
        public required bool IsNullable { get; init; }
    }

    // e.g. "int val1, int val2"
    private string GetBuilderVals() => string.Join(", ", record.Arguments.Select(a => a.VariableString));

    // e.g. "val1, val2"
    private string GetBuilderArgs() => string.Join(", ", record.Arguments.Select(a => a.ArgDefinition));
}