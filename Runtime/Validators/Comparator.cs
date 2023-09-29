using System;
using UnityEngine;

namespace DarkBestiary.Validators
{
    public static class Comparator
    {
        public static bool Compare(float value, float target, ComparatorMethod method)
        {
            switch (method)
            {
                case ComparatorMethod.EqualTo:
                    return Math.Abs(value - target) < Mathf.Epsilon;
                case ComparatorMethod.NotEqualTo:
                    return Math.Abs(value - target) > Mathf.Epsilon;
                case ComparatorMethod.GreaterThan:
                    return value > target;
                case ComparatorMethod.GreaterThanOrEqualTo:
                    return value >= target;
                case ComparatorMethod.LessThan:
                    return value < target;
                case ComparatorMethod.LessThanOrEqualTo:
                    return value <= target;
                default:
                    throw new ArgumentOutOfRangeException(nameof(method), method, null);
            }
        }
    }
}