using System;
using TMPro;
using UnityEngine;

namespace DarkBestiary.UI.Elements
{
    public class TreasureEncounterPanel : MonoBehaviour
    {
        public event Action CompleteButtonClicked;

        [SerializeField] private TextMeshProUGUI m_Text;
        [SerializeField] private Interactable m_CompleteButton;

        private void Start()
        {
            m_CompleteButton.PointerClick += OnCompleteButtonClicked;
        }

        private void OnCompleteButtonClicked()
        {
            CompleteButtonClicked?.Invoke();
        }

        public void ChangeBombCount(int marked, int total)
        {
            m_Text.text = $"{I18N.Instance.Translate("ui_explosives")}: {marked.ToString()}/{total.ToString()}";
        }
    }
}