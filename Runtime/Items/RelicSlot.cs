using System;

namespace DarkBestiary.Items
{
    public class RelicSlot
    {
        public event Action<RelicSlot, Relic> Equipped;
        public event Action<RelicSlot> Unequipped;
        public event Action<RelicSlot> ExperienceChanged;

        public Relic Relic { get; private set; }
        public bool IsEmpty => Relic.IsEmpty;

        public RelicSlot()
        {
            Relic = Relic.s_Empty;
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
            Relic = Relic.s_Empty;
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