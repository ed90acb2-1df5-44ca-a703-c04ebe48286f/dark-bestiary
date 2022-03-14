using System;

namespace DarkBestiary.Scenarios
{
    [Serializable]
    public struct Summary
    {
        public int Rounds;
        public int VisionsCompleted;
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

        public float DamagePerRound => this.Rounds > 0 ? this.DamageDealt / this.Rounds : 0;
        public float DamageTakenPerRound => this.Rounds > 0 ? this.DamageTaken / this.Rounds : 0;
        public float HealingTakenPerRound => this.Rounds > 0 ? this.HealingTaken / this.Rounds : 0;

        public Summary(
            int rounds,
            int visionsCompleted,
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
            this.Rounds = rounds;
            this.VisionsCompleted = visionsCompleted;
            this.Skills = skills;
            this.Legendaries = legendaries;
            this.MonstersSlain = monstersSlain;
            this.BossesSlain = bossesSlain;
            this.DamageDealt = damageDealt;
            this.DamageTaken = damageTaken;
            this.HighestDamageDealt = highestDamageDealt;
            this.HighestDamageTaken = highestDamageTaken;
            this.HealingTaken = healingTaken;
            this.HighestHealingTaken = highestHealingTaken;
        }
    }
}