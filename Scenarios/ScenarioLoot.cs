using System.Collections.Generic;
using DarkBestiary.Items;

namespace DarkBestiary.Scenarios
{
    public struct ScenarioLoot
    {
        public List<int> Experience { get; }
        public List<Item> Items { get; }

        public ScenarioLoot(List<int> experience, List<Item> items)
        {
            Experience = experience;
            Items = items;
        }
    }
}