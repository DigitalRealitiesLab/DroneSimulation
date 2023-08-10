using System;
using System.Collections.Generic;
using System.Linq;

namespace Support.Extensions {
    public static class EnumerableExtensions {
        public static bool IsEmpty<TSource>(this IEnumerable<TSource> source) => !source.Any();

        public static bool IsNotEmpty<TSource>(this IEnumerable<TSource> source) => !source.IsEmpty();
    }
}