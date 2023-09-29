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
        private readonly ICurrencyRepository m_CurrencyRepository;
        private readonly ISkillRepository m_SkillRepository;
        private readonly IUnitRepository m_UnitRepository;
        private readonly ITalentRepository m_TalentRepository;
        private readonly IMasteryRepository m_MasteryRepository;
        private readonly MasterySaveDataMapper m_MasterySaveDataMapper;
        private readonly RelicSaveDataMapper m_RelicSaveDataMapper;
        private readonly ItemSaveDataMapper m_ItemSaveDataMapper;

        public CharacterMapper()
        {
            m_CurrencyRepository = Container.Instance.Resolve<ICurrencyRepository>();
            m_SkillRepository = Container.Instance.Resolve<ISkillRepository>();
            m_UnitRepository = Container.Instance.Resolve<IUnitRepository>();
            m_TalentRepository = Container.Instance.Resolve<ITalentRepository>();
            m_MasteryRepository = Container.Instance.Resolve<IMasteryRepository>();

            m_MasterySaveDataMapper = Container.Instance.Resolve<MasterySaveDataMapper>();
            m_RelicSaveDataMapper = Container.Instance.Resolve<RelicSaveDataMapper>();
            m_ItemSaveDataMapper = Container.Instance.Resolve<ItemSaveDataMapper>();
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
                items = m_ItemSaveDataMapper.ToData(inventoryComponent.Items.ToList());
            }

            var equipmentComponent = character.Entity.GetComponent<EquipmentComponent>();

            var equipment = new List<ItemSaveData>();
            var altWeaponSet = new List<ItemSaveData>();

            if (equipmentComponent)
            {
                equipment = m_ItemSaveDataMapper.ToData(equipmentComponent.Slots.Select(slot => slot.Item).ToList());
                altWeaponSet = m_ItemSaveDataMapper.ToData(equipmentComponent.AltWeaponSet);
            }

            var spellbookComponent = character.Entity.GetComponent<SpellbookComponent>();
            var activeSkills = new List<CharacterActiveSkillData>();

            if (spellbookComponent)
            {
                activeSkills = spellbookComponent.Slots
                    .OrderBy(s => s.Index)
                    .Select(s => new CharacterActiveSkillData(s.Index, s.Skill.IsLearnable() ? s.Skill.Id : Skill.s_Empty.Id))
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
            var attributes = new CharacterAttributeData { Points = attributesComponent.Points };

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
                Active = m_RelicSaveDataMapper.ToData(reliquary.Slots.Select(s => s.Relic).ToList()),
                Available = m_RelicSaveDataMapper.ToData(reliquary.Available),
            };

            var masteries = character.Entity.GetComponent<MasteriesComponent>().Masteries
                .Select(m_MasterySaveDataMapper.ToData)
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
                Experience = experience,
                Currencies = currencies,
                ActiveSkills = activeSkills,
                Talents = characterTalents,
                Attributes = attributes,
                Relics = relics,
                Masteries = masteries,

                IsHelmHidden = !character.Entity.GetComponent<ActorComponent>().IsHelmVisible,

                HairstyleIndex = character.Data.HairstyleIndex,
                HairColorIndex = character.Data.HairColorIndex,
                SkinColorIndex = character.Data.SkinColorIndex,
                BeardColorIndex = character.Data.BeardColorIndex,
                BeardIndex = character.Data.BeardIndex,
                Timestamp = character.Data.Timestamp,
                UnlockedMonsters = character.Data.UnlockedMonsters,

                Map = character.Data.Map,
            };

            return characterData;
        }

        public override Character ToEntity(CharacterData data)
        {
            var entity = m_UnitRepository.Find(data.UnitId);

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
                entity.AddComponent<TalentsComponent>().Construct(
                    m_TalentRepository.Find(t => t.CategoryId != Constants.c_TalentCategoryIdDreams),
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

            foreach (var active in data.ActiveSkills)
            {
                var skill = m_SkillRepository.Find(active.SkillId);

                if (skill == null)
                {
                    continue;
                }

                try
                {
                    spellbookComponent.Learn(spellbookComponent.Slots[active.Index], skill);
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

            foreach (var relic in m_RelicSaveDataMapper.ToEntity(data.Relics.Available))
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

            foreach (var item in m_ItemSaveDataMapper.ToEntity(data.Equipment))
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

            equipment.SetAltWeaponSet(m_ItemSaveDataMapper.ToEntity(data.AltWeapon));
        }

        private void GiveItems(CharacterData data, GameObject entity)
        {
            var inventory = entity.GetComponent<InventoryComponent>();

            for (var i = 0; i < data.Items.Count; i++)
            {
                try
                {
                    var item = m_ItemSaveDataMapper.ToEntity(data.Items[i]);

                    try
                    {
                        inventory.PickupDoNotStack(item, inventory.Items[i]);
                    }
                    catch (Exception)
                    {
                        inventory.PickupDoNotStack(item);
                    }
                }
                catch (Exception)
                {
                    // ignored
                }
            }
        }

        private List<Mastery> LoadMasteries(CharacterData characterData)
        {
            var masteries = new List<Mastery>();

            foreach (var mastery in m_MasteryRepository.FindAll())
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

            foreach (var currency in m_CurrencyRepository.FindAll())
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