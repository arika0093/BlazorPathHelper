using System.Collections.Generic;
using BlazorPathHelper.Models;

namespace BlazorPathHelper.CodeBuilders;

internal class ParseRecordToRazorCls(ParseRecord record)
{
    public IEnumerable<string> ExportParametersCode()
    {
        foreach (var param in record.Parameters)
        {
            yield return $"/// <summary>{param.VariableName} from \"{record.PathRawValue}\"</summary>";
            yield return $"[Parameter]";
            yield return $"public {param.TypeDefinition} {param.VariableName} {{ get; set; }}";
        }
    }

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