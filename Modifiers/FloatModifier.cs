using System;
using DarkBestiary.Messaging;

namespace DarkBestiary.Modifiers
{
    public class FloatModifier : Modifier<float>
    {
        public event Payload<FloatModifier> StackChanged;

        public int StackCount { get; private set; } = 1;

        protected readonly float Amount;

        public FloatModifier(float amount, ModifierType type) : base(type)
        {
            this.Amount = amount;
        }

        public virtual float GetAmount()
        {
            return this.Amount * StackCount;
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