namespace DarkBestiary.Extensions
{
    public static class MathExtensions
    {
        public static bool InRange(this int value, int minimum, int maximum)
        {
            return value >= minimum && value <= maximum;
        }

        public static bool InRange(this float value, float minimum, float maximum)
        {
            return value >= minimum && value <= maximum;
        }

        public static int Clamp(this int value, int min, int max)
        {
            if (value < min)
            {
                value = min;
            }
            else if (value > max)
            {
                value = max;
            }

            return value;
        }
    }
}