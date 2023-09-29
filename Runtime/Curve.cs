using System;

namespace DarkBestiary
{
    public static class Curve
    {
        public static float Evaluate(float current, float min, float max, CurveType curveType)
        {
            switch (curveType)
            {
                case CurveType.Linear:
                    return current / 100.0f * (max - min) + min;
                case CurveType.Slow:
                    return current / 100.0f * (max - min) + min;
                case CurveType.Fast:
                    return current / 100.0f * (max - min) + min;
                default:
                    throw new ArgumentOutOfRangeException(nameof(curveType), curveType, null);
            }
        }
    }
}