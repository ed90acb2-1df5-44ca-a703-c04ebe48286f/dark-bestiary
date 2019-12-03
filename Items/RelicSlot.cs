using DarkBestiary.Messaging;

namespace DarkBestiary.Items
{
    public class RelicSlot
    {
        public event Payload<RelicSlot, Relic> Equipped;
        public event Payload<RelicSlot> Unequipped;

        public Relic Relic { get; private set; }
        public bool IsEmpty => Relic.IsEmpty;

        public RelicSlot()
        {
            Relic = Relic.Empty;
        }

        public void Equip(Relic relic)
        {
            Relic = relic;
            Equipped?.Invoke(this, relic);
        }

        public void Unequip()
        {
            Relic = Relic.Empty;
            Unequipped?.Invoke(this);
        }

        public bool CanEquip(Relic relic)
        {
            return true;
        }
    }
}