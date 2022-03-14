using DarkBestiary.Items;

namespace DarkBestiary.Randomization
{
    public class RandomizerItemModifierValue : RandomizerValue<ItemModifier>
    {
        public RandomizerItemModifierValue(ItemModifier value, float probability) : base(value, probability)
        {
        }

        public RandomizerItemModifierValue(ItemModifier value, float probability, bool unique, bool guaranteed, bool enabled) : base(value, probability, unique, guaranteed, enabled)
        {
        }
    }
}