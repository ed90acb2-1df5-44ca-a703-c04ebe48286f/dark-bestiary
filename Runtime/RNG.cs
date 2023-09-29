using System;

namespace DarkBestiary
{
    public static class Rng
    {
        public static readonly Random s_Random = new();

        public static int Range(int min, int max)
        {
            return s_Random.Next(min, max + 1);
        }

        public static float Range(float min, float max)
        {
            return (float) s_Random.NextDouble() * (max - min) + min;
        }

        public static float Float()
        {
            return (float) s_Random.NextDouble();
        }

        public static bool Test(float number)
        {
            return Float() <= number;
        }
    }
}