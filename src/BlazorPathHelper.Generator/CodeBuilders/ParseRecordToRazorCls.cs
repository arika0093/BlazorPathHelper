using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using BlazorPathHelper.Models;
using Microsoft.CodeAnalysis;

namespace BlazorPathHelper.CodeBuilders;

internal class ParseRecordToRazorCls(ParseRecord record, ImmutableArray<ParseRazorStructure> structures)
{
    // symbol of the page class
    public ITypeSymbol PageType => record.PageTypeSymbol!;

    // e.g. "Index" from "Pages/Index.razor"
    public string PageClassName => PageType.ToDisplayParts().LastOrDefault().ToString();

    // e.g. "Pages.Index" from "Pages/Index.razor"
    private string PageFullClassName => PageType.ToDisplayString();

    // export namespace code
    public string ExportNamespaceCode()
    {
        // If the PageTypeSymbol is thought not to exist in the source code
        // In this case, it is assumed that the information is obtained from the Razor side
        // If it does not exist there either, it is probably a typo, so the generation is omitted
        var exportNamespace = "";
        var syntaxReferences = PageType.DeclaringSyntaxReferences;
        if (syntaxReferences.Length == 0)
        {
            // => so namespace is not found in the source code
            // search for the namespace from the Razor side
            // first. search by FullClassName or PartialClassName
            var searchRazorInfo = structures
                .Where(s => s.FullClassName == PageFullClassName || s.PartialClassName == PageFullClassName)
                .ToList();
            if (searchRazorInfo.Count == 0)
            {
                // second. search by PageClassName
                searchRazorInfo = structures.Where(s => s.PageClassName == PageClassName).ToList();
            }
            switch (searchRazorInfo.Count())
            {
                case >= 2:
                    // namespace is ambiguous.
                    // ex. Pages.Index and Pages.Admin.Index
                    // error show: must replace 'Index' -> 'Pages.Index' or 'Pages.Admin.Index'
                    var detectItems = searchRazorInfo
                        .Select(v => v.FullClassName.Replace(v.DefaultNamespace, "").Trim('.'))
                        .ToArray();
                    throw new NamespaceAmbiguousException(PageFullClassName, detectItems);
                case 1:
                    // exact match
                    exportNamespace = $"namespace {searchRazorInfo.First().Namespace};";
                    break;
            }
        }
        else
        {
            var containedNamespace = PageType?.ContainingNamespace;
            if (containedNamespace?.IsGlobalNamespace != true)
            {
                exportNamespace = $"namespace {containedNamespace};";
            }
        }
        return exportNamespace;
    }

    // export parameter properties
    public IEnumerable<string> ExportParametersCode()
    {
        foreach (var param in record.Parameters)
        {
            yield return $"/// <summary>{param.VariableName} from \"{record.PathRawValue}\"</summary>";
            yield return $"[Parameter]";
            yield return $"public {param.TypeDefinition} {param.VariableName} {{ get; set; }}";
        }
    }

    // export query properties
    public IEnumerable<string> ExportQueryCode()
    {
        foreach (var query in record.QueryRecords)
        {
            var initValue = query.InitialValue is null ? "" : $" = {query.InitialValue.Value};";
            yield return $"/// <summary>{query.Name} from \"{record.QueryTypeSymbol?.ToDisplayString()}\"</summary>";
            yield return $"[SupplyParameterFromQuery(Name = \"{query.UrlName}\")]";
            yield return $"public {query.Type.ToDisplayString()} {query.PageVariableName} {{ get; set; }}{initValue}";
        }
    }
}

// exception for ambiguous namespace
internal class NamespaceAmbiguousException(string baseClass, string[] replaceStrings)
    : Exception(
        $"You must replace '{baseClass}' -> " +
        $"{string.Join(" or ", replaceStrings.Select(r => $"'{r}'"))}"
    )
{
}