using System.Collections.Generic;
using DarkBestiary.Messaging;
using UnityEngine;
using UnityEngine.UI;

namespace DarkBestiary.UI.Views.Unity
{
    public class DeveloperConsoleView : View, IDeveloperConsoleView
    {
        public event Payload<string> SuggestingCommand;
        public event Payload<string> SubmittingCommand;

        [SerializeField] private Text textPrefab;
        [SerializeField] private Transform textContainer;
        [SerializeField] private InputField input;
        [SerializeField] private CanvasGroup canvasGroup;

        private readonly LinkedList<string> commands = new LinkedList<string>();
        private bool isEnabled = true;

        private void Start()
        {
            this.input.onEndEdit.AddListener(OnInputChanged);
        }

        public override void Show()
        {
        }

        public override void Hide()
        {
        }

        private void OnInputChanged(string value)
        {
            if (Input.GetKey(KeyCode.Return))
            {
                SubmittingCommand?.Invoke(this.input.text);
                this.commands.AddFirst(this.input.text);

                this.input.text = "";
                this.input.Select();
                this.input.ActivateInputField();
            }
        }

        public void Info(string text)
        {
            var row = Instantiate(this.textPrefab, this.textContainer);
            row.color = Color.black;
            row.text = text;
        }

        public void Error(string text)
        {
            var row = Instantiate(this.textPrefab, this.textContainer);
            row.color = Color.red;
            row.text = text;
        }

        public void Success(string text)
        {
            var row = Instantiate(this.textPrefab, this.textContainer);
            row.color = new Color(0, 0.25f, 0);
            row.text = text;
        }

        public void Clear()
        {
            foreach (var text in this.textContainer.GetComponentsInChildren<Text>())
            {
                Destroy(text.gameObject);
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F12))
            {
                this.isEnabled = !this.isEnabled;

                if (this.isEnabled)
                {
                    this.canvasGroup.alpha = 1;
                    this.canvasGroup.blocksRaycasts = true;
                    this.canvasGroup.interactable = true;

                    this.input.interactable = true;
                    this.input.Select();
                    this.input.ActivateInputField();
                }
                else
                {
                    this.input.interactable = false;

                    this.canvasGroup.alpha = 0;
                    this.canvasGroup.blocksRaycasts = false;
                    this.canvasGroup.interactable = false;
                }
            }

            if (Input.GetKeyDown(KeyCode.Tab) && this.input.IsActive())
            {
                SuggestingCommand?.Invoke(this.input.text);
            }

            if (Input.GetKeyDown(KeyCode.UpArrow) && this.input.IsActive())
            {
                var command = this.commands.First == null ? "" : this.commands.First.Value;

                if (string.IsNullOrEmpty(command))
                {
                    return;
                }

                this.commands.RemoveFirst();
                this.commands.AddLast(command);

                this.input.text = command;
            }
        }
    }
}