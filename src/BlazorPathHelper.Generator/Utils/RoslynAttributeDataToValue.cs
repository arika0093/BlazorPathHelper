using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace BlazorPathHelper.Utils;

internal static class RoslynAttributeDataToValue
{
    // get symbol value from attribute data
    public static object? GetSymbol(this AttributeData attributeData, string key)
    {
        return attributeData.NamedArguments
            .Where(arg => arg.Key == key)
            .Select(arg => arg.Value.Value)
            .FirstOrDefault();
    }

    // get multiple values from attribute data
    public static Dictionary<string, string?> ToDictionary(this AttributeData attributeData)
    {
        return attributeData.NamedArguments.ToDictionary(arg => arg.Key, arg => arg.Value.Value?.ToString());
    }
}

internal static class DictionaryUtils
{
    // get value from dictionary with default value
    public static TValue? Get<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key)
        where TKey : notnull
    {
        return dictionary.TryGetValue(key, out var value) ? value : default;
    }
}