using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public static class ArrayExtensions
{
    public static int Count<T>(this T[] items, Func<T, bool> func)
    {
        return Enumerable.Count(items, func);
    }


    public static void ForEach<T>(this IEnumerable<T> items, Action<T> func)
    {
        if (items == null) throw new ArgumentNullException(nameof(items));
        if (func == null) throw new ArgumentNullException(nameof(func));
        foreach (var item in items)
        {
            func(item);
        }
    }
}