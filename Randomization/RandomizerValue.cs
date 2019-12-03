namespace DarkBestiary.Randomization
{
    public class RandomizerValue<T> : RandomizerObject, IRandomizerValue<T>
    {
        public T Value { get; protected set; }

        public RandomizerValue(T value, float probability) : base(probability, true, false, true)
        {
            Value = value;
        }

        public RandomizerValue(T value, float probability, bool unique, bool guaranteed, bool enabled)
            : base(probability, unique, guaranteed, enabled)
        {
            Value = value;
        }
    }
}