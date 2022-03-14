using System.Collections.Generic;
using DarkBestiary.Items;

namespace DarkBestiary.Scenarios
{
    public struct ScenarioLoot
    {
        public int Experience { get; }
        public int SkillPoints { get; }
        public List<Item> Items { get; }

        public ScenarioLoot(int experience, int skillPoints, List<Item> items)
        {
            Experience = experience;
            SkillPoints = skillPoints;
            Items = items;
        }
    }
}