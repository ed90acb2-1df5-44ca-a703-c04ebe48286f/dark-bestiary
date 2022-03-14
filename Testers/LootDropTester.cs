using System.Collections.Generic;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Items;
using UnityEngine;

namespace DarkBestiary.Testers
{
    public class LootDropTester : MonoBehaviour
    {
        [SerializeField] private int scenarioId;

        [Header("Single Table")]
        [SerializeField] private int monsterLevel;
        [SerializeField] private int lootId;
        [SerializeField] private int times;

        public List<Item> Test()
        {
            var items = new List<Item>();

            var lootDataRepository = Container.Instance.Resolve<ILootDataRepository>();

            var loot = lootDataRepository.Find(this.lootId);
            var scenario = Container.Instance.Resolve<IScenarioDataRepository>().Find(this.scenarioId);

            if (loot != null)
            {
                for (var i = 0; i < this.times; i++)
                {
                    foreach (var item in Container.Instance.Instantiate<Loot>(
                        new object[] {loot}).RollDrop(this.monsterLevel))
                    {
                        items.Add(item);
                    }
                }
            }

            if (scenario != null)
            {
                var unitRepository = Container.Instance.Resolve<IUnitDataRepository>();

                foreach (var episode in scenario.Episodes)
                {
                    foreach (var unitData in episode.Encounter.UnitTable.Units)
                    {
                        var unit = unitRepository.Find(unitData.UnitId);

                        foreach (var lootData in lootDataRepository.Find(unit.Loot))
                        {
                            items.AddRange(Container.Instance.Instantiate<Loot>(
                                new object[] {lootData}).RollDrop(this.monsterLevel));
                        }
                    }
                }
            }

            return items;
        }
    }
}