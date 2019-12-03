namespace DarkBestiary.Values
{
    public struct Healing
    {
        public float Amount { get; }
        public HealingFlags Flags { get; }

        public Healing(float amount, HealingFlags flags = HealingFlags.None)
        {
            Amount = amount;
            Flags = flags;
        }

        public static Healing operator +(Healing healing, float amount)
        {
            return new Healing(healing.Amount + amount, healing.Flags);
        }

        public static Healing operator *(Healing healing, float amount)
        {
            return new Healing(healing.Amount * amount, healing.Flags);
        }

        public static implicit operator float(Healing healing)
        {
            return healing.Amount;
        }

        public bool IsRegeneration()
        {
            return Flags.HasFlag(HealingFlags.Regeneration);
        }

        public bool IsVampirism()
        {
            return Flags.HasFlag(HealingFlags.Vampirism);
        }
    }
}