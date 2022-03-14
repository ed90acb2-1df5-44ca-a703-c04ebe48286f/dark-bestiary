using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Data.Mappers;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Extensions;
using DarkBestiary.GameStates;
using DarkBestiary.Managers;
using DarkBestiary.Messaging;
using DarkBestiary.Skills;
using DarkBestiary.UI.Views;
using UnityEngine;

namespace DarkBestiary.UI.Controllers
{
    public class CharacterCreationViewController : ViewController<ICharacterCreationView>
    {
        private readonly ICharacterDataRepository characterDataRepository;
        private readonly IBackgroundRepository backgroundRepository;
        private readonly ICharacterRepository characterRepository;
        private readonly CharacterMapper characterMapper;
        private readonly CharacterManager characterManager;

        private ActorComponent dummy;
        private Background background;
        private int skinColorIndex;
        private int hairColorIndex;
        private int beardColorIndex;
        private int beardIndex;
        private int hairstyleIndex;

        public CharacterCreationViewController(
            ICharacterCreationView view,
            ICharacterDataRepository characterDataRepository,
            IBackgroundRepository backgroundRepository,
            ICharacterRepository characterRepository,
            CharacterMapper characterMapper,
            CharacterManager characterManager) : base(view)
        {
            this.characterDataRepository = characterDataRepository;
            this.backgroundRepository = backgroundRepository;
            this.characterRepository = characterRepository;
            this.characterMapper = characterMapper;
            this.characterManager = characterManager;
        }

        protected override void OnInitialize()
        {
            var entity = this.characterMapper.ToEntity(new CharacterData {UnitId = 1}).Entity;
            this.dummy = entity.GetComponent<ActorComponent>();
            this.dummy.transform.rotation = Quaternion.identity;
            this.dummy.transform.position = new Vector3(0, -2, 0);
            this.dummy.transform.localScale *= 3.0f;

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
                    this.backgroundRepository.FindAll(),
                    customization.HairColors,
                    customization.SkinColors,
                    customization.Hairstyles.Count,
                    customization.Beards.Count
                )
            );
        }

        protected override void OnTerminate()
        {
            this.dummy.gameObject.Terminate();

            View.Create -= OnCreate;
            View.Cancel -= OnCancel;
            View.HairColorChanged -= OnHairColorChanged;
            View.SkinColorChanged -= OnSkinColorChanged;
            View.HairstyleChanged -= OnHairstyleChanged;
            View.BackgroundSelected -= OnBackgroundSelected;
        }

        private void OnHairstyleChanged(int index)
        {
            this.hairstyleIndex = index;
            this.dummy.Model.ChangeHairstyle(
                Object.Instantiate(CharacterCustomizationValues.Instance.Hairstyles[index]));

            OnHairColorChanged(this.hairColorIndex);
        }

        private void OnBeardChanged(int index)
        {
            this.beardIndex = index;
            this.dummy.Model.ChangeBeard(
                Object.Instantiate(CharacterCustomizationValues.Instance.Beards[index]));

            OnBeardColorChanged(this.beardColorIndex);
        }

        private void OnHairColorChanged(int index)
        {
            this.hairColorIndex = index;
            this.dummy.Model.ChangeHairColor(CharacterCustomizationValues.Instance.HairColors[index]);
        }

        private void OnBeardColorChanged(int index)
        {
            this.beardColorIndex = index;
            this.dummy.Model.ChangeBeardColor(CharacterCustomizationValues.Instance.HairColors[index]);
        }

        private void OnSkinColorChanged(int index)
        {
            this.skinColorIndex = index;
            this.dummy.Model.ChangeSkinColor(CharacterCustomizationValues.Instance.SkinColors[index]);
        }

        private void OnBackgroundSelected(Background background)
        {
            this.background?.Remove(this.dummy.gameObject);
            this.background = background;
            this.background.Apply(this.dummy.gameObject);

            var health = this.dummy.GetComponent<HealthComponent>();
            health.Health = health.HealthMax;

            var commonSkillSlots = this.dummy.gameObject.GetComponent<SpellbookComponent>().Slots.Where(s => !s.IsEmpty && s.SkillType == SkillType.Common).ToList();
            View.UpdateSkillSlots(commonSkillSlots);
        }

        private void OnCancel()
        {
            Game.Instance.SwitchState(new MainMenuGameState());
        }

        private void OnCreate(CharacterCreationEventData data)
        {
            this.characterManager.Character?.Entity.Terminate();

            InventoryComponent.IsAutoIngredientStashEnabled = true;
            this.characterManager.Select(CreateCharacter(data));

            this.characterManager.Character.Entity.transform.position = new Vector3(-100, 0, 0);
            this.background.Apply(this.characterManager.Character.Entity);

            ProceedToNextGameState();
        }

        private Character CreateCharacter(CharacterCreationEventData data)
        {
            return Game.Instance.IsVisions
                ? CreateTemporaryCharacter(data)
                : CreatePersistentCharacter(data);
        }

        private Character CreateTemporaryCharacter(CharacterCreationEventData data)
        {
            return this.characterMapper.ToEntity(new CharacterData
                {
                    UnitId = 1,
                    Name = data.Name,
                    HairstyleIndex = this.hairstyleIndex,
                    BeardIndex = this.beardIndex,
                    SkinColorIndex = this.skinColorIndex,
                    HairColorIndex = this.hairColorIndex,
                    BeardColorIndex = this.beardColorIndex,
                    Attributes = new CharacterAttributeData {Points = 0}
                }
            );
        }

        private Character CreatePersistentCharacter(CharacterCreationEventData data)
        {
            this.characterDataRepository.Save(new CharacterData
                {
                    UnitId = 1,
                    Name = data.Name,
                    HairstyleIndex = this.hairstyleIndex,
                    BeardIndex = this.beardIndex,
                    SkinColorIndex = this.skinColorIndex,
                    HairColorIndex = this.hairColorIndex,
                    BeardColorIndex = this.beardColorIndex,
                    Attributes = new CharacterAttributeData {Points = 0}
                }
            );

            var characterId = this.characterDataRepository
                .FindAll()
                .OrderByDescending(element => element.Id)
                .Select(element => element.Id)
                .First();

            return this.characterRepository.Find(characterId);
        }

        private static void ProceedToNextGameState()
        {
            if (Game.Instance.IsCampaign)
            {
                Game.Instance.ToIntro();
            }
            else
            {
                Game.Instance.ToVisionTalents();
            }
        }
    }
}