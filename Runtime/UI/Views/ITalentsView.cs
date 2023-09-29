using System;
using System.Collections.Generic;
using DarkBestiary.Talents;

namespace DarkBestiary.UI.Views
{
    public interface ITalentsView : IView, IHideOnEscape
    {
        event Action<Talent> Clicked;
        event Action Reseted;

        void Construct(List<TalentTier> tiers);
        void UpdatePoints(int points);
    }
}