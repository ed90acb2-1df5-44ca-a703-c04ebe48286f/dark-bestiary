using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DarkBestiary.UI.Views.Unity
{
    public class DeveloperConsoleView : View, IDeveloperConsoleView
    {
        public event Action<string> SuggestingCommand;
        public event Action<string> SubmittingCommand;

        [SerializeField] private Text m_TextPrefab;
        [SerializeField] private Transform m_TextContainer;
        [SerializeField] private InputField m_Input;
        [SerializeField] private CanvasGroup m_CanvasGroup;

        private readonly LinkedList<string> m_Commands = new();
        private bool m_IsEnabled = true;

        private void Start()
        {
            m_Input.onEndEdit.AddListener(OnInputChanged);
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
                SubmittingCommand?.Invoke(m_Input.text);
                m_Commands.AddFirst(m_Input.text);

                m_Input.text = "";
                m_Input.Select();
                m_Input.ActivateInputField();
            }
        }

        public void Info(string text)
        {
            var row = Instantiate(m_TextPrefab, m_TextContainer);
            row.color = Color.black;
            row.text = text;
        }

        public void Error(string text)
        {
            var row = Instantiate(m_TextPrefab, m_TextContainer);
            row.color = Color.red;
            row.text = text;
        }

        public void Success(string text)
        {
            var row = Instantiate(m_TextPrefab, m_TextContainer);
            row.color = new Color(0, 0.25f, 0);
            row.text = text;
        }

        public void Clear()
        {
            foreach (var text in m_TextContainer.GetComponentsInChildren<Text>())
            {
                Destroy(text.gameObject);
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F12))
            {
                m_IsEnabled = !m_IsEnabled;

                if (m_IsEnabled)
                {
                    m_CanvasGroup.alpha = 1;
                    m_CanvasGroup.blocksRaycasts = true;
                    m_CanvasGroup.interactable = true;

                    m_Input.interactable = true;
                    m_Input.Select();
                    m_Input.ActivateInputField();
                }
                else
                {
                    m_Input.interactable = false;

                    m_CanvasGroup.alpha = 0;
                    m_CanvasGroup.blocksRaycasts = false;
                    m_CanvasGroup.interactable = false;
                }
            }

            if (Input.GetKeyDown(KeyCode.Tab) && m_Input.IsActive())
            {
                SuggestingCommand?.Invoke(m_Input.text);
            }

            if (Input.GetKeyDown(KeyCode.UpArrow) && m_Input.IsActive())
            {
                var command = m_Commands.First == null ? "" : m_Commands.First.Value;

                if (string.IsNullOrEmpty(command))
                {
                    return;
                }

                m_Commands.RemoveFirst();
                m_Commands.AddLast(command);

                m_Input.text = command;
            }
        }
    }
}