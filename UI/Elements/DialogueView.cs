using DarkBestiary.Dialogues;
using TMPro;
using UnityEngine;

namespace DarkBestiary.UI.Elements
{
    public class DialogueView : Singleton<DialogueView>
    {
        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private Interactable closeButton;
        [SerializeField] private DialogueActionView actionViewPrefab;
        [SerializeField] private Transform actionViewContainer;

        private MonoBehaviourPool<DialogueActionView> actionViewPool;

        private void Start()
        {
            Instance.Hide();

            this.closeButton.PointerUp += Hide;

            this.actionViewPool = MonoBehaviourPool<DialogueActionView>.Factory(
                this.actionViewPrefab, this.actionViewContainer, 5);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void Show(Dialogue dialogue)
        {
            dialogue.OnRead();

            gameObject.SetActive(true);

            this.title.text = dialogue.Title;
            this.text.text = dialogue.Text;

            this.actionViewPool.DespawnAll();

            var index = 0;

            foreach (var action in dialogue.Actions)
            {
                index++;

                this.actionViewPool.Spawn().Construct(action, KeyCode.Alpha0 + index);
            }
        }
    }
}