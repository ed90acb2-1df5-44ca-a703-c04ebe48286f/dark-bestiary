namespace DarkBestiary.Randomization
{
    public readonly struct RandomTableItemEntryValue
    {
        public readonly int ItemId;
        public readonly int StackCount;

        public RandomTableItemEntryValue(int itemId, int stackCount)
        {
            ItemId = itemId;
            StackCount = stackCount;
        }
    }

    public class RandomTableItemEntry : RandomTableEntry<RandomTableItemEntryValue>
    {
        public RandomTableItemEntry(RandomTableItemEntryValue value, RandomTableEntryParameters parameters) : base(value, parameters)
        {
        }

        public override int GetHashCode()
        {
            return Value.ItemId;
        }

        public override bool Equals(object? other)
        {
            if (other is RandomTableItemEntry random)
            {
                return Value.ItemId == random.Value.ItemId;
            }

            return other?.Equals(this) ?? false;
        }
    }
}