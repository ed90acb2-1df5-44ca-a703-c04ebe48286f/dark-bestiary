using DarkBestiary.Components;
using DarkBestiary.Items;
using DarkBestiary.Managers;
using UnityEngine;

namespace DarkBestiary.UI.Elements
{
    public class RelicUnlockPopupManager : Singleton<RelicUnlockPopupManager>
    {
        [SerializeField] private ItemPickupPopup m_PopupPrefab;

        private MonoBehaviourPool<ItemPickupPopup> m_Pool;

        private void Start()
        {
            m_Pool = MonoBehaviourPool<ItemPickupPopup>.Factory(
                m_PopupPrefab, UIManager.Instance.PopupContainer, 2);

            ReliquaryComponent.AnyRelicUnlocked += OnAnyRelicUnlocked;
        }

        private void OnAnyRelicUnlocked(ReliquaryComponent reliquary, Relic relic)
        {
            m_Pool.Spawn().Construct(relic);
        }
    }
}