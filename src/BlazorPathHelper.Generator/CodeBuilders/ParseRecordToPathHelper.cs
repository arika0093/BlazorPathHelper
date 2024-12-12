using System.Collections.Generic;
using System.Linq;
using BlazorPathHelper.Models;

namespace BlazorPathHelper.CodeBuilders;

internal class ParseRecordToPathHelper(ParseRecord record)
{
    /// <summary>
    /// build path helper function.
    /// </summary>
    public IEnumerable<string> BuildPathHelpers()
    {
        // return default definition
        return record.IsRequireArgs
            ? BuildPathHelperWithArguments()
            : BuildPathHelperWithoutArguments();
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
        // e.g. "int val1, int val2"
        var builderArgs = string.Join(", ", record.Arguments.Select(a => a.ArgDefinition));
        // e.g. "val1, val2"
        var builderVals = string.Join(", ", record.Arguments.Select(a => a.VariableString));
        
        yield return $"/// <summary>Build Path String: {record.PathRawValue} </summary>";
        yield return $"public static string {record.VariableName}({builderArgs}) => string.Format(\"{record.PathFormatterBase}\", {builderVals});";
    }
}