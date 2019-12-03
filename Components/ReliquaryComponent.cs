using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Items;
using DarkBestiary.Messaging;

namespace DarkBestiary.Components
{
    public class ReliquaryComponent : Component
    {
        public event Payload<ReliquaryComponent, Relic> Equipped;
        public event Payload<ReliquaryComponent, Relic> Unequipped;
        public event Payload<ReliquaryComponent, Relic> Unlocked;

        public List<RelicSlot> Slots { get; private set; }
        public List<Relic> Available { get; private set; }

        private IRelicRepository relicRepository;

        public ReliquaryComponent Construct()
        {
            this.relicRepository = Container.Instance.Resolve<IRelicRepository>();

            Slots = new List<RelicSlot>
            {
                new RelicSlot(),
                new RelicSlot(),
                new RelicSlot(),
            };

            Available = new List<Relic>();

            return this;
        }

        public void GiveExperience(int experience)
        {
            var relics = GetActiveRelics();

            if (relics.Count == 0)
            {
                return;
            }

            var experiencePerRelic = experience / relics.Count;

            foreach (var relic in relics)
            {
                relic.Experience.CreateSnapshot(experiencePerRelic);
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

            Equipped?.Invoke(this, relic);
        }

        public void Unequip(Relic relic)
        {
            relic.Unequip();
            FindContainingSlot(relic)?.Unequip();

            Unequipped?.Invoke(this, relic);
        }

        public void Unlock(int relicId)
        {
            var relic = this.relicRepository.Find(relicId);
            relic.Construct(1, 0);

            Unlock(relic);
        }

        public void Unlock(Relic relic)
        {
            relic.Owner = gameObject;
            Available.Add(relic);

            Unlocked?.Invoke(this, relic);
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