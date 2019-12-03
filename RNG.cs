using System;

namespace DarkBestiary
{
    public static class RNG
    {
        private static readonly Random Random = new Random();

        public static int Range(int min, int max)
        {
            return Random.Next(min, max);
        }

        public static float Range(float min, float max)
        {
            return (float) Random.NextDouble() * (max - min) + min;
        }

        public static float Float()
        {
            return (float) Random.NextDouble();
        }

        public static bool Test(float number)
        {
            return Float() <= number;
        }
    }
}