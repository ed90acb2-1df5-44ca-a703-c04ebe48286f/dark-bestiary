using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Validators;
using UnityEngine;

namespace DarkBestiary.Extensions
{
    public static class CollectionExtensions
    {
        public static IEnumerable<Validator> ByPurpose(this IEnumerable<ValidatorWithPurpose> source, ValidatorPurpose purpose)
        {
            return source.Where(element => element.Purpose == purpose).Select(element => element.Validator);
        }

        public static bool Validate(this IEnumerable<Validator> source, GameObject caster, object target)
        {
            return source.All(validator => validator.Validate(caster, target));
        }

        public static IEnumerable<TSource> NotNull<TSource>(this IEnumerable<TSource> source)
        {
            return source.Where(element => element != null);
        }

        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(
            this IEnumerable<TSource> source, Func<TSource, TKey> selector)
        {
            return source.GroupBy(selector).Select(group => group.First()).ToList();
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

        public static void AddIfNotExists<TKey, TValue>(this Dictionary<TKey, TValue> source, TKey key, TValue value)
        {
            if (source.ContainsKey(key))
            {
                return;
            }

            source.Add(key, value);
        }

        public static void AddOrSet<TKey, TValue>(this Dictionary<TKey, TValue> source, TKey key, TValue value)
        {
            if (source.ContainsKey(key))
            {
                source[key] = value;
                return;
            }

            source.Add(key, value);
        }

        public static void RemoveIfExists<TKey, TValue>(this Dictionary<TKey, TValue> source, TKey key)
        {
            if (!source.ContainsKey(key))
            {
                return;
            }

            source.Remove(key);
        }

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
        {
            return source.OrderBy(_ => Rng.Float());
        }

        public static bool IndexInBounds<T>(this IList<T> list, int index)
        {
            return index >= 0 && index < list.Count;
        }
    }
}