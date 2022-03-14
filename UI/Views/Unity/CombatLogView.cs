using System;
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
            this.closeButton.PointerClick += OnCloseButtonPointerClick;
        }

        private void OnEnable()
        {
            Timer.Instance.Wait(0.01f, () =>
            {
                this.scrollRect.normalizedPosition = new Vector2(0, 0);
            });
        }

        public void Add(string text)
        {
            var row = Instantiate(this.rowPrefab, this.rowContainer);
            row.text = text;
            row.transform.SetAsLastSibling();
            this.rows.AddLast(row);

            if (this.rows.Count > CombatLogViewController.BufferSize)
            {
                Destroy(this.rows.First.Value.gameObject);
                this.rows.RemoveFirst();
            }
        }

        private void OnCloseButtonPointerClick()
        {
            CloseButtonClicked?.Invoke();
        }
    }
}