using System;
using System.Collections.Generic;
using DarkBestiary.Events;
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
        event Action Cancel;
        event Action<int> HairstyleChanged;
        event Action<int> BeardChanged;
        event Action<int> HairColorChanged;
        event Action<int> BeardColorChanged;
        event Action<int> SkinColorChanged;
        event Action<Background> BackgroundSelected;
        event Action<CharacterCreationEventData> Create;

        void Construct(CharacterCreationViewContext context);
        void UpdateSkillSlots(List<SkillSlot> slots);
    }
}