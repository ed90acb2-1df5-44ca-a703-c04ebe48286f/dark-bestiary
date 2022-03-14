using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Currencies;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Extensions;
using DarkBestiary.Managers;
using DarkBestiary.Masteries;
using DarkBestiary.Skills;
using UnityEngine;
using Component = DarkBestiary.Components.Component;
using Object = UnityEngine.Object;

namespace DarkBestiary.Data.Mappers
{
    public class CharacterMapper : Mapper<CharacterData, Character>
    {
        private readonly ICurrencyRepository currencyRepository;
        private readonly ISkillRepository skillRepository;
        private readonly IUnitRepository unitRepository;
        private readonly ITalentRepository talentRepository;
        private readonly IMasteryRepository masteryRepository;
        private readonly ISpecializationDataRepository specializationDataRepository;
        private readonly MasterySaveDataMapper masterySaveDataMapper;
        private readonly RelicSaveDataMapper relicSaveDataMapper;
        private readonly ItemSaveDataMapper itemSaveDataMapper;

        public CharacterMapper()
        {
            this.currencyRepository = Container.Instance.Resolve<ICurrencyRepository>();
            this.skillRepository = Container.Instance.Resolve<ISkillRepository>();
            this.unitRepository = Container.Instance.Resolve<IUnitRepository>();
            this.talentRepository = Container.Instance.Resolve<ITalentRepository>();
            this.masteryRepository = Container.Instance.Resolve<IMasteryRepository>();
            this.specializationDataRepository = Container.Instance.Resolve<ISpecializationDataRepository>();

            this.masterySaveDataMapper = Container.Instance.Resolve<MasterySaveDataMapper>();
            this.relicSaveDataMapper = Container.Instance.Resolve<RelicSaveDataMapper>();
            this.itemSaveDataMapper = Container.Instance.Resolve<ItemSaveDataMapper>();
        }

        public override CharacterData ToData(Character character)
        {
            var unitId = character.Entity.GetComponent<UnitComponent>().Id;

            var experienceComponent = character.Entity.GetComponent<ExperienceComponent>();
            var experience = 0;
            var level = 1;

            if (experienceComponent)
            {
                level = experienceComponent.Experience.Level;
                experience = experienceComponent.Experience.Current;
            }

            var inventoryComponent = character.Entity.GetComponent<InventoryComponent>();

            var items = new List<ItemSaveData>();

            if (inventoryComponent)
            {
                items = this.itemSaveDataMapper.ToData(inventoryComponent.Items.ToList());
            }

            var equipmentComponent = character.Entity.GetComponent<EquipmentComponent>();

            var equipment = new List<ItemSaveData>();
            var altWeaponSet = new List<ItemSaveData>();

            if (equipmentComponent)
            {
                equipment = this.itemSaveDataMapper.ToData(equipmentComponent.Slots.Select(slot => slot.Item).ToList());
                altWeaponSet = this.itemSaveDataMapper.ToData(equipmentComponent.AltWeaponSet);
            }

            var spellbookComponent = character.Entity.GetComponent<SpellbookComponent>();
            var unlockedSkills = new List<int>();
            var activeSkills = new List<CharacterActiveSkillData>();

            if (spellbookComponent)
            {
                unlockedSkills = spellbookComponent.Skills
                    .Tradable()
                    .Select(skill => skill.Id)
                    .ToList();

                activeSkills = spellbookComponent.Slots
                    .OrderBy(s => s.Index)
                    .Select(s => new CharacterActiveSkillData(
                        s.Index, s.Skill.IsTradable() ? s.Skill.Id : Skill.Empty.Id))
                    .ToList();
            }

            var talentsComponent = character.Entity.GetComponent<TalentsComponent>();
            var characterTalents = new CharacterTalentsData();

            if (talentsComponent)
            {
                characterTalents.Points = talentsComponent.Points;
                characterTalents.Learned = talentsComponent.Tiers
                    .SelectMany(tier => tier.Talents)
                    .Where(talent => talent.IsLearned)
                    .Select(talent => talent.Id)
                    .ToList();
            }

            var currenciesComponent = character.Entity.GetComponent<CurrenciesComponent>();
            var currencies = new List<CurrencyAmountData>();

            if (currenciesComponent)
            {
                foreach (var currency in currenciesComponent.Currencies)
                {
                    currencies.Add(new CurrencyAmountData()
                    {
                        CurrencyId = currency.Id,
                        Amount = currency.Amount,
                    });
                }
            }

            var attributesComponent = character.Entity.GetComponent<AttributesComponent>();
            var attributes = new CharacterAttributeData {Points = attributesComponent.Points};

            if (attributesComponent)
            {
                foreach (var attribute in attributesComponent.Attributes.Values)
                {
                    attributes.Attributes.Add(new SpentAttributePointsData
                    {
                        AttributeId = attribute.Id,
                        Points = attribute.Points
                    });
                }
            }

            var reliquary = character.Entity.GetComponent<ReliquaryComponent>();

            var relics = new CharacterRelicData
            {
                Active = this.relicSaveDataMapper.ToData(reliquary.Slots.Select(s => s.Relic).ToList()),
                Available = this.relicSaveDataMapper.ToData(reliquary.Available),
            };

            var specializationsComponent = character.Entity.GetComponent<SpecializationsComponent>();
            var specializations = specializationsComponent.Specializations
                .Select(specialization => new SpecializationSaveData(specialization))
                .ToList();

            var masteries = character.Entity.GetComponent<MasteriesComponent>().Masteries
                .Select(this.masterySaveDataMapper.ToData)
                .ToList();

            var characterData = new CharacterData
            {
                Id = character.Id,
                Name = character.Name,
                UnitId = unitId,
                Items = items,
                Equipment = equipment,
                AltWeapon = altWeaponSet,
                Level = level,
                IsStartScenarioCompleted = character.IsStartScenarioCompleted,
                Experience = experience,
                Currencies = currencies,
                UnlockedSkills = unlockedSkills,
                ActiveSkills = activeSkills,
                Talents = characterTalents,
                Attributes = attributes,
                AvailableScenarios = character.AvailableScenarios.Distinct().ToList(),
                CompletedScenarios = character.CompletedScenarios.Distinct().ToList(),
                Relics = relics,
                SkillPoints = specializationsComponent.SkillPoints,
                Specializations = specializations,
                Masteries = masteries,

                IsHelmHidden = !character.Entity.GetComponent<ActorComponent>().IsHelmVisible,

                Rerolls = character.Data.Rerolls,
                FreeSkills = character.Data.FreeSkills,
                HairstyleIndex = character.Data.HairstyleIndex,
                HairColorIndex = character.Data.HairColorIndex,
                SkinColorIndex = character.Data.SkinColorIndex,
                BeardColorIndex = character.Data.BeardColorIndex,
                BeardIndex = character.Data.BeardIndex,
                Timestamp = character.Data.Timestamp,
                UnlockedRecipes = character.Data.UnlockedRecipes,
                UnlockedMonsters = character.Data.UnlockedMonsters,
                ReadDialogues = character.Data.ReadDialogues,
            };

            return characterData;
        }

        public override Character ToEntity(CharacterData data)
        {
            var entity = this.unitRepository.Find(data.UnitId);

            var unit = entity.GetComponent<UnitComponent>();
            unit.Owner = Owner.Player;
            unit.TeamId = 1;
            unit.Name = new I18NString(new I18NStringData(data.Name));

            var components = new List<Component>
            {
                entity.AddComponent<ExperienceComponent>().Construct(data.Level, data.Experience),
                entity.AddComponent<CurrenciesComponent>().Construct(LoadCurrencies(data)),
                entity.AddComponent<MasteriesComponent>().Construct(LoadMasteries(data)),
                entity.AddComponent<InventoryComponent>().Construct(140),
                entity.GetOrAddComponent<EquipmentComponent>().Construct(),
                entity.AddComponent<ReliquaryComponent>().Construct(),
                entity.AddComponent<FoodComponent>().Construct(),
                entity.AddComponent<SpecializationsComponent>().Construct(LoadSpecializations(data), data.SkillPoints),
                entity.AddComponent<TalentsComponent>().Construct(
                    this.talentRepository.Find(t => t.CategoryId != Constants.TalentCategoryIdDreams),
                    data.Talents.Learned,
                    data.Talents.Points
                ),
            };

            components.ForEach(component => component.Initialize());

            GiveRelics(data, entity);
            GiveAttributes(data, entity);
            GiveAppearance(data, entity);
            GiveSkills(data, entity);
            GiveItems(data, entity);
            GiveEquipment(data, entity);

            var health = entity.GetComponent<HealthComponent>();
            health.Health = health.HealthMax;

            var actor = entity.GetComponent<ActorComponent>();
            actor.SetHelmVisible(!data.IsHelmHidden);
            actor.Model.FlipX(false);

            return new Character(data, entity);
        }

        private void GiveAppearance(CharacterData data, GameObject entity)
        {
            var actor = entity.GetComponent<ActorComponent>();

            actor.Model.ChangeHairstyle(
                Object.Instantiate(CharacterCustomizationValues.Instance.Hairstyles[data.HairstyleIndex]));
            actor.Model.ChangeHairColor(CharacterCustomizationValues.Instance.HairColors[data.HairColorIndex]);
            actor.Model.ChangeSkinColor(CharacterCustomizationValues.Instance.SkinColors[data.SkinColorIndex]);
            actor.Model.ChangeBeard(
                Object.Instantiate(CharacterCustomizationValues.Instance.Beards[data.BeardIndex]));
            actor.Model.ChangeBeardColor(CharacterCustomizationValues.Instance.HairColors[data.BeardColorIndex]);
        }

        private void GiveAttributes(CharacterData data, GameObject entity)
        {
            var attributes = entity.GetComponent<AttributesComponent>();

            attributes.Points = data.Attributes.Points;

            foreach (var attributePointsData in data.Attributes.Attributes)
            {
                attributes.Attributes.Values.FirstOrDefault(a => a.Id == attributePointsData.AttributeId)?.AddPoint(attributePointsData.Points);
            }
        }

        private void GiveSkills(CharacterData data, GameObject entity)
        {
            var spellbookComponent = entity.GetComponent<SpellbookComponent>();

            spellbookComponent.Add(
                this.skillRepository.Find(data.UnlockedSkills).Where(s => s.Type == SkillType.Common));

            foreach (var active in data.ActiveSkills)
            {
                var skill = spellbookComponent.Find(active.SkillId);

                if (skill == null)
                {
                    continue;
                }

                try
                {
                    spellbookComponent.PlaceOnActionBar(spellbookComponent.Slots[active.Index], skill);
                }
                catch (Exception exception)
                {
                    Debug.LogError($"Can't learn skill {skill.Name} of type {skill.Type}. Message: {exception.Message}");
                }
            }
        }

        private void GiveRelics(CharacterData data, GameObject entity)
        {
            var reliquary = entity.GetComponent<ReliquaryComponent>();

            foreach (var relic in this.relicSaveDataMapper.ToEntity(data.Relics.Available))
            {
                reliquary.UnlockSilently(relic);

                if (data.Relics.Active.Any(r => r.RelicId == relic.Id))
                {
                    reliquary.Equip(relic);
                }
            }
        }

        private void GiveEquipment(CharacterData data, GameObject entity)
        {
            var equipment = entity.GetComponent<EquipmentComponent>();
            var inventory = entity.GetComponent<InventoryComponent>();

            foreach (var item in this.itemSaveDataMapper.ToEntity(data.Equipment))
            {
                if (item.IsEmpty)
                {
                    continue;
                }

                item.Inventory = inventory;

                foreach (var rune in item.Runes)
                {
                    rune.Inventory = inventory;
                }

                equipment.Equip(item);

                if (!equipment.IsEquipped(item))
                {
                    inventory.Pickup(item);
                }
            }

            equipment.SetAltWeaponSet(data.AltWeapon.Select(this.itemSaveDataMapper.ToEntity).ToList());
        }

        private void GiveItems(CharacterData data, GameObject entity)
        {
            var inventory = entity.GetComponent<InventoryComponent>();

            for (var i = 0; i < data.Items.Count; i++)
            {
                var item = this.itemSaveDataMapper.ToEntity(data.Items[i]);

                try
                {
                    inventory.PickupDoNotStack(item, inventory.Items[i]);
                }
                catch (Exception exception)
                {
                    inventory.PickupDoNotStack(item);
                }
            }
        }

        private List<Specialization> LoadSpecializations(CharacterData characterData)
        {
            var specializations = new List<Specialization>();

            foreach (var data in this.specializationDataRepository.FindAll())
            {
                var specialization = new Specialization(data);
                specializations.Add(specialization);

                var specializationSaveData = characterData.Specializations.FirstOrDefault(x => x.SpecializationId == data.Id);

                if (specializationSaveData == null)
                {
                    specialization.Construct(1, 0);
                    continue;
                }

                specialization.Construct(specializationSaveData.Level, specializationSaveData.Experience);
            }

            return specializations;
        }

        private List<Mastery> LoadMasteries(CharacterData characterData)
        {
            var masteries = new List<Mastery>();

            foreach (var mastery in this.masteryRepository.FindAll())
            {
                var masteryData = characterData.Masteries.FirstOrDefault(data => data.MasteryId == mastery.Id);

                if (masteryData == null)
                {
                    mastery.Construct(1, 0);
                }
                else
                {
                    mastery.Construct(masteryData.Level, masteryData.Experience);
                }


                masteries.Add(mastery);
            }

            return masteries;
        }

        private List<Currency> LoadCurrencies(CharacterData characterData)
        {
            var currencies = new List<Currency>();

            foreach (var currency in this.currencyRepository.FindAll())
            {
                var characterCurrencyData = characterData.Currencies.FirstOrDefault(data => data.CurrencyId == currency.Id);
                var characterCurrencyValue = characterCurrencyData?.Amount ?? 0;

                currency.Add(characterCurrencyValue);
                currencies.Add(currency);
            }

            return currencies;
        }
    }
}