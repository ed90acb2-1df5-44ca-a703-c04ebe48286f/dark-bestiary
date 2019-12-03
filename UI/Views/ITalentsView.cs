using System.Collections.Generic;
using DarkBestiary.Messaging;
using DarkBestiary.Talents;

namespace DarkBestiary.UI.Views
{
    public interface ITalentsView : IView, IHideOnEscape
    {
        event Payload<Talent> Clicked;
        event Payload Reseted;

        void Construct(List<TalentTier> tiers);

        void UpdatePoints(int points);
    }
}