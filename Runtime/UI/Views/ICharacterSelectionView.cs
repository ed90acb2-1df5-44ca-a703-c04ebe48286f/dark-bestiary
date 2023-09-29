using System;
using System.Collections.Generic;

namespace DarkBestiary.UI.Views
{
    public interface ICharacterSelectionView : IView
    {
        event Action Cancel;
        event Action Create;
        event Action Start;
        event Action<Character, string> RenameCharacter;
        event Action<Character> SelectCharacter;
        event Action<Character> DeleteCharacter;

        void Refresh(List<Character> characters);
    }
}