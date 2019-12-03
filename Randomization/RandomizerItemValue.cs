using DarkBestiary.Data;
using DarkBestiary.Items;

namespace DarkBestiary.Randomization
{
    public class RandomizerItemValue : RandomizerValue<Item>
    {
        private readonly LootItemData data;

        public RandomizerItemValue(Item value, LootItemData data)
            : base(value, data.Probability, data.Unique, data.Guaranteed, data.Enabled)
        {
            this.data = data;
        }

        public override void OnHit()
        {
            Value.SetStack(RNG.Range(this.data.StackMin, this.data.StackMax + 1));
        }
    }
}