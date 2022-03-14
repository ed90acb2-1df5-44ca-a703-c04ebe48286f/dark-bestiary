using DarkBestiary.Messaging;

namespace DarkBestiary.Items
{
    public class RelicSlot
    {
        public event Payload<RelicSlot, Relic> Equipped;
        public event Payload<RelicSlot> Unequipped;
        public event Payload<RelicSlot> ExperienceChanged;

        public Relic Relic { get; private set; }
        public bool IsEmpty => Relic.IsEmpty;

        public RelicSlot()
        {
            Relic = Relic.Empty;
        }

        public void Equip(Relic relic)
        {
            Relic = relic;
            Relic.Experience.Changed += OnExperienceChanged;
            Relic.Experience.LevelUp += OnExperienceChanged;
            Equipped?.Invoke(this, relic);
        }

        public void Unequip()
        {
            Relic.Experience.Changed -= OnExperienceChanged;
            Relic.Experience.LevelUp -= OnExperienceChanged;
            Relic = Relic.Empty;
            Unequipped?.Invoke(this);
        }

        public bool CanEquip(Relic relic)
        {
            return true;
        }

        private void OnExperienceChanged(Experience experience)
        {
            ExperienceChanged?.Invoke(this);
        }
    }
}