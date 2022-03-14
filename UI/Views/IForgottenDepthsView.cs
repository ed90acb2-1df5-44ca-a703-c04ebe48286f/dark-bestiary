using System.Collections.Generic;
using DarkBestiary.Behaviours;
using DarkBestiary.Messaging;

namespace DarkBestiary.UI.Views
{
    public interface IForgottenDepthsView : IView
    {
        event Payload StartButtonClicked;

        void Construct(int depth, int monsterLevel, IReadOnlyList<Behaviour> behaviours);
    }
}