using DarkBestiary.Managers;
using DarkBestiary.Masteries;
using UnityEngine;

namespace DarkBestiary.UI.Elements
{
    public class MasteryLevelupPopupManager : Singleton<MasteryLevelupPopupManager>
    {
        [SerializeField] private MasteryLevelupPopup masteryPopupPrefab;

        private MonoBehaviourPool<MasteryLevelupPopup> pool;

        private void Start()
        {
            this.pool = MonoBehaviourPool<MasteryLevelupPopup>.Factory(
                this.masteryPopupPrefab, UIManager.Instance.PopupContainer, 2);

            Mastery.AnyMasteryLevelUp += OnAnyMasteryLevelUp;
        }

        private void OnAnyMasteryLevelUp(Mastery mastery)
        {
            if (Game.Instance == null || !Game.Instance.State.IsScenario)
            {
                return;
            }

            this.pool.Spawn().Construct(mastery);
        }
    }
}