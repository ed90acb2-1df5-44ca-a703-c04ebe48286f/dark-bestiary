using System;
using System.Collections.Generic;
using DarkBestiary.UI.Controllers;
using DarkBestiary.UI.Elements;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DarkBestiary.UI.Views.Unity
{
    public class CombatLogView : View, ICombatLogView
    {
        public event Action CloseButtonClicked;

        [SerializeField] private ScrollRect m_ScrollRect;
        [SerializeField] private TextMeshProUGUI m_RowPrefab;
        [SerializeField] private Transform m_RowContainer;
        [SerializeField] private Interactable m_CloseButton;

        private readonly LinkedList<TextMeshProUGUI> m_Rows = new();

        private void Start()
        {
            m_CloseButton.PointerClick += OnCloseButtonPointerClick;
        }

        private void OnEnable()
        {
            Timer.Instance.Wait(0.01f, () =>
            {
                m_ScrollRect.normalizedPosition = new Vector2(0, 0);
            });
        }

        public void Add(string text)
        {
            var row = Instantiate(m_RowPrefab, m_RowContainer);
            row.text = text;
            row.transform.SetAsLastSibling();
            m_Rows.AddLast(row);

            if (m_Rows.Count > CombatLogViewController.c_BufferSize)
            {
                Destroy(m_Rows.First.Value.gameObject);
                m_Rows.RemoveFirst();
            }
        }

        private void OnCloseButtonPointerClick()
        {
            CloseButtonClicked?.Invoke();
        }
    }
}