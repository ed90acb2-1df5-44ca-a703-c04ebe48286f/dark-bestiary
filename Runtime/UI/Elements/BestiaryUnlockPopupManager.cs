using DarkBestiary.Components;
using DarkBestiary.Managers;
using UnityEngine;

namespace DarkBestiary.UI.Elements
{
    public class BestiaryUnlockPopupManager : Singleton<BestiaryUnlockPopupManager>
    {
        [SerializeField] private TextPopup m_PopupPrefab;

        private MonoBehaviourPool<TextPopup> m_Pool;

        private void Start()
        {
            m_Pool = MonoBehaviourPool<TextPopup>.Factory(
                m_PopupPrefab, UIManager.Instance.PopupContainer, 2);

            BestiaryManager2.BestiaryUpdated += OnBestiaryUpdated;
        }

        private void OnBestiaryUpdated(UnitComponent unit)
        {
            m_Pool.Spawn().Construct(unit.Name);
        }
    }
}