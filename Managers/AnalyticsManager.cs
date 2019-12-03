using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Analytics;
using DarkBestiary.Components;
using DarkBestiary.Extensions;
using DarkBestiary.Messaging;
using DarkBestiary.Scenarios;
using Zenject;

namespace DarkBestiary.Managers
{
    public class AnalyticsManager : IInitializable
    {
        private readonly IAnalyticsService analytics;

        public AnalyticsManager(IAnalyticsService analytics)
        {
            this.analytics = analytics;
        }

        public void Initialize()
        {
            HealthComponent.AnyEntityDied += OnAnyEntityDied;
            Scenario.AnyScenarioCompleted += OnAnyScenarioCompleted;
        }

        private void OnAnyEntityDied(EntityDiedEventData data)
        {
            if (!data.Victim.IsCharacter())
            {
                return;
            }

            var killer = data.Killer.GetComponent<UnitComponent>();
            var scenario = Scenario.Active;

            this.analytics.Event("db_character_died", new Dictionary<string, object>
            {
                {"Scenario", scenario.Id},
                {"CharacterLevel", data.Victim.GetComponent<ExperienceComponent>().Experience.Level},

                {"Monster", killer.Id},
                {"MonsterLevel", killer.Level},

                {"Equipment", GetCharacterEquipmentInfo()},
                {"Skills", GetCharacterSkillInfo()},
                {"Talents", GetCharacterTalentInfo()},
                {"Relics", GetCharacterRelicInfo()},
            });
        }

        private void OnAnyScenarioCompleted(Scenario scenario)
        {
            this.analytics.Event("db_scenario_completed", new Dictionary<string, object>
            {
                {"Scenario", scenario.Id},
                {"CharacterLevel", CharacterManager.Instance.Character.Entity.GetComponent<ExperienceComponent>().Experience.Level},

                {"Equipment", GetCharacterEquipmentInfo()},
                {"Skills", GetCharacterSkillInfo()},
                {"Talents", GetCharacterTalentInfo()},
                {"Relics", GetCharacterRelicInfo()},
            });
        }

        private Dictionary<string, Dictionary<string, object>> GetCharacterEquipmentInfo()
        {
            var equipment = new Dictionary<string, Dictionary<string, object>>();

            foreach (var slot in CharacterManager.Instance.Character.Entity.GetComponent<EquipmentComponent>().Slots)
            {
                equipment.Add(slot.Label, new Dictionary<string, object>
                {
                    {"Id", slot.Item.Id},
                    {"SuffixId", slot.Item.Suffix?.Id},
                    {"ForgeLevel", slot.Item.ForgeLevel},
                    {"Sockets", slot.Item.Sockets.Select(i => i.Id).ToArray()},
                });
            }

            return equipment;
        }

        private List<int> GetCharacterTalentInfo()
        {
            var talents = new List<int>();

            foreach (var tier in CharacterManager.Instance.Character.Entity.GetComponent<TalentsComponent>().Tiers)
            {
                foreach (var talent in tier.Talents)
                {
                    if (!talent.IsLearned)
                    {
                        continue;
                    }

                    talents.Add(talent.Id);
                }
            }

            return talents;
        }

        private List<int> GetCharacterSkillInfo()
        {
            var skills = new List<int>();

            foreach (var slot in CharacterManager.Instance.Character.Entity.GetComponent<SpellbookComponent>().Slots)
            {
                skills.Add(slot.Skill.Id);
            }

            return skills;
        }

        private List<int> GetCharacterRelicInfo()
        {
            var relics = new List<int>();

            foreach (var slot in CharacterManager.Instance.Character.Entity.GetComponent<ReliquaryComponent>().Slots)
            {
                relics.Add(slot.Relic.Id);
            }

            return relics;
        }
    }
}