using System;
using System.Collections.Generic;
using DarkBestiary.Behaviours;

namespace DarkBestiary.UI.Views
{
    public interface IBuffSelectionView : IView, IFullscreenView
    {
        event Action<Behaviour> BuffSelected;

        void Construct(List<Behaviour> behaviours);
    }
}