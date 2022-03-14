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
        [SerializeField] private InventoryItem itemPrefab;
        [SerializeField] private Transform itemContainer;
        [SerializeField] private GameObject legendaryParticles;
        [SerializeField] private GameObject mythicParticles;
        [SerializeField] private GameObject visionParticles;

        private MonoBehaviourPool<InventoryItem> itemPool;

        public void Initialize()
        {
            this.itemPool = MonoBehaviourPool<InventoryItem>.Factory(this.itemPrefab, this.itemContainer);
        }

        public void Terminate()
        {
            StopAllCoroutines();
            this.itemPool.Clear();
        }

        public void Simulate(List<Item> items)
        {
            StartCoroutine(LootCoroutine(items, 1));
        }

        public void Clear()
        {
            foreach (var inventoryItem in this.itemContainer.GetComponentsInChildren<InventoryItem>())
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

            var itemView = this.itemPool.Spawn();
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
                    Instantiate(this.legendaryParticles, itemView.transform).DestroyAsVisualEffect();
                    break;
                case RarityType.Mythic:
                    Instantiate(this.mythicParticles, itemView.transform).DestroyAsVisualEffect();
                    break;
                case RarityType.Vision:
                    Instantiate(this.visionParticles, itemView.transform).DestroyAsVisualEffect();
                    break;
            }
        }
    }
}