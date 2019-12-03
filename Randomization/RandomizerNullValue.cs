namespace DarkBestiary.Randomization
{
    public class RandomizerNullValue : RandomizerValue<object>
    {
        public RandomizerNullValue(float probability) : base(null, probability, false, false, true)
        {
        }
    }
}