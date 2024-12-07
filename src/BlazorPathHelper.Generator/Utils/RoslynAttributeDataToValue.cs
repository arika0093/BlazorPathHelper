using System.Linq;
using Microsoft.CodeAnalysis;

namespace BlazorPathHelper.Utils;

internal static class RoslynAttributeDataToValue
{
    public static string? ToStringFromKey(this AttributeData attributeData, string key)
    {
        var typedConsts = attributeData.NamedArguments.Where(arg => arg.Key == key).ToArray();
        if(!typedConsts.Any())
        {
            return null;
        }

        return typedConsts.First().Value.Value?.ToString();
    }
}