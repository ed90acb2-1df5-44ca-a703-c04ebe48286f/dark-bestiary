using DarkBestiary.Components;
using DarkBestiary.Items;
using DarkBestiary.Managers;
using DarkBestiary.UI.Views;

namespace DarkBestiary.UI.Controllers
{
    public class ReliquaryViewController : ViewController<IReliquaryView>
    {
        private readonly ReliquaryComponent reliquary;

        public ReliquaryViewController(IReliquaryView view, CharacterManager characterManager) : base(view)
        {
            this.reliquary = characterManager.Character.Entity.GetComponent<ReliquaryComponent>();
        }

        protected override void OnInitialize()
        {
            this.reliquary.Unlocked += OnRelicUnlocked;

            View.Equip += OnEquip;
            View.Unequip += OnUnequip;
            View.EquipIntoSlot += OnEquipIntoSlot;
            View.Construct(this.reliquary.Slots, this.reliquary.Available);
        }

        protected override void OnTerminate()
        {
            this.reliquary.Unlocked -= OnRelicUnlocked;

            View.Equip -= OnEquip;
            View.Unequip -= OnUnequip;
            View.EquipIntoSlot -= OnEquipIntoSlot;
        }

        private void OnRelicUnlocked(ReliquaryComponent reliquary, Relic relic)
        {
            View.AddRelic(relic);
        }

        private void OnEquipIntoSlot(Relic relic, RelicSlot slot)
        {
            this.reliquary.Equip(relic, slot);
        }

        private void OnUnequip(Relic relic)
        {
            this.reliquary.Unequip(relic);
        }

        private void OnEquip(Relic relic)
        {
            this.reliquary.Equip(relic);
        }
    }
}