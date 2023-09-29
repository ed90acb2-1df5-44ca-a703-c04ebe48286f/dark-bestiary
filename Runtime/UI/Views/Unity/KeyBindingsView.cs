using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.UI.Elements;
using UnityEngine;

namespace DarkBestiary.UI.Views.Unity
{
    public class KeyBindingsView : View, IKeyBindingsView
    {
        public event Action<IEnumerable<KeyBindingInfo>> Applied;
        public event Action Cancelled;
        public event Action Reseted;

        [SerializeField] private KeyBindingRow m_KeyBindingRowPrefab;
        [SerializeField] private Transform m_KeyBindingRowContainer;
        [SerializeField] private Interactable m_ResetButton;
        [SerializeField] private Interactable m_ApplyButton;
        [SerializeField] private Interactable m_CancelButton;
        [SerializeField] private Interactable m_CloseButton;

        private KeyBindingRow m_SelectedKeyBindingRow;
        private List<KeyBindingRow> m_KeyBindingRows;

        private void Start()
        {
            m_ResetButton.PointerClick += OnResetButtonPointerClick;
            m_ApplyButton.PointerClick += OnApplyButtonPointerClick;
            m_CancelButton.PointerClick += OnCancelButtonPointerClick;
            m_CloseButton.PointerClick += OnCloseButtonPointerClick;
        }

        public void Construct(IEnumerable<KeyBindingInfo> keyBindingsInfo)
        {
            m_KeyBindingRows = new List<KeyBindingRow>();

            foreach (var keyBindingInfo in keyBindingsInfo)
            {
                var keyBindingRow = Instantiate(m_KeyBindingRowPrefab, m_KeyBindingRowContainer);
                keyBindingRow.Clicked += OnKeyBindingRowClicked;
                keyBindingRow.Construct(keyBindingInfo);
                m_KeyBindingRows.Add(keyBindingRow);
            }
        }

        public void Refresh(IEnumerable<KeyBindingInfo> keyBindingsInfo)
        {
            foreach (var keyBindingInfo in keyBindingsInfo)
            {
                var keyBindingRow = m_KeyBindingRows.First(r => r.KeyBindingInfo.Type == keyBindingInfo.Type);
                keyBindingRow.Construct(keyBindingInfo);
            }
        }

        private void OnKeyBindingRowClicked(KeyBindingRow keyBindingRow)
        {
            m_SelectedKeyBindingRow?.Deselect();
            m_SelectedKeyBindingRow = keyBindingRow;
            m_SelectedKeyBindingRow.Select();
        }

        private void DeselectCurrentRow()
        {
            m_SelectedKeyBindingRow.Deselect();
            m_SelectedKeyBindingRow = null;
        }

        private static KeyCode DetectPressedKey()
        {
            var exclude = new List<KeyCode>()
            {
                KeyCode.Mouse0,
                KeyCode.Mouse1
            };

            return Enum.GetValues(typeof(KeyCode))
                .Cast<KeyCode>()
                .Where(key => !exclude.Contains(key))
                .FirstOrDefault(Input.GetKeyDown);
        }

        private void OnResetButtonPointerClick()
        {
            Reseted?.Invoke();
        }

        private void OnApplyButtonPointerClick()
        {
            Applied?.Invoke(m_KeyBindingRows.Select(row => row.KeyBindingInfo));
        }

        private void OnCloseButtonPointerClick()
        {
            Hide();
        }

        private void OnCancelButtonPointerClick()
        {
            Cancelled?.Invoke();
        }

        private void Update()
        {
            if (m_SelectedKeyBindingRow?.KeyBindingInfo == null)
            {
                return;
            }

            var pressedKey = KeyCode.None;

            if (Input.anyKeyDown)
            {
                pressedKey = DetectPressedKey();
            }

            if (pressedKey == KeyCode.None)
            {
                return;
            }

            if (pressedKey != KeyCode.Escape)
            {
                foreach (var keyBindingRow in m_KeyBindingRows.Where(row => row.KeyBindingInfo.Code == pressedKey))
                {
                    keyBindingRow.ChangeKeyCode(KeyCode.None);
                }

                m_SelectedKeyBindingRow.ChangeKeyCode(pressedKey);
            }

            DeselectCurrentRow();
        }
    }
}