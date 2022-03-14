using System.Collections.Generic;
using DarkBestiary.Behaviours;
using DarkBestiary.Messaging;

namespace DarkBestiary.UI.Views
{
    public interface IBuffSelectionView : IView, IFullscreenView
    {
        event Payload<Behaviour> BuffSelected;

        void Construct(List<Behaviour> behaviours);
    }
}