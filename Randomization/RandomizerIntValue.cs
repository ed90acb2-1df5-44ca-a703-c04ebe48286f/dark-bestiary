namespace DarkBestiary.Randomization
{
    public class RandomizerIntValue : RandomizerValue<int>
    {
        public RandomizerIntValue(int value, float probability) : base(value, probability)
        {
        }

        public RandomizerIntValue(int value, float probability, bool unique, bool guaranteed, bool enabled)
            : base(value, probability, unique, guaranteed, enabled)
        {
        }
    }
}