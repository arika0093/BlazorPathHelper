using System;
using System.Linq;

namespace BlazorPathHelper;

/// <summary>
/// Flatten utility
/// </summary>
public static class FlattenUtility
{
    /// <summary>
    /// Multi-level array flattening
    /// </summary>
    /// <param name="items">flattening target</param>
    /// <param name="childrenSelector">child selector</param>
    /// <typeparam name="T">type of the array</typeparam>
    /// <returns>flattened array</returns>
    public static T[] ToFlatten<T>(this T[] items, Func<T, T[]> childrenSelector)
    {
        return items
            .SelectMany(i => childrenSelector(i).ToFlatten(childrenSelector).Prepend(i))
            .ToArray();
    }
}