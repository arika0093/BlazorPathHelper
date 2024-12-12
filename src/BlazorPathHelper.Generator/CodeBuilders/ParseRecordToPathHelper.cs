using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using BlazorPathHelper.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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

        var membersRecord = members.Select(m => new QueryMembersRecord(m)).ToList();

        // make query string placeholder. e.g. /foo/bar/{val1}/{val2}?q=... -> /foo/bar/{0}/{1}{2} -> {2}
        var memberQueryString = $"{{{record.Arguments.Count}}}";

        var isAnyRequired = membersRecord.Any(m => m.IsRequireInitialize);
        string[] queryArgs = isAnyRequired ?[$"{queryType} query"] : [$"{queryType}? query = null"];
        var argNullChar = isAnyRequired ? "" : "?";
        var queryTuples = membersRecord.Select(m => $"ToEscapedStrings(\"{m.Name}\", query{argNullChar}.{m.Name})").ToArray();
        string[] queryValue = [$"BuildQuery([{string.Join(",", queryTuples)}])"];

        yield return $"/// <summary>Build Path String with Query: {record.PathRawValue} </summary>";
        yield return $"public static string {record.VariableName}({GetBuilderArgs(queryArgs)})";
        yield return $"    => string.Format(\"{record.PathFormatterBase + memberQueryString}\", {GetBuilderVals(queryValue)});";
    }

    record QueryMembersRecord(IPropertySymbol Symbol)
    {
        public string Name => Symbol.Name;
        public string TypeName => Symbol.Type.ToDisplayString();
        private bool IsRequired => Symbol.IsRequired;
        private bool HasInitializer => Symbol.DeclaringSyntaxReferences.Any(syntaxRef =>
        {
            var syntaxNode = syntaxRef.GetSyntax();
            if (syntaxNode is PropertyDeclarationSyntax propertyDeclaration)
            {
                return propertyDeclaration.Initializer != null;
            }
            return false;
        });
        private bool IsNullable => Symbol.NullableAnnotation == NullableAnnotation.Annotated;
        public bool IsRequireInitialize => (!HasInitializer && !IsNullable);
        public string NullChar => IsNullable ? "?" : "";
    }

    // e.g. "int val1, int val2"
    private string GetBuilderVals(string[]? optionals = null)
        => string.Join(", ", record.Arguments.Select(a => a.VariableString).Concat(optionals ?? []));

    // e.g. "val1, val2"
    private string GetBuilderArgs(string[]? optionals = null)
        => string.Join(", ", record.Arguments.Select(a => a.ArgDefinition).Concat(optionals ?? []));
}