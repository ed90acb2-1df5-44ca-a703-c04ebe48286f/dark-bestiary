using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Messaging;
using DarkBestiary.UI.Elements;
using UnityEngine;

namespace DarkBestiary.UI.Views.Unity
{
    public class KeyBindingsView : View, IKeyBindingsView
    {
        public event Payload<IEnumerable<KeyBindingInfo>> Applied;
        public event Payload Cancelled;
        public event Payload Reseted;

        [SerializeField] private KeyBindingRow keyBindingRowPrefab;
        [SerializeField] private Transform keyBindingRowContainer;
        [SerializeField] private Interactable resetButton;
        [SerializeField] private Interactable applyButton;
        [SerializeField] private Interactable cancelButton;
        [SerializeField] private Interactable closeButton;

        private KeyBindingRow selectedKeyBindingRow;
        private List<KeyBindingRow> keyBindingRows;

        private void Start()
        {
            this.resetButton.PointerClick += OnResetButtonPointerClick;
            this.applyButton.PointerClick += OnApplyButtonPointerClick;
            this.cancelButton.PointerClick += OnCancelButtonPointerClick;
            this.closeButton.PointerClick += OnCloseButtonPointerClick;
        }

        public void Construct(IEnumerable<KeyBindingInfo> keyBindingsInfo)
        {
            this.keyBindingRows = new List<KeyBindingRow>();

            foreach (var keyBindingInfo in keyBindingsInfo)
            {
                var keyBindingRow = Instantiate(this.keyBindingRowPrefab, this.keyBindingRowContainer);
                keyBindingRow.Clicked += OnKeyBindingRowClicked;
                keyBindingRow.Construct(keyBindingInfo);
                this.keyBindingRows.Add(keyBindingRow);
            }
        }

        public void Refresh(IEnumerable<KeyBindingInfo> keyBindingsInfo)
        {
            foreach (var keyBindingInfo in keyBindingsInfo)
            {
                var keyBindingRow = this.keyBindingRows.First(r => r.KeyBindingInfo.Type == keyBindingInfo.Type);
                keyBindingRow.Construct(keyBindingInfo);
            }
        }

        private void OnKeyBindingRowClicked(KeyBindingRow keyBindingRow)
        {
            this.selectedKeyBindingRow?.Deselect();
            this.selectedKeyBindingRow = keyBindingRow;
            this.selectedKeyBindingRow.Select();
        }

        private void DeselectCurrentRow()
        {
            this.selectedKeyBindingRow.Deselect();
            this.selectedKeyBindingRow = null;
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
            Applied?.Invoke(this.keyBindingRows.Select(row => row.KeyBindingInfo));
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
            if (this.selectedKeyBindingRow?.KeyBindingInfo == null)
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
                foreach (var keyBindingRow in this.keyBindingRows.Where(row => row.KeyBindingInfo.Code == pressedKey))
                {
                    keyBindingRow.ChangeKeyCode(KeyCode.None);
                }

                this.selectedKeyBindingRow.ChangeKeyCode(pressedKey);
            }

            DeselectCurrentRow();
        }
    }
}