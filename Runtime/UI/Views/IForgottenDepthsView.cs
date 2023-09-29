using System;
using System.Collections.Generic;
using DarkBestiary.Behaviours;

namespace DarkBestiary.UI.Views
{
    public interface IForgottenDepthsView : IView
    {
        event Action StartButtonClicked;

        void Construct(int depth, int monsterLevel, IReadOnlyList<Behaviour> behaviours);
    }
}