using DarkBestiary.Messaging;
using TMPro;
using UnityEngine;

namespace DarkBestiary.UI.Elements
{
    public class TreasureEncounterPanel : MonoBehaviour
    {
        public event Payload CompleteButtonClicked;

        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private Interactable completeButton;

        private void Start()
        {
            this.completeButton.PointerClick += OnCompleteButtonClicked;
        }

        private void OnCompleteButtonClicked()
        {
            CompleteButtonClicked?.Invoke();
        }

        public void ChangeBombCount(int marked, int total)
        {
            this.text.text = $"{I18N.Instance.Translate("ui_explosives")}: {marked.ToString()}/{total.ToString()}";
        }
    }
}