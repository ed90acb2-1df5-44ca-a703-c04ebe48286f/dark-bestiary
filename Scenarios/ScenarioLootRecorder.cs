using System.Collections.Generic;
using DarkBestiary.Components;
using DarkBestiary.Extensions;
using DarkBestiary.Items;
using DarkBestiary.Messaging;

namespace DarkBestiary.Scenarios
{
    public class ScenarioLootRecorder
    {
        private readonly List<int> experience = new List<int>();
        private readonly List<Item> items = new List<Item>();

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
            return new ScenarioLoot(this.experience, this.items);
        }

        private void OnAnyEntityDied(EntityDiedEventData data)
        {
            if (data.Victim.IsSummoned() || !data.Killer.IsAllyOfPlayer() || !data.Victim.IsEnemyOf(data.Killer))
            {
                return;
            }

            var killExperience = data.Victim.GetComponent<UnitComponent>().GetKillExperience();

            this.experience.Add(killExperience);
            this.items.AddRange(data.Victim.GetComponent<LootComponent>().RollDrop());
        }
    }
}