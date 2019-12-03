using DarkBestiary.Components;
using DarkBestiary.Managers;
using DarkBestiary.Messaging;
using UnityEngine;

namespace DarkBestiary.UI.Elements
{
    public class ItemPickupPopupManager : Singleton<ItemPickupPopupManager>
    {
        [SerializeField] private ItemPickupPopup itemPopupPrefab;

        private MonoBehaviourPool<ItemPickupPopup> pool;

        private void Start()
        {
            this.pool = MonoBehaviourPool<ItemPickupPopup>.Factory(
                this.itemPopupPrefab, UIManager.Instance.PopupContainer, 2);

            InventoryComponent.AnyItemPicked += OnItemPickup;
        }

        private void OnItemPickup(ItemPickupEventData data)
        {
            if (Game.Instance == null || !Game.Instance.State.IsScenario)
            {
                return;
            }

            this.pool.Spawn().Construct(data.Item);
        }

        public void Clear()
        {
            this.pool.Clear();
        }
    }
}