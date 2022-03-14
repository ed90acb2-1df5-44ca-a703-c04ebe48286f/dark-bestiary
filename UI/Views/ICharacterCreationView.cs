using System.Collections.Generic;
using DarkBestiary.Messaging;
using DarkBestiary.Skills;
using UnityEngine;

namespace DarkBestiary.UI.Views
{
    public struct CharacterCreationViewContext
    {
        public List<Background> Backgrounds { get; }
        public List<Color> HairColors { get; }
        public List<Color> SkinColors { get; }
        public int HairstyleCount { get; }
        public int BeardCount { get; }

        public CharacterCreationViewContext(
            List<Background> backgrounds,
            List<Color> hairColors,
            List<Color> skinColors,
            int hairstyleCount,
            int beardCount)
        {
            Backgrounds = backgrounds;
            HairColors = hairColors;
            SkinColors = skinColors;
            HairstyleCount = hairstyleCount;
            BeardCount = beardCount;
        }
    }

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

        void Construct(CharacterCreationViewContext context);
        void UpdateSkillSlots(List<SkillSlot> slots);
    }
}