using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Extensions;
using DarkBestiary.Items;
using DarkBestiary.Messaging;

namespace DarkBestiary.Scenarios
{
    public class ScenarioLootRecorder
    {
        private readonly List<Item> items = new List<Item>();
        private readonly Scenario scenario;
        private int skillPoints;
        private int experience;

        public ScenarioLootRecorder(Scenario scenario)
        {
            this.scenario = scenario;
        }

        public void Start()
        {
            HealthComponent.AnyEntityDied += OnAnyEntityDied;
        }

        public void Stop()
        {
            HealthComponent.AnyEntityDied -= OnAnyEntityDied;
        }

        public ScenarioLoot GetResult()
        {
            return new ScenarioLoot(this.experience, this.skillPoints, this.items);
        }

        public void AddExperience(int amount)
        {
            this.experience += amount;
        }

        public void AddItems(IEnumerable<Item> items)
        {
            this.items.AddRange(items);
        }

        private void OnAnyEntityDied(EntityDiedEventData data)
        {
            if (data.Victim.IsSummoned() || !data.Killer.IsAllyOfPlayer() || !data.Victim.IsEnemyOf(data.Killer))
            {
                return;
            }

            var unit = data.Victim.GetComponent<UnitComponent>();

            var killExperience = unit.GetKillExperience();
            var killSkillPoints = unit.ChallengeRating * 3;

            this.skillPoints += killSkillPoints;
            this.experience += killExperience;

            var loot = data.Victim.GetComponent<LootComponent>().RollDrop();

            if (this.scenario.IsAscension)
            {
                loot = loot.Where(item => item.Rarity.Type > RarityType.Rare || item.Type.Type == ItemTypeType.Ingredient || item.Type.Type == ItemTypeType.Enchantment).ToList();
            }

            this.items.AddRange(loot);
        }
    }
}