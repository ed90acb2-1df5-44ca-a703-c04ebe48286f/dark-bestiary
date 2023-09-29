using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Extensions;
using DarkBestiary.Items;
using DarkBestiary.Managers;
using UnityEngine;

namespace DarkBestiary.UI.Elements
{
    public class VictoryPanelLoot : MonoBehaviour
    {
        [SerializeField] private InventoryItem m_ItemPrefab;
        [SerializeField] private Transform m_ItemContainer;
        [SerializeField] private GameObject m_LegendaryParticles;
        [SerializeField] private GameObject m_MythicParticles;
        [SerializeField] private GameObject m_VisionParticles;

        private MonoBehaviourPool<InventoryItem> m_ItemPool;

        public void Initialize()
        {
            m_ItemPool = MonoBehaviourPool<InventoryItem>.Factory(m_ItemPrefab, m_ItemContainer);
        }

        public void Terminate()
        {
            StopAllCoroutines();
            m_ItemPool.Clear();
        }

        public void Simulate(List<Item> items)
        {
            StartCoroutine(LootCoroutine(items, 1));
        }

        public void Clear()
        {
            foreach (var inventoryItem in m_ItemContainer.GetComponentsInChildren<InventoryItem>())
            {
                Destroy(inventoryItem.gameObject);
            }
        }

        private IEnumerator LootCoroutine(List<Item> items, float duration)
        {
            foreach (var group in items.OrderBy(item => item.Rarity.Type).GroupBy(item => item.Id))
            {
                var first = group.First();

                if (first.IsStackable)
                {
                    CreateItemView(first, group.Sum(i => i.StackCount));
                }
                else
                {
                    foreach (var item in group)
                    {
                        CreateItemView(item, 1);
                    }
                }

                yield return new WaitForSeconds(duration / items.Count);
            }
        }

        private void CreateItemView(Item item, int count)
        {
            AudioManager.Instance.PlayWhoosh();

            var itemView = m_ItemPool.Spawn();
            itemView.Change(item, false);
            itemView.OverwriteStackCount(count);
            itemView.IsDraggable = false;

            if (item.IsIngredient)
            {
                return;
            }

            switch (item.Rarity.Type)
            {
                case RarityType.Legendary:
                    Instantiate(m_LegendaryParticles, itemView.transform).DestroyAsVisualEffect();
                    break;
                case RarityType.Mythic:
                    Instantiate(m_MythicParticles, itemView.transform).DestroyAsVisualEffect();
                    break;
                case RarityType.Vision:
                    Instantiate(m_VisionParticles, itemView.transform).DestroyAsVisualEffect();
                    break;
            }
        }
    }
}