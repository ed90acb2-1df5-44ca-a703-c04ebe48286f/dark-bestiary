using System;
using System.Collections.Generic;
using DarkBestiary.Items;

namespace DarkBestiary.UI.Views
{
    public interface IReliquaryView : IView, IHideOnEscape
    {
        event Action<Relic> Equip;
        event Action<Relic> Unequip;
        event Action<Relic, RelicSlot> EquipIntoSlot;

        void Construct(List<RelicSlot> slots, List<Relic> relics);
        void AddRelic(Relic relic);
    }
}