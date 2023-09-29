using DarkBestiary.Managers;
using DarkBestiary.Masteries;
using UnityEngine;

namespace DarkBestiary.UI.Elements
{
    public class MasteryLevelupPopupManager : Singleton<MasteryLevelupPopupManager>
    {
        [SerializeField]
        private MasteryLevelupPopup m_MasteryPopupPrefab = null!;

        private MonoBehaviourPool<MasteryLevelupPopup> m_Pool = null!;

        private void Start()
        {
            m_Pool = MonoBehaviourPool<MasteryLevelupPopup>.Factory(m_MasteryPopupPrefab, UIManager.Instance.PopupContainer, 2);
            Mastery.AnyMasteryLevelUp += OnAnyMasteryLevelUp;
        }

        private void OnAnyMasteryLevelUp(Mastery mastery)
        {
            if (!Game.Instance.IsScenario)
            {
                return;
            }

            m_Pool.Spawn().Construct(mastery);
        }
    }
}