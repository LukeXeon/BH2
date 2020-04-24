using System;
using System.Collections.Generic;
using System.Linq;

namespace Dash.Scripts.Core
{
    public static class EnumerableExtensions
    {
        public static SortedDictionary<TKey, TSource> ToSortedDictionary<TSource, TKey>(
            this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            return new SortedDictionary<TKey, TSource>(source.ToDictionary(keySelector));
        }

        public static SortedDictionary<TKey, TSource> ToSortedDictionary<TSource, TKey, TElement>(
            this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey> comparer)
        {
            return new SortedDictionary<TKey, TSource>(source.ToDictionary(keySelector), comparer);
        }

        public static SortedDictionary<TKey, TElement> ToSortedDictionary<TSource, TKey, TElement>(
            this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
        {
            return new SortedDictionary<TKey, TElement>(source.ToDictionary(keySelector, elementSelector));
        }

        public static SortedDictionary<TKey, TElement> ToSortedDictionary<TSource, TKey, TElement>(
            this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector,
            IComparer<TKey> comparer)
        {
            return new SortedDictionary<TKey, TElement>(source.ToDictionary(keySelector, elementSelector), comparer);
        }
    }
}