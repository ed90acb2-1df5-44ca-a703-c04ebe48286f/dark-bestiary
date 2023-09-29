using UnityEngine;

namespace DarkBestiary.UI.Views
{
    public interface ICharacterUnitFrameView : IView
    {
        void Construct(GameObject entity);
    }
}