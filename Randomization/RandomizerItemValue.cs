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
            if (Value == null)
            {
                return;
            }

            if (Value.IsVisionsOnly && !Game.Instance.IsVisions ||
                Value.IsCampaignOnly && !Game.Instance.IsCampaign ||
                Value.Type.Type == ItemTypeType.Ingredient && !Game.Instance.IsCampaign)
            {
                Value = null;
                return;
            }

            Value.SetStack(RNG.Range(this.data.StackMin, this.data.StackMax));
        }

        public override int GetHashCode()
        {
            return Value == null ? base.GetHashCode() : Value.Id;
        }

        public override bool Equals(object other)
        {
            if (other is RandomizerItemValue random)
            {
                return random.Value != null && Value != null && Value.Id == random.Value.Id;
            }

            return other?.Equals(this) ?? false;
        }
    }
}