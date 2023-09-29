using DarkBestiary.Components;
using DarkBestiary.Items;
using DarkBestiary.UI.Views;

namespace DarkBestiary.UI.Controllers
{
    public class ReliquaryViewController : ViewController<IReliquaryView>
    {
        private readonly ReliquaryComponent m_Reliquary;

        public ReliquaryViewController(IReliquaryView view) : base(view)
        {
            m_Reliquary = Game.Instance.Character.Entity.GetComponent<ReliquaryComponent>();
        }

        protected override void OnInitialize()
        {
            m_Reliquary.Unlocked += OnRelicUnlocked;

            View.Equip += OnEquip;
            View.Unequip += OnUnequip;
            View.EquipIntoSlot += OnEquipIntoSlot;
            View.Construct(m_Reliquary.Slots, m_Reliquary.Available);
        }

        protected override void OnTerminate()
        {
            m_Reliquary.Unlocked -= OnRelicUnlocked;

            View.Equip -= OnEquip;
            View.Unequip -= OnUnequip;
            View.EquipIntoSlot -= OnEquipIntoSlot;
        }

        private void OnRelicUnlocked(Relic relic)
        {
            View.AddRelic(relic);
        }

        private void OnEquipIntoSlot(Relic relic, RelicSlot slot)
        {
            m_Reliquary.Equip(relic, slot);
        }

        private void OnUnequip(Relic relic)
        {
            m_Reliquary.Unequip(relic);
        }

        private void OnEquip(Relic relic)
        {
            m_Reliquary.Equip(relic);
        }
    }
}