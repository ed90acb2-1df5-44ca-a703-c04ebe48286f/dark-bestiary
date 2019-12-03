namespace DarkBestiary.Scenarios
{
    public struct ScenarioSummary
    {
        public int Rounds { get; }
        public float DamageDealt { get; }
        public float DamagePerRound => Rounds > 0 ? DamageDealt / Rounds : 0;
        public float DamageTaken { get; }
        public float DamageTakenPerRound => Rounds > 0 ? DamageTaken / Rounds : 0;
        public float Healing { get; }
        public float HealingPerRound => Rounds > 0 ? Healing / Rounds : 0;

        public ScenarioSummary(int rounds, float damageDealt, float damageTaken, float healing)
        {
            Rounds = rounds;
            DamageDealt = damageDealt;
            DamageTaken = damageTaken;
            Healing = healing;
        }
    }
}