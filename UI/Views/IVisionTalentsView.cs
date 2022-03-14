using DarkBestiary.Components;
using DarkBestiary.Messaging;
using DarkBestiary.Talents;

namespace DarkBestiary.UI.Views
{
    public interface IVisionTalentsView : IView
    {
        event Payload ContinueButtonPressed;
        event Payload<Talent> TalentClicked;

        void Construct(TalentsComponent talents);
    }
}