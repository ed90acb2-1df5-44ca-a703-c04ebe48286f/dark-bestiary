using DarkBestiary.Skills;

namespace DarkBestiary.Values
{
    public struct Healing
    {
        public float Amount { get; }
        public HealFlags Flags { get; }
        public Skill Skill { get; }

        public Healing(float amount, HealFlags flags, Skill skill)
        {
            Amount = amount;
            Flags = flags;
            Skill = skill;
        }

        public static Healing operator +(Healing healing, float amount)
        {
            return new Healing(healing.Amount + amount, healing.Flags, healing.Skill);
        }

        public static Healing operator *(Healing healing, float amount)
        {
            return new Healing(healing.Amount * amount, healing.Flags, healing.Skill);
        }

        public static implicit operator float(Healing healing)
        {
            return healing.Amount;
        }

        public bool IsRegeneration()
        {
            return Flags.HasFlag(HealFlags.Regeneration);
        }

        public bool IsVampirism()
        {
            return Flags.HasFlag(HealFlags.Vampirism);
        }
    }
}