using System;
using System.Collections.Generic;
using System.Linq;

namespace BlazorPathHelper;

public static class FlattenUtility
{
    public static T[] ToFlatten<T>(this T[] items, Func<T, T[]> childrenSelector)
    {
        return items
            .SelectMany(i => childrenSelector(i).ToFlatten(childrenSelector).Prepend(i))
            .ToArray();
    }
}