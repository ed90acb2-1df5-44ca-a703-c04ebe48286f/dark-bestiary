using System.Collections.Generic;
using DarkBestiary.Messaging;
using UnityEngine;

namespace DarkBestiary.UI.Views
{
    public interface ICharacterCreationView : IView
    {
        event Payload Cancel;
        event Payload<int> HairstyleChanged;
        event Payload<int> BeardChanged;
        event Payload<int> HairColorChanged;
        event Payload<int> BeardColorChanged;
        event Payload<int> SkinColorChanged;
        event Payload<Background> BackgroundSelected;
        event Payload<CharacterCreationEventData> Create;

        void Construct(List<Background> backgrounds, List<Color> hairColors, List<Color> skinColors, int hairstyleCount, int beardCount);
    }
}