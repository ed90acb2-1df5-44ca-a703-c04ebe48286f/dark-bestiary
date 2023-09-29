using System;
using System.Collections.Generic;
using DarkBestiary.Components;
using DarkBestiary.Data;

namespace DarkBestiary.UI.Views
{
    public interface IBestiaryView : IView, IHideOnEscape
    {
        event Action<UnitData> Selected;
        event Action<int> LevelChanged;

        void Construct(List<UnitData> units, int level);
        void Display(UnitComponent unit);
        void RefreshProperties(UnitComponent unit);
    }
}