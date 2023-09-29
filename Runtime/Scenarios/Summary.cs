using System;

namespace DarkBestiary.Scenarios
{
    [Serializable]
    public struct Summary
    {
        public int Rounds;
        public int EncountersCompleted;
        public int Skills;
        public int Legendaries;
        public int MonstersSlain;
        public int BossesSlain;
        public float DamageDealt;
        public float HighestDamageDealt;
        public float DamageTaken;
        public float HighestDamageTaken;
        public float HealingTaken;
        public float HighestHealingTaken;

        public float DamagePerRound => Rounds > 0 ? DamageDealt / Rounds : 0;
        public float DamageTakenPerRound => Rounds > 0 ? DamageTaken / Rounds : 0;
        public float HealingTakenPerRound => Rounds > 0 ? HealingTaken / Rounds : 0;

        public Summary(
            int rounds,
            int encountersCompleted,
            int skills,
            int legendaries,
            int monstersSlain,
            int bossesSlain,
            float damageDealt,
            float highestDamageDealt,
            float damageTaken,
            float highestDamageTaken,
            float healingTaken,
            float highestHealingTaken)
        {
            Rounds = rounds;
            EncountersCompleted = encountersCompleted;
            Skills = skills;
            Legendaries = legendaries;
            MonstersSlain = monstersSlain;
            BossesSlain = bossesSlain;
            DamageDealt = damageDealt;
            DamageTaken = damageTaken;
            HighestDamageDealt = highestDamageDealt;
            HighestDamageTaken = highestDamageTaken;
            HealingTaken = healingTaken;
            HighestHealingTaken = highestHealingTaken;
        }
    }
}