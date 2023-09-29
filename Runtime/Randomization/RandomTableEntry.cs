namespace DarkBestiary.Randomization
{
    public readonly struct RandomTableEntryParameters
    {
        public readonly float Weight;
        public readonly bool IsUnique;
        public readonly bool IsGuaranteed;
        public readonly bool IsEnabled;

        public RandomTableEntryParameters(float weight, bool isUnique, bool isGuaranteed, bool isEnabled)
        {
            Weight = weight;
            IsUnique = isUnique;
            IsGuaranteed = isGuaranteed;
            IsEnabled = isEnabled;
        }

        public RandomTableEntryParameters(float weight)
        {
            Weight = weight;
            IsUnique = false;
            IsGuaranteed = false;
            IsEnabled = true;
        }
    }

    public abstract class RandomTableEntry
    {
        public readonly float Weight;
        public readonly bool IsUnique;
        public readonly bool IsGuaranteed;
        public readonly bool IsEnabled;

        protected RandomTableEntry(RandomTableEntryParameters parameters)
        {
            Weight = parameters.Weight;
            IsUnique = parameters.IsUnique;
            IsGuaranteed = parameters.IsGuaranteed;
            IsEnabled = parameters.IsEnabled;
        }
    }

    public abstract class RandomTableEntry<T> : RandomTableEntry
    {
        public readonly T Value;

        protected RandomTableEntry(T value, RandomTableEntryParameters parameters) : base(parameters)
        {
            Value = value;
        }
    }
}