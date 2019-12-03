using System.Collections.Generic;
using DarkBestiary.Messaging;
using DarkBestiary.UI.Controllers;
using DarkBestiary.UI.Elements;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DarkBestiary.UI.Views.Unity
{
    public class CombatLogView : View, ICombatLogView
    {
        public event Payload CloseButtonClicked;

        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private TextMeshProUGUI rowPrefab;
        [SerializeField] private Transform rowContainer;
        [SerializeField] private Interactable closeButton;

        private readonly LinkedList<TextMeshProUGUI> rows = new LinkedList<TextMeshProUGUI>();

        private void Start()
        {
            this.closeButton.PointerUp += OnCloseButtonPointerUp;
        }

        public void AddRow(string text)
        {
            var row = Instantiate(this.rowPrefab, this.rowContainer);
            row.text = text;
            this.rows.AddLast(row);

            if (this.rows.Count > CombatLogViewController.BufferSize)
            {
                Destroy(this.rows.First.Value.gameObject);
                this.rows.RemoveFirst();
            }

            Timer.Instance.WaitForFixedUpdate(() =>
            {
                this.scrollRect.normalizedPosition = new Vector2(0, 0);
            });
        }

        private void OnCloseButtonPointerUp()
        {
            CloseButtonClicked?.Invoke();
        }
    }
}