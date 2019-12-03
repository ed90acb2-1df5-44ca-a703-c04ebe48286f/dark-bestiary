using System.Collections.Generic;
using DarkBestiary.Items;
using DarkBestiary.Messaging;

namespace DarkBestiary.UI.Views
{
    public interface IReliquaryView : IView, IHideOnEscape
    {
        event Payload<Relic> Equip;
        event Payload<Relic> Unequip;
        event Payload<Relic, RelicSlot> EquipIntoSlot;

        void Construct(List<RelicSlot> slots, List<Relic> relics);

        void AddRelic(Relic relic);
    }
}