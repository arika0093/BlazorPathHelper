using System.Linq;
using System;

namespace BlazorPathHelper;

/// <summary>
/// utilities for Generated items
/// </summary>
public static class BlazorPathHelperUtility
{
    /// <summary>
    /// Multi-level array flattening
    /// </summary>
    /// <param name="items">flattening target</param>
    /// <param name="childrenSelector">child selector</param>
    /// <typeparam name="T">type of the array</typeparam>
    /// <returns>flattened array</returns>
    public static T[] ToFlatten<T>(this T[] items, Func<T, T[]> childrenSelector) where T : BlazorPathMenuItem
    {
        return items
            .SelectMany(i => childrenSelector(i).ToFlatten(childrenSelector).Prepend(i))
            .ToArray();
    }

    /// <summary>
    /// Build Query String
    /// </summary>
    public static string BuildQuery((string key, string? val)[] queryTuples)
    {
        var filteredTuples = queryTuples.Where(t => t.val != null).ToArray();
        if (filteredTuples.Any())
        {
            return "?" + string.Join("&", filteredTuples.Select(t => $"{t.key}={t.val}"));
        }
        return "";
    }

    /// <summary>
    /// Convert any key/value to string for URL
    /// </summary>
    public static string? ToEscapedString<T>( T? item)
    {
        return item != null ? Uri.EscapeDataString(ToStringForUrl(item)) : null;
    }

    /// <summary>
    /// Convert any to string for URL
    /// </summary>
    public static string ToStringForUrl<T>(T? item)
    {
        return item switch
        {
            DateTime dt => ToStringForUrlDateTime(dt),
            _ => item?.ToString() ?? string.Empty
        };
    }

    /// <summary>
    /// Convert DateTime to string for URL
    /// </summary>
    public static string ToStringForUrlDateTime(DateTime? item)
    {
        return item.HasValue ? item.Value.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ") : string.Empty;
    }

}