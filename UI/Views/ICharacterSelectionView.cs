using System.Collections.Generic;
using DarkBestiary.Messaging;

namespace DarkBestiary.UI.Views
{
    public interface ICharacterSelectionView : IView
    {
        event Payload Cancel;
        event Payload Create;
        event Payload Start;
        event Payload<Character, string> RenameCharacter;
        event Payload<Character> SelectCharacter;
        event Payload<Character> DeleteCharacter;

        void Refresh(List<Character> characters);
    }
}