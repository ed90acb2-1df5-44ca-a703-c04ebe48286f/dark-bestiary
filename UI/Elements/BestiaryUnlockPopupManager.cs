using DarkBestiary.Components;
using DarkBestiary.Managers;
using UnityEngine;

namespace DarkBestiary.UI.Elements
{
    public class BestiaryUnlockPopupManager : Singleton<BestiaryUnlockPopupManager>
    {
        [SerializeField] private BestiaryUnlockPopup popupPrefab;

        private MonoBehaviourPool<BestiaryUnlockPopup> pool;

        private void Start()
        {
            this.pool = MonoBehaviourPool<BestiaryUnlockPopup>.Factory(
                this.popupPrefab, UIManager.Instance.PopupContainer, 2);

            CharacterManager.BestiaryUpdated += OnBestiaryUpdated;
        }

        private void OnBestiaryUpdated(UnitComponent unit)
        {
            this.pool.Spawn().Construct(unit);
        }
    }
}