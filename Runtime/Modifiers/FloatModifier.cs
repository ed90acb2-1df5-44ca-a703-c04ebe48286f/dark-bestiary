using System;

namespace DarkBestiary.Modifiers
{
    public class FloatModifier : Modifier<float>
    {
        public event Action<FloatModifier> StackChanged;

        public int StackCount { get; private set; } = 1;

        protected readonly float Amount;

        public FloatModifier(float amount, ModifierType type) : base(type)
        {
            Amount = amount;
        }

        public virtual float GetAmount()
        {
            return Amount * StackCount;
        }

        public void ChangeStack(int stack)
        {
            StackCount = Math.Max(1, stack);
            StackChanged?.Invoke(this);
        }

        public override float Modify(float value)
        {
            switch (Type)
            {
                case ModifierType.Flat:
                    return value + GetAmount();
                case ModifierType.Fraction:
                    return value + value * GetAmount();
                default:
                    throw new Exception("Unknown modifier type " + Type);
            }
        }
    }
}