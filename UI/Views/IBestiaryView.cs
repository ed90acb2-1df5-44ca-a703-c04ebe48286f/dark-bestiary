using System.Collections.Generic;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Messaging;

namespace DarkBestiary.UI.Views
{
    public interface IBestiaryView : IView, IHideOnEscape
    {
        event Payload<UnitData> Selected;
        event Payload<int> LevelChanged;

        void Construct(List<UnitData> units, int level);
        void Display(UnitComponent unit);
        void RefreshProperties(UnitComponent unit);
    }
}