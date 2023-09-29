using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Items;

namespace DarkBestiary.Components
{
    public class ReliquaryComponent : Component
    {
        public static event Action<ReliquaryComponent, Relic> AnyRelicUnlocked;

        public event Action<Relic> Equipped;
        public event Action<Relic> Unequipped;
        public event Action<Relic> Unlocked;

        public List<RelicSlot> Slots { get; private set; }
        public List<Relic> Available { get; private set; }

        private IRelicRepository m_RelicRepository;

        public ReliquaryComponent Construct()
        {
            m_RelicRepository = Container.Instance.Resolve<IRelicRepository>();

            Slots = new List<RelicSlot>
            {
                new(),
                new(),
                new(),
            };

            Available = new List<Relic>();

            return this;
        }

        public void GiveExperience(int experience)
        {
            var activeRelics = GetActiveRelics();
            var levelingRelics = activeRelics.Where(relic => !relic.Experience.IsMaxLevel).ToList();
            var experiencePerRelic = levelingRelics.Count > 0 ? experience / levelingRelics.Count : 0;

            foreach (var relic in activeRelics)
            {
                relic.Experience.CreateSnapshot(relic.Experience.IsMaxLevel ? 0 : experiencePerRelic);
                relic.Experience.Add(experiencePerRelic);
            }
        }

        public List<Relic> GetActiveRelics()
        {
            return Slots
                .Where(s => !s.IsEmpty)
                .Select(s => s.Relic)
                .ToList();
        }

        public void Equip(Relic relic)
        {
            if (FindContainingSlot(relic) != null)
            {
                return;
            }

            var suitableSlot = FindSuitableSlot(relic) ?? Slots.First();

            Equip(relic, suitableSlot);
        }

        public void Equip(Relic relic, RelicSlot slot)
        {
            var previousSlot = FindContainingSlot(relic);

            if (previousSlot != null)
            {
                if (!slot.IsEmpty)
                {
                    var temp = previousSlot.Relic;
                    previousSlot.Equip(slot.Relic);
                    slot.Equip(temp);
                    return;
                }

                Unequip(relic);
            }

            if (!slot.IsEmpty)
            {
                Unequip(slot.Relic);
            }

            relic.Owner = gameObject;
            relic.Equip();

            slot.Equip(relic);

            Equipped?.Invoke(relic);
        }

        public void Unequip(Relic relic)
        {
            relic.Unequip();
            FindContainingSlot(relic)?.Unequip();

            Unequipped?.Invoke(relic);
        }

        public void Unlock(int relicId)
        {
            var relic = m_RelicRepository.Find(relicId);
            relic.Construct(1, 0);

            Unlock(relic);
        }

        public void Unlock(Relic relic)
        {
            UnlockSilently(relic);
            Unlocked?.Invoke(relic);
            AnyRelicUnlocked?.Invoke(this, relic);
        }

        public void UnlockSilently(Relic relic)
        {
            relic.Owner = gameObject;
            Available.Add(relic);
        }

        private RelicSlot FindContainingSlot(Relic relic)
        {
            return Slots.FirstOrDefault(s => !s.IsEmpty && s.Relic.Id == relic.Id);
        }

        private RelicSlot FindSuitableSlot(Relic relic)
        {
            return Slots.OrderBy(s => !s.IsEmpty).FirstOrDefault(s => s.IsEmpty || s.CanEquip(relic));
        }
    }
}