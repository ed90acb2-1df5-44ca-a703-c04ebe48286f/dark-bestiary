using DarkBestiary.Components;
using DarkBestiary.Events;
using DarkBestiary.Managers;
using UnityEngine;

namespace DarkBestiary.UI.Elements
{
    public class ItemPickupPopupManager : Singleton<ItemPickupPopupManager>
    {
        [SerializeField]
        private ItemPickupPopup m_ItemPopupPrefab = null!;

        private MonoBehaviourPool<ItemPickupPopup> m_Pool = null!;

        private void Start()
        {
            m_Pool = MonoBehaviourPool<ItemPickupPopup>.Factory(m_ItemPopupPrefab, UIManager.Instance.PopupContainer, 2);

            InventoryComponent.AnyItemPicked += OnItemPickup;
        }

        private void OnItemPickup(ItemPickupEventData data)
        {
            if (!Game.Instance.IsScenario)
            {
                return;
            }

            m_Pool.Spawn().Construct(data.Item);
        }
    }
}