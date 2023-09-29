namespace DarkBestiary.UI.Controllers
{
    public class DamageMeter
    {
        public readonly int Id;
        public readonly string Name;
        public readonly bool IsAlly;

        public bool IsAlive = true;
        public float DamageDone { get; private set; }
        public float DamageDonePerTurn { get; private set; }
        public float DamageTaken { get; private set; }
        public float DamageTakenPerTurn { get; private set; }
        public float HealingDone { get; private set; }
        public float HealingDonePerTurn { get; private set; }
        public float HealingTaken { get; private set; }
        public float HealingTakenPerTurn { get; private set; }
        public int Turns { get; private set; } = 1;

        public DamageMeter(int id, string name, bool isAlly)
        {
            Id = id;
            Name = name;
            IsAlly = isAlly;
        }

        public void AddTurns(int amount)
        {
            Turns += amount;
            UpdatePerTurnMetrics();
        }

        public void AddDamageDone(float amount)
        {
            DamageDone += amount;
            UpdatePerTurnMetrics();
        }

        public void AddDamageTaken(float amount)
        {
            DamageTaken += amount;
            UpdatePerTurnMetrics();
        }

        public void AddHealingDone(float amount)
        {
            HealingDone += amount;
            UpdatePerTurnMetrics();
        }

        public void AddHealingTaken(float amount)
        {
            HealingTaken += amount;
            UpdatePerTurnMetrics();
        }

        private void UpdatePerTurnMetrics()
        {
            DamageDonePerTurn = DamageDone / Turns;
            DamageTakenPerTurn = DamageTaken / Turns;
            HealingDonePerTurn = HealingDone / Turns;
            HealingTakenPerTurn = HealingTaken / Turns;
        }
    }
}