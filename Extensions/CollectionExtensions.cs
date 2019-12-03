using System;
using System.Collections.Generic;
using System.Linq;

namespace DarkBestiary.Extensions
{
    public static class CollectionExtensions
    {
        public static IEnumerable<TSource> NotNull<TSource>(this IEnumerable<TSource> source)
        {
            return source.Where(element => element != null);
        }

        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(
            this IEnumerable<TSource> source, Func<TSource, TKey> selector)
        {
            return source.GroupBy(selector).Select(group => group.First()).ToList();
        }

        public static TValue GetValueOrDefault<TKey, TValue>(
            this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue)
        {
            return dictionary.TryGetValue(key, out var value) && value != null ? value : defaultValue;
        }

        public static T Random<T>(this IEnumerable<T> source)
        {
            return source.Shuffle().FirstOrDefault();
        }

        public static IEnumerable<T> Random<T>(this IEnumerable<T> source, int count)
        {
            return source.Shuffle().Take(count);
        }

        public static Dictionary<TKey, TValue> Clone<TKey, TValue>(this Dictionary<TKey, TValue> source)
        {
            return source.ToDictionary(pair => pair.Key, pair => pair.Value);
        }

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
        {
            var array = source.ToArray();

            var n = array.Length;

            while (n > 1)
            {
                var k = RNG.Range(0, n--);

                var temp = array[n];
                array[n] = array[k];
                array[k] = temp;
            }

            return array;
        }

        public static void Resize<T>(this List<T> list, int size, T element = default)
        {
            if (size < list.Count)
            {
                list.RemoveRange(size, list.Count - size);
                return;
            }

            if (size > list.Capacity)
            {
                list.Capacity = size;
            }

            list.AddRange(Enumerable.Repeat(element, size - list.Count));
        }

        public static void Shrink<T>(this List<T> list, int size)
        {
            if (size >= list.Count)
            {
                return;
            }

            list.RemoveRange(size, list.Count - size);
        }

        public static bool IndexInBounds<T>(this IList<T> list, int index)
        {
            return index >= 0 && index < list.Count;
        }
    }
}