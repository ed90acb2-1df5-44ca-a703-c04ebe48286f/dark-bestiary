using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Events;
using DarkBestiary.Extensions;
using DarkBestiary.Items;

namespace DarkBestiary.Scenarios
{
    public class ScenarioLootRecorder
    {
        private readonly List<Item> m_Items = new();
        private readonly Scenario m_Scenario;
        private int m_Experience;

        public ScenarioLootRecorder(Scenario scenario)
        {
            m_Scenario = scenario;
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
            return new ScenarioLoot(m_Experience, m_Items);
        }

        public void AddExperience(int amount)
        {
            m_Experience += amount;
        }

        public void AddItems(IEnumerable<Item> items)
        {
            m_Items.AddRange(items);
        }

        private void OnAnyEntityDied(EntityDiedEventData data)
        {
            if (data.Victim.IsSummoned() || !data.Killer.IsAllyOfPlayer() || !data.Victim.IsEnemyOf(data.Killer))
            {
                return;
            }

            var unit = data.Victim.GetComponent<UnitComponent>();

            var killExperience = unit.GetKillExperience();
            m_Experience += killExperience;

            var loot = data.Victim.GetComponent<LootComponent>().RollDrop();

            if (m_Scenario.IsAscension)
            {
                loot = loot.Where(item => item.Rarity.Type > RarityType.Rare || item.Type.Type == ItemTypeType.Ingredient || item.Type.Type == ItemTypeType.Enchantment).ToList();
            }

            m_Items.AddRange(loot);
        }
    }
}