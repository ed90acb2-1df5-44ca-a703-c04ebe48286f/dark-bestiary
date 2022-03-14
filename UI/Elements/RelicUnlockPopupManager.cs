using DarkBestiary.Components;
using DarkBestiary.Items;
using DarkBestiary.Managers;
using UnityEngine;

namespace DarkBestiary.UI.Elements
{
    public class RelicUnlockPopupManager : Singleton<RelicUnlockPopupManager>
    {
        [SerializeField] private ItemPickupPopup popupPrefab;

        private MonoBehaviourPool<ItemPickupPopup> pool;

        private void Start()
        {
            this.pool = MonoBehaviourPool<ItemPickupPopup>.Factory(
                this.popupPrefab, UIManager.Instance.PopupContainer, 2);

            ReliquaryComponent.AnyRelicUnlocked += OnAnyRelicUnlocked;
        }

        private void OnAnyRelicUnlocked(ReliquaryComponent reliquary, Relic relic)
        {
            this.pool.Spawn().Construct(relic);
        }
    }
}