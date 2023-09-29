using System.Collections.Generic;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Items;
using UnityEngine;

namespace DarkBestiary.Testers
{
    public class LootDropTester : MonoBehaviour
    {
        [SerializeField]
        private int m_LootId;

        [SerializeField]
        private int m_Times;

        public List<Item> Test()
        {
            var items = new List<Item>();

            if (m_LootId > 0)
            {
                var loot = Container.Instance.Instantiate<Loot>();
                var data = Container.Instance.Resolve<ILootDataRepository>().FindOrFail(m_LootId);

                for (var i = 0; i < m_Times; i++)
                {
                    items.AddRange(loot.RollDrop(data));
                }
            }

            return items;
        }
    }
}