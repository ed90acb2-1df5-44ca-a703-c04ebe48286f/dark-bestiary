using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Data.Mappers;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Events;
using DarkBestiary.Extensions;
using DarkBestiary.Managers;
using DarkBestiary.Skills;
using DarkBestiary.UI.Views;
using UnityEngine;

namespace DarkBestiary.UI.Controllers
{
    public class CharacterCreationViewController : ViewController<ICharacterCreationView>
    {
        private readonly ICharacterDataRepository m_CharacterDataRepository;
        private readonly IBackgroundRepository m_BackgroundRepository;
        private readonly ICharacterRepository m_CharacterRepository;
        private readonly CharacterMapper m_CharacterMapper;

        private ActorComponent m_Dummy;
        private Background m_Background;
        private int m_SkinColorIndex;
        private int m_HairColorIndex;
        private int m_BeardColorIndex;
        private int m_BeardIndex;
        private int m_HairstyleIndex;

        public CharacterCreationViewController(
            ICharacterCreationView view,
            ICharacterDataRepository characterDataRepository,
            IBackgroundRepository backgroundRepository,
            ICharacterRepository characterRepository,
            CharacterMapper characterMapper) : base(view)
        {
            m_CharacterDataRepository = characterDataRepository;
            m_BackgroundRepository = backgroundRepository;
            m_CharacterRepository = characterRepository;
            m_CharacterMapper = characterMapper;
        }

        protected override void OnInitialize()
        {
            var entity = m_CharacterMapper.ToEntity(new CharacterData { UnitId = 1 }).Entity;
            m_Dummy = entity.GetComponent<ActorComponent>();
            m_Dummy.transform.rotation = Quaternion.identity;
            m_Dummy.transform.position = new Vector3(0, -2, 0);
            m_Dummy.transform.localScale *= 3.0f;

            View.Create += OnCreate;
            View.Cancel += OnCancel;
            View.HairColorChanged += OnHairColorChanged;
            View.BeardColorChanged += OnBeardColorChanged;
            View.SkinColorChanged += OnSkinColorChanged;
            View.HairstyleChanged += OnHairstyleChanged;
            View.BeardChanged += OnBeardChanged;
            View.BackgroundSelected += OnBackgroundSelected;

            var customization = CharacterCustomizationValues.Instance;

            View.Construct(new CharacterCreationViewContext(
                    m_BackgroundRepository.FindAll(),
                    customization.HairColors,
                    customization.SkinColors,
                    customization.Hairstyles.Count,
                    customization.Beards.Count
                )
            );
        }

        protected override void OnTerminate()
        {
            m_Dummy.gameObject.Terminate();

            View.Create -= OnCreate;
            View.Cancel -= OnCancel;
            View.HairColorChanged -= OnHairColorChanged;
            View.SkinColorChanged -= OnSkinColorChanged;
            View.HairstyleChanged -= OnHairstyleChanged;
            View.BackgroundSelected -= OnBackgroundSelected;
        }

        private void OnHairstyleChanged(int index)
        {
            m_HairstyleIndex = index;
            m_Dummy.Model.ChangeHairstyle(
                Object.Instantiate(CharacterCustomizationValues.Instance.Hairstyles[index]));

            OnHairColorChanged(m_HairColorIndex);
        }

        private void OnBeardChanged(int index)
        {
            m_BeardIndex = index;
            m_Dummy.Model.ChangeBeard(
                Object.Instantiate(CharacterCustomizationValues.Instance.Beards[index]));

            OnBeardColorChanged(m_BeardColorIndex);
        }

        private void OnHairColorChanged(int index)
        {
            m_HairColorIndex = index;
            m_Dummy.Model.ChangeHairColor(CharacterCustomizationValues.Instance.HairColors[index]);
        }

        private void OnBeardColorChanged(int index)
        {
            m_BeardColorIndex = index;
            m_Dummy.Model.ChangeBeardColor(CharacterCustomizationValues.Instance.HairColors[index]);
        }

        private void OnSkinColorChanged(int index)
        {
            m_SkinColorIndex = index;
            m_Dummy.Model.ChangeSkinColor(CharacterCustomizationValues.Instance.SkinColors[index]);
        }

        private void OnBackgroundSelected(Background background)
        {
            m_Background?.Remove(m_Dummy.gameObject);
            m_Background = background;
            m_Background.Apply(m_Dummy.gameObject);

            var health = m_Dummy.GetComponent<HealthComponent>();
            health.Health = health.HealthMax;

            var commonSkillSlots = m_Dummy.gameObject.GetComponent<SpellbookComponent>().Slots.Where(s => !s.IsEmpty && s.SkillType == SkillType.Common).ToList();
            View.UpdateSkillSlots(commonSkillSlots);
        }

        private void OnCancel()
        {
            Game.Instance.ToMainMenu();
        }

        private void OnCreate(CharacterCreationEventData data)
        {
            Game.Instance.Character?.Entity.Terminate();

            InventoryComponent.IsAutoIngredientStashEnabled = true;

            var character = CreateCharacter(data);
            character.Entity.transform.position = new Vector3(-100, 0, 0);
            m_Background.Apply(character.Entity);

            Game.Instance.EnterTownWithNewCharacter(character);
        }

        private Character CreateCharacter(CharacterCreationEventData data)
        {
            return CreatePersistentCharacter(data);
        }

        private Character CreateTemporaryCharacter(CharacterCreationEventData data)
        {
            return m_CharacterMapper.ToEntity(new CharacterData
                {
                    UnitId = 1,
                    Name = data.Name,
                    HairstyleIndex = m_HairstyleIndex,
                    BeardIndex = m_BeardIndex,
                    SkinColorIndex = m_SkinColorIndex,
                    HairColorIndex = m_HairColorIndex,
                    BeardColorIndex = m_BeardColorIndex,
                    Attributes = new CharacterAttributeData { Points = 0 }
                }
            );
        }

        private Character CreatePersistentCharacter(CharacterCreationEventData data)
        {
            m_CharacterDataRepository.Save(new CharacterData
                {
                    UnitId = 1,
                    Name = data.Name,
                    HairstyleIndex = m_HairstyleIndex,
                    BeardIndex = m_BeardIndex,
                    SkinColorIndex = m_SkinColorIndex,
                    HairColorIndex = m_HairColorIndex,
                    BeardColorIndex = m_BeardColorIndex,
                    Attributes = new CharacterAttributeData { Points = 0 }
                }
            );

            var characterId = m_CharacterDataRepository
                .FindAll()
                .OrderByDescending(element => element.Id)
                .Select(element => element.Id)
                .First();

            return m_CharacterRepository.Find(characterId);
        }
    }
}